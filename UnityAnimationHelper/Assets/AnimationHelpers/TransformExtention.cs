using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimationHelpers
{
    public static class TransformExtention
    {
        public static TransformAnimator Animate(this Transform transform)
        {
            return TransformAnimator.Create(transform);
        }
    }

    public class TransformAnimator : MonoBehaviour, IDisposable
    {
        internal Transform Transform { get; private set; }
        private readonly List<IAnmationSequence> _sequences = new List<IAnmationSequence>();

        private bool _disposed = false;

        public Coroutine CurrentCoroutine { get; private set; }

        private TransformAnimator()
        {
        }

        public static TransformAnimator Create(Transform transform)
        {
            var go = new GameObject("TransformAnimator", typeof(TransformAnimator));
            var transformAnimator = go.GetComponent<TransformAnimator>();
            transformAnimator.Transform = transform;
            return go.GetComponent<TransformAnimator>();
        }

        public TransformAnimator Move(Vector3 pos, float duration, Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);

            Sequence(duration).Move(Transform.position, pos, easeFunction);
            return this;
        }

        public TransformAnimator MoveWithSpeed(Vector3 pos, float speed, Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            _sequences.Add(
                TransformAnimatorSpeedSequence.CreateMoveAnimation(this, transform.position, pos, speed, easeFunction));
            return this;
        }

        public TransformAnimator Scale(Vector3 scale, float duration, Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);

            Sequence(duration).Scale(Transform.localScale, scale, easeFunction);
            return this;
        }

        public TransformAnimator ScaleWithSpeed(Vector3 scale, float speed, Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            _sequences.Add(TransformAnimatorSpeedSequence.CreateScaleAnimation(this, Transform.localScale, scale, speed,
                easeFunction));
            return this;
        }

        public TransformAnimator LocalMove(Vector3 pos, float duration, Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);

            Sequence(duration).LocalMove(Transform.localPosition, pos, easeFunction);
            return this;
        }

        /// <summary>
        /// Move the transform along a path. The start point will be the current position of the Transform (Do not include this position in the path IEnumerable)
        /// Example how the different parameters will be applied
        /// <code>
        /// transform.postion                           path[0]
        /// o ------------------durations[0]-----------►•
        ///                                             │
        ///                                        durations[1]
        ///                                             │
        ///                                             ▼
        /// •◄------------------durations[2]------------•
        /// path[2]                                     path[1]
        /// </code>
        /// </summary>
        /// <param name="path">The path points without the current transform position</param>
        /// <param name="durations">The duration </param>
        /// <param name="easeFunction">The ease function will be applied for each transition form point to point and not for the hole animation</param>
        /// <returns>The TramsformAnimator</returns>
        public TransformAnimator MovePath(IList<Vector3> path, IList<float> durations,
            Func<float, float> easeFunction = null)
        {
            Debug.Assert(path.Count == durations.Count,
                "The path list need to be the same length as the duration list");
            var currentPos = Transform.position;

            for (int i = 0; i < path.Count; i++)
            {
                Sequence(durations[i]).Move(currentPos, path[i], easeFunction);
                currentPos = path[i];
            }
            return this;
        }

        /// <summary>
        /// Move the transform along a path. The start point will be the current position of the Transform (Do not include this position in the path IEnumerable)
        /// Example how the different parameters will be applied
        /// <code>
        /// transform.postion                           path[0]
        /// o ------------------durations[0]-----------►•
        ///                                             │
        ///                                        durations[1]
        ///                                             │
        ///                                             ▼
        /// •◄------------------durations[2]------------•
        /// path[2]                                     path[1]
        /// </code>
        /// </summary>
        /// <param name="path">The path points without the current transform position</param>
        /// <param name="durations">The duration </param>
        /// <param name="easeFunction">The ease function will be applied for each transition form point to point and not for the hole animation</param>
        /// <returns>The TramsformAnimator</returns>
        public TransformAnimator LocalMovePath(IList<Vector3> path, IList<float> durations,
            Func<float, float> easeFunction = null)
        {
            Debug.Assert(path.Count == durations.Count,
                "The path list need to be the same length as the duration list");
            var currentPos = Transform.localPosition;

            for (int i = 0; i < path.Count; i++)
            {
                Sequence(durations[i]).LocalMove(currentPos, path[i], easeFunction);
                currentPos = path[i];
            }
            return this;
        }


        /// <summary>
        /// Move the transform along a path. The start point will be the current position of the Transform (Do not include this position in the path IEnumerable)
        /// </summary>
        /// <param name="path">The path points without the current transform position</param>
        /// <param name="durations">The duration of the animation</param>
        /// <param name="easeFunction">The ease function will be applied for each transition form point to point and not for the hole animation</param>
        /// <returns>The TramsformAnimator</returns>
        public TransformAnimator MovePath(IList<Vector3> path, float durations, Func<float, float> easeFunction = null)
        {
            return MovePath(path, Enumerable.Repeat(durations, path.Count).ToArray(), easeFunction);
        }

        public TransformAnimator MovePath(float duration, Func<float, float> easeFunction = null, params Vector3[] path)
        {
            return MovePath(path, Enumerable.Repeat(duration, path.Length).ToArray(), easeFunction);
        }

        /// <summary>
        /// Move the transform along a path. The start point will be the current position of the Transform (Do not include this position in the path IEnumerable)
        /// </summary>
        /// <param name="path">The path points without the current transform position</param>
        /// <param name="durations">The duration of the animation</param>
        /// <param name="easeFunction">The ease function will be applied for each transition form point to point and not for the hole animation</param>
        /// <returns>The TramsformAnimator</returns>
        public TransformAnimator LocalMovePath(IList<Vector3> path, float duration,
            Func<float, float> easeFunction = null)
        {
            return LocalMovePath(path, Enumerable.Repeat(duration, path.Count).ToArray(), easeFunction);
        }

        public TransformAnimator LocalMovePath(float duration, Func<float, float> easeFunction = null,
            params Vector3[] path)
        {
            return LocalMovePath(path, Enumerable.Repeat(duration, path.Length).ToArray(), easeFunction);
        }

        public TransformAnimator LocalMoveWithSpeed(Vector3 pos, float speed, Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            _sequences.Add(TransformAnimatorSpeedSequence.CreateLocalMoveAnimation(this, Transform.localPosition, pos,
                speed, easeFunction));
            return this;
        }

        public TransformAnimator Rotate(Quaternion endRotation, float duration, Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);

            Sequence(duration).Rotate(Transform.rotation, endRotation, easeFunction);
            return this;
        }

        public TransformAnimator RotateWithSpeed(Quaternion startRotaion, Quaternion endRotation, float speed,
            Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            _sequences.Add(
                TransformAnimatorSpeedSequence.CreateRotaionAnimation(this, startRotaion, endRotation, speed,
                    easeFunction));
            return this;
        }

        public TransformAnimator LocalRotate(Quaternion startRotaion, Quaternion endRotation, float duration,
            Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);

            Sequence(duration).LocalRotate(startRotaion, endRotation, easeFunction);
            return this;
        }

        public TransformAnimator LocalRotateWithSpeed(Quaternion startRotaion, Quaternion endRotation, float speed,
            Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            _sequences.Add(TransformAnimatorSpeedSequence.CreateLocalRotaionAnimation(this, startRotaion,
                endRotation, speed, easeFunction));
            return this;
        }

        public TransformAnimator Do(Action action)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            _sequences.Add(new OneShotSquence(action));
            return this;
        }

        public TransformAnimatorDurationSequence Sequence(float duration)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            var q = new TransformAnimatorDurationSequence(this, duration);
            _sequences.Add(q);
            return q;
        }

        public TransformAnimator WaitForSeconds(float duration)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            var q = new WaitForSecondsSequence(duration);
            _sequences.Add(q);
            return this;
        }

        public TransformAnimator WaitForCondition(WaitForCondition.TimePredicate condition)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            var q = new WaitForCondition(condition);
            _sequences.Add(q);
            return this;
        }

        public TransformAnimator Execute()
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            CurrentCoroutine = StartCoroutine(_Execute());
            return this;
        }

        public TransformAnimator LoopExecute()
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            CurrentCoroutine = StartCoroutine(_LoopExecute());
            return this;
        }

        public TransformAnimator LoopExecute(int count)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            CurrentCoroutine = StartCoroutine(_Loop(count));
            return this;
        }

        private IEnumerator _Execute()
        {
            foreach (var q in _sequences)
            {
                var co = q.ConstructCoroutine();
                while (!_disposed && co.MoveNext())
                {
                    yield return co.Current;
                }
            }
            Dispose();
        }

        private IEnumerator _LoopExecute()
        {
            int i = 0;

            while (!_disposed)
            {
                var co = _sequences[i].ConstructCoroutine();
                while (!_disposed && co.MoveNext())
                    yield return co.Current;

                i = (i + 1) % _sequences.Count;
            }
        }

        private IEnumerator _Loop(int count)
        {
            for (int k = 0; k < count; k++)
            {
                foreach (var q in _sequences)
                {
                    var co = q.ConstructCoroutine();
                    while (!_disposed && co.MoveNext())
                        yield return co.Current;
                }
            }

            Dispose();
        }

        public void Stop() => Dispose();

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Destroy(gameObject);
        }
    }

    public class TransformAnimatorDurationSequence : DurationAnimationSequence
    {
        private readonly TransformAnimator _transformAnimator;
        private readonly List<Action<float>> _animations = new List<Action<float>>();

        public TransformAnimatorDurationSequence(TransformAnimator transformAnimator, float duration) : base(duration)
        {
            _transformAnimator = transformAnimator;
        }

        public TransformAnimatorDurationSequence Move(Vector3 startPosition, Vector3 endPosition,
            Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            _animations.Add(t => _transformAnimator.Transform.position =
                Vector3.Lerp(startPosition, endPosition, easeFunction(t)));
            return this;
        }

        public TransformAnimatorDurationSequence LocalMove(Vector3 startPosition, Vector3 endPosition,
            Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            _animations.Add(t => _transformAnimator.Transform.localPosition =
                Vector3.Lerp(startPosition, endPosition, easeFunction(t)));
            return this;
        }

        public TransformAnimatorDurationSequence Scale(Vector3 startScale, Vector3 endScale,
            Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            _animations.Add(t => _transformAnimator.Transform.localScale =
                Vector3.Lerp(startScale, endScale, easeFunction(t)));
            return this;
        }

        public TransformAnimatorDurationSequence Rotate(Quaternion startRotaion, Quaternion endRotation,
            Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            _animations.Add(t => _transformAnimator.Transform.rotation =
                Quaternion.Lerp(startRotaion, endRotation, easeFunction(t)));
            return this;
        }

        public TransformAnimatorDurationSequence LocalRotate(Quaternion startRotaion, Quaternion endRotation,
            Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            _animations.Add(t => _transformAnimator.Transform.localRotation =
                Quaternion.Lerp(startRotaion, endRotation, easeFunction(t)));
            return this;
        }

        public TransformAnimatorDurationSequence Custom(Action<float> action)
        {
            _animations.Add(action);
            return this;
        }

        public TransformAnimator Done()
        {
            return _transformAnimator;
        }

        protected override void AnimationStep(float t)
        {
            foreach (var action in _animations)
            {
                action?.Invoke(t);
            }
        }

        private Func<float, float> WhenNullUseDefault(Func<float, float> func)
        {
            return func ?? Easings.Linear;
        }
    }

    public class TransformAnimatorSpeedSequence : SpeedAnimationSequence
    {
        private readonly Action<float> _animation;

        private TransformAnimatorSpeedSequence(float speed, float distance, Action<float> animation) : base(speed,
            distance)
        {
            _animation = animation;
        }

        public static TransformAnimatorSpeedSequence CreateMoveAnimation(TransformAnimator transformAnimator,
            Vector3 startPosition, Vector3 endPosition, float speed, Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            var dist = Vector3.Distance(startPosition, endPosition);

            return new TransformAnimatorSpeedSequence(speed, dist,
                t => transformAnimator.Transform.position = Vector3.Lerp(startPosition, endPosition, easeFunction(t)));
        }

        public static TransformAnimatorSpeedSequence CreateLocalMoveAnimation(TransformAnimator transformAnimator,
            Vector3 startPosition, Vector3 endPosition, float speed, Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            var dist = Vector3.Distance(startPosition, endPosition);

            return new TransformAnimatorSpeedSequence(speed, dist,
                t => transformAnimator.Transform.localPosition =
                    Vector3.Lerp(startPosition, endPosition, easeFunction(t)));
        }

        public static TransformAnimatorSpeedSequence CreateScaleAnimation(TransformAnimator transformAnimator,
            Vector3 startScale, Vector3 endScale, float speed, Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            var dist = Vector3.Distance(startScale, endScale);

            return new TransformAnimatorSpeedSequence(speed, dist,
                t => transformAnimator.Transform.localScale = Vector3.Lerp(startScale, endScale, easeFunction(t)));
        }

        public static TransformAnimatorSpeedSequence CreateRotaionAnimation(TransformAnimator transformAnimator,
            Quaternion startRotation, Quaternion endRotation, float speed, Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            var dist = Quaternion.Angle(startRotation, endRotation);

            return new TransformAnimatorSpeedSequence(speed, dist,
                t => transformAnimator.Transform.rotation =
                    Quaternion.Lerp(startRotation, endRotation, easeFunction(t)));
        }

        public static TransformAnimatorSpeedSequence CreateLocalRotaionAnimation(TransformAnimator transformAnimator,
            Quaternion startRotation, Quaternion endRotation, float speed, Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            var dist = Quaternion.Angle(startRotation, endRotation);

            return new TransformAnimatorSpeedSequence(speed, dist,
                t => transformAnimator.Transform.localRotation =
                        Quaternion.Lerp(startRotation, endRotation, easeFunction(t))
            );
        }

        public static TransformAnimatorSpeedSequence CreateCustomAnimation(TransformAnimator transformAnimator,
            Action<float> animation, float speed, float distance, Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            return new TransformAnimatorSpeedSequence(speed, distance, t => animation?.Invoke(easeFunction(t)));
        }

        protected override void AnimationStep(float t)
        {
            _animation?.Invoke(t);
        }

        private static Func<float, float> WhenNullUseDefault(Func<float, float> func)
        {
            return func ?? Easings.Linear;
        }
    }
}