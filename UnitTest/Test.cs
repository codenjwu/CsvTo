using CsvTo;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest
{
    public class Test1
    {
        public int int1 { get; set; }
        public int int2 { get; set; }
        public int? int3 { get; set; }
        public int? int4 { get; set; }
        public int? int5 { get; set; }
        public int? int6 { get; set; }
        public string prop1 { get; set; }
        public string prop2 { get; set; }
        public string prop3 { get; set; }
        public string prop4 { get; set; }
        public string prop5 { get; set; }
        public DateTime datetime1 { get; set; }
        public DateTime datetime2 { get; set; }
        public DateTime? datetime3 { get; set; }
        public DateTime? datetime4 { get; set; }
        public double double1 { get; set; }
        public decimal? decimal1 { get; set; }
        [CsvColumn("map1")]
        public int mapped_prop1 { get; set; }
        [CsvColumn("map2")]
        public int mapped_prop2 { get; set; }
        [CsvColumn("map3")]
        public string mapped_prop3 { get; set; }
        public int non_listed_prop1 { get; set; }
        public string non_listed_prop2 { get; set; }
        [CsvIgnore]
        public int ignore1 { get; set; }
        [CsvIgnore]
        public string ignore2 { get; set; }
        public IgnoreTest ignore3 { get; set; }
    }
    public class Test2
    {
        public int int1 { get; set; }
        public int? int3 { get; set; }
        public string prop1 { get; set; }
        public DateTime datetime1 { get; set; }
        public DateTime? datetime3 { get; set; }
        public double double1 { get; set; }
        [CsvColumn("map1")]
        public int mapped_prop1 { get; set; }
        [CsvColumn("map3")]
        public string mapped_prop3 { get; set; }
        public string non_listed_prop2 { get; set; }
        [CsvIgnore]
        public int ignore1 { get; set; }
        public IgnoreTest ignore3 { get; set; }
    }
    public class IgnoreTest
    {

    }
}
