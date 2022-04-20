namespace Unify.Core.Net.Formatting;

public delegate void ObjectWriteDelegate(Stream stream, object value);
public delegate object ObjectReadDelegate(Stream stream, ObjectPrefix prefix);
