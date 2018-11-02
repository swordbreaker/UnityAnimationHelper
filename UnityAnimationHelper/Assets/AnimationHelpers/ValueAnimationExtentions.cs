using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace AnimationHelpers
{
    public static class ValueAnimationExtentions
    {
        public static ValueDurationAnimation<Vector3> CreateValueDurationAnimation(this Vector3 v, float duration, Vector3 end)
        {
            return new ValueDurationAnimation<Vector3>(duration, t => Vector3.Lerp(v, end, t));
        }

        public static ValueSpeedAnimation<Vector3> CreateValueSpeedAnimation(this Vector3 v, float speed, Vector3 end)
        {
            return new ValueSpeedAnimation<Vector3>(speed, Vector3.Distance(v, end), t => Vector3.Lerp(v, end, t));
        }

        public static ValueDurationAnimation<Color> CreateValueDurationAnimation(this Color c, float duration,
            Color end)
        {
            return new ValueDurationAnimation<Color>(duration, t => Color.Lerp(c, end, t));
        }

        public static ValueSpeedAnimation<Color> CreateValueSpeedAnimation(this Color c, float speed, Color end)
        {
            return new ValueSpeedAnimation<Color>(speed, c.Distance(end), t => Color.Lerp(c, end, t));
        }

        public static ValueDurationAnimation<float> CreateValueDurationAnimation(this float f, float duration, float end)
        {
            return new ValueDurationAnimation<float>(duration, t => Mathf.Lerp(f, end, t));
        }

        public static ValueSpeedAnimation<float> CreateValueSpeedAnimation(this float f, float speed, float end)
        {
            return new ValueSpeedAnimation<float>(speed, Mathf.Abs(f - end), t => Mathf.Lerp(f, end, t));
        }
    }
}
