using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using WordApprox_Core.Models;

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
        public static HashSet<UnmappedQuestionAnswerModel> GetQuestionAnswerMap(List<string> paths)
        {
            if (paths == null)
            {
                throw new System.ArgumentNullException(nameof(paths));
            }

            HashSet<UnmappedQuestionAnswerModel> FAQS = new HashSet<UnmappedQuestionAnswerModel>();
            HashSet<string> questionInserted = new HashSet<string>();
            foreach (string path in paths)
            {
                string rawData = File.ReadAllText(path);
                string[] eachLines = rawData.Split('\n');
                for (int line = 1; line < eachLines.Length; line++)
                {
                    string[] lineSplit = eachLines[line].Split('\t');
                    if (lineSplit.Length > 2 && !questionInserted.Contains(lineSplit[0]))
                    {
                        questionInserted.Add(lineSplit[0]);
                        UnmappedQuestionAnswerModel model = new UnmappedQuestionAnswerModel()
                        {
                            Question = lineSplit[0],
                            Answer = lineSplit[1],
                        };

                        if (lineSplit.Length >= 3)
                        {
                            model.Source = string.IsNullOrEmpty(lineSplit[2]) ? null : lineSplit[2];
                            if (lineSplit.Length >= 4)
                            {
                                model.MetaInfo = string.IsNullOrEmpty(lineSplit[3]) ? null : lineSplit[3];
                            }
                        }

                        FAQS.Add(model);
                    }
                }
            }

            return FAQS;
        }

        public static HashSet<UnmappedQuestionAnswerModel> GetQuestionAnswerMap(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new System.ArgumentException("Path cannot be null.", nameof(path));
            }

            return GetQuestionAnswerMap(new List<string>{ path });
        }
    }
}