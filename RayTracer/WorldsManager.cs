using System.Reflection;

namespace RayTracer;

public class WorldsManager
{
    private readonly string _location;
    private readonly List<World> _worlds;

    public WorldsManager(string location)
    {
        JsonConvert.DefaultSettings = () => new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new ConstructorResolver(),
        };

        if (!File.Exists(location))
        {
            File.WriteAllText(location, JsonConvert.SerializeObject(new List<World>() { { World.Default } }));
        }

        _location = location;
        _worlds = JsonConvert.DeserializeObject<List<World>>(File.ReadAllText(location))!;
    }

    public IReadOnlyList<World> Worlds { get => _worlds; }

    public void Add(World world)
    {
        _worlds.Add(world);
        File.WriteAllText(_location, JsonConvert.SerializeObject(_worlds));
    }

    public void Remove(World world)
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
