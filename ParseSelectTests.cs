﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptDomDemo;

namespace ScriptDomBlogSampleTests
{
    /// <summary>
    /// Summary description for ParseSelectTests
    /// </summary>
    [TestClass]
    public class ParseSelectTests
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
        public void ParsingHelloWorldExampleGetsHelloColumnFromWorldTable()
        {
            var sql = "select hello from world";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("hello", columns[0].Name, "Column name does not match");
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("world", tables[0].Name, "Table name does not match");
        }

        [TestMethod]
        public void ParsingSelectWithOneColumnAndOneTableReturnsCorrectObject()
        {
            const string sql = "SELECT column_name FROM table_name";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column_name", columns[0].Name, "Column name does not match");
            Assert.AreEqual("table_name", columns[0].BelongsToTable, "Column table does not match");
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table_name", tables[0].Name, "Table name does not match");
        }

        [TestMethod]
        public void ParsingSelectIntoReturnsCorrectObject()
        {
            const string sql = "select col1, col2 into table2 from table1";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(2, tables.Count, "Table counts don't match");
            Assert.AreEqual("table2", tables[0].Name, "Table names don't match");
            Assert.AreEqual("table1", tables[1].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count, "Column counts don't match");
            Assert.AreEqual("col1", columns[0].Name, "Column names don't match");
            Assert.AreEqual("col2", columns[1].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParsingSelectStarWithOneTableReturnsCorrectObject()
        {
            const string sql = "SELECT * FROM table_name";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            var columns = result.GetColumns();
            Assert.AreEqual(0, columns.Count, "Column counts don't match");
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table_name", tables[0].Name, "Table name does not match");
        }

        [TestMethod]
        public void ParsingSelectWithMultipleColumnsAndOneTableReturnsCorrectObject()
        {
            const string sql = "SELECT column0, column1 FROM table_name";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column name does not match");
            Assert.AreEqual("column1", columns[1].Name, "Column name does not match");
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table_name", tables[0].Name, "Table name does not match");
        }

        [TestMethod]
        public void ParsingSelectWithOneColumnAndMultipleTablesReturnsCorrectObject()
        {
            const string sql = "SELECT column_name FROM table0, table1";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column_name", columns[0].Name, "Column name does not match");
            Assert.AreEqual(2, tables.Count, "Table counts don't match");
            Assert.AreEqual("table0", tables[0].Name, "Table name does not match");
            Assert.AreEqual("table1", tables[1].Name, "Table name does not match");
        }

        [TestMethod]
        public void ParsingSelectWithMultipleColumnsAndMultipleTablesReturnsCorrectObject()
        {
            const string sql = "SELECT column0, column1 FROM table0, table1";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column name does not match");
            Assert.AreEqual("column1", columns[1].Name, "Column name does not match");
            Assert.AreEqual(2, tables.Count, "Table counts don't match");
            Assert.AreEqual("table0", tables[0].Name, "Table name does not match");
            Assert.AreEqual("table1", tables[1].Name, "Table name does not match");
        }

        [TestMethod]
        public void ParsingSelectStarWithMultipleTablesReturnsCorrectObject()
        {
            const string sql = "SELECT * FROM table0, table1";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            var columns = result.GetColumns();
            Assert.AreEqual(0, columns.Count, "Column counts don't match");
            Assert.AreEqual(2, tables.Count, "Table counts don't match");
            Assert.AreEqual("table0", tables[0].Name, "Table name does not match");
            Assert.AreEqual("table1", tables[1].Name, "Table name does not match");
        }

        [TestMethod]
        public void ParsingSelectWithDistinctColumnAndOneTableReturnsCorrectObject()
        {
            const string sql = "SELECT DISTINCT column_name FROM table_name";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            var columns = result.GetColumns();
            Assert.IsTrue(result.IsDistinctSelection);
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column_name", columns[0].Name, "Column name does not match");
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table_name", tables[0].Name, "Table name does not match");
        }

        [TestMethod]
        public void ParsingSelectWithDistinctMultipleColumnsAndOneTableReturnsCorrectObject()
        {
            const string sql = "SELECT DISTINCT column0, column1 FROM table_name";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            var columns = result.GetColumns();
            Assert.IsTrue(result.IsDistinctSelection);
            Assert.AreEqual(2, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column name does not match");
            Assert.AreEqual("column1", columns[1].Name, "Column name does not match");
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table_name", tables[0].Name, "Table name does not match");
        }

        [TestMethod]
        public void ParsingSelectTopWithOneColumnAndOneTableReturnsCorrectObject()
        {
            const string sql = "SELECT TOP 10 column_name FROM table_name";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            var columns = result.GetColumns();
            Assert.IsTrue(result.IsTopSelection);
            Assert.IsFalse(result.IsTopPercent);
            Assert.AreEqual(10, result.TopCount);
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column_name", columns[0].Name, "Column name does not match");
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table_name", tables[0].Name, "Table name does not match");
        }

        [TestMethod]
        public void ParsingSelectParenthesizedTopSetsProperties()
        {
            const string sql = "SELECT TOP (10) column_name FROM table_name";
            var result = _parser.Parse(sql);
            Assert.IsTrue(result.IsTopSelection);
            Assert.IsFalse(result.IsTopPercent);
            Assert.AreEqual(10, result.TopCount);
        }

        [TestMethod]
        public void ParsingSelectTopPercentWithOneColumnAndOneTableReturnsCorrectObject()
        {
            const string sql = "SELECT TOP 20 PERCENT column_name FROM table_name";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            var columns = result.GetColumns();
            Assert.IsTrue(result.IsTopSelection);
            Assert.IsTrue(result.IsTopPercent);
            Assert.AreEqual(20, result.TopCount);
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column_name", columns[0].Name, "Column name does not match");
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table_name", tables[0].Name, "Table name does not match");
        }

        [TestMethod]
        public void ParsingSelectWithAliasedTableCorrectlyAddsAliasToTable()
        {
            const string sql = "SELECT column_name FROM table_name as tm";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table_name", tables[0].Name, "Table name does not match");
            Assert.AreEqual("tm", tables[0].Alias, "Table alias does not match");
        }

        [TestMethod]
        public void ParsingSelectWithAliasedColumnCorrectlyAddsAliasToColumn()
        {
            const string sql = "SELECT column_name as cm FROM table_name";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column_name", columns[0].Name, "Column name does not match");
            Assert.AreEqual("cm", columns[0].Alias, "Column alias does not match");
        }

        [TestMethod]
        public void ParsingSelectWithTableSpecificColumnAddsTableColumnReference()
        {
            const string sql = "SELECT table_name.column_name FROM table_name";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column_name", columns[0].Name, "Column name does not match");
            Assert.AreEqual("table_name", columns[0].BelongsToTable, "Column table name does not match");
        }

        [TestMethod]
        public void ParsingSelectWithAliasedColumnAddsTableColumnReference()
        {
            const string sql = "SELECT tm.column_name FROM table_name tm";
            var result = _parser.Parse(sql);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column_name", columns[0].Name, "Column name does not match");
            Assert.AreEqual("table_name", columns[0].BelongsToTable, "Column table name does not match");
        }

        [TestMethod]
        public void ParsingSelectWithMultipleAliasedTablesAndColumnsReturnsCorrectObject()
        {
            const string sql = "SELECT t1.column_name as col1, t2.column_name as col2 FROM table_1 t1, table_2 t2";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count, "Column counts don't match");
            Assert.AreEqual("column_name", columns[0].Name, "Column name does not match");
            Assert.AreEqual("col1", columns[0].Alias, "Column alias does not match");
            Assert.AreEqual("table_1", columns[0].BelongsToTable, "Column table name does not match");
            Assert.AreEqual("column_name", columns[1].Name, "Column name does not match");
            Assert.AreEqual("col2", columns[1].Alias, "Column alias does not match");
            Assert.AreEqual("table_2", columns[1].BelongsToTable, "Column table name does not match");
            Assert.AreEqual(2, tables.Count, "Table counts don't match");
            Assert.AreEqual("table_1", tables[0].Name, "Table name does not match");
            Assert.AreEqual("t1", tables[0].Alias, "Table alias does not match");
            Assert.AreEqual("table_2", tables[1].Name, "Table name does not match");
            Assert.AreEqual("t2", tables[1].Alias, "Table alias does not match");
        }

        [TestMethod]
        public void ParsingSelectIdentityExitsParsing()
        {
            const string sql = "SELECT @@IDENTITY";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(0, tables.Count, "Table counts don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(0, columns.Count, "Column counts don't match");
        }

        [TestMethod]
        public void ParsingSelectFunctionReturnsCorrectObject()
        {
            const string sql = "SELECT Count(column_name) FROM table_name";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table_name", tables[0].Name);
            var columns = result.GetColumns();
            Assert.AreEqual(1, columns.Count, "Column counts don't match");
            Assert.AreEqual("column_name", columns[0].Name);
        }

        [TestMethod]
        public void ParsingUnionSelectReturnsCorrectObject()
        {
            const string sql = "SELECT column1 FROM table1 UNION SELECT column1 FROM table2";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(2, tables.Count, "Table counts don't match");
            Assert.AreEqual("table1", tables[0].Name, "Table names don't match");
            Assert.AreEqual("table2", tables[1].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count, "Column counts don't match");
            Assert.AreEqual("column1", columns[0].Name, "Column names don't match");
            Assert.AreEqual("table1", columns[0].BelongsToTable, "Incorrect table reference");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParsingNestedUnionSelectReturnsCorrectObject()
        {
            const string sql = "SELECT column1 FROM table1 UNION SELECT column1 FROM table2 UNION " +
                "SELECT column1 FROM table3";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(3, tables.Count, "Table counts don't match");
            Assert.AreEqual("table1", tables[0].Name, "Table names don't match");
            Assert.AreEqual("table2", tables[1].Name, "Table names don't match");
            Assert.AreEqual("table3", tables[2].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count, "Column counts don't match");
            Assert.AreEqual("column1", columns[0].Name, "Column names don't match");
            Assert.AreEqual("table1", columns[0].BelongsToTable, "Incorrect table reference");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParsingIntersectSelectReturnsCorrectObject()
        {
            const string sql = "SELECT column1 FROM table1 INTERSECT SELECT column1 FROM table2";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(2, tables.Count, "Table counts don't match");
            Assert.AreEqual("table1", tables[0].Name, "Table names don't match");
            Assert.AreEqual("table2", tables[1].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count, "Column counts don't match");
            Assert.AreEqual("column1", columns[0].Name, "Column names don't match");
            Assert.AreEqual("table1", columns[0].BelongsToTable, "Incorrect table reference");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParsingExceptSelectReturnsCorrectObject()
        {
            const string sql = "SELECT column1 FROM table1 EXCEPT SELECT column1 FROM table2";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(2, tables.Count, "Table counts don't match");
            Assert.AreEqual("table1", tables[0].Name, "Table names don't match");
            Assert.AreEqual("table2", tables[1].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count, "Column counts don't match");
            Assert.AreEqual("column1", columns[0].Name, "Column names don't match");
            Assert.AreEqual("table1", columns[0].BelongsToTable, "Incorrect table reference");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParsingSimpleJoinReturnsCorrectObject()
        {
            const string sql = "SELECT column1 FROM table1 JOIN table2 ON table1.id=table2.id";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(2, tables.Count, "Table counts don't match");
            Assert.AreEqual("table1", tables[0].Name, "Table names don't match");
            Assert.AreEqual("table2", tables[1].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(3, columns.Count, "Column counts don't match");
            Assert.AreEqual("id", columns[0].Name, "Column names don't match");
            Assert.AreEqual("table1", columns[0].BelongsToTable, "Incorrect table reference");
            Assert.AreEqual("id", columns[1].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[2].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParsingLeftJoinReturnsCorrectObject()
        {
            const string sql = "SELECT column1 FROM table1 LEFT JOIN table2 ON table1.id=table2.id";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(2, tables.Count, "Table counts don't match");
            Assert.AreEqual("table1", tables[0].Name, "Table names don't match");
            Assert.AreEqual("table2", tables[1].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(3, columns.Count, "Column counts don't match");
            Assert.AreEqual("id", columns[0].Name, "Column names don't match");
            Assert.AreEqual("table1", columns[0].BelongsToTable, "Incorrect table reference");
            Assert.AreEqual("id", columns[1].Name, "Column names don't match");
            Assert.AreEqual("table2", columns[1].BelongsToTable, "Incorrect table reference");
            Assert.AreEqual("column1", columns[2].Name, "Column names don't match");
        }

        [TestMethod]
        public void ParsingSimpleJoinWithAliasesReturnsCorrectObject()
        {
            const string sql = "SELECT column1 FROM table1 t1 JOIN table2 t2 ON t1.id=t2.id";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(2, tables.Count, "Table counts don't match");
            Assert.AreEqual("table1", tables[0].Name, "Table names don't match");
            Assert.AreEqual("table2", tables[1].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(3, columns.Count, "Column counts don't match");
            Assert.AreEqual("id", columns[0].Name, "Column names don't match");
            Assert.AreEqual("table1", columns[0].BelongsToTable, "Incorrect table reference");
            Assert.AreEqual("id", columns[1].Name, "Column names don't match");
            Assert.AreEqual("table2", columns[1].BelongsToTable, "Incorrect table reference");
            Assert.AreEqual("column1", columns[2].Name, "Column names don't match");
        }
    }
}
