using ICSharpCode.Decompiler.CSharp.Syntax;

public class LanguageKey
{
  public static Dictionary<string, int> getLanguageKeyToLineNumberMapping(Decompiler decompiler)
  {
    var ast = decompiler.GetSyntaxTree("LanguageKey");

    var typeDeclaration = (TypeDeclaration)ast.Children.First(node => node as TypeDeclaration != null);
    var fieldDeclaration = (FieldDeclaration)typeDeclaration.Children.First(node => node is FieldDeclaration && (((FieldDeclaration)node).Modifiers & Modifiers.Private) != 0);
    var variableInitializer = (VariableInitializer)fieldDeclaration.Children.First(node => node is VariableInitializer);
    var objectCreateExpression = (ObjectCreateExpression)variableInitializer.Children.First(node => node is ObjectCreateExpression);
    var arrayInitializer = (ArrayInitializerExpression)objectCreateExpression.Children.First(node => node is ArrayInitializerExpression);

    return arrayInitializer.Children.Aggregate(new Dictionary<string, int>(), (acc, node) =>
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
  }
}
