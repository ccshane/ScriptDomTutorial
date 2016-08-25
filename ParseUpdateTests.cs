using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptDomDemo;

namespace ScriptDomBlogSampleTests
{
    /// <summary>
    /// Summary description for ParseUpdateTests
    /// </summary>
    [TestClass]
    public class ParseUpdateTests
    {
        [TestMethod]
        public void ParseUpdateWithTableAndColumnsCreatesCorrectObject()
        {
            var _parser = new SqlStatementsParser();
            const string sql = "UPDATE table_name SET column0 = value0, column1 = value1";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(1, tables.Count, "Table counts don't match");
            Assert.AreEqual("table_name", tables[0].Name, "Table names don't match");
            var columns = result.GetColumns();
            Assert.AreEqual(2, columns.Count, "Column counts don't match");
            Assert.AreEqual("column0", columns[0].Name, "Column names don't match");
            Assert.AreEqual("column1", columns[1].Name, "Column names don't match");
        }
    }
}
