# CsvTo 使用示例

这个项目包含了 CsvTo 库的完整使用示例，涵盖从基础到高级的所有功能。

## 运行示例

```bash
cd Examples
dotnet run
```

然后选择您想要查看的示例编号。

## 示例列表

### 1. 基本用法 - DataTable 转换
展示如何将 CSV 文件转换为 DataTable 对象。

**功能:**
- 创建 CsvConverter 实例
- 转换为 DataTable
- 访问行和列数据

### 2. 基本用法 - 集合转换
展示如何将 CSV 文件转换为字符串数组集合。

**功能:**
- 转换为 IEnumerable<string[]>
- 使用 LINQ 查询数据
- 过滤和处理数据

### 3. 泛型转换示例
展示如何将 CSV 转换为强类型对象集合。

**功能:**
- 使用 CsvConverter<T> 泛型转换器
- CsvAttribute 属性映射
- 强类型 LINQ 查询

### 4. 多语言编码示例
展示如何处理不同编码的 CSV 文件。

**功能:**
- UTF-8 编码（中文）
- UTF-16 编码（日文）
- 其他编码支持（GB2312, Big5 等）

### 5. 自定义分隔符示例
展示如何处理使用不同分隔符的文件。

**功能:**
- 分号分隔 (;)
- Tab 分隔 (\t)
- 管道符分隔 (|)
- 自定义转义字符

### 6. 反向读取示例
展示如何从文件末尾开始读取数据。

**功能:**
- CsvReverseConverter 反向读取
- 获取最新记录（如日志）
- 泛型反向转换

### 7. Stream 输入示例
展示如何从 Stream 读取 CSV 数据。

**功能:**
- 从内存流读取
- 从文件流读取
- Stream 的优势和应用场景

### 8. 高级用法 - 复杂类型转换
展示如何处理复杂数据类型和业务逻辑。

**功能:**
- DateTime 类型转换
- Decimal 类型转换
- Boolean 类型转换
- 可空类型 (Nullable<T>)
- 计算属性
- 数据分析和统计

### 9. 错误处理示例
展示如何优雅地处理各种错误情况。

**功能:**
- 文件不存在处理
- 空文件处理
- 格式错误处理
- 类型转换错误处理
- 最佳实践建议

## 示例代码结构

```
Examples/
├── Program.cs                      # 主程序入口
├── BasicDataTableExample.cs        # 示例 1
├── BasicCollectionExample.cs       # 示例 2
├── GenericConverterExample.cs      # 示例 3
├── EncodingExample.cs              # 示例 4
├── CustomDelimiterExample.cs       # 示例 5
├── ReverseReadingExample.cs        # 示例 6
├── StreamInputExample.cs           # 示例 7
├── AdvancedTypesExample.cs         # 示例 8
└── ErrorHandlingExample.cs         # 示例 9
```

## 学习路径

1. **初学者**: 从示例 1 和 2 开始，了解基本用法
2. **进阶**: 学习示例 3（泛型转换）和示例 8（复杂类型）
3. **国际化**: 查看示例 4（编码支持）
4. **特殊需求**: 示例 5（自定义分隔符）、示例 6（反向读取）、示例 7（Stream）
5. **生产环境**: 示例 9（错误处理）学习如何编写健壮的代码

## 注意事项

- 所有示例都会自动生成测试用的 CSV 文件
- 示例是交互式的，可以重复运行
- 每个示例都包含详细的注释和说明

## 扩展练习

基于这些示例，您可以尝试:
1. 读取更大的 CSV 文件
2. 实现自定义的数据验证逻辑
3. 将多个 CSV 文件合并
4. 导出数据到不同格式
5. 实现数据转换管道

## 反馈

如果您有任何问题或建议，请随时提出！
