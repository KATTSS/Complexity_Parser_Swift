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
                if x < 100 {
                    print(""nested"")
                }
            } else {
                print(""small"")
            }

            switch y {
                case 1: print(""1"")
                case 2: print(""2"")
                case 3: print(""3"")
                default: print(""other"")
            }
        ";

        try 
        {
            var input = new AntlrInputStream(code);
            var lexer = new Swift5Lexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new Swift5Parser(tokens);
            var tree = parser.top_level(); 

            // Метрика Маккейба (уже была)
            var mccabeVisitor = new SwiftMcCabeVisitor();
            mccabeVisitor.Visit(tree); 
            Console.WriteLine($"Mc Cabe complexity: {mccabeVisitor.GetComplexity()}");

            // Метрика Джилба (новая)
            var gilbVisitor = new SwiftGilbVisitor();
            gilbVisitor.Visit(tree);
            gilbVisitor.DisplayMetrics();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}