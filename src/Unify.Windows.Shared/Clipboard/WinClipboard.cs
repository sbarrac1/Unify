using System.ComponentModel;
using Unify.Core.Common.Clipboard;
using Unify.Core.CommonServices.DataMarshal.Data;
using Unify.Core.CommonServices.FileSys.Contexts;
using Unify.Core.CommonServices.FileSys.Contexts.Local;
using Unify.Core.Events;
using Unify.Core.Events.Target;
using Unify.Windows.Shared.Clipboard.Interop;

namespace Unify.Windows.Shared.Clipboard;

public sealed class WinClipboard : IWinClipboard
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IWindow _window;
    private readonly IEventTarget _eventTarget;

    private DataObjectProvider _currentClipboardObject;

    public WinClipboard(IWindow window, IEventTarget eventTarget)
    {
        _window = window;
        _eventTarget = eventTarget;
        _window.MessageReceived += WindowOnMessageReceived;
        
        _window.WindowDispatcher.InvokeWait(() =>
        {
            if(!User32.AddClipboardFormatListener(_window.Handle))
                _logger.Error(new Win32Exception(), "Failed to add clipboard format listener");
        });
    }

    private void WindowOnMessageReceived(User32.MSG obj)
    {
        if (obj.message == WindowMessage.WM_CLIPBOARDUPDATE)
        {
            _logger.Trace("Got WM_CLIPBOARDCHANGE");

            if (_currentClipboardObject !=null && Ole32.OleIsCurrentClipboard(_currentClipboardObject) == 0)
                return;

            using (var wrapper = GetWrapperForCurrentClipboard())
            {
                if (wrapper.IsUnifyObject())
                {
                    _logger.Trace("Clipboard object is an unify owned object, ignoring change");
                    return;
                }
            }

            _logger.Trace("We didn't set the clipboard, sending clipboard change event...");
            _eventTarget.PostEvent(new ClipboardChangedEvent());
        }
    }

    public ClipboardFormats GetFormats()
    {
        return _window.WindowDispatcher.InvokeReturn(() =>
        {
            using (var wrapper = GetWrapperForCurrentClipboard())
            {
                return wrapper.GetSupportedFormats();
            }
        });
    }

    public IDataContainer<string> GetText()
    {
        return _window.WindowDispatcher.InvokeReturn(() =>
        {
            using (var wrapper = GetWrapperForCurrentClipboard())
            {
                return new DataContainer<string>(wrapper.GetText());
            }
        });
    }

    public IFileSysContext GetFiles()
    {
        return _window.WindowDispatcher.InvokeReturn(() =>
        {
            using (var wrapper = GetWrapperForCurrentClipboard())
            {
                string[] filePaths = wrapper.GetFileList();

                return new LocalFileSysContext(filePaths);
            }
        });
    }

    public void TakeOwnership(IClipboard clipboard)
    {
        _window.WindowDispatcher.InvokePost(() =>
        {
            _logger.Debug($"Taking clipboard ownership with {clipboard}");

            try
            {
                _currentClipboardObject = new DataObjectProvider(clipboard);
                Ole32.OleSetClipboard(_currentClipboardObject);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Failed to take clipboard ownership");
            }
            
        });
    }

    private DataObjectWrapper GetWrapperForCurrentClipboard()
    {
        if (!Win32Helpers.TryGetClipboardObject(10, out var dataObject))
            throw new Win32Exception("Failed to open clipboard");

        return new DataObjectWrapper(dataObject, _window);
    }

    public void Dispose()
    {
        _window.Dispose();
        _currentClipboardObject = null;
    }
}