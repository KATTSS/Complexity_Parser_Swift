using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;

namespace AntlrCSharpLib
{
    public class SwiftGilbVisitor : Swift5ParserBaseVisitor<object>
    {
        public int CL { get; private set; } = 0;
        public int TotalOperators { get; private set; } = 0;
        public int CLI { get; private set; } = 0;
        private int _currentDepth = 0;

        public double cl => TotalOperators == 0 ? 0 : (double)CL / TotalOperators;

        // Метод для централизованного обновления CLI
        private void UpdateCLI()
        {
            if (_currentDepth > CLI)
            {
                CLI = _currentDepth;
            }
        }

        // --- Учет общего количества операторов ---
        public override object VisitStatement([NotNull] Swift5Parser.StatementContext context)
        {
            // Исключаем из общего числа операторов сами управляющие конструкции
            bool isControlStructure = 
                (context.branch_statement()?.if_statement() != null) ||
                (context.branch_statement()?.switch_statement() != null) ||
                (context.branch_statement()?.guard_statement() != null) ||
                (context.loop_statement()?.for_in_statement() != null) ||
                (context.loop_statement()?.while_statement() != null) ||
                (context.loop_statement()?.repeat_while_statement() != null);

            if (!isControlStructure)
            {
                TotalOperators++;
            }
            return base.VisitStatement(context);
        }

        public override object VisitDeclaration([NotNull] Swift5Parser.DeclarationContext context)
        {
            // Объявления считаем как операторы, если они не являются частью стейтмента (предотвращение двойного счета)
            if (!(context.Parent is Swift5Parser.StatementContext))
            {
                TotalOperators++;
            }
            return base.VisitDeclaration(context);
        }

        // --- Обработка условий и циклов ---

        public override object VisitIf_statement([NotNull] Swift5Parser.If_statementContext context)
        {
            CL++;
            UpdateCLI();

            _currentDepth++;
            var result = base.VisitIf_statement(context);
            _currentDepth--;

            return result;
        }

        public override object VisitGuard_statement([NotNull] Swift5Parser.Guard_statementContext context)
        {
            CL++;
            UpdateCLI();

            // Guard специфичен: его "тело" (else) выполняется только при провале условия.
            // Но логически это ветвление, увеличивающее сложность.
            _currentDepth++;
            var result = base.VisitGuard_statement(context);
            _currentDepth--;

            return result;
        }

        public override object VisitSwitch_statement([NotNull] Swift5Parser.Switch_statementContext context)
        {
            var cases = CollectSwitchCases(context.switch_cases());
            int n = cases.Count;

            if (n > 0)
            {
                CL += (n - 1);
                
                // CLI для switch: согласно логике эквивалентности if-else-if, 
                // каждый последующий кейс находится глубже предыдущего.
                for (int i = 0; i < n; i++)
                {
                    int savedDepth = _currentDepth;
                    _currentDepth += i; 
                    UpdateCLI();
                    
                    VisitSwitch_case(cases[i]);
                    
                    _currentDepth = savedDepth;
                }
                return null; 
            }
            return base.VisitSwitch_statement(context);
        }

        // Циклы: For, While, Repeat-While
        public override object VisitFor_in_statement([NotNull] Swift5Parser.For_in_statementContext context)
        {
            CL++;
            UpdateCLI();
            _currentDepth++;
            var result = base.VisitFor_in_statement(context);
            _currentDepth--;
            return result;
        }

        public override object VisitWhile_statement([NotNull] Swift5Parser.While_statementContext context)
        {
            CL++;
            UpdateCLI();
            _currentDepth++;
            var result = base.VisitWhile_statement(context);
            _currentDepth--;
            return result;
        }

        public override object VisitRepeat_while_statement([NotNull] Swift5Parser.Repeat_while_statementContext context)
        {
            CL++;
            UpdateCLI();
            _currentDepth++;
            var result = base.VisitRepeat_while_statement(context);
            _currentDepth--;
            return result;
        }

        // Тернарный оператор (condition ? expr1 : expr2)
        public override object VisitConditional_operator([NotNull] Swift5Parser.Conditional_operatorContext context)
        {
            CL++;
            UpdateCLI();
            
            // Тернарный оператор создает краткосрочное ветвление
            _currentDepth++;
            var result = base.VisitConditional_operator(context);
            _currentDepth--;
            
            return result;
        }

        // Вспомогательный метод для сбора кейсов
        private static List<Swift5Parser.Switch_caseContext> CollectSwitchCases(Swift5Parser.Switch_casesContext casesContext)
        {
            var cases = new List<Swift5Parser.Switch_caseContext>();
            if (casesContext == null) return cases;

            var current = casesContext;
            while (current != null)
            {
                var switchCase = current.switch_case();
                if (switchCase != null) cases.Add(switchCase);
                current = current.switch_cases();
            }
            return cases;
        }

