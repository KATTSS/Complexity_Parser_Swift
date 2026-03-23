using AntlrCSharpLib;
using Antlr4.Runtime;
using System;

class Program 
{
    static void Main() 
    {
        // string code ="func checkNestedConditions(y: Int, x: Int) -> String {    if y > 50 {        if x < 100 {            return 5        } else {            return 4        }    } else {        return 1     }}";
        // string code = "func processValue(_ value: Int) -> String {    switch value {    case 0:        return 5    case 1...10:        if value % 2 == 0 {            return 4        } else {            return 3        }    case 11...100:        if value < 50 {            return 2        } else {            return 1       }    default:       return 0    }}";   
         string code = "switch x {case 1: y=1 case 2: y=2 case 3:y=4 case 4: y=4 default: y=5}";
        try 
        {
            var input = new AntlrInputStream(code);
            var lexer = new Swift5Lexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new Swift5Parser(tokens);
            var tree = parser.top_level(); 

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