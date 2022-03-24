using System.Configuration;
using Unify.Core.StationHosts;

namespace Unify.Client.Config;
public sealed class AppSettingsService : IClientConfigService
{
    public StationHostConfig StationHostConfig
    {
        get
        {
            return new StationHostConfig
            {
                EnableClipboard = ConfigurationManager.AppSettings["Server.EnableClipboard"] == "True",
                EnableHooks = ConfigurationManager.AppSettings["Server.EnableHooks"] == "True"
            };
        }
    }
}
