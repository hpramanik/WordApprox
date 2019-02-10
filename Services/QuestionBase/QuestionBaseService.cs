using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordApprox_Core.Models;
using WordApprox_Core.Services.Core.Classifier;

namespace WordApprox_Core.Services.QuestionBase
{
    public class QuestionBaseService
    {
        private const string ExceptionMessageAnswer1 = "Answer cannot be empty or null.";
        private const string ExceptionMessageQuestion1 = "Question cannot be empty or null.";
        private const string ExceptionMessageQuestion2 = "One question cannot have multiple answers.";
        private const string ExceptionMessageQuestion3 = "Question Id cannot be null or empty";
        private const string ExceptionMessageQuestion4 = "Question cannot be null or empty";
        private readonly QuestionBaseModel _questionBase;
        private readonly DamerauLevensteinClassifier _classifier;

        public QuestionBaseService(QuestionBaseModel questionBase, DamerauLevensteinClassifier classifier, Dictionary<string, string> FAQRawData = null)
        {
            _questionBase = questionBase ?? throw new ArgumentNullException(nameof(questionBase));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            AddFAQ(FAQRawData);
        }

        public virtual string GetQuestionIdByQuestion(string question)
        {
            if (string.IsNullOrEmpty(question))
            {
                throw new ArgumentException(ExceptionMessageQuestion1, nameof(question));
            }

            question = question.Trim().Replace("\n", string.Empty).ToLowerInvariant();

            if(_questionBase.Questions != null)
            {
                foreach (QuestionModel eachQuestion in _questionBase.Questions.Values)
                {
                    if (eachQuestion.Question.Equals(question))
                    {
                        return eachQuestion.QuestionId.ToString();
                    }
                }
            }

            return null;
        }

        public QuestionModel GetQuestionById(Guid questionId)
        {
            if (_questionBase.Questions.ContainsKey(questionId))
            {
                return _questionBase.Questions[questionId];
            }

            return null;
        }

        public HashSet<QuestionModel> GetQuestionByIds(HashSet<Guid> questionIds)
        {
            HashSet<QuestionModel> questions = new HashSet<QuestionModel>();
            foreach (Guid questionId in questionIds)
            {
                questions.Add(GetQuestionById(questionId));
            }
            
            return questions;
        }

        public virtual string GetAnswerIdByAnswer(string answer)
        {
            if (string.IsNullOrEmpty(answer))
            {
                throw new ArgumentException(ExceptionMessageAnswer1, nameof(answer));
            }

            answer = answer.Trim().Replace("\n", string.Empty).ToLowerInvariant();

            if (_questionBase.Answers != null)
            {
                foreach (AnswerModel eachAnswer in _questionBase.Answers.Values)
                {
                    if (eachAnswer.Answer.Equals(answer))
                    {
                        return eachAnswer.AnswerId.ToString();
                    }
                }
            }

            return null;
        }

