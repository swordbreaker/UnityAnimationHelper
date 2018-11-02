using System;
using System.Collections;
using UnityEngine;


// Animation Sequences are a sequence of steps where a step is a single update/manipulation.
namespace AnimationHelpers
{
    public abstract class DurationAnimationSequence : IAnmationSequence
    {
        protected readonly float _duration;
        private bool _reversed;
        private float _startTime;
        private bool _manualAnmationStated;

        public DurationAnimationSequence(float duration, bool reversed = false)
        {
            _duration = duration;
            _reversed = reversed;
        }

        public void StartManualAnimation()
        {
            _startTime = Time.time;
            _manualAnmationStated = true;
        }

        public bool UpdateAnimation()
        {
            if(!_manualAnmationStated) Debug.LogError($"Call first {nameof(StartManualAnimation)} before calling {nameof(UpdateAnimation)}");

            var t = (Time.time - _startTime) / _duration;
            Step(Mathf.Clamp01(t));
            return t <= 1;
        }

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

    public abstract class SpeedAnimationSequence : IAnmationSequence
    {
        protected readonly float _speed;
        protected readonly float _travelDistance;
        private float _startTime;
        private bool _reversed;
        private bool _manualAnmationStated;


        public SpeedAnimationSequence(float speed, float travelDistance, bool reversed = false)
        {
            _speed = speed;
            _travelDistance = travelDistance;
        }

        public void StartManualAnimation()
        {
            _startTime = Time.time;
            _manualAnmationStated = true;
        }

        public bool UpdateAnimation()
        {
            if (!_manualAnmationStated) Debug.LogError($"Call first {nameof(StartManualAnimation)} before calling {nameof(UpdateAnimation)}");

            var distCovered = (Time.time - _startTime) * _speed;
            var t = distCovered / _travelDistance;
            Step(Mathf.Clamp01(t));

            return t <= 1;
        }

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

    public class ValueDurationAnimation<T> : DurationAnimationSequence
    {
        private readonly Func<float, T> _lerpFunction;
        private bool _isTerminated;
        public T CurrentValue { get; private set; }

        public ValueDurationAnimation(float duration, Func<float, T> lerpFunction, bool reversed = false) : base(duration, reversed)
        {
            _lerpFunction = lerpFunction;
        }

        protected override void AnimationStep(float t)
        {
            CurrentValue = _lerpFunction(t);
        }

        public T CalculateCurrentValue(out bool isActive)
        {
            isActive = UpdateAnimation();
            return CurrentValue;
        }
    }

    public class ValueSpeedAnimation<T> : SpeedAnimationSequence
    {
        private readonly Func<float, T> _lerpFunction;
        public T CurrentValue { get; private set; }

        public ValueSpeedAnimation(float speed, float distance, Func<float, T> lerpFunction, bool reversed = false) : base(speed, distance, reversed)
        {
            _lerpFunction = lerpFunction;
        }

        protected override void AnimationStep(float t)
        {
            CurrentValue = _lerpFunction(t);
        }

        public T CalculateCurrentValue(out bool isActive)
        {
            isActive = UpdateAnimation();
            return CurrentValue;
        }
    }

    public class WaitForSecondsSequence : IAnmationSequence
    {
        private readonly float _duration;
        private float _startTime;

        public WaitForSecondsSequence(float duration)
        {
            _duration = duration;
        }

        public void StartManualAnimation()
        {
            _startTime = Time.time;
        }

        public bool UpdateAnimation()
        {
            return _startTime + _duration >= Time.time;
        }

        public void Reverse() {}

        public IEnumerator ConstructCoroutine()
        {
            _startTime = Time.time;
            while (_startTime + _duration >= Time.time)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public class WaitForCondition : IAnmationSequence
    {
        public delegate bool TimePredicate(float unityTime);

        private readonly TimePredicate _predicate;

        public WaitForCondition(TimePredicate condition)
        {
            _predicate = condition;
        }

        public void StartManualAnimation()
        {
        }

        public bool UpdateAnimation()
        {
            return _predicate(Time.time);
        }

        public void Reverse() { }

        public IEnumerator ConstructCoroutine()
        {
            while (!_predicate(Time.time))
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public class OneShotSquence : IAnmationSequence
    {
        private readonly Action _action;

        public OneShotSquence(Action action)
        {
            _action = action;
        }

        public void StartManualAnimation()
        {
            _action?.Invoke();
        }

        public bool UpdateAnimation()
        {
            return false;
        }

        public void Reverse() { }

        public IEnumerator ConstructCoroutine()
        {
            yield return null;
            _action?.Invoke();
        }
    }
}