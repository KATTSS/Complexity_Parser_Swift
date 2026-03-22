using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;

namespace AntlrCSharpLib
{
   
    public class SwiftMcCabeVisitor : Swift5ParserBaseVisitor<object>
    {
        private int _complexity = 1;

        public int Complexity => _complexity;

        public override object VisitIf_statement([NotNull] Swift5Parser.If_statementContext context)
        {
            _complexity++;
            return base.VisitIf_statement(context);
        }

        public override object VisitWhile_statement([NotNull] Swift5Parser.While_statementContext context)
        {
            _complexity++;
            return base.VisitWhile_statement(context);
        }

        public override object VisitFor_in_statement([NotNull] Swift5Parser.For_in_statementContext context)
        {
            _complexity++;
            return base.VisitFor_in_statement(context);
        }

        public override object VisitRepeat_while_statement([NotNull] Swift5Parser.Repeat_while_statementContext context)
        {
            _complexity++;
            return base.VisitRepeat_while_statement(context);
        }
        public override object VisitGuard_statement([NotNull] Swift5Parser.Guard_statementContext context)
        {
            _complexity++;
            return base.VisitGuard_statement(context);
        }

        public override object VisitSwitch_statement([NotNull] Swift5Parser.Switch_statementContext context)
        {
            int before = _complexity;
            base.VisitSwitch_statement(context);
         
            if (_complexity > before)
            {
                _complexity--;
            }
            return null;
        }

        public override object VisitSwitch_case([NotNull] Swift5Parser.Switch_caseContext context)
        {
            _complexity++;
            return base.VisitSwitch_case(context);
        }

        public override object VisitDefault_label([NotNull] Swift5Parser.Default_labelContext context)
        {
            _complexity++;
            return base.VisitDefault_label(context);
        }

        public override object VisitCatch_clause([NotNull] Swift5Parser.Catch_clauseContext context)
        {
            _complexity++;
            return base.VisitCatch_clause(context);
        }

        public override object VisitConditional_operator([NotNull] Swift5Parser.Conditional_operatorContext context)
        {
            _complexity++;
            return base.VisitConditional_operator(context);
        }
        public override object VisitOperator([NotNull] Swift5Parser.OperatorContext context)
        {
            string op = context.GetText();
            if (op == "&&" || op == "||" || op == "??")
            {
                _complexity++;
            }
            return base.VisitOperator(context);
        }
        public override object VisitWhere_clause([NotNull] Swift5Parser.Where_clauseContext context)
        {
            _complexity++;
            return base.VisitWhere_clause(context);
        }

        public void DisplayMetrics()
        {
            Console.WriteLine("--- Метрика Маккейба ---");
            Console.WriteLine($"Цикломатическая сложность V(G) = {_complexity}");
        }

        public int GetComplexity() => _complexity;
    }
}