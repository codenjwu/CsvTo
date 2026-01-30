# CsvTo

A Csv tool used for converting csv file or stream to datatable/object(IEnumerable), support csv with or without header, support multiple encodings for international languages, do not support type with complex type property.

## Features

- ✅ Forward and reverse reading of CSV files
- ✅ Convert to DataTable or IEnumerable<T>
- ✅ Support for files with or without headers
- ✅ Multiple encoding support (UTF-8, UTF-16, etc.) for international languages
- ✅ Custom delimiter and escape character support
- ✅ Stream and file path input
- ✅ Optimized for large files
- ✅ .NET Framework 4.7+ and .NET 5-9 support

## Installation

```
Install-Package CsvTo
```

## How to use

### Non-generic converter (returns values as string)

#### Forward read
```csharp
var converter = new CsvTo.CsvConverter(file);
     
DataTable dt = converter.ToDataTable();
       
IEnumerable<string[]> c = converter.ToCollection();
```

#### Reverse read
```csharp
var r_converter = new CsvTo.CsvReverseConverter(file);

DataTable r_dt = r_converter.ToDataTable();

IEnumerable<string[]> r_c = r_converter.ToCollection();
```

### Generic converter (returns DataTable with typed columns / IEnumerable\<Type\>)

#### Forward read
```csharp
var  g_converter_1 = new CsvTo.CsvConverter<Test1>(file1);

DataTable g_dt = g_converter_1.ToDataTable();

IEnumerable<Test1> g_c = g_converter_1.ToCollection();
```

#### Reverse read
```csharp
var  g_r_converter_1 = new CsvTo.CsvReverseConverter<Test1>(file1);
 
DataTable g_r_dt = g_r_converter_1.ToDataTable();

IEnumerable<Test1> g_r_c = g_r_converter_1.ToCollection();
```

### Encoding Support (New in v3.0)

Support for multiple encodings including international languages:

```csharp
// Chinese CSV with UTF-8 encoding
var converter = new CsvTo.CsvConverter(file, encoding: Encoding.UTF8);

// Japanese CSV  
var converter2 = new CsvTo.CsvConverter(japaneseFile, encoding: Encoding.UTF8);

// Custom encoding
var converter3 = new CsvTo.CsvConverter(file, encoding: Encoding.GetEncoding("GB2312"));

// Default encoding is UTF-8
var converter4 = new CsvTo.CsvConverter(file); // Uses UTF-8 by default
```

### Custom Delimiter and Escape

```csharp
// Semicolon delimiter
var converter = new CsvTo.CsvConverter(file, delimiter: ";");

// Tab delimiter
var converter2 = new CsvTo.CsvConverter(file, delimiter: "\t");

// Custom escape character
var converter3 = new CsvTo.CsvConverter(file, escape: "'");
```

### Stream Input

```csharp
using (var stream = File.OpenRead(filePath))
{
    var converter = new CsvTo.CsvConverter(stream, encoding: Encoding.UTF8);
    var dt = converter.ToDataTable();
}
```

## Supported Frameworks

- .NET Framework 4.7, 4.7.1, 4.7.2, 4.8
- .NET Core 2.0, 2.1, 2.2, 3.1
- .NET Standard 2.0, 2.1
- .NET 5, 6, 7, 8, 9

## License

MIT
