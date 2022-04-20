using System.Reflection;
using Unify.Core.Common;
using Unify.Core.Net.Formatting.Formatters;

namespace Unify.Core.Net.Formatting;

public sealed class ObjectManager : IObjectManager
{
    public static IObjectManager Instance { get; } = new ObjectManager();

    private readonly Map<int, Type> _objectTypesMap = new();
    private readonly Map<int, ObjectWriteDelegate> _writerDelegatesMap = new();
    private readonly Map<int, ObjectReadDelegate> _readerDelegatesMap = new();

    private Autofac.IContainer _container;

    public ObjectReadDelegate GetReader(int objectId)
    {   
        return _readerDelegatesMap[objectId];
    }

    public ObjectReadDelegate GetReader(Type objectType)
    {
        return _readerDelegatesMap[_objectTypesMap[objectType]];
    }

    public ObjectWriteDelegate GetWriter(int objectId)
    {
        return _writerDelegatesMap[objectId];
    }

    public ObjectWriteDelegate GetWriter(Type objectType)
    {
        return _writerDelegatesMap[_objectTypesMap[objectType]];
    }

    public void Setup()
    {
        lock (this)
        {
            if (_container == null)
                InternalSetup();
        }
    }

    private void InternalSetup()
    {
        _container = Configure();
        PopulateObjectIds();
        CreateDelegates();
    }

    private IContainer Configure()
    {
        var builder = new ContainerBuilder();
        Type formatterInterface = typeof(IFormatter<>);

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsInterface || type.IsAbstract || type == typeof(ProtoFormatter<>))
                continue;

            if(type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == formatterInterface))
            {
                builder.RegisterType(type).AsImplementedInterfaces().SingleInstance();
            }
        }

        return builder.Build();
    }

    private void PopulateObjectIds()
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var formattableAttribute = type.GetCustomAttribute<FormattableAttribute>();

            if(formattableAttribute != null)
            {
                _objectTypesMap.Add(type, formattableAttribute.ObjectId);
            }
        }
    }

    private void CreateDelegates()
    {
        foreach (var valuePair in _objectTypesMap)
        {
            Type type = valuePair.Value;

            ObjectWriteDelegate writer = this.CallGenericMethod<ObjectWriteDelegate>(nameof(InternalCreateWriter), new Type[] { type });
            ObjectReadDelegate reader = this.CallGenericMethod<ObjectReadDelegate>(nameof(InternalCreateReader), new Type[] { type });

            _writerDelegatesMap.Add(_objectTypesMap[type], writer);
            _readerDelegatesMap.Add(_objectTypesMap[type], reader);
        }
    }

    private ObjectWriteDelegate InternalCreateWriter<T>()
    {
        if (!_container.TryResolve<IFormatter<T>>(out var formatter))
            formatter = new ProtoFormatter<T>();

        return (stream, value) => formatter.Write(stream, (T)value);
    }

    private ObjectReadDelegate InternalCreateReader<T>()
    {
        if (!_container.TryResolve<IFormatter<T>>(out var formatter))
            formatter = new ProtoFormatter<T>();

        return (stream, prefix) => formatter.Read(stream, prefix);
    }

    public void WritePrefixForType<T>(Stream stream, int length)
    {
        new ObjectPrefix((short)_objectTypesMap[typeof(T)], length).Write(stream);
    }
}
