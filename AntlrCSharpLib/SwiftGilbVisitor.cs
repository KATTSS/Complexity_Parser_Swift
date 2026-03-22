using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic; // Важно для коллекций

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

        private void UpdateNesting(int additionalNesting = 1)
        {
            _currentNesting += additionalNesting;
            if (_currentNesting > _maxNesting)
                _maxNesting = _currentNesting;
        }

        public override object VisitIf_statement([NotNull] Swift5Parser.If_statementContext context)
        {
            _cl++;
            _totalOperators++;
            UpdateNesting();
            var result = base.VisitIf_statement(context);
            _currentNesting--;
            return result;
        }

        public override object VisitGuard_statement([NotNull] Swift5Parser.Guard_statementContext context)
        {
            _cl++;
            _totalOperators++;
            UpdateNesting();
            var result = base.VisitGuard_statement(context);
            _currentNesting--;
            return result;
        }

        public override object VisitConditional_operator([NotNull] Swift5Parser.Conditional_operatorContext context)
        {
            _cl++;
            _totalOperators++;
            return base.VisitConditional_operator(context);
        }

        public override object VisitSwitch_statement([NotNull] Swift5Parser.Switch_statementContext context)
        {
            var cases = context.GetRuleContexts<Swift5Parser.Switch_caseContext>();
            int n = cases != null ? cases.Count() : 0;
            
            if (n > 0)
            {
                _cl += (n - 1);
                _totalOperators++; 

                int switchEquivalentNesting = Math.Max(0, n - 2);
                UpdateNesting(switchEquivalentNesting);
                var result = base.VisitSwitch_statement(context);
                _currentNesting -= switchEquivalentNesting;
                return result;
            }
            return base.VisitSwitch_statement(context);
        }

        public override object VisitAssignment_operator([NotNull] Swift5Parser.Assignment_operatorContext context)
        {
            _totalOperators++;
            return base.VisitAssignment_operator(context);
        }

        public override object VisitOperator([NotNull] Swift5Parser.OperatorContext context)
        {
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
    }
}