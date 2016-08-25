using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptDomDemo;

namespace ScriptDomBlogSampleTests
{
    [TestClass]
    public class ParseExecuteTests
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
        public void ParseWithFullExecuteNameNoParametersAddsToTable()
        {
            const string sql = "EXECUTE MyProcedureName";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(1, tables.Count, "Procedure counts don't match");
            Assert.AreEqual("MyProcedureName", tables[0].Name, "Procedure names don't match");
        }

        [TestMethod]
        public void ParseWithShortExecuteNameNoParametersAddsToTable()
        {
            const string sql = "EXEC MyProcedureName";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(1, tables.Count, "Procedure counts don't match");
            Assert.AreEqual("MyProcedureName", tables[0].Name, "Procedure names don't match");
        }

        [TestMethod]
        public void ParseWithFullExecuteNameWithParametersAddsToTable()
        {
            const string sql = "EXECUTE MyProcedureName 1, 3.5, 'hello'";
            var result = _parser.Parse(sql);
            var tables = result.GetTables();
            Assert.AreEqual(1, tables.Count, "Procedure counts don't match");
            Assert.AreEqual("MyProcedureName", tables[0].Name, "Procedure names don't match");
        }
    }
}
