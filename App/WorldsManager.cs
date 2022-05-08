using RayTracer;
using System.Reflection;

namespace App;

static class WorldsManager
{
    private const string LOCATION = "worlds.json";

    private static readonly Dictionary<string, World> _worlds;

    static WorldsManager()
    {
        JsonConvert.DefaultSettings = () => new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new ConstructorResolver(),
        };

        if (!File.Exists(LOCATION))
        {
            File.WriteAllText(LOCATION, JsonConvert.SerializeObject(new Dictionary<string, World>() { { "Default", World.Default } }));
        }
        _worlds = JsonConvert.DeserializeObject<Dictionary<string, World>>(File.ReadAllText(LOCATION))!;
    }
    
    public static IReadOnlyDictionary<string, World> Worlds { get => _worlds; }

    public static void Add(string name, World world)
    {
        _worlds.Add(name, world);
        File.WriteAllText(LOCATION, JsonConvert.SerializeObject(_worlds));
    }

    public static void Remove(string name)
    {
        _worlds.Remove(name);
        File.WriteAllText(LOCATION, JsonConvert.SerializeObject(_worlds));
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
