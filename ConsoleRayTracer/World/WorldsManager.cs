using System.Reflection;

namespace ConsoleRayTracer;

public class WorldsManager<E, C> where E : IAnimatedEntity where C : IControllableCamera
{
    private readonly string _location;
    private readonly List<World<E, C>> _worlds;

    public WorldsManager(string location, Func<World<E, C>> fallback)
    {
        JsonConvert.DefaultSettings = () => new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new ConstructorResolver(),
        };

        if (!File.Exists(location))
        {
            File.WriteAllText(location, JsonConvert.SerializeObject(new List<World<E, C>>() { fallback() }, Formatting.Indented));
        }

        _location = location;
        _worlds = JsonConvert.DeserializeObject<List<World<E, C>>>(File.ReadAllText(_location))!;
    }

    public IReadOnlyList<World<E, C>> Worlds { get => _worlds; }

    public void Add(World<E, C> world)
    {
        _worlds.Add(world);
        File.WriteAllText(_location, JsonConvert.SerializeObject(_worlds));
    }

    public void Remove(World<E, C> world)
    {
        _worlds.Remove(world);
        File.WriteAllText(_location, JsonConvert.SerializeObject(_worlds));
    }
}

class ConstructorResolver : DefaultContractResolver
{
    private static readonly IEnumerable<Type> _systemTypes = typeof(Assembly).Assembly.GetExportedTypes();

    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
        var ctor = objectType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .MaxBy(ctor => ctor.GetParameters().Length);

        var contract = base.CreateObjectContract(objectType);
        if (!_systemTypes.Contains(objectType) && ctor is not null)
        {
            contract.OverrideCreator = o => ctor.Invoke(o);
            foreach (var param in CreateConstructorParameters(ctor, contract.Properties))
            {
                if (!contract.CreatorParameters.Any(p => p.PropertyName == param.PropertyName))
                {
                    contract.CreatorParameters.Add(param);
                }
            }
        }
        return contract;
    }
}
