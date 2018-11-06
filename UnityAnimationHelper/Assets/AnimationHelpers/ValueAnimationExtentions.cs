using UnityEngine;

namespace AnimationHelpers
{
    /// <summary>
    /// Extension methods for the value animations
    /// </summary>
    public static class ValueAnimationExtentions
    {
        /// <summary>
        /// Create a value duration animation for the vector
        /// </summary>
        /// <param name="v">The vector</param>
        /// <param name="duration">The animation duration in seconds</param>
        /// <param name="end">The end value of the animation</param>
        /// <returns>A new ValueDurationAnimation</returns>
        public static ValueDurationAnimation<Vector3> CreateValueDurationAnimation(this Vector3 v, float duration, Vector3 end)
        {
            return new ValueDurationAnimation<Vector3>(duration, t => Vector3.Lerp(v, end, t));
        }

        /// <summary>
        /// Create a value speed animation for the vector.
        /// <remarks>
        /// The t value for the interpolation is calculated as follow: <code>travelTime * speed / travelDistance</code>
        /// Where the travelDistance is the Euclidean distance between v and end. 
        /// </remarks>
        /// </summary>
        /// <param name="v">The vector</param>
        /// <param name="speed">The speed of the animation.</param>
        /// <param name="end">The end value of the animation</param>
        /// <returns>A new ValueSpeedAnimation</returns>
        public static ValueSpeedAnimation<Vector3> CreateValueSpeedAnimation(this Vector3 v, float speed, Vector3 end)
        {
            return new ValueSpeedAnimation<Vector3>(speed, Vector3.Distance(v, end), t => Vector3.Lerp(v, end, t));
        }

        /// <summary>
        /// Create a value duration animation for the color.
        /// </summary>
        /// <param name="c">The color</param>
        /// <param name="duration">Duration of the animation</param>
        /// <param name="end">The end value of the animation</param>
        /// <returns>A new ValueDurationAnimation</returns>
        public static ValueDurationAnimation<Color> CreateValueDurationAnimation(this Color c, float duration,
            Color end)
        {
            return new ValueDurationAnimation<Color>(duration, t => Color.Lerp(c, end, t));
        }

        /// <summary>
        /// Create a value speed animation for the color
        /// <remarks>
        /// The t value for the interpolation is calculated as follow: <code>travelTime * speed / travelDistance</code>
        /// Where the travelDistance is the Manhattan distance between c and end. 
        /// </remarks>
        /// </summary>
        /// <param name="c">The color</param>
        /// <param name="speed">The speed of the animation.</param>
        /// <param name="end">The end value of the animation</param>
        /// <returns>A new ValueSpeedAnimation</returns>
        public static ValueSpeedAnimation<Color> CreateValueSpeedAnimation(this Color c, float speed, Color end)
        {
            return new ValueSpeedAnimation<Color>(speed, c.Distance(end), t => Color.Lerp(c, end, t));
        }

        /// <summary>
        /// Create a value duration animation for the float value.
        /// </summary>
        /// <param name="f">The float value</param>
        /// <param name="duration">Duration of the animation</param>
        /// <param name="end">The end value of the animation</param>
        /// <returns>A new ValueDurationAnimation</returns>
        public static ValueDurationAnimation<float> CreateValueDurationAnimation(this float f, float duration, float end)
        {
            return new ValueDurationAnimation<float>(duration, t => Mathf.Lerp(f, end, t));
        }

        /// <summary>
        /// Create a value speed animation for the float value
        /// <remarks>
        /// The t value for the interpolation is calculated as follow: <code>travelTime * speed / travelDistance</code>
        /// Where the travelDistance is the absolute difference between f and end <code>abs(f - end)</code>. 
        /// </remarks>
        /// </summary>
        /// <param name="f">The float value</param>
        /// <param name="speed">The speed of the animation.</param>
        /// <param name="end">The end value of the animation</param>
        /// <returns>A new ValueSpeedAnimation</returns>
        public static ValueSpeedAnimation<float> CreateValueSpeedAnimation(this float f, float speed, float end)
        {
            return new ValueSpeedAnimation<float>(speed, Mathf.Abs(f - end), t => Mathf.Lerp(f, end, t));
        }
    }
}
