using System;
using System.Collections;
using UnityEngine;


// Animation Sequences are a sequence of steps where a step is a single update/manipulation.
namespace AnimationHelpers
{
    /// <summary>
    /// Abstract class for a animation which lives for a certain duration.
    /// </summary>
    public abstract class DurationAnimationSequence : IAnmationSequence
    {
        protected readonly float _duration;
        private bool _reversed;
        private float _startTime;
        private bool _manualAnmationStated;

        /// <summary>
        /// Create a new instance of DurationAnimationSequence
        /// </summary>
        /// <param name="duration">The duration in seconds</param>
        /// <param name="reversed">If true start the animation in reversed state</param>
        public DurationAnimationSequence(float duration, bool reversed = false)
        {
            _duration = duration;
            _reversed = reversed;
        }

        /// <summary>
        /// Start a manual animation. This will remember the time when this method is called.
        /// </summary>
        public void StartManualAnimation()
        {
            _startTime = Time.time;
            _manualAnmationStated = true;
        }

        /// <summary>
        /// Update the animation. 
        /// <remarks>This requires <see cref="StartManualAnimation"/> to be called before.</remarks>
        /// </summary>
        /// <returns>True if the animation is finished</returns>
        public bool UpdateAnimation()
        {
            if(!_manualAnmationStated) Debug.LogError($"Call first {nameof(StartManualAnimation)} before calling {nameof(UpdateAnimation)}");

            var t = (Time.time - _startTime) / _duration;
            Step(Mathf.Clamp01(t));
            return t <= 1;
        }

