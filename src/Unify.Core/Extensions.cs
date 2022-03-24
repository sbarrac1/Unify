using NLog.Config;
using NLog.Targets;
using System.Reflection;
using System;

namespace Unify.Core;

public static class Extensions
{
    /// <summary>
    /// Calls a generic method within the current object.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="methodName"></param>
    /// <param name="typeArgs"></param>
    /// <param name="args"></param>
    /// <typeparam name="TReturn">The return type of the generic method</typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static TReturn CallGenericMethod<TReturn>(this object obj, string methodName, Type[] typeArgs, params object[] args)
    {
        MethodInfo mi = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

        if (mi == null)
            throw new InvalidOperationException("Could not find generic resolve method!");

        MethodInfo genericMethod = mi.MakeGenericMethod(typeArgs);
        return (TReturn)genericMethod.Invoke(obj, args);
    }

    public static TReturn CallGenericMethodStatic<TReturn>(Type callerType, string methodName, Type[] typeArgs, params object[] args)
    {
        MethodInfo mi = callerType.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);

        if (mi == null)
            throw new InvalidOperationException("Could not find generic resolve method!");

        MethodInfo genericMethod = mi.MakeGenericMethod(typeArgs);
        return (TReturn)genericMethod.Invoke(null, args);
    }

    public static void SetupNLogConsole()
    {
        var logCfg = new LoggingConfiguration();
        var consoleTarget = new ConsoleTarget("Console")
        {
            Layout = "${logger}|${message:withexception=true}"
        };
        logCfg.AddTarget(consoleTarget);
        logCfg.AddRule(LogLevel.Trace, LogLevel.Fatal, "Console");
        LogManager.Configuration = logCfg;
    }

    
    public static void AddEventHandlers(this ContainerBuilder builder, params Assembly[] assemblies)
    {
        Type baseHandlerType = typeof(IEventHandler<>);
        Type baseRequestType = typeof(IRequestHandler<,>);

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == baseHandlerType))
                    builder.RegisterType(type).AsImplementedInterfaces().InstancePerLifetimeScope();
                else if (type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == baseRequestType))
                    builder.RegisterType(type).AsImplementedInterfaces().InstancePerLifetimeScope();
            }
        }
    }
    
    public static void ReadExact(this Stream stream, Span<byte> buffer, int count)
    {
        int bIn = 0;

        while (bIn != count)
        {
            if ((bIn += stream.Read(buffer[bIn..count])) == 0)
                throw new EndOfStreamException();

            if (bIn > count)
                throw new IOException("Received too many bytes");
        }
    }

    public static async Task ReadExactAsync(this Stream stream, byte[] buffer, int count, CancellationToken ct = default)
    {
        int bIn = 0;

        while (bIn != count)
        {
            if ((bIn += await stream.ReadAsync(buffer.AsMemory(bIn, count - bIn), ct)) == 0)
                throw new EndOfStreamException();

            if (bIn > count)
                throw new IOException("Received too many bytes");
        }
    }

    public static void ReadExact(this Stream stream, byte[] buffer, int count)
    {
        int bIn = 0;

        while (bIn != count)
        {
            if ((bIn += stream.Read(buffer, bIn, count - bIn)) == 0)
                throw new EndOfStreamException();
        }
    }
}