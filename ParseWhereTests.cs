﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptDomDemo;

namespace ScriptDomBlogSampleTests
{
    /// <summary>
    /// Summary description for ParseWQhereTests
    /// </summary>
    [TestClass]
    public class ParseWhereTests
    {
        private SqlStatementsParser _parser;

        [TestInitialize]
        public void SetUp()
        {
            _parser = new SqlStatementsParser();
        }

        [TestCleanup]
        public void TearDown()
        {
            _parser = null;
        }

        [TestMethod]
        public void ParseSimpleAssignmentWhereClauseAddsColumn()
        {
            const string sql = "SELECT * FROM table_name WHERE column_name=5";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column_name", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseAndedWhereClauseAddsBothColumns()
        {
            const string sql = "SELECT * FROM table_name WHERE column0=5 AND column1 = 6";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseParenthesizedAndedWhereClauseAddsBothColumns()
        {
            const string sql = "SELECT * FROM table_name WHERE (column0=5 AND column1 = 6)";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseOredWhereClauseAddsBothColumns()
        {
            const string sql = "SELECT * FROM table_name WHERE column0=5 OR column1 = 6";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseParenthesizedOredWhereClauseAddsBothColumns()
        {
            const string sql = "SELECT * FROM table_name WHERE (column0=5 OR column1 = 6)";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseNestParenthesizedAndsOrsWhereClauseAddsAllColumns()
        {
            const string sql = "SELECT * FROM table_name WHERE ((column0=5 OR column1 = 6) AND column2=7)";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(3, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
            Assert.AreEqual("column2", columns[2].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseNotWhereClauseAddsColumn()
        {
            const string sql = "SELECT * FROM table_name WHERE NOT (column0=5)";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseBetweenWhereClauseAddsColumn()
        {
            const string sql = "SELECT * FROM table_name WHERE column0 BETWEEN 1 AND 10";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseNotBetweenWhereClauseAddsColumn()
        {
            const string sql = "SELECT * FROM table_name WHERE column0 NOT BETWEEN 1 AND 10";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseDifferentNotBetweenWhereClauseAddsColumn()
        {
            const string sql = "SELECT * FROM table_name WHERE NOT column0 BETWEEN 1 AND 10";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseIsNullWhereClauseAddsColumn()
        {
            const string sql = "SELECT * FROM table_name WHERE column0 IS NULL";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseIsNotNullWhereClauseAddsColumn()
        {
            const string sql = "SELECT * FROM table_name WHERE column0 IS NOT NULL";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseNotIsNullWhereClauseAddsColumn()
        {
            const string sql = "SELECT * FROM table_name WHERE NOT column0 IS NULL";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseInWhereClauseAddsColumn()
        {
            const string sql = "SELECT * FROM table_name WHERE column0 IN (1,2,3)";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseNotXInWhereClauseAddsColumn()
        {
            const string sql = "SELECT * FROM table_name WHERE NOT column0 IN (1,2,3)";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseXNotInWhereClauseAddsColumn()
        {
            const string sql = "SELECT * FROM table_name WHERE column0 NOT IN (1,2,3)";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseInWithExpressionsWhereClauseAddsColumns()
        {
            const string sql = "SELECT * FROM table_name WHERE column0 IN (column1, column2)";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(3, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
            Assert.AreEqual("column2", columns[2].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseLikeWhereClauseAddsColumn()
        {
            const string sql = "SELECT * FROM table_name WHERE column0 LIKE '%something%'";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseNotXLikeWhereClauseAddsColumn()
        {
            const string sql = "SELECT * FROM table_name WHERE NOT column0 LIKE '%something%'";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseXNotLkeWhereClauseAddsColumn()
        {
            const string sql = "SELECT * FROM table_name WHERE column0 NOT LIKE '%something%'";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseSimpleInSubqueryWherClauseAddsTablesAndColumns()
        {
            const string sql = "SELECT column0 FROM table0 WHERE column1 IN (SELECT column2 FROM table1)";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(2, tables.Count, "Table counts don't match");
            Assert.AreEqual("table0", tables[0].Name, "Table names don't match");
            Assert.AreEqual("table1", tables[1].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(3, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
            Assert.AreEqual("column2", columns[2].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseNestedInSubqueryWhereClauseAddsTablesAndColumns()
        {
            const string sql = "SELECT column0 FROM table0 WHERE column1 IN (SELECT column2 FROM table1 " +
                " WHERE column3 IN (SELECT column4 FROM table2))";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(3, tables.Count, "Table counts don't match");
            Assert.AreEqual("table0", tables[0].Name, "Table names don't match");
            Assert.AreEqual("table1", tables[1].Name, "Table names don't match");
            Assert.AreEqual("table2", tables[2].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(5, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
            Assert.AreEqual("column2", columns[2].Name, "Column names don't match");
            Assert.AreEqual("column3", columns[3].Name, "Column names don't match");
            Assert.AreEqual("column4", columns[4].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseSimpleAssignSubqueryWherClauseAddsTablesAndColumns()
        {
            const string sql = "SELECT column0 FROM table0 WHERE column1 = (SELECT column2 FROM table1)";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(2, tables.Count, "Table counts don't match");
            Assert.AreEqual("table0", tables[0].Name, "Table names don't match");
            Assert.AreEqual("table1", tables[1].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(3, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
            Assert.AreEqual("column2", columns[2].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseNestedAssignSubqueryWhereClauseAddsTablesAndColumns()
        {
            const string sql = "SELECT column0 FROM table0 WHERE column1 = (SELECT column2 FROM table1 " +
                " WHERE column3 = (SELECT column4 FROM table2))";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(3, tables.Count, "Table counts don't match");
            Assert.AreEqual("table0", tables[0].Name, "Table names don't match");
            Assert.AreEqual("table1", tables[1].Name, "Table names don't match");
            Assert.AreEqual("table2", tables[2].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(5, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
            Assert.AreEqual("column2", columns[2].Name, "Column names don't match");
            Assert.AreEqual("column3", columns[3].Name, "Column names don't match");
            Assert.AreEqual("column4", columns[4].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseInSubqueryNestedAssignSubqueryWhereClauseAddsTablesAndColumns()
        {
            const string sql = "SELECT column0 FROM table0 WHERE column1 = (SELECT column2 FROM table1 " +
                " WHERE column3 IN (SELECT column4 FROM table2))";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(3, tables.Count, "Table counts don't match");
            Assert.AreEqual("table0", tables[0].Name, "Table names don't match");
            Assert.AreEqual("table1", tables[1].Name, "Table names don't match");
            Assert.AreEqual("table2", tables[2].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(5, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
            Assert.AreEqual("column2", columns[2].Name, "Column names don't match");
            Assert.AreEqual("column3", columns[3].Name, "Column names don't match");
            Assert.AreEqual("column4", columns[4].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseAssignSubqueryNestedInSubqueryWhereClauseAddsTablesAndColumns()
        {
            const string sql = "SELECT column0 FROM table0 WHERE column1 IN (SELECT column2 FROM table1 " +
                " WHERE column3 = (SELECT column4 FROM table2))";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(3, tables.Count, "Table counts don't match");
            Assert.AreEqual("table0", tables[0].Name, "Table names don't match");
            Assert.AreEqual("table1", tables[1].Name, "Table names don't match");
            Assert.AreEqual("table2", tables[2].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(5, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
            Assert.AreEqual("column2", columns[2].Name, "Column names don't match");
            Assert.AreEqual("column3", columns[3].Name, "Column names don't match");
            Assert.AreEqual("column4", columns[4].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseWhereClauseInDeleteAddsTablesAndColumns()
        {
            const string sql = "DELETE FROM table0 WHERE column0=5";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table0", tables[0].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count);
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParseWhereClauseInUpdateAddsTablesAndColumns()
        {
            const string sql = "UPDATE table0 SEt column0=5 WHERE column1=6";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table0", tables[0].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count);
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
        }
    }
}
