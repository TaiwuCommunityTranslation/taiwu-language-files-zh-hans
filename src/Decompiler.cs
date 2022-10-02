using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

public class Decompiler
{
  private CSharpDecompiler decompiler;

  public Decompiler(string assemblyPath)
  {
    var module = new PEFile(assemblyPath);
    var resolver = new UniversalAssemblyResolver(assemblyPath, false, module.DetectTargetFrameworkId());

    var settings = new DecompilerSettings(LanguageVersion.Latest)
    {
      ThrowOnAssemblyResolveErrors = true,
    };
    decompiler = new CSharpDecompiler(assemblyPath, resolver, settings);
  }

  public SyntaxTree GetSyntaxTree(string fullTypeName)
  {
    return decompiler.DecompileType(new FullTypeName(fullTypeName));
  }
}