        public void DisplayMetrics()
        {
            Console.WriteLine("--- Метрика Джилба ---");
            Console.WriteLine($"CL (Абсолютная сложность) = {CL}");
            Console.WriteLine($"cl (Относительная сложность) = {cl:F4}");
            Console.WriteLine($"CLI (Макс. вложенность) = {CLI}");
            Console.WriteLine($"Общее количество операторов = {TotalOperators}");
        }
    }
}
// using Antlr4.Runtime.Misc;
// using Antlr4.Runtime.Tree;
// using System;
// using System.Collections.Generic; 

// namespace AntlrCSharpLib
// {
//     public class SwiftGilbVisitor : Swift5ParserBaseVisitor<object>
//     {
//         private int _cl = 0;
//         private int _totalOperators = 0;
//         private int _maxNesting = 0;
//         private int _currentNesting = 0;

//         public int CL => _cl;
//         public double Cl => _totalOperators > 0 ? (double)_cl / _totalOperators : 0;
//         public int CLI => _maxNesting;

//         private void UpdateNesting(int additionalNesting = 1)
//         {  // PrintMetrics("UpdateNesting");
//             _currentNesting += additionalNesting;
//             if (_currentNesting - 1 > _maxNesting)
//                  _maxNesting = _currentNesting - 1;
//                 //_maxNesting = _currentNesting;
//           //  PrintMetrics("UpdateNesting (after update)");
//         }

//         public override object VisitIf_statement([NotNull] Swift5Parser.If_statementContext context)
//         {
//             PrintMetrics("VisitIf_statement");
//             _cl++;
//             UpdateNesting();
//             var result = base.VisitIf_statement(context);
//             _currentNesting--;
//             return result;
//         }

//         public override object VisitGuard_statement([NotNull] Swift5Parser.Guard_statementContext context)
//         {
//             PrintMetrics("VisitGuard_statement");
//             _cl++;
//             UpdateNesting();
//             var result = base.VisitGuard_statement(context);
//             _currentNesting--;
//             return result;
//         }

//         public override object VisitConditional_operator([NotNull] Swift5Parser.Conditional_operatorContext context)
//         {
//             PrintMetrics("VisitConditional_operator");
//             _cl++;
//             // conditional operator is part of expression, not a separate statement
//             return base.VisitConditional_operator(context);
//         }

//         private static List<Swift5Parser.Switch_caseContext> CollectSwitchCases(Swift5Parser.Switch_casesContext casesContext)
//         {
//             var cases = new List<Swift5Parser.Switch_caseContext>();
//             var current = casesContext;
//             while (current != null)
//             {
//                 var switchCase = current.switch_case();
//                 if (switchCase != null)
//                     cases.Add(switchCase);

//                 current = current.switch_cases();
//             }
//             return cases;
//         }

//         public override object VisitSwitch_statement([NotNull] Swift5Parser.Switch_statementContext context)
//         {
//             PrintMetrics("VisitSwitch_statement");
//             var switchCasesContext = context.switch_cases();
//             var cases = switchCasesContext != null ? CollectSwitchCases(switchCasesContext) : new List<Swift5Parser.Switch_caseContext>();
//             int n = cases.Count;
//             Console.WriteLine($"Switch cases count: {n}");

//             if (n > 0)
//             {
//                 _cl += (n - 1);
//                 PrintMetrics("VisitSwitch_statement (before nesting update)");
//                 int switchEquivalentNesting = Math.Max(0, n - 1);
//                 UpdateNesting(switchEquivalentNesting);
//                 var result = base.VisitSwitch_statement(context);
//                 _currentNesting -= switchEquivalentNesting;
//                 PrintMetrics("VisitSwitch_statement (with cases)");
//                 return result;
//             }
//             PrintMetrics("VisitSwitch_statement (no cases)");
//             return base.VisitSwitch_statement(context);
//         }

//         public override object VisitStatement([NotNull] Swift5Parser.StatementContext context)
//         {
//             PrintMetrics("VisitStatement");
//             _totalOperators++;
//             return base.VisitStatement(context);
//         }

//         public override object VisitAssignment_operator([NotNull] Swift5Parser.Assignment_operatorContext context)
//         {
//            // PrintMetrics("VisitAssignment_operator");
//             // Assignment in expression is part of a higher-level statement, not separate operator count
//             return base.VisitAssignment_operator(context);
//         }

//         public override object VisitOperator([NotNull] Swift5Parser.OperatorContext context)
//         {
//            // PrintMetrics("VisitOperator");
//             // Mathematical/logical operators no longer increment _totalOperators directly
//             return base.VisitOperator(context);
//         }

//         public void DisplayMetrics()
//         {
//             Console.WriteLine("--- Метрика Джилба ---");
//             Console.WriteLine($"CL (Абсолютная сложность) = {CL}");
//             Console.WriteLine($"cl (Относительная сложность) = {Cl:F4}");
//             Console.WriteLine($"CLI (Макс. вложенность) = {CLI}");
//             Console.WriteLine($"Общее количество операторов = {_totalOperators}");
//         }

//         private void PrintMetrics(string name)
//         {
//             Console.WriteLine($"Method {name}: CL = {CL}, cl = {Cl:F4}, CLI = {CLI}");
//         }
//     }
// }