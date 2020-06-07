using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CsvTo
{
    internal static class Parser
    {
        internal static readonly Regex CsvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
    }
}
