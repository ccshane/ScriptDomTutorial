using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptDomDemo;

namespace ScriptDomBlogSampleTests
{
    [TestClass]
    public class ParseDeleteTests
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
        public void ParseDeleteAddsTable()
        {
            const string sql = "DELETE FROM table_name";
            var results = _parser.Parse(sql);
            var tables = results.GetTables();
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table_name", tables[0].Name, "Table names don't match");
            var columns = results.GetColumns();
            Assert.AreEqual(0, columns.Count);
        }

        [TestMethod]
        public void ParseDeleteTopAddsTableAndSetsProperties()
        {
            const string sql = "DELETE TOP (10) FROM table_name";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.IsTrue(result.IsTopSelection);
            Assert.IsFalse(result.IsTopPercent);
            Assert.AreEqual(10, result.TopCount);
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table_name", tables[0].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(0, columns.Count);
        }

        [TestMethod]
        public void ParseDeleteTopPercentAddsTableAndSetsProperties()
        {
            const string sql = "DELETE TOP (10) PERCENT FROM table_name";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.IsTrue(result.IsTopSelection);
            Assert.IsTrue(result.IsTopPercent);
            Assert.AreEqual(10, result.TopCount);
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table_name", tables[0].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(0, columns.Count);
        }
    }
}
