using UnityEngine;

namespace AnimationHelpers
{
    /// <summary>
    /// Simple extentions for the Unity Color classs
    /// </summary>
    public static class ColorExtention
    {
        /// <summary>
        /// Creates a new color object based on the given color with a new alpha value.
        /// </summary>
        /// <param name="color">The base color</param>
        /// <param name="value">The alpha value [0, 1]</param>
        /// <returns>A modified Color value with the new alpha value</returns>
        public static Color SetAlpha(this Color color, float value)
        {
            return new Color(color.r, color.g, color.b, value);
        }

        /// <summary>
        /// Calculates the Manhattan (L1 norm) distance of a color
        /// </summary>
        /// <param name="color">The first color</param>
        /// <param name="otherColor">The second color</param>
        /// <returns>The distance between the two colors</returns>
        public static float Distance(this Color color, Color otherColor)
        {
            float dist = 0f;
            for (int i = 0; i < 4; i++)
            {
                dist += Mathf.Abs(color[i] - otherColor[i]);
            }
            return dist;
        }
    }
}
