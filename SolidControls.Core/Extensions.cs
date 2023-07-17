using System;

namespace Stardust.OpenSource.SolidControls.Core
{
    public static class Extensions
    {
        public static bool IsBetween<T>(this T value, T border1, T border2, bool inclusive = true) where T : IComparable<T> {
            return inclusive
                ? Math.Abs(Math.Sign(value.CompareTo(border1)) + Math.Sign(value.CompareTo(border2))) < 2
                : Math.Sign(value.CompareTo(border1)) + Math.Sign(value.CompareTo(border2)) == 0;
        }
    }
}
