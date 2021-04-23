using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CsvTo
{
    internal sealed class Parser
    {
        string _delimiter;
        string _escape;
        Regex _delimiterRegex;
        Regex _escapeRegex;
        public Parser(string delimiter = ",", string escape = "\"")
        {
            _delimiter = delimiter;
            _escape = escape;
            var delstr = @"(?:^""|" + _delimiter + @""")(""""|[\w\W]*?)(?=""" + _delimiter + @"|""$)|(?:^(?!"")|" + _delimiter + @"(?!""))([^,]*?)(?=$|" + _delimiter + @")|(\r\n|\n)";
            _delimiterRegex = new Regex(delstr);
            _escapeRegex = new Regex(_escape);
        }

        //internal static readonly Regex CsvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
        internal Regex DelimiterRegex => _delimiterRegex;
        internal Regex EscapeRegex => _escapeRegex;
        internal int EscapeCount(string str)
        {
            return EscapeRegex.Matches(str).Count;
        }
        internal string[] Split(string str)
        {
            return DelimiterRegex.Split(str);
        }
    }
}
