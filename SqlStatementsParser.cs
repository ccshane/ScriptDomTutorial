using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace ScriptDomBlogSample
{
    public class SqlStatementsParser
    {
        private TSqlParser _parser;
        private IList<ParseError> _errors;

        public DataContainer Parse(string text)
        {
            var container = new DataContainer();
            //begin the parser code
            _parser = new TSql100Parser(false);
            var reader = new StringReader(text);
            var statements = _parser.ParseStatementList(reader, out _errors);
            if (_errors.Count > 0)
            {
                container.AddErrors(_errors);
                return container;
            }
            foreach (var statement in statements.Statements)
            {
                if (statement is SelectStatement)
                {
                    var select = statement as SelectStatement;
                    var into = select.Into;
                    if (into != null)
                    {
                        var table = into.BaseIdentifier.Value;
                        container.AddTable(table, "");
                    }
                    if (select.QueryExpression is QuerySpecification)
                    {
                        var querySpec = select.QueryExpression as QuerySpecification;
                        ParseQuerySpecification(querySpec, container);
                    }
                    else if (select.QueryExpression is BinaryQueryExpression)
                    {
                        var binaryQuery = select.QueryExpression as BinaryQueryExpression;
                        ParseBinaryQueryExpression(binaryQuery, container);
                    }
                }
                else if (statement is InsertStatement)
                {
                    var insertStatement = statement as InsertStatement;
                    var insertSpec = insertStatement.InsertSpecification;
                    var target = insertSpec.Target;
                    if (target is NamedTableReference)
                    {
                        ParsedNamedTableReference(target, container);
                    }
                    foreach (var column in insertSpec.Columns)
                    {
                        ParseColumnReferenceExpression(column, container, "");
                    }
                }
                else if (statement is UpdateStatement)
                {
                    var updateStatement = statement as UpdateStatement;
                    var updateSpec = updateStatement.UpdateSpecification;
                    var target = updateSpec.Target;
                    if (target is NamedTableReference)
                    {
                        ParsedNamedTableReference(target, container);
                    }
                    foreach (var setClause in updateSpec.SetClauses)
                    {
                        if (setClause is AssignmentSetClause)
                        {
                            var assignment = setClause as AssignmentSetClause;
                            ParseColumnReferenceExpression(assignment.Column, container, "");
                        }
                    }
                    if (updateSpec.WhereClause != null)
                    {
                        ParseWhereClause(updateSpec.WhereClause, container);
                    }
                }
                else if (statement is DeleteStatement)
                {
                    var deleteStatement = statement as DeleteStatement;
                    var deleteSpec = deleteStatement.DeleteSpecification;
                    if (deleteSpec.TopRowFilter != null)
                    {
                        //check for top
                        container.IsTopSelection = true;
                        container.IsTopPercent = deleteSpec.TopRowFilter.Percent;
                        var expression = deleteSpec.TopRowFilter.Expression as ParenthesisExpression;
                        if (expression != null && expression.Expression is IntegerLiteral)
                        {
                            container.TopCount = Int32.Parse((expression.Expression as IntegerLiteral).Value);
                        }
                    }
                    var target = deleteSpec.Target;
                    if (target is NamedTableReference)
                    {
                        ParsedNamedTableReference(target, container);
                    }
                    if (deleteSpec.WhereClause != null)
                    {
                        ParseWhereClause(deleteSpec.WhereClause, container);
                    }
                }
                else if (statement is ExecuteStatement)
                {
                    var execute = statement as ExecuteStatement;
                    var executeSpec = execute.ExecuteSpecification;
                    if (executeSpec.ExecutableEntity is ExecutableProcedureReference)
                    {
                        var entity = executeSpec.ExecutableEntity as ExecutableProcedureReference;
                        var referenceName = entity.ProcedureReference;
                        var procedure = referenceName.ProcedureReference;
                        var name = procedure.Name;
                        var storedProcName = name.BaseIdentifier.Value;
                        container.AddTable(storedProcName, "");
                    }
                    var parameters = executeSpec.ExecutableEntity.Parameters;
                    foreach (var executeParameter in parameters)
                    {
                        var value = executeParameter.ParameterValue;
                    }
                }
            }
            //end parser code
            return container;
        }

        private void ParseBinaryQueryExpression(BinaryQueryExpression binaryQuery, DataContainer container)
        {
            var first = binaryQuery.FirstQueryExpression;
            if (first is QuerySpecification)
            {
                ParseQuerySpecification(first as QuerySpecification, container);
            }
            else if (first is BinaryQueryExpression)
            {
                ParseBinaryQueryExpression(first as BinaryQueryExpression, container);
            }
            var second = binaryQuery.SecondQueryExpression;
            if (second is QuerySpecification)
            {
                ParseQuerySpecification(second as QuerySpecification, container);
            }
            else if (second is BinaryQueryExpression)
            {
                ParseBinaryQueryExpression(second as BinaryQueryExpression, container);
            }
        }

        private void ParseQuerySpecification(QuerySpecification querySpec, DataContainer container)
        {
            //check for distinct
            container.IsDistinctSelection = querySpec.UniqueRowFilter.ToString().ToUpper().Equals("DISTINCT");
            if (querySpec.TopRowFilter != null)
            {
                //check for top
                container.IsTopSelection = true;
                container.IsTopPercent = querySpec.TopRowFilter.Percent;
                ScalarExpression expression;
                if (querySpec.TopRowFilter.Expression is ParenthesisExpression)
                {
                    expression = (querySpec.TopRowFilter.Expression as ParenthesisExpression).Expression;
                }
                else
                {
                    expression = querySpec.TopRowFilter.Expression;
                }
                if (expression is IntegerLiteral)
                {
                    container.TopCount = Int32.Parse((expression as IntegerLiteral).Value);
                }
            }
            ParseQueryExpression(querySpec, container);
        }

        private void ParseQueryExpression(QuerySpecification querySpec, DataContainer container)
        {
            if (querySpec.FromClause == null)
            {
                return;
            }
            var tables = querySpec.FromClause.TableReferences;
            foreach (var table in tables)
            {
                if (table is NamedTableReference)
                {
                    ParsedNamedTableReference(table, container);
                }
                else if (table is QualifiedJoin)
                {
                    var join = table as QualifiedJoin;
                    if (join.FirstTableReference is NamedTableReference)
                    {
                        ParsedNamedTableReference(join.FirstTableReference as NamedTableReference, container);
                    }
                    if (join.SecondTableReference is NamedTableReference)
                    {
                        ParsedNamedTableReference(join.SecondTableReference as NamedTableReference, container);
                    }
                    ParseBooleanExpression(join.SearchCondition, container);
                }
            }
            var columns = querySpec.SelectElements;
            foreach (var column in columns)
            {
                if (column is SelectStarExpression)
                {
                    continue;
                }
                if (column is SelectScalarExpression)
                {
                    var scalar = column as SelectScalarExpression;
                    var alias = scalar.ColumnName == null ? "" : scalar.ColumnName.Value;
                    var expression = scalar.Expression;
                    if (expression is ColumnReferenceExpression)
                    {
                        var columnExpression = expression as ColumnReferenceExpression;
                        ParseColumnReferenceExpression(columnExpression, container, alias);
                    }
                    else if (expression is FunctionCall)
                    {
                        var functionExpression = expression as FunctionCall;
                        ParseFunctionCall(functionExpression, container, alias);
                    }
                }
            }
            if (querySpec.WhereClause != null)
            {
                ParseWhereClause(querySpec.WhereClause, container);
            }
        }

        private void ParseFunctionCall(FunctionCall functionExpression, DataContainer container, string alias)
        {
            var parameters = functionExpression.Parameters;
            foreach (var parameter in parameters)
            {
                if (parameter is ColumnReferenceExpression)
                {
                    var columnExpression = parameter as ColumnReferenceExpression;
                    ParseColumnReferenceExpression(columnExpression, container, alias);
                }
                else if (parameter is FunctionCall)
                {
                    var function = parameter as FunctionCall;
                    ParseFunctionCall(function, container, alias);
                }
            }
        }

        private void ParsedNamedTableReference(TableReference table, DataContainer container)
        {
            var reference = table as NamedTableReference;
            var tableName = reference.SchemaObject.BaseIdentifier.Value;
            var alias = reference.Alias == null ? "" : reference.Alias.Value;
            container.AddTable(tableName, alias);
        }

        private void ParseColumnReferenceExpression(ColumnReferenceExpression columnExpression, DataContainer container,
            string alias)
        {
            var identifier = columnExpression.MultiPartIdentifier;
            if (identifier.Identifiers.Count == 1)
            {
                var columnName = identifier.Identifiers.First().Value;
                var tableName = "";
                var foundTables = container.GetTables();
                if (foundTables.Count == 1)
                {
                    tableName = foundTables[0].Name;
                }
                container.AddColumn(columnName, alias, tableName);
            }
            else
            {
                var tableRef = identifier.Identifiers.First().Value;
                string tableName = (from x in container.GetTables()
                                    where (x.Name.Equals(tableRef) || x.Alias.Equals(tableRef))
                                    select x).First().Name;
                var columnName = identifier.Identifiers.Skip(1).First().Value;
                container.AddColumn(columnName, alias, tableName);
            }
        }

        private void ParseWhereClause(WhereClause clause, DataContainer container)
        {
            var condition = clause.SearchCondition;
            if (condition == null)
            {
                return;
            }
            if (condition is BooleanParenthesisExpression)
            {
                ParseBooleanParenthesisExpression(condition as BooleanParenthesisExpression, container);
            }
            else if (condition is BooleanComparisonExpression)
            {
                ParseBooleanComparisonExpression(condition as BooleanComparisonExpression, container);
            }
            else if (condition is BooleanBinaryExpression)
            {
                ParseBooleanBinaryExpression(condition as BooleanBinaryExpression, container);
            }
            else if (condition is BooleanNotExpression)
            {
                ParseBooleanNotExpression(condition as BooleanNotExpression, container);
            }
            else if (condition is BooleanTernaryExpression)
            {
                ParseBooleanTernaryExpression(condition as BooleanTernaryExpression, container);
            }
            else if (condition is BooleanIsNullExpression)
            {
                ParseBooleanIsNullExpression(condition as BooleanIsNullExpression, container);
            }
            else if (condition is InPredicate)
            {
                ParseInPredicate(condition as InPredicate, container);
            }
            else if (condition is LikePredicate)
            {
                ParseLikePredicate(condition as LikePredicate, container);
            }
        }

        private void ParseLikePredicate(LikePredicate predicate, DataContainer container)
        {
            var initial = predicate.FirstExpression;
            ParseScalarExpression(initial, container);
            var compareTo = predicate.SecondExpression;
            ParseScalarExpression(compareTo, container);
            var escape = predicate.EscapeExpression;
            ParseScalarExpression(escape, container);
        }

        private void ParseInPredicate(InPredicate predicate, DataContainer container)
        {
            ParseScalarExpression(predicate.Expression, container);
            foreach (var scalarExpression in predicate.Values)
            {
                ParseScalarExpression(scalarExpression, container);
            }
            if (predicate.Subquery != null)
            {
                ParseScalarSubquery(predicate.Subquery, container);
            }
        }

        private void ParseScalarSubquery(ScalarSubquery subquery, DataContainer container)
        {
            var expression = subquery.QueryExpression as QuerySpecification;
            ParseQueryExpression(expression, container);
        }

        private void ParseBooleanIsNullExpression(BooleanIsNullExpression expression, DataContainer container)
        {
            if (expression.IsNot)
            {
                Debug.WriteLine("IS NOT NULL");
            }
            else
            {
                Debug.WriteLine("IS NULL");
            }
            ParseScalarExpression(expression.Expression, container);
        }

        private void ParseBooleanTernaryExpression(BooleanTernaryExpression expression, DataContainer container)
        {
            var initial = expression.FirstExpression;
            ParseScalarExpression(initial, container);
            var lower = expression.SecondExpression;
            ParseScalarExpression(lower, container);
            var higher = expression.ThirdExpression;
            ParseScalarExpression(higher, container);
        }

        private void ParseBooleanNotExpression(BooleanNotExpression expression, DataContainer container)
        {
            ParseBooleanExpression(expression.Expression, container);
        }

        private void ParseBooleanParenthesisExpression(BooleanParenthesisExpression expression, DataContainer container)
        {
            ParseBooleanExpression(expression.Expression, container);
        }

        private void ParseBooleanBinaryExpression(BooleanBinaryExpression expression, DataContainer container)
        {
            var left = expression.FirstExpression;
            ParseBooleanExpression(left, container);
            var right = expression.SecondExpression;
            ParseBooleanExpression(right, container);
        }

        private void ParseBooleanExpression(BooleanExpression expression, DataContainer container)
        {
            if (expression is BooleanComparisonExpression)
            {
                ParseBooleanComparisonExpression(expression as BooleanComparisonExpression, container);
            }
            else if (expression is BooleanBinaryExpression)
            {
                ParseBooleanBinaryExpression(expression as BooleanBinaryExpression, container);
            }
            else if (expression is BooleanParenthesisExpression)
            {
                ParseBooleanParenthesisExpression(expression as BooleanParenthesisExpression, container);
            }
            else if (expression is BooleanTernaryExpression)
            {
                ParseBooleanTernaryExpression(expression as BooleanTernaryExpression, container);
            }
            else if (expression is BooleanIsNullExpression)
            {
                ParseBooleanIsNullExpression(expression as BooleanIsNullExpression, container);
            }
            else if (expression is InPredicate)
            {
                ParseInPredicate(expression as InPredicate, container);
            }
            else if (expression is LikePredicate)
            {
                ParseLikePredicate(expression as LikePredicate, container);
            }
        }

        private void ParseBooleanComparisonExpression(BooleanComparisonExpression expression, DataContainer container)
        {
            var left = expression.FirstExpression;
            ParseScalarExpression(left, container);
            var right = expression.SecondExpression;
            ParseScalarExpression(right, container);
        }

        private void ParseScalarExpression(ScalarExpression expression, DataContainer container)
        {
            if (expression is ColumnReferenceExpression)
            {
                ParseColumnReferenceExpression(expression as ColumnReferenceExpression, container, "");
            }
            else if (expression is ScalarSubquery)
            {
                ParseScalarSubquery(expression as ScalarSubquery, container);
            }
        }
    }
}
