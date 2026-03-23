using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic; 

namespace AntlrCSharpLib
{
    public class SwiftGilbVisitor : Swift5ParserBaseVisitor<object>
    {
        private int _cl = 0;
        private int _totalOperators = 0;
        private int _maxNesting = 0;
        private int _currentNesting = 0;

        public int CL => _cl;
        public double Cl => _totalOperators > 0 ? (double)_cl / _totalOperators : 0;
        public int CLI => _maxNesting;
        public int TotalOperators => _totalOperators;

        private void UpdateNesting(int additionalNesting = 1)
        {   PrintMetrics("UpdateNesting");
            _currentNesting += additionalNesting;
            if (_currentNesting - 1 > _maxNesting)
                 _maxNesting = _currentNesting - 1;
                //_maxNesting = _currentNesting;
            PrintMetrics("UpdateNesting (after update)");
        }

        public override object VisitIf_statement([NotNull] Swift5Parser.If_statementContext context)
        {
            PrintMetrics("VisitIf_statement");
            _cl++;
            _totalOperators++;
            UpdateNesting();
            var result = base.VisitIf_statement(context);
            _currentNesting--;
            return result;
        }

        public override object VisitGuard_statement([NotNull] Swift5Parser.Guard_statementContext context)
        {
            PrintMetrics("VisitGuard_statement");
            _cl++;
            _totalOperators++;
            UpdateNesting();
            var result = base.VisitGuard_statement(context);
            _currentNesting--;
            return result;
        }

        public override object VisitConditional_operator([NotNull] Swift5Parser.Conditional_operatorContext context)
        {
            PrintMetrics("VisitConditional_operator");
            _cl++;
            _totalOperators++;
            return base.VisitConditional_operator(context);
        }

        private static List<Swift5Parser.Switch_caseContext> CollectSwitchCases(Swift5Parser.Switch_casesContext casesContext)
        {
            var cases = new List<Swift5Parser.Switch_caseContext>();
            var current = casesContext;
            while (current != null)
            {
                var switchCase = current.switch_case();
                if (switchCase != null)
                    cases.Add(switchCase);

                current = current.switch_cases();
            }
            return cases;
        }

        public override object VisitSwitch_statement([NotNull] Swift5Parser.Switch_statementContext context)
        {
            PrintMetrics("VisitSwitch_statement");
            var switchCasesContext = context.switch_cases();
            var cases = switchCasesContext != null ? CollectSwitchCases(switchCasesContext) : new List<Swift5Parser.Switch_caseContext>();
            int n = cases.Count;
            Console.WriteLine($"Switch cases count: {n}");

            if (n > 0)
            {
                _cl += (n - 1);
                _totalOperators++; 
                PrintMetrics("VisitSwitch_statement (before nesting update)");
                // int switchEquivalentNesting = Math.Max(0, n - 2);
                 int switchEquivalentNesting = Math.Max(0, n - 1);
                UpdateNesting(switchEquivalentNesting);
                var result = base.VisitSwitch_statement(context);
                _currentNesting -= switchEquivalentNesting;
                PrintMetrics("VisitSwitch_statement (with cases)");
                return result;
            }
            PrintMetrics("VisitSwitch_statement (no cases)");
            return base.VisitSwitch_statement(context);
        }

        public override object VisitAssignment_operator([NotNull] Swift5Parser.Assignment_operatorContext context)
        {
            PrintMetrics("VisitAssignment_operator");
            _totalOperators++;
            return base.VisitAssignment_operator(context);
        }

        public override object VisitOperator([NotNull] Swift5Parser.OperatorContext context)
        {
            PrintMetrics("VisitOperator");
            _totalOperators++;
            return base.VisitOperator(context);
        }

        public void DisplayMetrics()
        {
            Console.WriteLine("--- Метрика Джилба ---");
            Console.WriteLine($"CL (Абсолютная сложность) = {CL}");
            Console.WriteLine($"cl (Относительная сложность) = {Cl:F4}");
            Console.WriteLine($"CLI (Макс. вложенность) = {CLI}");
            Console.WriteLine($"Общее количество операторов = {_totalOperators}");
        }

        private void PrintMetrics(string name)
        {
            Console.WriteLine($"Method {name}: CL = {CL}, cl = {Cl:F4}, CLI = {CLI}");
        }
    }
}