using System;
using System.Collections.Generic;
using System.Text;

namespace CsvTo
{
    internal static class Parser
    {
        internal static readonly Regex CsvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
    }
}
