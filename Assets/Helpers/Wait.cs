using UnityEngine;

namespace Assets.Helper
{
    public static class Wait
    {
        public static WaitForSeconds OneTick() => new WaitForSeconds(Interval.OneTick);
        public static WaitForSeconds Ticks(int amount) => new WaitForSeconds(Interval.OneTick * amount);
        public static WaitForSeconds For(float seconds) => new WaitForSeconds(seconds);

        public static readonly WaitForEndOfFrame eof = new WaitForEndOfFrame();
        public static object None() => eof;
    }
}