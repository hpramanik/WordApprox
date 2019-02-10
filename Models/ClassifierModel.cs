namespace WordApprox_Core.Models
{
    public class ClassifierModel
    {
        public int MaxChanges { get; set; } = 255;

        public float ShortSynnonymAcceptanceThreshold { get; set; } = 0.75F;

        public float StringLengthThreshold { get; set; } = 0.5F;

        public bool BreakOnCompleteMatch { get; set; } = false;
    }
}