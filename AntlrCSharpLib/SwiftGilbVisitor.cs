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

        public double Cl => TotalOperators == 0 ? 0 : (double)CL / TotalOperators;


        private void UpdateCLI()
        {
            if (_currentDepth > CLI)
            {
                CLI = _currentDepth;
            }
        }

        public override object VisitStatement([NotNull] Swift5Parser.StatementContext context)
        {
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
            if (!(context.Parent is Swift5Parser.StatementContext))
            {
                TotalOperators++;
            }
            return base.VisitDeclaration(context);
        }

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

        public override object VisitConditional_operator([NotNull] Swift5Parser.Conditional_operatorContext context)
        {
            CL++;
            UpdateCLI();
            
            _currentDepth++;
            var result = base.VisitConditional_operator(context);
            _currentDepth--;
            
            return result;
        }

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
            Console.WriteLine($"cl (Относительная сложность) = {Cl:F4}");
            Console.WriteLine($"CLI (Макс. вложенность) = {CLI}");
            Console.WriteLine($"Общее количество операторов = {TotalOperators}");
        }
    }
}