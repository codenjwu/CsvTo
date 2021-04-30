# CsvTo
A Csv tool used for converting csv file or stream to datatable/object(IEnumerable), support csv with or without heaeder, do not support type with complex type property.

## How to use

### non-generic converter will return value as string
#### forward read
 var converter = new CsvTo.CsvConverter(file);
<br>        
 DataTable dt = converter.ToDataTable();
<br>        
 IEnumerable<string[]> c = converter.ToCollection();
 ### reverse read
<br>
 var r_converter = new CsvTo.CsvReverseConverter(file);
<br>
 DataTable r_dt = r_converter.ToDataTable();
<br>
 IEnumerable<string[]> r_c = r_converter.ToCollection();
 ### non-generic converter will return value as string
#### forward read
<br>
 var  g_converter_1 = new CsvTo.CsvConverter<Test1>(file1);
<br>
 DataTable g_dt = g_converter_1.ToDataTable();
<br>
 IEnumerable<Test1> g_c = g_converter_1.ToCollection();
  ### reverse read
<br>
 var  g_r_converter_1 = new CsvTo.CsvReverseConverter<Test1>(file1);
<br>
 DataTable g_r_c = g_r_converter_1.ToDataTable();
<br>
 IEnumerable<Test1> g_r_c = g_r_converter_1.ToCollection();
