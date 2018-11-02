using System.Collections;
using AnimationHelpers;
using TMPro;
using UnityEngine;

namespace Assets.AnimationHelpers.Sample.Scripts
{
    [RequireComponent(typeof(TMP_Text))]
    public class ValueAnimationExample : MonoBehaviour
    {
        private TMP_Text _text;

        private void Start()
        {
            _text = GetComponent<TMP_Text>();
            StartCoroutine(Count());
        }

        private IEnumerator Count()
        {
            var count = 0f;
            //create a new value Animator
            var animator = count.CreateValueDurationAnimation(5, 1000);

            //start the animation manually
            animator.StartManualAnimation();

            //update the text as long the animation is running
            while (animator.UpdateAnimation())
            {
                _text.text = animator.CurrentValue.ToString("F1");
                yield return new WaitForEndOfFrame();
            }

            //you can reverse the animation
            animator.Reverse();
            animator.StartManualAnimation();

            //The UpdateAnimation will return true when t reaches 1 therefore the value will never be 0. Use CalculateCurrentValue to prevent this issue.
            var isActive = true;
            while (isActive)
            {
                _text.text = animator.CalculateCurrentValue(out isActive).ToString("F1");
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