        public virtual KeyValuePair<string, string> AddFAQ(string question, string answer)
        {
            KeyValuePair<string, string> result = new KeyValuePair<string, string>(null, null);
            if (string.IsNullOrEmpty(question))
            {
                throw new System.ArgumentException(ExceptionMessageQuestion1, nameof(question));
            }

            if (string.IsNullOrEmpty(answer))
            {
                throw new System.ArgumentException(ExceptionMessageAnswer1, nameof(answer));
            }

            string questionId = GetQuestionIdByQuestion(question);
            string answerId = GetAnswerIdByAnswer(answer);
            if (_questionBase.Answers == null)
            {
                _questionBase.Answers = new Dictionary<Guid, AnswerModel>();
            }

            if (_questionBase.Questions == null)
            {
                _questionBase.Questions = new Dictionary<Guid, QuestionModel>();
            }

            if (_questionBase.QuestionToAnswerMap == null)
            {
                _questionBase.QuestionToAnswerMap = new Dictionary<Guid, Guid>();
            }

            if (_questionBase.AnswerToQuestionsMap == null)
            {
                _questionBase.AnswerToQuestionsMap = new Dictionary<Guid, HashSet<Guid>>();
            }

            if (questionId == null)
            {
                QuestionModel questionModel = new QuestionModel(question);
                AnswerModel answerModel = null;
                if (answerId != null)
                {
                    answerModel = _questionBase.Answers[Guid.Parse(answerId)];
                }
                else
                {
                    answerModel = new AnswerModel(answer);
                    _questionBase.Answers.Add(answerModel.AnswerId, answerModel);
                }

                _questionBase.Questions.Add(questionModel.QuestionId, questionModel);
                _questionBase.QuestionToAnswerMap.Add(questionModel.QuestionId, answerModel.AnswerId);
                if (_questionBase.AnswerToQuestionsMap.ContainsKey(answerModel.AnswerId))
                {
                    _questionBase.AnswerToQuestionsMap[answerModel.AnswerId].Add(questionModel.QuestionId);
                }
                else
                {
                    _questionBase.AnswerToQuestionsMap.Add(answerModel.AnswerId, new HashSet<Guid> { questionModel.QuestionId });
                }

                result = new KeyValuePair<string, string>(questionModel.QuestionId.ToString(), answerModel.AnswerId.ToString());
            }
            else
            {
                if (answerId == null)
                {
                    throw new InvalidOperationException(ExceptionMessageQuestion2);
                }
                else
                {
                    if (_questionBase.QuestionToAnswerMap[Guid.Parse(questionId)].Equals(Guid.Parse(answerId)))
                    {
                        result = new KeyValuePair<string, string>(questionId, answerId);
                    }
                    else
                    {
                        throw new InvalidOperationException(ExceptionMessageQuestion2);
                    }
                }
            }

            return result;
        }

        public virtual Dictionary<string, string> AddFAQ(Dictionary<string, string> FAQs)
        {
            Dictionary<string, string> FAQIds = new Dictionary<string, string>();
            foreach (var FAQ in FAQs)
            {
                var addOperation = AddFAQ(FAQ.Key, FAQ.Value);
                FAQIds.Add(addOperation.Key, addOperation.Value);
            }

            return FAQIds;
        }

        public virtual QuestionModel RemoveQuestion(Guid questionId, out AnswerModel associatedAnswer)
        {
            QuestionModel question = null;
            associatedAnswer = null;
            if(_questionBase.Questions.ContainsKey(questionId))
            {
                question = _questionBase.Questions[questionId];
                _questionBase.Questions.Remove(questionId);
            }

            if (_questionBase.QuestionToAnswerMap.TryGetValue(questionId, out Guid answerId))
            {
                _questionBase.QuestionToAnswerMap.Remove(questionId);
                _questionBase.AnswerToQuestionsMap[answerId].Remove(questionId);
                if (_questionBase.AnswerToQuestionsMap[answerId].Count == 0)
                {
                    associatedAnswer = _questionBase.Answers[answerId];
                    _questionBase.Answers.Remove(answerId);
                    _questionBase.AnswerToQuestionsMap.Remove(answerId);
                }
            }

            return question;
        }

        public virtual QuestionModel RemoveQuestion(string questionString, out AnswerModel associatedAnswer)
        {
            string questionId = GetQuestionIdByQuestion(questionString);
            associatedAnswer = null;
            QuestionModel question = null;
            if (!string.IsNullOrEmpty(questionId))
            {
                question = RemoveQuestion(Guid.Parse(questionId), out associatedAnswer);
            }

            return question;
        }

