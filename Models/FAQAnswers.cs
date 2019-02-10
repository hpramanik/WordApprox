using System.Collections.Generic;

namespace WordApprox_Core.Models
{
    public class FAQAnswer
    {
        public string TopMatchedQuestion { get; set; }

        public HashSet<QuestionModel> Questions { get; set; }

        public AnswerModel Answer { get; set; }

        public float Score { get; set;}

        public int CompareTo(object obj)
        {
            if (!(obj is FAQAnswer))
            {
                return -1;
            }
            
            FAQAnswer objAns = obj as FAQAnswer;
            float diff = Score - objAns.Score;
            return (int)diff;
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}