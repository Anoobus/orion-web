using System;
namespace anoobus.mssql.datagen;

public class SpecialConfig
    {
        private Dictionary<string, ColumnSetup> specialCols = new Dictionary<string, ColumnSetup>();

        public ColumnSetup AddSpecialColumn(string column)
        {
            var setup = new ColumnSetup(this);
            specialCols.Add(column, setup);
            return setup;
        }

        public Dictionary<string, IColumnSetup> ExportColumnConfig()
        {
            return specialCols.ToDictionary(x => x.Key, x => x.Value.columnSetup);
        }
    }

