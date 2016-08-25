﻿using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace ScriptDomBlogSample
{
    public class DataContainer
    {
        private IList<Table> _tables;
        private IList<Column> _columns;
        private IList<ParseError> _errors;
        public bool IsDistinctSelection { get; set; }
        public bool IsTopSelection { get; set; }
        public bool IsTopPercent { get; set; }
        public int TopCount { get; set; }

        public DataContainer()
        {
            _tables = new List<Table>();
            _columns = new List<Column>();
        }
        public void AddTable(string tableName, string alias)
        {
            if (!ContainsTable(tableName))
            {
                _tables.Add(new Table { Name = tableName, Alias = alias });
            }
        }
        private bool ContainsTable(string tableName)
        {
            foreach (var table in _tables)
            {
                if (table.Name.Equals(tableName))
                {
                    return true;
                }
            }
            return false;
        }

        public void AddColumn(string columnName, string alias, string tableName)
        {
            if (!ContainsColumn(columnName, alias, tableName))
            {
                _columns.Add(new Column { Name = columnName, Alias = alias, BelongsToTable = tableName }); //CHANGE DATATYPE
            }
        }
        private bool ContainsColumn(string columnName, string alias, string tableName)
        {
            foreach (var column in _columns)
            {
                if (column.Name.Equals(columnName) && column.Alias.Equals(alias) && column.BelongsToTable.Equals(tableName))
                {
                    return true;
                }
            }
            return false;
        }


        public void AddErrors(IList<ParseError> errors)
        {
            _errors = errors;
        }


        public IList<Table> GetTables()
        {
            return _tables;
        }


        public IList<Column> GetColumns()
        {
            return _columns;
        }


        public IList<ParseError> GetErrors()
        {
            return _errors;
        }
    }

}
