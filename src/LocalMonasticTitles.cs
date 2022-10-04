using ICSharpCode.Decompiler.CSharp.Syntax;

public class LocalMonasticTitles
{
  public static Dictionary<int, string> getIdToLocalMonasticTitleMapping(Decompiler decompiler)
  {
    var ast = decompiler.GetSyntaxTree("Config.LocalMonasticTitles");

    var namespaceDeclaration = (NamespaceDeclaration)ast.Children.First(node => node is NamespaceDeclaration);
    var typeDeclaration = (TypeDeclaration)namespaceDeclaration.Children.First(node => node is TypeDeclaration);
    var methodDeclaration = (MethodDeclaration)typeDeclaration.Children.First(node => node is MethodDeclaration && ((MethodDeclaration)node).Name == "Init");
    var blockStatement = (BlockStatement)methodDeclaration.Children.First(node => node is BlockStatement);
    var expressionStatement = (ExpressionStatement)blockStatement.Children.First(node => node is ExpressionStatement);
    var assignmentExpression = (AssignmentExpression)expressionStatement.Children.First(node => node is AssignmentExpression);
    var arrayCreateExpression = (ArrayCreateExpression)assignmentExpression.Children.First(node => node is ArrayCreateExpression);
    var arrayInitializer = (ArrayInitializerExpression)arrayCreateExpression.Children.First(node => node is ArrayInitializerExpression);

    return (arrayInitializer.Children.Where(node => node is ObjectCreateExpression).ToArray()).Aggregate(new Dictionary<int, string>(), (acc, node) =>
    {
      if (!(node is ObjectCreateExpression)) throw new Exception("invalid object create expression");
      var objectCreateExpression = (ObjectCreateExpression)node;
      var objectCreateExpressionChildren = objectCreateExpression.Children.ToArray();

      if (!(objectCreateExpressionChildren[1] is PrimitiveExpression)) throw new Exception("invalid key node");
      var keyNode = (PrimitiveExpression)objectCreateExpressionChildren[1];
      var key = (int)keyNode.Value;

      if (!(objectCreateExpressionChildren[2] is PrimitiveExpression)) throw new Exception("invalid val node");
      var valNode = (PrimitiveExpression)objectCreateExpressionChildren[2];
      var val = (string)valNode.Value;

      acc[key] = val;

      return acc;
    });
  }
}

