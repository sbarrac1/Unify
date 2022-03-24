using Unify.Core.Common;
using Unify.Core.Common.Input;

namespace Unify.Server.Config;
public class StationConfig
{
    public Hotkey Hotkey { get; set; }
    
    public string LeftStation { get; set; }
    public string RightStation { get; set; }
    public string BottomStation { get; set; }
    public string TopStation { get; set; }

    public bool TryGetStationAtSide(Side side, out string stationName)
    {
        stationName = side switch
        {
            Side.Left => LeftStation,
            Side.Right => RightStation,
            Side.Top => TopStation,
            Side.Bottom => BottomStation,
        };

        return !string.IsNullOrEmpty(stationName);
    }
}
