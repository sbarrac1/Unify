using Unify.Core.Common.Clipboard;

namespace Unify.Windows.Shared.Clipboard;

/// <summary>
/// Allows placing and retrieving data to/from the 
/// windows clipboard
/// </summary>
public interface IWinClipboard : IClipboard, IDisposable
{
    /// <summary>
    /// Takes clipboard ownership with the given clipboard source
    /// </summary>
    /// <param name="clipboard"></param>
    void TakeOwnership(IClipboard clipboard);
}