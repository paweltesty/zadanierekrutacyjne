namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataReader
    {
        IEnumerable<ImportedObject> ImportedObjects;

        public void ImportAndPrintData(string fileToImport)
        {
            ImportedObjects = new List<ImportedObject>();

            var streamReader = new StreamReader(fileToImport);

            var importedLines = new List<string>();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                importedLines.Add(line);
            }

            for (int i = 1; i < importedLines.Count; i++)
            {
                var importedLine = importedLines[i];
                var values = importedLine.Split(';');
                if (values.Count() == 7)
                {
                    var importedObject = new ImportedObject();
                    importedObject.Type = values[0].Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                    importedObject.Name = values[1].Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    importedObject.Schema = values[2].Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    importedObject.ParentName = values[3].Trim().Replace(" ", "").Replace(Environment.NewLine, ""); ;
                    importedObject.ParentType = values[4].Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper(); ;
                    importedObject.DataType = values[5];
                    importedObject.IsNullable = values[6];
                    ((List<ImportedObject>)ImportedObjects).Add(importedObject);
                }
            }

            // assign number of children
            foreach (var importedObject in ImportedObjects)
            {
                foreach (var impObj in ImportedObjects)
                {
                    if (impObj.ParentType == importedObject.Type && impObj.ParentName == importedObject.Name)
                    {
                        importedObject.NumberOfChildren++;
                    }
                }
            }

            foreach (var database in ImportedObjects)
            {
                if (database.Type == "DATABASE")
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in ImportedObjects)
                    {
                        if (table.ParentType == database.Type && table.ParentName == database.Name)
                        {
                            Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                            // print all table's columns
                            foreach (var column in ImportedObjects)
                            {
                                if (column.ParentType == table.Type && column.ParentName == table.Name)
                                {
                                    Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");                                        //}
                                }
                            }
                        }
                    }
                }
            }

            Console.ReadLine();
        }
    }

    class ImportedObject : ImportedObjectBaseClass
    {
        public string Schema { get; set; }
        public string ParentName { get; set; }
        public string ParentType { get; set; }
        public string DataType { get; set; }
        public string IsNullable { get; set; }

        public int NumberOfChildren { get; set; }
    }

    class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
