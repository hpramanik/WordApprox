using System;
using System.Collections.Generic;
using WordApprox_Core.Utilities;

namespace WordApprox_Core.Models
{
    public class QuestionModel
    {
        public QuestionModel(string question)
        {
            if (string.IsNullOrEmpty(question))
            {
                throw new ArgumentException("Question cannot be null or empty.", nameof(question));
            }

            QuestionId = Guid.NewGuid();
            DisplayQuestion = question;
            Question = DisplayQuestion.Trim().Replace("\n", string.Empty).ToLowerInvariant();
        }

        public Guid QuestionId { get; internal set; }

        public string Question { get; internal set; }

        public string DisplayQuestion { get; internal set; }

        public void UpdateQuestion(string question)
        {
            DisplayQuestion = question;
            Question = DisplayQuestion.Trim().Replace("\n", string.Empty).ToLowerInvariant();
        }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            QuestionModel objM = obj as QuestionModel;
            return this.Question.Equals(objM.Question);
        }
        
        public override int GetHashCode()
        {
            return QuestionId.GetHashCode();
        }
    }
}