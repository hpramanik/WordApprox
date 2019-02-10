using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WordApprox_Core.Utilities
{
    public class FAQ_Service_Utility
    {
        public static List<string> Splitter(string stringToSplit)
        {
            if (string.IsNullOrEmpty(stringToSplit))
            {
                throw new System.ArgumentNullException(nameof(stringToSplit));
            }

            List<string> result = new List<string>();
            var pattern = new Regex(
                @"( [^\W_\d]              # starting with a letter
                                        # followed by a run of either...
                ( [^\W_\d] |          #   more letters or
                    [-'\d](?=[^\W_\d])  #   ', -, or digit followed by a letter
                )*
                [^\W_\d]              # and finishing with a letter
                )",
            RegexOptions.IgnorePatternWhitespace);

            foreach (Match m in pattern.Matches(stringToSplit))
            {
                result.Add(m.Groups[1].Value);
            }

            return result;
        }
        public static Dictionary<string, string> GetQuestionAnswerMap(List<string> paths)
        {
            if (paths == null)
            {
                throw new System.ArgumentNullException(nameof(paths));
            }

            Dictionary<string, string> FAQS = new Dictionary<string, string>();
            foreach (string path in paths)
            {
                string rawData = File.ReadAllText(path);
                string[] eachLines = rawData.Split('\n');
                for (int line = 1; line < eachLines.Length; line++)
                {
                    string[] lineSplit = eachLines[line].Split('\t');
                    if (lineSplit.Length > 2 && !FAQS.ContainsKey(lineSplit[0]))
                    {
                        FAQS.Add(lineSplit[0], lineSplit[1]);
                    }
                }
            }

            return FAQS;
        }

        public static Dictionary<string, string> GetQuestionAnswerMap(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new System.ArgumentException("Path cannot be null.", nameof(path));
            }

            return GetQuestionAnswerMap(new List<string>{ path });
        }
    }
}