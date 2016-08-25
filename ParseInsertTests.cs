using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptDomDemo;

namespace ScriptDomBlogSampleTests
{
    [TestClass]
    public class ParseInsertTests
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
        public void ParseInsertWithDefaultValuesAddsTableAndNoColumns()
        {
            const string sql = "INSERT INTO table_name DEFAULT VALUES";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(1, tables.Count, "Table counts don’t match");
            Assert.AreEqual("table_name", tables[0].Name, "Table name does not match");
            var columns = result.GetColumns();
            Assert.AreEqual(0, columns.Count, "Column counts don’t match");
        }

        [TestMethod]
        public void ParseInsertWithSpecifiedValuesAddsToTableAndNoColumns()
        {
            const string sql = "INSERT INTO table_name VALUES (value1, value2)";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(1, tables.Count, "Table counts don’t match");
            Assert.AreEqual("table_name", tables[0].Name, "Table name does not match");
            var columns = result.GetColumns();
            Assert.AreEqual(0, columns.Count, "Column counts don’t match");
        }

        [TestMethod]
        public void ParseInsertWithSpecifiedColumnsAddsToTableAndColumns()
        {
            const string sql = "INSERT INTO table_name (column0, column1) VALUES (value1, value2)";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(1, tables.Count, "Table counts don’t match");
            Assert.AreEqual("table_name", tables[0].Name, "Table name does not match");
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count, "Column counts don’t match");
            Assert.AreEqual("column0", columns[0].Name, "Column name does not match");
            Assert.AreEqual("column1", columns[1].Name, "Column name does not match");
        }

    }
}
