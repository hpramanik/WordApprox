using System;
using System.Collections.Generic;

namespace WordApprox_Core.Models
{
    public class AnswerModel
    {
        public AnswerModel(string answer, string source = null, string metaInfo = null)
        {
            if (string.IsNullOrEmpty(answer))
            {
                throw new ArgumentException("Answer cannot be null or empty.", nameof(answer));
            }

            AnswerId = Guid.NewGuid();
            DisplayAnswer = answer;
            metaInfo = string.IsNullOrEmpty(metaInfo) ? string.Empty : metaInfo.Trim();
            if (!string.IsNullOrEmpty(metaInfo))
            {
                string[] splits = metaInfo.Split(';');
                foreach (string split in splits)
                {
                    string[] innerSplits = split.Split(':');
                    string metaInfoName = innerSplits[0];
                    string metaInfoValue = string.Empty;
                    if (innerSplits.Length > 1)
                    {
                        metaInfoValue = innerSplits[1];
                    }

                    MetaInfoMap.Add(metaInfoName, metaInfoValue);
                }
            }

            Source = source;
            Answer = DisplayAnswer.Trim().Replace("\n", string.Empty).ToLowerInvariant();
        }

        public Guid AnswerId { get; internal set; }

        public Dictionary<string, string> MetaInfoMap { get; internal set; } = new Dictionary<string, string>();

        public string Source { get; internal set; }

        public string Answer { get; internal set; }

        public string DisplayAnswer { get; internal set; }

        public void MergeMetaInfo(string metaInfo)
        {
            metaInfo = string.IsNullOrEmpty(metaInfo) ? string.Empty : metaInfo.Trim();
            if (!string.IsNullOrEmpty(metaInfo))
            {
                string[] splits = metaInfo.Split(';');
                foreach (string split in splits)
                {
                    string[] innerSplits = split.Split(':');
                    string metaInfoName = innerSplits[0];
                    string metaInfoValue = string.Empty;
                    if (innerSplits.Length > 1)
                    {
                        metaInfoValue = innerSplits[1];
                    }

                    if (!MetaInfoMap.ContainsKey(metaInfoName))
                    {
                        MetaInfoMap.Add(metaInfoName, metaInfoValue);
                    }
                }
            }
        }
        public bool MatchMetaInfo(string metaInfo)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(metaInfo))
            {
                string[] splits = metaInfo.Split(';');
                foreach (string split in splits)
                {
                    string[] innerSplits = split.Split(':');
                    string metaInfoName = innerSplits[0];
                    string metaInfoValue = null;
                    if (innerSplits.Length > 1)
                    {
                        metaInfoValue = innerSplits[1];
                    }

                    if (MetaInfoMap.ContainsKey(metaInfoName))
                    {
                        if (string.IsNullOrEmpty(metaInfoValue))
                        {
                            result = string.IsNullOrEmpty(MetaInfoMap[metaInfoName]);
                        }
                        else
                        {
                            result = metaInfoValue.Equals(MetaInfoMap[metaInfoName]);
                        }
                    }
                }
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            AnswerModel objM = obj as AnswerModel;
            return this.Answer.Equals(objM.Answer);
        }

        public override int GetHashCode()
        {
            return AnswerId.GetHashCode();
        }
    }
}
