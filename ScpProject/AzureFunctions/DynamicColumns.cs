using System;

namespace AzureFunctions
{
    public struct DynamicColumns
    {
        public bool turnOnSets;
        public bool turnOnReps;
        public bool turnOnWeight;
        public bool turnOnTime;
        public bool turnOnDistance;
        public bool turnOnRepsAchieved;
        //public bool turnOnOther;
        public int dynamicColCount;

        public string GetWeekCSS()
        {
            var shit = GetWeekSize();
            return $"week{shit.Item1}a{shit.Item2}c";
        }
        /// <summary>
        /// a/item_1 columns are columns that are 25px, c/item_2 columns are columsn that are 65px;
        /// </summary>
        /// <returns>item_1 = a columns (25px) , item_b = c columns (65px)</returns>
        public Tuple<int, int> GetWeekSize()
        {
            var a = 0;
            var c = 0;

            a += turnOnSets ? 1 : 0;
            a += turnOnReps ? 1 : 0;
            a += turnOnWeight ? 1 : 0;
            a += turnOnRepsAchieved ? 1 : 0;

            c += turnOnDistance ? 1 : 0;
            c += turnOnRepsAchieved ? 1 : 0;

            return new Tuple<int, int>(a, c);
        }
        public static bool operator <(DynamicColumns left, DynamicColumns right)
        {
            var leftWeekSize = left.GetWeekSize();
            var rightWeekSize = right.GetWeekSize();
            return (leftWeekSize.Item1 * 25 + leftWeekSize.Item2 * 65) < (rightWeekSize.Item1 * 25 + rightWeekSize.Item2 * 65);
        }
        public static bool operator >(DynamicColumns left, DynamicColumns right)
        {
            var leftWeekSize = left.GetWeekSize();
            var rightWeekSize = right.GetWeekSize();
            return (leftWeekSize.Item1 * 25 + leftWeekSize.Item2 * 65) > (rightWeekSize.Item1 * 25 + rightWeekSize.Item2 * 65);
        }
        public static bool operator <=(DynamicColumns left, DynamicColumns right)
        {
            var leftWeekSize = left.GetWeekSize();
            var rightWeekSize = right.GetWeekSize();
            if ((leftWeekSize.Item1 * 25 + leftWeekSize.Item2 * 65) == (rightWeekSize.Item1 * 25 + rightWeekSize.Item2 * 65))
                return true;
            return (leftWeekSize.Item1 * 25 + leftWeekSize.Item2 * 65) < (rightWeekSize.Item1 * 25 + rightWeekSize.Item2 * 65);
        }
        public static bool operator >=(DynamicColumns left, DynamicColumns right)
        {
            var leftWeekSize = left.GetWeekSize();
            var rightWeekSize = right.GetWeekSize();
            if ((leftWeekSize.Item1 * 25 + leftWeekSize.Item2 * 65) == (rightWeekSize.Item1 * 25 + rightWeekSize.Item2 * 65))
                return true;
            return (leftWeekSize.Item1 * 25 + leftWeekSize.Item2 * 65) > (rightWeekSize.Item1 * 25 + rightWeekSize.Item2 * 65);
        }
        public static bool operator ==(DynamicColumns left, DynamicColumns right)
        {
            var leftWeekSize = left.GetWeekSize();
            var rightWeekSize = right.GetWeekSize();
            return (leftWeekSize.Item1 * 25 + leftWeekSize.Item2 * 65) == (rightWeekSize.Item1 * 25 + rightWeekSize.Item2 * 65);
        }
        public static bool operator !=(DynamicColumns left, DynamicColumns right)
        {
            var leftWeekSize = left.GetWeekSize();
            var rightWeekSize = right.GetWeekSize();
            return (leftWeekSize.Item1 * 25 + leftWeekSize.Item2 * 65) != (rightWeekSize.Item1 * 25 + rightWeekSize.Item2 * 65);
        }
    }
}

