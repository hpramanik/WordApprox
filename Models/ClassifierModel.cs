namespace WordApprox_Core.Models
{
    public class ClassifierModel
    {
        public int MaxChanges { get; set; } = 255;

        public float ShortSynnonymAcceptanceThreshold { get; set; } = 0.75F;

        public float StringLengthThreshold { get; set; } = 0.3F;

        public bool BreakOnCompleteMatch { get; set; } = false;

        public float FullStringMatchProportion { get; set; } = 0.3F;

        public float EachWordMatchProportion { get; set; } = 0.7F;

        public int MaxIterativeWordGroupValue { get; set; } = 5;
    }
}