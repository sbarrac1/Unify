# Unify
Unify is a software KVM solution that allows one mouse and keyboard to be used across multiple devices. It also creates a global clipboard that is syncronized across clients, meaning files, text and images can be transferred by simply copying and pasting between them.

# Current features
- Switch input to another client by simply moving the cursor to the edge of the screen
- Switch input to another client via hotkeys
- Copy & paste files, text and images across clients
- (Soon) Drag & drop files, text and images across clients

# Supported platforms
Currently only Windows (10+) is supported. Dotnet 6 runtime is required.

## Concepts
### Server
The server is the machine with the mouse & keyboard that you want to use across other devices.

### Station
A station represents a client, inlcudes a mouse, keyboard and clipboard. Named after the [Windows 'Window station' concept](https://docs.microsoft.com/en-us/windows/win32/winstation/window-stations).

# Usage

The server can be ran by running 'Unify.Cli server 1234', where 1234 is the port that the server will run on.

## Server config

The server can be configured via the Unify.Cli.Dll.config file. The config file contains the virtual positions of each station. The station 'Localhost' represents the server station.

In this example, the client 'Laptop' is set to the left of the server station. In this example, Localhost is assigned the hotkey Shift+F1; hotkeys are written in the format {key}:{mod}:{mod}:{mod} where the possible modifiers are Alt,Ctrl,Shift and Win (Windows key).

To allow a client to connect, the config file must specify that the station is enabled, in this example the entry 'key="Server.Station.Laptop.Enabled" value="True"' allows a client called 'Laptop' to connect.

### Example config
```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<appSettings>
		<add key="Server.EnableClipboard" value="True"/>
		<add key="Server.EnableHooks" value="True"/>
		<add key="Server.StopHotkey" value="Q:Ctrl:Shift:Alt"/>
		
		<add key="Server.Station.Localhost.Hotkey" value="F1:Shift"/>
		<add key="Server.Station.Localhost.LeftStation" value="Laptop"/>
		
		<add key="Server.Station.Laptop.Enabled" value="True"/>
		<add key="Server.Station.Laptop.Hotkey" value="F2:Shift"/>
		<add key="Server.Station.Laptop.RightStation" value="Localhost"/>
	</appSettings>
</configuration>
```

### Client

The client can be ran with the command 'Unify.Cli client 127.0.0.1:1234 Laptop', where 'Laptop' is the name of the client.

### Roadmap

- Run as windows service to allow access to secure desktop, and allow simulating alt+ctrl+del
- Optimize file read requests
- Improve multi monitor support
- Linux support

# Demo
## Copying text

https://user-images.githubusercontent.com/46637437/159817506-108049ac-4e38-4649-8338-5d5f56626b6a.mp4

## Copying files



https://user-images.githubusercontent.com/46637437/159817523-1e21a834-3ac9-478e-99fa-fd24f62d43a0.mp4

