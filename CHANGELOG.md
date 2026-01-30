# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.0.0] - 2026-01-29

### Added
- **Examples Project**: 9 interactive example programs demonstrating all library features
  - Basic DataTable conversion
  - Basic Collection conversion with LINQ
  - Generic type conversion
  - Multiple encoding examples (UTF-8, UTF-16)
  - Custom delimiters (semicolon, tab, pipe)
  - Reverse reading for log files
  - Stream input examples
  - Advanced types with DateTime, Decimal, Nullable
  - Error handling demonstrations

- **PerformanceTest Project**: Comprehensive performance benchmarks using BenchmarkDotNet
  - Small, medium, and large file benchmarks (100 - 100,000 rows)
  - Encoding performance tests
  - Reverse reading performance tests
  - Memory diagnostics

- **Enhanced Test Coverage**: 100+ comprehensive test scenarios
  - Complex scenario tests (26 tests)
  - Advanced performance tests (10 tests)
  - Integration tests (10 tests)
  - Robustness tests (24 tests)
  - Multi-language encoding tests (Chinese, Japanese, Spanish)

- **Documentation Improvements**:
  - NEW_PROJECTS.md - New projects overview
  - TEST_SCENARIOS.md - Detailed test documentation
  - IMPROVEMENTS.md - Project improvements summary

### Enhanced
- Multi-language support verification with comprehensive encoding tests
- Test coverage now includes real-world scenarios
- Performance benchmarking infrastructure

### Fixed
- Various edge cases identified through expanded test suite

### Technical Details
- Supports 15 target frameworks (.NET Framework 4.7-4.8, .NET Core 2.0-3.1, .NET 5-9, .NET Standard 2.0-2.1)
- UTF-8, UTF-16, and other encoding support verified
- Custom delimiter support (single character)
- Stream and file input support
- Forward and reverse reading modes

## [2.x.x] - Previous Versions

### Features
- CSV to DataTable conversion
- CSV to IEnumerable<string[]> conversion
- Generic converter support (CsvConverter<T>)
- Forward and reverse reading
- Header detection
- Custom delimiter support
- Multiple encoding support
- CsvColumn attribute for property mapping
- CsvIgnore attribute for excluding properties

## Migration Guide

### From 2.x to 3.0

No breaking changes - version 3.0 is fully backward compatible with 2.x versions.

The 3.0 release focuses on:
- Enhanced documentation
- Comprehensive testing
- Performance benchmarks
- Educational examples

All existing code will continue to work without modifications.

## Future Roadmap

### Planned for 3.1.0
- Enhanced error messages
- Additional encoding support
- Performance optimizations
- More real-world examples

### Under Consideration
- Multi-character delimiter support
- Enhanced null handling
- Async API support
- Streaming improvements for very large files

---

For more information, see:
- [README.md](README.md) - Usage documentation
- [Examples/](Examples/) - Code examples
- [PerformanceTest/](PerformanceTest/) - Performance benchmarks
- [TEST_SCENARIOS.md](TEST_SCENARIOS.md) - Test documentation
