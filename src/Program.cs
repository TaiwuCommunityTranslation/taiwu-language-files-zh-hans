using System.Text.RegularExpressions;
using Kaitai;
using Newtonsoft.Json;
using ToolGood.Words;

const string OUTPUT_DIR = "zh-hans";
const string OUTPUT_TRANSLATION_DIR = "Translations/en";

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


var decompiler = new Decompiler(managedAssembly);
var languageKeyToLineMapping = LanguageKey.getLanguageKeyToLineNumberMapping(decompiler);

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

if (!Directory.Exists(OUTPUT_TRANSLATION_DIR)) Directory.CreateDirectory(OUTPUT_TRANSLATION_DIR);

var allPinyin = new Dictionary<int, string[]>();

Console.WriteLine("[+] saving LocalMonasticTitles...");
var idToLocalMonasticTitleMapping = LocalMonasticTitles.getIdToLocalMonasticTitleMapping(decompiler);
var idToLocalMonasticTitleTranslatedMapping = idToLocalMonasticTitleMapping.Aggregate(new Dictionary<int, string>(), (acc, pair) =>
{
  var vals = WordsHelper.GetAllPinyin(pair.Value[0], true).FindAll(val => !Regex.IsMatch(val, @"^[A-z]*$")).ToArray();
  if (vals.Length > 1)
  {
    var all = new string[] { pair.Value };
    allPinyin[pair.Key] = all.Concat(vals).ToArray();
  }

  var val = WordsHelper.GetPinyinForName(pair.Value, true);

  // apply fixes by contexts
  switch (pair.Value)
  {
    // 了却
    case "了": val = "Liǎo"; break;
    // 德行
    case "行": val = "Xíng"; break;
    // 传承
    case "传": val = "Chuán"; break;
    // 九重天
    case "重": val = "Chóng"; break;
    // 能够
    case "能": val = "Néng"; break;
    // 纪律
    case "纪": val = "Jì"; break;
    // 为了
    case "为": val = "Wèi"; break;
  }
  acc[pair.Key] = val;

  return acc;
});
File.WriteAllText(Path.Join(OUTPUT_DIR, "LocalMonasticTitles.json"), JsonConvert.SerializeObject(idToLocalMonasticTitleMapping, Formatting.Indented));
File.WriteAllText(Path.Join(OUTPUT_TRANSLATION_DIR, "LocalMonasticTitles.json"), JsonConvert.SerializeObject(idToLocalMonasticTitleTranslatedMapping, Formatting.Indented));
File.WriteAllText(Path.Join(OUTPUT_DIR, "XXX.json"), JsonConvert.SerializeObject(allPinyin, Formatting.Indented));

Console.WriteLine("[+] saving EventLanguages...");

DirectoryInfo d = new DirectoryInfo(eventsDirectory); //Assuming Test is your Folder
Console.WriteLine("Loading files");
Dictionary<string, FileInfo> files = d.GetFiles("*.txt").ToDictionary(file => file.Name); //Getting Text files
Console.WriteLine("Generating Templates");
Dictionary<string, TaiWuTemplate> parsedTemplates = files.ToDictionary(f => f.Key, f => new TaiWuTemplate(f.Value));

Dictionary<string, string> flatDict = parsedTemplates.Values
    .ToList()
    .SelectMany(template => template.FlattenTemplateToDict())
    .ToDictionary(pair => pair.Key, pair => pair.Value);
File.WriteAllText(Path.Join(OUTPUT_DIR, "events.json"), JsonConvert.SerializeObject(flatDict, Formatting.Indented));
