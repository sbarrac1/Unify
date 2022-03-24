using NLog;
using System.Net;
using Unify.Client;
using Unify.Core.Events.Dispatch;
using Unify.Core.Events.Target;
using Unify.Server;
using Unify.Server.Config;
using Unify.Windows.Shared.Input.Driver;
using Unify.Windows.Shared.StationHost;


if (args.Length == 0)
{
    PrintUsage();
    return;
}

if (string.Compare(args[0], "client", StringComparison.OrdinalIgnoreCase) == 0)
{
    await RunClientAsync(args);
    return;
}


if (string.Compare(args[0], "server", StringComparison.OrdinalIgnoreCase) == 0)
{
    RunServer(args);
    return;
}

PrintUsage();
return;

void RunServer(string[] args)
{
    if (args.Length != 2)
    {
        PrintUsage();
        return;
    }

    if (!int.TryParse(args[1], out var port))
    {
        PrintUsage();
        return;
    }

    var BindAddress = new IPEndPoint(IPAddress.Any, port);

    var server = Server.Create();

    Console.CancelKeyPress += (_, args) =>
    {
        server.Dispose();

        args.Cancel = true;
    };

    server.Start(BindAddress);

    Console.WriteLine("Press Ctrl+C to stop server");
}

async Task RunClientAsync(string[] args)
{
    if (args.Length != 3)
    {
        PrintUsage();
        return;
    }

    if (!IPEndPoint.TryParse(args[1], out IPEndPoint serverEndpoint))
    {
        PrintUsage();
        return;
    }

    if (string.IsNullOrEmpty(args[2]))
    {
        PrintUsage();
        return;
    }

    try
    {
        var client = await Client.CreateAsync(serverEndpoint, args[2]);

        Console.CancelKeyPress += (_, args) =>
        {
            client.Dispose();

            args.Cancel = true;
        };


        Console.WriteLine("Press Ctrl+C to stop client");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to start client: {ex}");
        return;
    }
}

void PrintUsage()
{
    Console.WriteLine("Usage: ");
    Console.WriteLine("Unify.Cli client [server:port] [station name]");
    Console.WriteLine("Unify.Cli server [port]");
}
