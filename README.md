# CsvTo
A Csv tool used for converting csv file or stream to datatable/array/object(Ienumerable), support csv with or without heaeder, do not support type with complex type property.

## How to use

 CsvTo.CsvConverter converter = new CsvTo.CsvConverter(file);
 CsvTo.CsvReverseConverter r_converter = new CsvTo.CsvReverseConverter(file);
        
 DataTable dt = converter.ToDataTable();

 IEnumerable<string[]> r_c = r_converter.ToCollection();
        
