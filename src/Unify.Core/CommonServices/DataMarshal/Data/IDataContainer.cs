namespace Unify.Core.CommonServices.DataMarshal.Data;


/// <summary>
/// Represents a single object of an uknown type that can be 
/// serialized into a stream and deserialized by a client
/// </summary>
/// <typeparam name="TObject"></typeparam>
public interface IDataContainer : IDisposable
{ /// <summary>
  /// Returns the object stored in this <see cref="IDataContainer"/>
  /// in serialized form
  /// </summary>
  /// <returns></returns>
    Stream GetStream();
}

/// <summary>
/// Represents a single object of type <typeparamref name="TObject"/> that can be 
/// accessed represented as either the object itself, or a stream containing the the serialized object
/// </summary>
/// <typeparam name="TObject"></typeparam>
public interface IDataContainer<TObject> : IDataContainer
{
    /// <summary>
    /// Returns the object stored in this <see cref="IDataContainer{TObject}"/>
    /// </summary>
    /// <returns></returns>
    TObject GetObject();
}