        public virtual bool UpdateQuestion(string questionIdString, string updatedQuestion)
        {
            bool result = false;
            if (string.IsNullOrEmpty(questionIdString))
            {
                throw new ArgumentException(ExceptionMessageQuestion3, nameof(questionIdString));
            }

            if (string.IsNullOrEmpty(updatedQuestion))
            {
                throw new ArgumentException(ExceptionMessageQuestion4, nameof(updatedQuestion));
            }

            Guid questionId = Guid.Parse(questionIdString);

            if (_questionBase.Questions.ContainsKey(questionId))
            {
                QuestionModel question = _questionBase.Questions[questionId];
                string questionIdMatching = GetQuestionIdByQuestion(updatedQuestion);
                bool isNotAlreadyPresent = string.IsNullOrEmpty(questionIdMatching);
                if (isNotAlreadyPresent)
                {
                    question.UpdateQuestion(updatedQuestion);
                }
                else
                {
                    throw new InvalidOperationException($"Updated question matches other question in QuestionBase : {questionIdMatching}");
                }
            }

            return result;

        }

        public virtual AnswerModel RemoveAnswer(Guid answerId, out List<QuestionModel> associatedQuestions)
        {
            AnswerModel answer = null;
            associatedQuestions = new List<QuestionModel>();
            if (_questionBase.Answers.ContainsKey(answerId))
            {
                HashSet<Guid> questionIds = _questionBase.AnswerToQuestionsMap[answerId];
                foreach(var questionId in questionIds)
                {
                    associatedQuestions.Add(RemoveQuestion(questionId, out answer));
                }
            }

            return answer;
        }

        public virtual AnswerModel RemoveAnswer(string answerString, out List<QuestionModel> associatedQuestions)
        {
            AnswerModel answer = null;
            associatedQuestions = new List<QuestionModel>()
            ;
            string answerId = GetAnswerIdByAnswer(answerString);
            if (!string.IsNullOrEmpty(answerId))
            {
                answer = RemoveAnswer(Guid.Parse(answerId), out associatedQuestions);
            }

            return answer;
        }

        public async Task<List<FAQAnswer>> GetFAQAnswerAsync(string query, float _scoreThreshold = 0.5f, int _top = 1)
        {
            List<FAQAnswer> result = new List<FAQAnswer>();
            foreach (QuestionModel ques in _questionBase.Questions.Values)
            {
                _classifier.SetString1(ques.Question);
                _classifier.SetString2(query);
                float score = await _classifier.GetSentenceMatchResultAsync();
                if(score > _scoreThreshold)
                {
                    FAQAnswer ans = new FAQAnswer()
                    {
                        Score = score,
                        TopMatchedQuestion = ques.Question,
                        Questions = GetQuestionByIds(_questionBase.AnswerToQuestionsMap[_questionBase.QuestionToAnswerMap[ques.QuestionId]]),
                        Answer = _questionBase.Answers[_questionBase.QuestionToAnswerMap[ques.QuestionId]],
                    };
                    result.Add(ans);
                }
            }
            
            result = result.OrderByDescending(ob => ob.Score).ToList();
            HashSet<Guid> mapper = new HashSet<Guid>();
            List<FAQAnswer> filterSimilar = new List<FAQAnswer>();
            foreach(FAQAnswer answer in result)
            {
                if(answer.Answer != null && !mapper.Contains(answer.Answer.AnswerId))
                {
                    mapper.Add(answer.Answer.AnswerId);
                    filterSimilar.Add(answer);
                }
            }

            result = filterSimilar;

            List<FAQAnswer> outR = new List<FAQAnswer>();
            for (int count = 0; count < _top && count < result.Count; count++)
            {
                outR.Add(result[count]);
            }

            return outR;
        }
        public virtual Dictionary<Guid, HashSet<Guid>> GetAnswerToQuestionsMap()
        {
            return _questionBase.AnswerToQuestionsMap;
        }

        public virtual Dictionary<Guid, Guid> GetQuestionToAnswerMap()
        {
            return _questionBase.QuestionToAnswerMap;
        }

        public virtual Dictionary<Guid, QuestionModel> GetQuestions()
        {
            return _questionBase.Questions;
        }

        public virtual Dictionary<Guid, AnswerModel> GetAnswers()
        {
            return _questionBase.Answers;
        }

        public virtual Guid GetQusetionBaseId()
        {
            return _questionBase.QuestionBaseId;
        }

        public virtual string GetQuestionBaseName()
        {
            return _questionBase.QuestionBaseName;
        }
    }
}
