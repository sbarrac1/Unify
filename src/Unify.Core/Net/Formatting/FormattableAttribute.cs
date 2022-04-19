namespace Unify.Core.Net.Formatting;

public sealed class FormattableAttribute : Attribute
{
    public FormattableAttribute(int objectId)
    {
        ObjectId = objectId;
    }

    public int ObjectId { get; }
}
