namespace WordApprox_Core.Models
{
	public class UnmappedQuestionAnswerModel
	{
		public string Question { get; set; }

		public string Answer { get; set; }

		public string Source { get; set; }

		public string MetaInfo { get; set; }

		public override int GetHashCode()
		{
			int hash = Question.GetHashCode() * Answer.GetHashCode();
			return hash;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is UnmappedQuestionAnswerModel))
			{
				return false;
			}

			UnmappedQuestionAnswerModel model = obj as UnmappedQuestionAnswerModel;

			return (model.Answer.Equals(this.Answer) && model.Question.Equals(this.Question));
		}
	}
}