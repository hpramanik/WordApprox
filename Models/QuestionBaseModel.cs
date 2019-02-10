using System;
using System.Collections.Generic;

namespace WordApprox_Core.Models
{
    public class QuestionBaseModel
    {
        public QuestionBaseModel(string questionBaseName)
        {
            if (string.IsNullOrEmpty(questionBaseName))
            {
                throw new ArgumentException("QuestionBase name cannot be null or empty.", nameof(questionBaseName));
            }

            QuestionBaseId = Guid.NewGuid();
            QuestionBaseName = questionBaseName.ToLowerInvariant();
        }

        public Guid QuestionBaseId { get; internal set; }

        public string QuestionBaseName { get; internal set; }

        public Dictionary<Guid, AnswerModel> Answers { get; set; }

        public Dictionary<Guid, QuestionModel> Questions { get; set; }

        public Dictionary<Guid, Guid> QuestionToAnswerMap { get; set; }

        public Dictionary<Guid, HashSet<Guid>> AnswerToQuestionsMap { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            QuestionBaseModel model = obj as QuestionBaseModel;
            return this.QuestionBaseId.Equals(model.QuestionBaseId);
        }
        
        public override int GetHashCode()
        {
            return QuestionBaseId.GetHashCode();
        }
    }
}