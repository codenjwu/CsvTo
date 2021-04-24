using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CsvTo
{
    internal sealed class Parser
    {
        string _delimiter;
        char _delimiterChar;
        string _escape;
        Regex _delimiterRegex;
        Regex _escapeRegex;
        public Parser(string delimiter = ",", string escape = "\"")
        {
            _delimiter = delimiter;
            _delimiterChar = char.Parse(delimiter);
            _escape = escape;
            //var delstr = @"(?:^""|" + _delimiter + @""")(""""|[\w\W]*?)(?=""" + _delimiter + @"|""$)|(?:^(?!"")|" + _delimiter + @"(?!""))([^,]*?)(?=$|" + _delimiter + @")|(\r\n|\n)";
            //var linestr = @"(?m)^[^""\r\n]*(?:(?:""[^""]*"")+[^""\r\n]*)*";
            //var delstr = "^(?:(?:\"((?:\"\"|[^\"])+)\"|([^,]*))(?:$|,))+$";
            //var delstr = @"(?:^|,)(?=[^""]|("")?)""?((?(1)[^""]*|[^,""]*))""?(?=,|$)";
            var delstr = "(?:" + _delimiter + "|\\n|^)(\"(?:(?:\"\")*[^\"]*)*\"|[^\"" + _delimiter + "\\n]*|(?:\\n|$))";

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
            var ms = DelimiterRegex.Matches(str);
            var res = new string[ms.Count];
            for (int i = 0; i < ms.Count; i++)
                res[i] = ms[i].Value.TrimStart(_delimiterChar);
            return res;
        }
    }
}
