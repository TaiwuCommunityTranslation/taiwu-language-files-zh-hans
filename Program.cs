using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

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
Console.WriteLine($"Using game directory: {gameDirectory}");

var managedAssembly = Path.Join(gameDirectory, "The Scroll of Taiwu_Data", "Managed", "Assembly-CSharp.dll");
if (!File.Exists(managedAssembly))
{
  Console.Error.WriteLine($"Invalid managed assembly: {managedAssembly}!");
  Environment.Exit(1);
}
Console.WriteLine($"Using managed assembly: {managedAssembly}");

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

Console.ReadKey();
