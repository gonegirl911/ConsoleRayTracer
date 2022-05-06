using ConsoleRayTracer;
using System.Reflection;

static class WorldsManager
{
    private static Dictionary<string, World> _worlds;

    static WorldsManager()
    {
        JsonConvert.DefaultSettings = () => new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new ConstructorResolver(),
        };

        if (!File.Exists("worlds.json"))
        {
            _worlds = new() { { "Default", World.Default } };
            File.WriteAllText("worlds.json", JsonConvert.SerializeObject(_worlds));
        }
        else
        {
            _worlds = JsonConvert.DeserializeObject<Dictionary<string, World>>(File.ReadAllText("worlds.json"))!;
        }
    }

    public static World Get(string name) => _worlds[name];
    public static string[] Names() => _worlds.Keys.ToArray();
    public static void Start(string name) => _worlds[name].Start();
}

class ConstructorResolver : DefaultContractResolver
{
    private static readonly IEnumerable<Type> SYSTEM_TYPES = typeof(Assembly).Assembly.GetExportedTypes();

    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
        var ctor = objectType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .MaxBy(ctor => ctor.GetParameters().Length);

        var contract = base.CreateObjectContract(objectType);
        if (!SYSTEM_TYPES.Contains(objectType) && ctor is not null)
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
