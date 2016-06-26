namespace qHUD.Framework
{
    public class IntRange
    {
        public int Min { get; }
        public int Max { get; }

        public IntRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public override string ToString()
        {
            return string.Concat(Min, " - ", Max);
        }

        internal bool HasSpread()
        {
            return Max != Min;
        }
    }
}