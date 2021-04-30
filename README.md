# CsvTo
A Csv tool used for converting csv file or stream to datatable/object(IEnumerable), support csv with or without heaeder, do not support type with complex type property.

## How to use

### non-generic converter will return value as string
#### forward read
 ```csharp
 var converter = new CsvTo.CsvConverter(file);
     
 DataTable dt = converter.ToDataTable();
       
 IEnumerable<string[]> c = converter.ToCollection();
 ```
 ### reverse read
  ```csharp
 var r_converter = new CsvTo.CsvReverseConverter(file);

 DataTable r_dt = r_converter.ToDataTable();

 IEnumerable<string[]> r_c = r_converter.ToCollection();
 ```
### generic converter will return DataTable with typed column / IEnumerable\<Type\>
#### forward read
  ```csharp
 var  g_converter_1 = new CsvTo.CsvConverter<Test1>(file1);

 DataTable g_dt = g_converter_1.ToDataTable();

 IEnumerable<Test1> g_c = g_converter_1.ToCollection();
 ```
  #### reverse read
  ```csharp
 var  g_r_converter_1 = new CsvTo.CsvReverseConverter<Test1>(file1);
 
 DataTable g_r_c = g_r_converter_1.ToDataTable();

 IEnumerable<Test1> g_r_c = g_r_converter_1.ToCollection();
 ```
