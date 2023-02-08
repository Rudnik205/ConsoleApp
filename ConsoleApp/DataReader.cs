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
        List<ImportedObject> ImportedObjects;        // (2) 

        const string DataBaseType = "DATABASE";

        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
            ImportedObjects = new List<ImportedObject>(); // (3)

            var streamReader = new StreamReader(fileToImport);

            var importedLines = new List<string>();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                importedLines.Add(line);
            }

            foreach(var importedLine in importedLines)  // (4)
            {
                var values = importedLine.Split(';');
                var importedObject = new ImportedObject();

                if (values.Count() == 1 && values[0] == string.Empty)       // (5)
                    continue;

                importedObject.Type = values[0];
                if (values.Count() > 1)                 // (6)
                    importedObject.Name = values[1];
                if(values.Count() > 2)
                    importedObject.Schema = values[2];
                if(values.Count() > 3)
                    importedObject.ParentName = values[3];
                if(values.Count() > 4)
                    importedObject.ParentType = values[4];
                if(values.Count() > 5)
                    importedObject.DataType = values[5];
                if(values.Count() > 6)
                    importedObject.IsNullable = values[6] == "1" ? true : false;
                ImportedObjects.Add(importedObject); // (2)
            }

            // clear and correct imported data
            foreach (var importedObject in ImportedObjects)
            {
                importedObject.Type = importedObject.Type?.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();      // (7)
                importedObject.Name = importedObject.Name?.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.Schema = importedObject.Schema?.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.ParentName = importedObject.ParentName?.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.ParentType = importedObject.ParentType?.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
            }

            // assign number of children
            foreach (var importedObject in ImportedObjects)     // (8)
            {
                importedObject.NumberOfChildren = ImportedObjects.Where(x => x.ParentType == importedObject.Type && x.ParentName == importedObject.Name).Count(); // (9)
            }

            var databases = ImportedObjects.Where(x => x.Type == DataBaseType); // (10)
            // (11)
            foreach (var database in databases)
            {
                Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");
                var tables = ImportedObjects.Where(x => x.ParentType?.ToUpper() == database.Type && x.ParentName == database.Name);
                foreach (var table in tables)
                {
                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");
                    var columns = ImportedObjects.Where(x => x.ParentType?.ToUpper() == table.Type && x.ParentName == table.Name);
                    foreach (var column in columns)
                    {
                        Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable ? "accepts nulls" : "with no nulls")}");
                    }
                }
            }

            Console.ReadLine();
        }
    }

    class ImportedObject : ImportedObjectBaseClass
    {
        public string Schema { get; set; }  // (13)

        public string ParentName { get; set; }
        public string ParentType { get; set; }    // (14)

        public string DataType { get; set; }
        public bool IsNullable { get; set; }      // (16)

        public int NumberOfChildren { get; set; }       // (15)
    }

    class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
