namespace Unify.Core.Common.Clipboard;

/// <summary>
/// Represents data formats that can be supported by a clipboard source
/// </summary>
[Flags]
public enum ClipboardFormats
{
    None = 0,
    Text = 1,
    Bitmap = 2,
    HDrop = 4,
    FileSys = 8
}
