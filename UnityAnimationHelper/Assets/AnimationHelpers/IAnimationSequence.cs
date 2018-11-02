using System.Collections;

namespace AnimationHelpers
{
    public interface IAnmationSequence
    {
        /// <summary>
        /// Start the animation manual
        /// </summary>
        void StartManualAnimation();
        /// <summary>
        /// Updates the animation values
        /// </summary>
        /// <returns>Is the animation still running (false when finished)</returns>
        bool UpdateAnimation();
        /// <summary>
        /// Reverse the animation
        /// </summary>
        void Reverse();
        /// <summary>
        /// Construct a coroutine which perfomece the animation every frame
        /// </summary>
        /// <returns></returns>
        IEnumerator ConstructCoroutine();
    }
}
