using RayTracer;
using System.Reflection;

namespace App;

public partial class Home : Form
{
    private const string LOCATION = "worlds.json";

    private readonly List<World> worlds;

    public Home()
    {
        JsonConvert.DefaultSettings = () => new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new ConstructorResolver(),
        };

        if (!File.Exists(LOCATION))
        {
            File.WriteAllText(LOCATION, JsonConvert.SerializeObject(new List<World>() { { World.Default } }));
        }

        worlds = JsonConvert.DeserializeObject<List<World>>(File.ReadAllText(LOCATION))!;
        InitializeComponent();
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        new Thread(worlds[0].Start).Start();
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
