using System;
using System.Collections.Generic;

namespace WordApprox_Core.Models
{
    public class AnswerModel
    {
        public AnswerModel(string answer)
        {
            if (string.IsNullOrEmpty(answer))
            {
                throw new ArgumentException("Answer cannot be null or empty.", nameof(answer));
            }

            AnswerId = Guid.NewGuid();
            DisplayAnswer = answer;
            Answer = DisplayAnswer.Trim().Replace("\n", string.Empty).ToLowerInvariant();
        }

        public Guid AnswerId { get; internal set; }
        
        public string Answer { get; internal set; }

        public string DisplayAnswer { get; internal set; }

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