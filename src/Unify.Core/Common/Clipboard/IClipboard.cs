using Unify.Core.CommonServices.DataMarshal.Data;
using Unify.Core.CommonServices.FileSys.Contexts;

namespace Unify.Core.Common.Clipboard;

/// <summary>
/// A clipboard source
/// </summary>
public interface IClipboard
{
    ClipboardFormats GetFormats();

    IDataContainer<string> GetText();
    IFileSysContext GetFiles();
}
