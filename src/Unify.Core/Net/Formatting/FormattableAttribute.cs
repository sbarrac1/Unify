using Unify.Core.Net.Formatting.Formatters;

namespace Unify.Core.Net.Formatting;

/// <summary>
/// Marks the object as formattable via an available <seealso cref="IFormatter{T}"/>
/// </summary>
public sealed class FormattableAttribute : Attribute
{
    public FormattableAttribute(int objectId)
    {
        ObjectId = objectId;
    }

    /// <summary>
    /// A unique ID of the object
    /// </summary>
    public int ObjectId { get; }
}
