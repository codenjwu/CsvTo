# CsvTo 性能测试

## 概述

本项目使用 BenchmarkDotNet 对 CsvTo 库进行性能基准测试。

## 测试场景

### 文件大小测试
- **小文件**: 100 行数据
- **中等文件**: 10,000 行数据
- **大文件**: 100,000 行数据

### 测试内容
1. **DataTable 转换性能**
   - 小文件 ToDataTable
   - 中等文件 ToDataTable
   - 大文件 ToDataTable

2. **集合转换性能**
   - 小文件 ToCollection
   - 中等文件 ToCollection
   - 大文件 ToCollection

3. **泛型转换性能**
   - 小文件泛型转换
   - 中等文件泛型转换
   - 大文件泛型转换

4. **编码性能**
   - UTF-8 编码
   - UTF-16 编码

5. **反向读取性能**
   - 小文件反向读取
   - 中等文件反向读取

## 运行测试

```bash
cd PerformanceTest
dotnet run -c Release
```

## 测试框架

- .NET 9.0
- .NET 8.0
- BenchmarkDotNet 0.14.0

## 查看结果

测试完成后，结果将保存在 `BenchmarkDotNet.Artifacts` 目录中，包括：
- HTML 报告
- Markdown 报告
- CSV 数据

## 性能指标

测试会报告以下指标：
- **执行时间** (Mean, Median, StdDev)
- **内存分配** (Gen0, Gen1, Gen2, Allocated)
- **吞吐量**

## 自定义测试

您可以修改 `CsvConverterBenchmarks.cs` 来：
- 调整测试文件大小
- 添加新的测试场景
- 测试不同的配置选项
