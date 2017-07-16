using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SetQuotedIdentifierRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "set-quoted"; } }
        public string RULE_TEXT { get { return "SET QUOTED_IDENTIFIER ON at top of file"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        private bool ErrorLogged;

        public SetQuotedIdentifierRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlScript node)
        {
            var childQuotedidentifierVisitor = new ChildQuotedidentifierVisitor();
            node.AcceptChildren(childQuotedidentifierVisitor);
            if (!childQuotedidentifierVisitor.QuotedIdentifierFound && !ErrorLogged)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
                ErrorLogged = true;
            }
        }

        public class ChildQuotedidentifierVisitor : TSqlFragmentVisitor
        {
            public bool QuotedIdentifierFound;

            public override void Visit(PredicateSetStatement node)
            {
                if (node.Options == SetOptions.QuotedIdentifier)
                {
                    QuotedIdentifierFound = true;
                }
            }
        }
    }
}