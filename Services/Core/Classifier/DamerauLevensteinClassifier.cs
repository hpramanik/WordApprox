using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WordApprox_Core.Models;
using WordApprox_Core.Services.Core.DamerauLevenstein;
using static WordApprox_Core.Utilities.FAQ_Service_Utility;

namespace WordApprox_Core.Services.Core.Classifier
{
    public class DamerauLevensteinClassifier
    {
        private readonly int MAX;
        private readonly float SHORTSYNNONYMACCEPTANCETHRESHOLD;
        private readonly float STRINGLENGTHTHRESHOLD;

        private readonly bool BREAKONCOMPLETEMATCH;

        private string _string1;
        private string _string2;

        private DamerauLevensteinMetric dLmetric = null;

        public DamerauLevensteinClassifier()
        : this(new ClassifierModel())
        {
            
        }

        public DamerauLevensteinClassifier(ClassifierModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            MAX = model.MaxChanges;
            SHORTSYNNONYMACCEPTANCETHRESHOLD = model.ShortSynnonymAcceptanceThreshold;
            STRINGLENGTHTHRESHOLD = model.StringLengthThreshold;
            BREAKONCOMPLETEMATCH = model.BreakOnCompleteMatch;
            dLmetric = new DamerauLevensteinMetric(new DamerauLevensteinMetricModel { MaxLength = MAX });
        }

        public void SetString1(string string1)
        {
            if (string.IsNullOrEmpty(string1))
            {
                throw new ArgumentException("String_1 cannot be empty", nameof(string1));
            }

            _string1 = string1.Trim().ToLower();
        }

        public void SetString2(string string2)
        {
            if (string.IsNullOrEmpty(string2))
            {
                throw new ArgumentException("String_2 cannot be empty", nameof(string2));
            }

            _string2 = string2.Trim().ToLower();
        }

        public async Task<float> GetResultAsync()
        {
            float result = 0F;
            int maxLen = _string1.Length;
            int numOfAlterations = await dLmetric.GetDistanceAsync(_string1, _string2, MAX);
            maxLen = maxLen > _string2.Length ? maxLen : _string2.Length;
            result = (float)(maxLen - numOfAlterations)/maxLen;
            return result;
        }

        public float GetResult()
        {
            float result = 0F;
            int maxLen = _string1.Length;
            int numOfAlterations = dLmetric.GetDistance(_string1, _string2, MAX);
            maxLen = maxLen > _string2.Length ? maxLen : _string2.Length;
            result = (float)(maxLen - numOfAlterations)/maxLen;
            return result;
        }

        private async Task<float> GetResultForSentencesAsync(string string1, string string2)
        {
            float result = 0F;
            int maxLen = string1.Length;
            maxLen = maxLen > string2.Length ? maxLen : string2.Length;
            int numOfAlterations = maxLen;
            float shortSynnonymSize = SHORTSYNNONYMACCEPTANCETHRESHOLD * maxLen;
            if (string1.Length >= shortSynnonymSize && string2.Length >= shortSynnonymSize)
            {
                numOfAlterations = await dLmetric.GetDistanceAsync(string1, string2, MAX);
            }

            result = (float)(maxLen - numOfAlterations)/maxLen;
            return result;
        }

        private float GetResultForSentences(string string1, string string2)
        {
            float result = 0F;
            int maxLen = string1.Length;
            maxLen = maxLen > string2.Length ? maxLen : string2.Length;
            int numOfAlterations = maxLen;
            float shortSynnonymSize = SHORTSYNNONYMACCEPTANCETHRESHOLD * maxLen;
            if (string1.Length >= shortSynnonymSize && string2.Length >= shortSynnonymSize)
            {
                numOfAlterations = dLmetric.GetDistance(string1, string2, MAX);
            }

            result = (float)(maxLen - numOfAlterations)/maxLen;
            return result;
        }

        public async Task<float> GetSentenceMatchResultAsync()
        {
            float result = 0F;
            int maxLength = _string1.Length;
            int minLength = _string1.Length;
            maxLength = _string1.Length > _string2.Length ? _string1.Length : _string2.Length;
            minLength = _string1.Length < _string2.Length ? _string1.Length : _string2.Length;

            if (maxLength * STRINGLENGTHTHRESHOLD > minLength)
            {
                return result;
            }

            List<string> string1SplitsWithDup = Splitter(_string1);
            List<string> string2SplitsWithDup = Splitter(_string2);
            HashSet<string> string1Splits = new HashSet<string>(string1SplitsWithDup);
            HashSet<string> string2Splits = new HashSet<string>(string2SplitsWithDup);
            if (string1Splits.Count < string2Splits.Count)
            {
                HashSet<string> temp = string1Splits;
                string1Splits = string2Splits;
                string2Splits = temp;
            }

            float[] matchMap = new float[string1Splits.Count];
            int count = 0;
            foreach (string split1 in string1Splits)
            {
                matchMap[count] = 0;
                foreach (string split2 in string2Splits)
                {
                    float currMatch = 0;
                    if (split1[0] == split2[0])
                    {
                        currMatch = await GetResultForSentencesAsync(split1, split2);
                    }

                    matchMap[count] = currMatch > matchMap[count] ? currMatch : matchMap[count];
                    if (BREAKONCOMPLETEMATCH && currMatch == 1)
                    {
                        break;
                    }
                }
                count++;
            }

            foreach (float eachWordMaxMatch in matchMap)
            {
                result += eachWordMaxMatch;
            }

            result /= matchMap.Length;
            result += await GetResultAsync();
            result /= 2;

            return result;
        }

        public float GetSentenceMatchResult()
        {
            float result = 0F;
            int maxLength = _string1.Length;
            int minLength = _string1.Length;
            maxLength = _string1.Length > _string2.Length ? _string1.Length : _string2.Length;
            minLength = _string1.Length < _string2.Length ? _string1.Length : _string2.Length;

            if (maxLength * STRINGLENGTHTHRESHOLD > minLength)
            {
                return result;
            }

            List<string> string1SplitsWithDup = Splitter(_string1);
            List<string> string2SplitsWithDup = Splitter(_string2);
            HashSet<string> string1Splits = new HashSet<string>(string1SplitsWithDup);
            HashSet<string> string2Splits = new HashSet<string>(string2SplitsWithDup);
            if (string1Splits.Count < string2Splits.Count)
            {
                HashSet<string> temp = string1Splits;
                string1Splits = string2Splits;
                string2Splits = temp;
            }

            float[] matchMap = new float[string1Splits.Count];
            int count = 0;
            foreach (string split1 in string1Splits)
            {
                matchMap[count] = 0;
                foreach (string split2 in string2Splits)
                {
                    float currMatch = 0;
                    if (split1[0] == split2[0])
                    {
                        currMatch = GetResultForSentences(split1, split2);
                    }

                    matchMap[count] = currMatch > matchMap[count] ? currMatch : matchMap[count];
                    if (currMatch == 1)
                    {
                        break;
                    }
                }
                count++;
            }

            foreach (float eachWordMaxMatch in matchMap)
            {
                result += eachWordMaxMatch;
            }

            result /= matchMap.Length;
            result += GetResult();
            result /= 2;

            return result;
        }
    }
}