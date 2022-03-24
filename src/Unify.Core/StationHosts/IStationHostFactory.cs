using Unify.Core.Net.IO;

namespace Unify.Core.StationHosts;

/// <summary>
/// Creates a station host on a background thread. 
/// 
/// A station host consists of a clipboard, Input driver, and input hook service
/// </summary>
public interface IStationHostFactory
{
    IDisposable Create(IEventStream eventStream, StationHostConfig config, Action<Exception> onFault);
}