        /// <summary>
        /// Create a coroutine which executes the animation.
        /// Can be called by a MonoBehaviour with.
        /// <code>
        /// StartCoroutine(animation.ConstructCoroutine());
        /// </code>
        /// </summary>
        /// <returns>The IEnumarator for the coroutine</returns>
        public IEnumerator ConstructCoroutine()
        {
            var startTime = Time.time;

            float t = 0f;
            while (t <= 1)
            {
                t = (Time.time - startTime) / _duration;
                Step(Mathf.Clamp01(t));
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// reverse the animation
        /// <remarks>This requires <see cref="StartManualAnimation"/> to be called again before.</remarks>
        /// </summary>
        public void Reverse()
        {
            _reversed = !_reversed;
            _manualAnmationStated = false;
        }

        private void Step(float t)
        {
            AnimationStep((_reversed) ? 1 - t : t);
        }

        protected abstract void AnimationStep(float t);
    }

    /// <summary>
    /// Abstract class for an animation which uses speed for its animation. (Duration is different with a different distance)
    /// </summary>
    public abstract class SpeedAnimationSequence : IAnmationSequence
    {
        protected readonly float _speed;
        protected readonly float _travelDistance;
        private float _startTime;
        private bool _reversed;
        private bool _manualAnmationStated;


        /// <summary>
        /// Create a new instance of SpeedAnimationSequence
        /// The t value for the interpolation is calculated as follow: <code>travelTime * speed / travelDistance</code>
        /// </summary>
        /// <param name="speed">A multiplier which will be applied to the time the animation is running.</param>
        /// <param name="travelDistance">The distance the animation needs to travel</param>
        /// <param name="reversed">If true start the animation in reversed state</param>
        public SpeedAnimationSequence(float speed, float travelDistance, bool reversed = false)
        {
            _speed = speed;
            _travelDistance = travelDistance;
        }

        /// <summary>
        /// Start a manual animation. This will remember the time when this method is called.
        /// </summary>
        public void StartManualAnimation()
        {
            _startTime = Time.time;
            _manualAnmationStated = true;
        }

        /// <summary>
        /// Update the animation. 
        /// <remarks>This requires <see cref="StartManualAnimation"/> to be called before.</remarks>
        /// </summary>
        /// <returns>True if the animation is finished</returns>
        public bool UpdateAnimation()
        {
            if (!_manualAnmationStated) Debug.LogError($"Call first {nameof(StartManualAnimation)} before calling {nameof(UpdateAnimation)}");

            var distCovered = (Time.time - _startTime) * _speed;
            var t = distCovered / _travelDistance;
            Step(Mathf.Clamp01(t));

            return t <= 1;
        }

        /// <summary>
        /// Create a coroutine which executes the animation.
        /// Can be called by a MonoBehaviour with.
        /// <code>
        /// StartCoroutine(animation.ConstructCoroutine());
        /// </code>
        /// </summary>
        /// <returns>The IEnumarator for the coroutine</returns>
        public IEnumerator ConstructCoroutine()
        {
            var startTime = Time.time;
            var t = 0f;

            while (t <= 1)
            {
                var distCovered = (Time.time - startTime) * _speed;
                t = distCovered / _travelDistance;
                Step(Mathf.Clamp01(t));
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// reverse the animation
        /// <remarks>This requires <see cref="StartManualAnimation"/> to be called again before.</remarks>
        /// </summary>
        public void Reverse()
        {
            _reversed = !_reversed;
            _manualAnmationStated = false;
        }

        private void Step(float t)
        {
            AnimationStep((_reversed) ? 1 - t : t);
        }
        
        protected abstract void AnimationStep(float t);
    }

    /// <summary>
    /// A simple duration animation which only changes a value over time.
    /// </summary>
    /// <typeparam name="T">The type of the value to animate</typeparam>
    public class ValueDurationAnimation<T> : DurationAnimationSequence
    {
        private readonly Func<float, T> _lerpFunction;
        private bool _isTerminated;

        /// <summary>
        /// Returns the last calculated value.
        /// </summary>
        public T CurrentValue { get; private set; }
        
        /// <summary>
        /// Create a new instance of ValueDurationAnimation
        /// </summary>
        /// <param name="duration">The duration in seconds</param>
        /// <param name="lerpFunction">A function which takes a float as the parameter t of the interpolation and outputs the value T</param>
        /// <param name="reversed">If true start the animation in reversed state</param>
        public ValueDurationAnimation(float duration, Func<float, T> lerpFunction, bool reversed = false) : base(duration, reversed)
        {
            _lerpFunction = lerpFunction;
        }

        protected override void AnimationStep(float t)
        {
            CurrentValue = _lerpFunction(t);
        }

        /// <summary>
        /// Calculate the value for the current time. This will call UpdateAnimation and the returns the <see cref="CurrentValue"/>
        /// </summary>
        /// <param name="isActive">false when the animation is finished</param>
        /// <returns>The current value</returns>
        public T CalculateCurrentValue(out bool isActive)
        {
            isActive = UpdateAnimation();
            return CurrentValue;
        }
    }

    /// <summary>
    /// A simple speed animation which only changes a value over time.
    /// </summary>
    /// <typeparam name="T">The type of the value to animate</typeparam>
    public class ValueSpeedAnimation<T> : SpeedAnimationSequence
    {
        private readonly Func<float, T> _lerpFunction;

        /// <summary>
        /// Returns the last calculated value.
        /// </summary>
        public T CurrentValue { get; private set; }


        /// <summary>
        /// Create a new instance of ValueSpeedAnimation
        /// The t value for the interpolation is calculated as follow: <code>travelTime * speed / travelDistance</code>
        /// </summary>
        /// <param name="speed">A multiplier which will be applied to the time the animation is running.</param>
        /// <param name="distance">The distance the animation needs to travel</param>
        /// <param name="lerpFunction">A function which takes a float as the parameter t of the interpolation and outputs the value T</param>
        /// <param name="reversed">If true start the animation in reversed state</param>
        public ValueSpeedAnimation(float speed, float distance, Func<float, T> lerpFunction, bool reversed = false) : base(speed, distance, reversed)
        {
            _lerpFunction = lerpFunction;
        }

        protected override void AnimationStep(float t)
        {
            CurrentValue = _lerpFunction(t);
        }

        /// <summary>
        /// Calculate the value for the current time. This will call UpdateAnimation and the returns the <see cref="CurrentValue"/>
        /// </summary>
        /// <param name="isActive">false when the animation is finished</param>
        /// <returns>The current value</returns>
        public T CalculateCurrentValue(out bool isActive)
        {
            isActive = UpdateAnimation();
            return CurrentValue;
        }
    }

    /// <summary>
    /// Animation which waits for a given duration.
    /// </summary>
    public class WaitForSecondsSequence : IAnmationSequence
    {
        private readonly float _duration;
        private float _startTime;

        /// <summary>
        /// Crate a new instance of WaitForSecondsSequence
        /// </summary>
        /// <param name="duration">Duration in seconds</param>
        public WaitForSecondsSequence(float duration)
        {
            _duration = duration;
        }

        /// <inheritdoc />
        public void StartManualAnimation()
        {
            _startTime = Time.time;
        }

        /// <inheritdoc />
        public bool UpdateAnimation()
        {
            return _startTime + _duration >= Time.time;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Reverse() {}

        /// <inheritdoc />
        public IEnumerator ConstructCoroutine()
        {
            _startTime = Time.time;
            while (_startTime + _duration >= Time.time)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    /// <summary>
    /// Animation which waits till a certain condition is true.
    /// </summary>
    public class WaitForCondition : IAnmationSequence
    {
        /// <summary>
        /// Delegate for the condition takes the unity time as an parameter and returns a bool (waits till true)
        /// </summary>
        /// <param name="unityTime">Unity Time.time</param>
        /// <returns>The animation waits till this is true</returns>
        public delegate bool TimePredicate(float unityTime);

        private readonly TimePredicate _predicate;

        /// <summary>
        /// Create a new instance of WaitForCondition
        /// </summary>
        /// <param name="condition">The condition <see cref="TimePredicate"/></param>
        public WaitForCondition(TimePredicate condition)
        {
            _predicate = condition;
        }

        /// <summary>
        /// Does noting
        /// </summary>
        public void StartManualAnimation()
        {
        }

        /// <inheritdoc/>
        public bool UpdateAnimation()
        {
            return _predicate(Time.time);
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        public void Reverse() { }

        /// <inheritdoc cref="ConstructCoroutine"/>
        public IEnumerator ConstructCoroutine()
        {
            while (!_predicate(Time.time))
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    /// <summary>
    /// A animation which only is called once and then is finished.
    /// </summary>
    public class OneShotSquence : IAnmationSequence
    {
        private readonly Action _action;

        /// <summary>
        /// Create a new instance of OneShotSquence
        /// </summary>
        /// <param name="action">The action to perfome one time</param>
        public OneShotSquence(Action action)
        {
            _action = action;
        }

        /// <inheritdoc cref="StartManualAnimation"/>
        public void StartManualAnimation()
        {
            _action?.Invoke();
        }

        /// <summary>
        /// Will return always false.
        /// </summary>
        /// <returns>False</returns>
        public bool UpdateAnimation()
        {
            return false;
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        public void Reverse() { }

        /// <inheritdoc cref="ConstructCoroutine"/>
        public IEnumerator ConstructCoroutine()
        {
            yield return null;
            _action?.Invoke();
        }
    }
}