using System.Text.RegularExpressions;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using Kaitai;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

const string OUTPUT_DIR = "zh-hans";

var cliArgs = Environment.GetCommandLineArgs();
if (cliArgs.Length != 2)
{
  Console.Error.WriteLine("Error: game directory is missing, pass game directory as the first argument!");
  Environment.Exit(1);
}

var gameDirectory = cliArgs[1];
if (!Directory.Exists(gameDirectory))
{
  Console.Error.WriteLine($"Invalid game directory: {gameDirectory}!");
  Environment.Exit(1);
}

var managedAssembly = Path.Join(gameDirectory, "The Scroll of Taiwu_Data", "Managed", "Assembly-CSharp.dll");
if (!File.Exists(managedAssembly))
{
  Console.Error.WriteLine($"Invalid managed assembly: {managedAssembly}!");
  Environment.Exit(1);
}

var module = new PEFile(managedAssembly);
var resolver = new UniversalAssemblyResolver(managedAssembly, false, module.DetectTargetFrameworkId());

var settings = new DecompilerSettings(LanguageVersion.Latest)
{
  ThrowOnAssemblyResolveErrors = true,
};
var decompiler = new CSharpDecompiler(managedAssembly, resolver, settings);
var fullTypeName = new FullTypeName("LanguageKey");
var ast = decompiler.DecompileType(fullTypeName);

var typeDeclaration = (TypeDeclaration)ast.Children.First(node => node as TypeDeclaration != null);
var fieldDeclaration = (FieldDeclaration)typeDeclaration.Children.First(node => node is FieldDeclaration && (((FieldDeclaration)node).Modifiers & Modifiers.Private) != 0);
var variableInitializer = (VariableInitializer)fieldDeclaration.Children.First(node => node is VariableInitializer);
var objectCreateExpression = (ObjectCreateExpression)variableInitializer.Children.First(node => node is ObjectCreateExpression);
var arrayInitializer = (ArrayInitializerExpression)objectCreateExpression.Children.First(node => node is ArrayInitializerExpression);

var languageKeyToLineMapping = arrayInitializer.Children.Aggregate(new Dictionary<string, int>(), (acc, node) =>
{
  if (!(node is ArrayInitializerExpression)) throw new Exception("invalid array node");
  var arrayNode = (ArrayInitializerExpression)node;

  if (!(arrayNode.FirstChild is PrimitiveExpression)) throw new Exception("invalid key node");
  var keyNode = (PrimitiveExpression)arrayNode.FirstChild;
  var key = (string)keyNode.Value;

  if (!(arrayNode.LastChild is PrimitiveExpression)) throw new Exception("invalid val node");
  var valNode = (PrimitiveExpression)arrayNode.LastChild;
  var val = (int)valNode.Value;

  acc[key] = val;

  return acc;
});

var languageCnAssetBundle = Path.Join(gameDirectory, "The Scroll of Taiwu_Data", "GameResources", "language_cn.uab");
if (!File.Exists(languageCnAssetBundle))
{
  Console.Error.WriteLine($"Invalid language_cn.uab: {languageCnAssetBundle}!");
  Environment.Exit(1);
}

var uab = UnityBundle.FromFile(languageCnAssetBundle);
var uabData = uab.Blocks.Aggregate(new byte[0], (acc, block) =>
{
  return acc.Concat(block.Data).ToArray();
});

if (!Directory.Exists(OUTPUT_DIR)) Directory.CreateDirectory(OUTPUT_DIR);
uab.BlockInfoAndDirectory.Data.DirectoryInfo.ForEach(directoryInfo =>
{
  var assetData = new byte[directoryInfo.Size];
  Array.Copy(uabData, directoryInfo.Offset, assetData, 0, directoryInfo.Size);

  var asset = new Assets(new KaitaiStream(assetData));
  asset.Metadata.Objects.Data.ForEach((obj) =>
  {
    if (obj.ClassId != Assets.ClassId.UnityTextAsset3acc9e530e323df61040bf9358a9076c109 &&
        obj.ClassId != Assets.ClassId.UnityTextAsset3acc9e530e323df61040bf9358a9076c49)
    {
      return;
    }

    var data = (Assets.UnityTextAsset3acc9e530e323df61040bf9358a9076c)obj.Data;

    var name = data.MName.Data;
    var text = data.MScript.Data;

    Console.WriteLine($"[+] saving {name}...");
    if (name == "ui_language")
    {
      var lines = text.Split("\n");
      var entries = languageKeyToLineMapping.Aggregate(new Dictionary<string, string>(), (acc, pair) =>
      {
        acc[pair.Key] = lines[pair.Value];

        return acc;
      });
      File.WriteAllText(Path.Join(OUTPUT_DIR, $"{name}.json"), JsonConvert.SerializeObject(entries, Formatting.Indented));

      return;
    }

    File.WriteAllText(Path.Join(OUTPUT_DIR, $"{name}.txt"), text);
  });
});

var eventsDirectory = Path.Join(gameDirectory, "Event", "EventLanguages");
if (!Directory.Exists(eventsDirectory))
{
  Console.Error.WriteLine($"Invalid events directory: {eventsDirectory}!");
  Environment.Exit(1);
}

Console.WriteLine("[+] saving EventLanguages...");
var eventsList = new SortedDictionary<string, Dictionary<string, string>>();

var yamlDeserializer = new Deserializer();

var eventFiles = Directory.GetFiles(eventsDirectory);
Array.ForEach(eventFiles, eventFile =>
{
  Console.WriteLine($"  [+] processing {Path.GetFileName(eventFile)}...");

  var data = File.ReadAllText(eventFile);

  var yaml = data.Replace("\r", "").Replace("\v", @"\v");
  yaml = Regex.Replace(yaml, @"^- (Group|GroupName|Language) :.*$", "", RegexOptions.Multiline);
  yaml = Regex.Replace(yaml, @"^- EventGuid : (.+)$", "GUID-$1:", RegexOptions.Multiline);
  yaml = Regex.Replace(yaml, @"^\t+--? ([^ ]+) : (.*)$", @"  $1: ""$2""", RegexOptions.Multiline);

  var events = yamlDeserializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(yaml);
  Array.ForEach(events.ToArray(), eventPair => eventsList[eventPair.Key] = eventPair.Value);
});

var eventLanguages = new Dictionary<string, string>();
Array.ForEach(eventsList.ToArray(), eventPair =>
{
  var prefix = eventPair.Key;
  Array.ForEach(eventPair.Value.ToArray(), entry =>
  {
    var key = $"{prefix}-{entry.Key}";
    eventLanguages[key] = entry.Value;
  });
});

File.WriteAllText(Path.Join(OUTPUT_DIR, $"event_language.json"), JsonConvert.SerializeObject(eventLanguages, Formatting.Indented));
