using AntlrCSharpLib;
using Antlr4.Runtime;
using System;

class Program 
{
    static void Main() 
    {
        string code = @"
            let x = 10 + 20
            let y = x * 5
            if y > 50 {
                print(""large"")
            } else {
                print(""small"")
            }

            if y+x>30 {
            print(""hello"")}
            else{
            print(""bye"")}
        ";

       
        try 
        {
            var input = new AntlrInputStream(code);
            var lexer = new Swift5Lexer(input);
            var tokens = new CommonTokenStream(lexer);

            var parser = new Swift5Parser(tokens);
            var tree = parser.top_level(); 
            var mccabeVisitor = new SwiftMcCabeVisitor();
            mccabeVisitor.Visit(tree); 
            var mcCabeComplexity = mccabeVisitor.GetComplexity();
            Console.WriteLine($"Mc Cabe complexity: {mcCabeComplexity}");
           // mccabeVisitor.DisplayMetrics(); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}