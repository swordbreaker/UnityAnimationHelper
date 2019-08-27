using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimationHelpers
{
    /// <summary>
    /// Extenuation Method for the transform animations
    /// </summary>
    public static class TransformExtention
    {
        /// <summary>
        /// Create a transform animation
        /// </summary>
        /// <param name="transform">The transform to animate</param>
        /// <returns>A transform animation</returns>
        public static TransformAnimator Animate(this Transform transform)
        {
            return TransformAnimator.Create(transform);
        }
    }

    /// <summary>
    /// Transform Animation for animating a unity transform
    /// </summary>
    public class TransformAnimator : MonoBehaviour, IDisposable
    {
        internal Transform Transform { get; private set; }
        private readonly List<IAnmationSequence> _sequences = new List<IAnmationSequence>();

        private bool _disposed = false;
        private bool _isRunning = false;

        /// <summary>
        /// True if the animation is running 
        /// </summary>
        public bool IsRunning => !_disposed && _isRunning;

        public event EventHandler OnDispose;
        
        /// <summary>
        /// The current coroutine useful when used in an other coroutine to wait till the animation is finished with <code>yield return animation.CurrentCoroutine</code>
        /// </summary>
        public Coroutine CurrentCoroutine { get; private set; }

        private TransformAnimator()
        {
        }

        /// <summary>
        /// Create a new TransformAnimator
        /// </summary>
        /// <param name="transform">the transform to animate</param>
        /// <returns>a new TransformAnimator</returns>
        public static TransformAnimator Create(Transform transform)
        {
            var go = new GameObject("TransformAnimator", typeof(TransformAnimator));
            var transformAnimator = go.GetComponent<TransformAnimator>();
            transformAnimator.Transform = transform;
            return go.GetComponent<TransformAnimator>();
        }

        #region Move
        /// <summary>
        /// Add a Move animation. Animate form current position (before the start of the hole animation) till pos
        /// </summary>
        /// <param name="pos">The position to move to</param>
        /// <param name="duration">The duration of the animation in seconds</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator Move(Vector3 pos, float duration, Func<float, float> easeFunction = null)
        {
            return Move(transform.position, pos, duration, easeFunction);
        }

        /// <summary>
        /// Add a Move animation.
        /// </summary>
        /// <param name="startPos">The position to move to</param>
        /// <param name="endPos">The position to move to</param>
        /// <param name="duration">The duration of the animation in seconds</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator Move(Vector3 startPos, Vector3 endPos, float duration, Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);

            Sequence(duration).Move(startPos, endPos, easeFunction);
            return this;
        }

        /// <summary>
        /// Add a Move animation which uses speed and not a duration. Animate form current position (before the start of the hole animation) till pos
        /// </summary>
        /// <param name="pos">The position to move to</param>
        /// <param name="speed">The speed of the animation. When the distance is 1 unity unit with the speed 1 it will take 1 seconds to animate.</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator MoveWithSpeed(Vector3 pos, float speed, Func<float, float> easeFunction = null)
        {
            return MoveWithSpeed(transform.position, pos, speed, easeFunction);
        }

        /// <summary>
        /// Add a Move animation which uses speed and not a duration.
        /// </summary>
        /// <param name="startPos">The start position</param>
        /// <param name="endPos">The end position to move to</param>
        /// <param name="speed">The speed of the animation. When the distance is 1 unity unit with the speed 1 it will take 1 seconds to animate.</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator MoveWithSpeed(Vector3 startPos, Vector3 endPos, float speed, Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            _sequences.Add(
                TransformAnimatorSpeedSequence.CreateMoveAnimation(this, startPos, endPos, speed, easeFunction));
            return this;
        }
        
        /// <summary>
        /// Add a Move animation uses the local position. Animate form current local position (before the start of the hole animation) till pos
        /// </summary>
        /// <param name="pos">The local position to move to</param>
        /// <param name="duration">The duration of the animation in seconds</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator LocalMove(Vector3 pos, float duration, Func<float, float> easeFunction = null)
        {
            return LocalMove(Transform.localPosition, pos, duration, easeFunction);
        }

        /// <summary>
        /// Add a Move animation uses the local position.
        /// </summary>
        /// <param name="startpos">The local start position to move to</param>
        /// <param name="endPos">The local end position to move to</param>
        /// <param name="duration">The duration of the animation in seconds</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator LocalMove(Vector3 startpos, Vector3 endPos, float duration, Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);

            Sequence(duration).LocalMove(startpos, endPos, easeFunction);
            return this;
        }

        /// <summary>
        /// Add a Move animation uses the local position and uses speed and not a duration. Animate form current local position (before the start of the hole animation) till pos
        /// </summary>
        /// <param name="pos">The local position to move to</param>
        /// <param name="speed">The speed of the animation. When the distance is 1 unity unit with the speed 1 it will take 1 seconds to animate.</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator LocalMoveWithSpeed(Vector3 pos, float speed, Func<float, float> easeFunction = null)
        {
            return LocalMoveWithSpeed(Transform.localPosition, pos, speed, easeFunction);
        }

        /// <summary>
        /// Add a Move animation uses the local position and uses speed and not a duration.
        /// </summary>
        /// <param name="startPos">The local start position</param>
        /// <param name="endPos">The local end position to move to</param>
        /// <param name="speed">The speed of the animation. When the distance is 1 unity unit with the speed 1 it will take 1 seconds to animate.</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator LocalMoveWithSpeed(Vector3 startPos, Vector3 endPos, float speed, Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            _sequences.Add(TransformAnimatorSpeedSequence.CreateLocalMoveAnimation(this, startPos, endPos, speed, easeFunction));
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
        /// <param name="durations">The durations for each path point</param>
        /// <param name="easeFunction">The ease function will be applied for each transition form point to point and not for the hole animation</param>
        /// <returns>The TramsformAnimator</returns>
        public TransformAnimator LocalMovePath(IList<Vector3> path, IList<float> durations, Func<float, float> easeFunction = null)
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

        /// <summary>
        /// Move the transform along a path. The start point will be the current position of the Transform (Do not include this position in the path IEnumerable)
        /// </summary>
        /// <param name="duration">The duration of the animation</param>
        /// <param name="easeFunction">The ease function will be applied for each transition form point to point and not for the hole animation</param>
        /// <param name="path">The path points without the current transform position</param>
        /// <returns>The TramsformAnimator</returns>
        public TransformAnimator MovePath(float duration, Func<float, float> easeFunction = null, params Vector3[] path)
        {
            return MovePath(path, Enumerable.Repeat(duration, path.Length).ToArray(), easeFunction);
        }

        /// <summary>
        /// Move the transform along a path uses the local position. The start point will be the current position of the Transform (Do not include this position in the path IEnumerable)
        /// </summary>
        /// <param name="path">The path points without the current transform position</param>
        /// <param name="duration">The duration of the animation</param>
        /// <param name="easeFunction">The ease function will be applied for each transition form point to point and not for the hole animation</param>
        /// <returns>The TramsformAnimator</returns>
        public TransformAnimator LocalMovePath(IList<Vector3> path, float duration,
            Func<float, float> easeFunction = null)
        {
            return LocalMovePath(path, Enumerable.Repeat(duration, path.Count).ToArray(), easeFunction);
        }

        /// <summary>
        /// Move the transform along a path. The start point will be the current position of the Transform (Do not include this position in the path IEnumerable)
        /// </summary>
        /// <param name="path">The path points without the current transform position</param>
        /// <param name="duration">The duration of the animation</param>
        /// <param name="easeFunction">The ease function will be applied for each transition form point to point and not for the hole animation</param>
        /// <returns>The TramsformAnimator</returns>
        public TransformAnimator LocalMovePath(float duration, Func<float, float> easeFunction = null,
            params Vector3[] path)
        {
            return LocalMovePath(path, Enumerable.Repeat(duration, path.Length).ToArray(), easeFunction);
        }
        #endregion

        #region Scale
        /// <summary>
        /// Add a scale animation. Animate from the current scale (before the start of the hole animation) till scale
        /// </summary>
        /// <param name="scale">The end scale of the animation</param>
        /// <param name="duration">The duration of the animation</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator Scale(Vector3 scale, float duration, Func<float, float> easeFunction = null)
        {
            return Scale(Transform.localScale, scale, duration, easeFunction);
        }

        /// <summary>
        /// Add a scale animation.
        /// </summary>
        /// <param name="startScale">The start scale of the animation</param>
        /// <param name="endScale">The end scale of the animation</param>
        /// <param name="duration">The duration of the animation</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator Scale(Vector3 startScale, Vector3 endScale, float duration, Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);

            Sequence(duration).Scale(startScale, endScale, easeFunction);
            return this;
        }

        /// <summary>
        /// Add a scale animation which uses speed and not a duration. Animate from the current scale (before the start of the hole animation) till scale
        /// </summary>
        /// <param name="scale">The end scale of the animation</param>
        /// <param name="speed">The speed of the animation. When the scale has a difference of 1 unit with the speed of 1 it takes 1 second to animate </param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator ScaleWithSpeed(Vector3 scale, float speed, Func<float, float> easeFunction = null)
        {
            return ScaleWithSpeed(Transform.localScale, scale, speed, easeFunction);
        }

        /// <summary>
        /// Add a scale animation which uses speed and not a duration.
        /// </summary>
        /// <param name="startScale">The start scale of the animation</param>
        /// <param name="endScale">The end scale of the animation</param>
        /// <param name="speed">The speed of the animation. When the scale has a difference of 1 unit with the speed of 1 it takes 1 second to animate </param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator ScaleWithSpeed(Vector3 startScale, Vector3 endScale, float speed, Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            _sequences.Add(TransformAnimatorSpeedSequence.CreateScaleAnimation(this, startScale, endScale, speed,
                easeFunction));
            return this;
        }
        #endregion

        #region Rotate
        /// <summary>
        /// Rotate the transform. The start rotation will be the current rotation of the Transform (before the start of the hole animation).
        /// </summary>
        /// <param name="endRotation">The end rotation after the animation</param>
        /// <param name="duration">The duration of the animation</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator Rotate(Quaternion endRotation, float duration, Func<float, float> easeFunction = null)
        {
            return Rotate(Transform.rotation, endRotation, duration, easeFunction);
        }

        /// <summary>
        /// Rotate the transform.
        /// </summary>
        /// <param name="startRotation">The start rotation after the animation</param>
        /// <param name="endRotation">The end rotation after the animation</param>
        /// <param name="duration">The duration of the animation</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator Rotate(Quaternion startRotation, Quaternion endRotation, float duration, Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);

            Sequence(duration).Rotate(startRotation, endRotation, easeFunction);
            return this;
        }

        /// <summary>
        /// Rotate the transform which uses speed and not a duration. The start rotation will be the current rotation of the Transform (before the start of the hole animation).
        /// </summary>
        /// <param name="endRotation">The end rotation after the animation</param>
        /// <param name="speed">The speed of the animation. When the rotation distance is 1 Euler angle and the speed is 1 second the animation will take 1 second.</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator RotateWithSpeed(Quaternion endRotation, float speed,
            Func<float, float> easeFunction = null)
        {
            return RotateWithSpeed(transform.rotation, endRotation, speed, easeFunction);
        }

        /// <summary>
        /// Rotate the transform which uses speed and not a duration.
        /// </summary>
        /// <param name="startRotaion">The end rotation after the animation</param>
        /// <param name="endRotation">The end rotation after the animation</param>
        /// <param name="speed">The speed of the animation. When the rotation distance is 1 Euler angle and the speed is 1 second the animation will take 1 second.</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator RotateWithSpeed(Quaternion startRotaion, Quaternion endRotation, float speed,
            Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            _sequences.Add(
                TransformAnimatorSpeedSequence.CreateRotationAnimation(this, startRotaion, endRotation, speed,
                    easeFunction));
            return this;
        }

        /// <summary>
        /// Rotate the local transform which uses speed and not a duration. The start rotation will be the current rotation of the Transform (before the start of the hole animation).
        /// </summary>
        /// <param name="endRotation">The end rotation after the animation</param>
        /// <param name="duration">The duration of the animation</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator LocalRotate(Quaternion endRotation, float duration,
            Func<float, float> easeFunction = null)
        {
            return LocalRotate(transform.localRotation, endRotation, duration, easeFunction);
        }

        /// <summary>
        /// Rotate the local transform which uses speed and not a duration.
        /// </summary>
        /// <param name="startRotaion">The end rotation after the animation</param>
        /// <param name="endRotation">The end rotation after the animation</param>
        /// <param name="duration">The duration of the animation</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator LocalRotate(Quaternion startRotaion, Quaternion endRotation, float duration,
            Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);

            Sequence(duration).LocalRotate(startRotaion, endRotation, easeFunction);
            return this;
        }

        /// <summary>
        /// Rotate the local transform which uses speed and not a duration.The start rotation will be the current rotation of the Transform (before the start of the hole animation).
        /// </summary>
        /// <param name="endRotation">The end rotation after the animation</param>
        /// <param name="speed">The speed of the animation.</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator LocalRotateWithSpeed(Quaternion endRotation, float speed,
            Func<float, float> easeFunction = null)
        {
            return LocalRotateWithSpeed(transform.localRotation, endRotation, speed, easeFunction);
        }

        /// <summary>
        /// Rotate the local transform which uses speed and not a duration.
        /// </summary>
        /// <param name="startRotaion">The start rotation</param>
        /// <param name="endRotation">The end rotation after the animation</param>
        /// <param name="speed">The speed of the animation.</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimator LocalRotateWithSpeed(Quaternion startRotaion, Quaternion endRotation, float speed,
            Func<float, float> easeFunction = null)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            _sequences.Add(TransformAnimatorSpeedSequence.CreateLocalRotaionAnimation(this, startRotaion,
                endRotation, speed, easeFunction));
            return this;
        }
        #endregion

        /// <summary>
        /// Do a Action (will be executed one time)
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <returns>it self</returns>
        public TransformAnimator Do(Action action)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            _sequences.Add(new OneShotSquence(action));
            return this;
        }

        /// <summary>
        /// Create a sequence of Animation which all take the same amount of time
        /// </summary>
        /// <param name="duration">The duration of the sequence</param>
        /// <returns>A new sequence object to chain animations on</returns>
        public TransformAnimatorDurationSequence Sequence(float duration)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            var q = new TransformAnimatorDurationSequence(this, duration);
            _sequences.Add(q);
            return q;
        }

        /// <summary>
        /// Wait for x seconds and the continue with the next animation
        /// </summary>
        /// <param name="duration">Time to wait in seconds</param>
        /// <returns>it self</returns>
        public TransformAnimator WaitForSeconds(float duration)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            var q = new WaitForSecondsSequence(duration);
            _sequences.Add(q);
            return this;
        }

        /// <summary>
        /// Wait till a certain condition is true
        /// </summary>
        /// <param name="condition">The condition to wait for</param>
        /// <returns>it self</returns>
        public TransformAnimator WaitForCondition(WaitForCondition.TimePredicate condition)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            var q = new WaitForCondition(condition);
            _sequences.Add(q);
            return this;
        }

        /// <summary>
        /// Execute all added animations and sequences
        /// </summary>
        /// <returns>it self</returns>
        public TransformAnimator Execute()
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            CurrentCoroutine = StartCoroutine(_Execute());
            return this;
        }

        /// <summary>
        /// Loop all added animations and sequences till the animator is stopped manually <see cref="Stop"/>
        /// </summary>
        /// <returns>it self</returns>
        public TransformAnimator LoopExecute()
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            CurrentCoroutine = StartCoroutine(_LoopExecute());
            return this;
        }

        /// <summary>
        /// Loop all added animations and sequences for a certain count
        /// </summary>
        /// <param name="count">The number of iterations</param>
        /// <returns>it self</returns>
        public TransformAnimator LoopExecute(int count)
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            CurrentCoroutine = StartCoroutine(_Loop(count));
            return this;
        }

        private IEnumerator _Execute()
        {
            if(_isRunning) Debug.LogError("The animation is already running do not execute the animation twice");
            _isRunning = true;

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
            if (_isRunning) Debug.LogError("The animation is already running do not execute the animation twice");
            _isRunning = true;

            int i = 0;

            while (!_disposed)
            {
                var co = _sequences[i].ConstructCoroutine();
                while (!_disposed && co.MoveNext())
                    yield return co.Current;

                i = (i + 1) % _sequences.Count;
            }
            
            Dispose();
        }

        private IEnumerator _Loop(int count)
        {
            if (_isRunning) Debug.LogError("The animation is already running do not execute the animation twice");
            _isRunning = true;

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

        /// <summary>
        /// Stop the animation. After stopping the animation the object will be disposed and cannot be used again.
        /// </summary>
        public void Stop() => Dispose();

        /// <summary>
        /// Does the same as stop. Does not need to be called for cleanup will be called after the execution has finished.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            OnDispose?.Invoke(this, EventArgs.Empty);
            _isRunning = false;
            _disposed = true;
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// When the GameObject is destroyed stop the animation to prevent access of destroyed transforms
        /// </summary>
        private void OnDestroy()
        {
            Dispose();
        }
    }

    /// <summary>
    /// Class for animations sequences which are running for a certain duration
    /// </summary>
    public class TransformAnimatorDurationSequence : DurationAnimationSequence
    {
        private readonly TransformAnimator _transformAnimator;
        private readonly List<Action<float>> _animations = new List<Action<float>>();

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="transformAnimator">The transform animator for the transform</param>
        /// <param name="duration">Duration of the animations</param>
        public TransformAnimatorDurationSequence(TransformAnimator transformAnimator, float duration) : base(duration)
        {
            _transformAnimator = transformAnimator;
        }

        /// <summary>
        /// Add a move animation
        /// </summary>
        /// <param name="startPosition">The start position</param>
        /// <param name="endPosition">The end position</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimatorDurationSequence Move(Vector3 startPosition, Vector3 endPosition,
            Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            _animations.Add(t => _transformAnimator.Transform.position =
                Vector3.Lerp(startPosition, endPosition, easeFunction(t)));
            return this;
        }

        /// <summary>
        /// Add a move animation which uses the local transform position 
        /// </summary>
        /// <param name="startPosition">The start position</param>
        /// <param name="endPosition">The end position</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimatorDurationSequence LocalMove(Vector3 startPosition, Vector3 endPosition,
            Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            _animations.Add(t => _transformAnimator.Transform.localPosition =
                Vector3.Lerp(startPosition, endPosition, easeFunction(t)));
            return this;
        }

        /// <summary>
        /// Add a scale animation.
        /// </summary>
        /// <param name="startScale">Start scale</param>
        /// <param name="endScale">End scale</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimatorDurationSequence Scale(Vector3 startScale, Vector3 endScale,
            Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            _animations.Add(t => _transformAnimator.Transform.localScale =
                Vector3.Lerp(startScale, endScale, easeFunction(t)));
            return this;
        }

        /// <summary>
        /// Add a rotation animation.
        /// </summary>
        /// <param name="startRotaion">The start rotation</param>
        /// <param name="endRotation">The end rotation</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimatorDurationSequence Rotate(Quaternion startRotaion, Quaternion endRotation,
            Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            _animations.Add(t => _transformAnimator.Transform.rotation =
                Quaternion.Lerp(startRotaion, endRotation, easeFunction(t)));
            return this;
        }

        /// <summary>
        /// Add a rotation animation which uses the local rotation.
        /// </summary>
        /// <param name="startRotaion">The start rotation</param>
        /// <param name="endRotation">The end rotation</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public TransformAnimatorDurationSequence LocalRotate(Quaternion startRotaion, Quaternion endRotation,
            Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            _animations.Add(t => _transformAnimator.Transform.localRotation =
                Quaternion.Lerp(startRotaion, endRotation, easeFunction(t)));
            return this;
        }

        /// <summary>
        /// Add a custom animation
        /// </summary>
        /// <param name="action">The action to call every step. The action will receive the t [0,1] value as a parameter</param>
        /// <returns>it self</returns>
        public TransformAnimatorDurationSequence Custom(Action<float> action)
        {
            _animations.Add(action);
            return this;
        }

        /// <summary>
        /// Finishes the Sequence.
        /// </summary>
        /// <returns>The TransformAnimator of the transform</returns>
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

    /// <summary>
    /// Class for animations which uses a animation speed. 
    /// It is not possible to add more than one animation to a speed sequence 
    /// because the speed parameter can have a different meaning for different animations 
    /// therefore the animation does not have the same duration with the same speed.
    /// </summary>
    public class TransformAnimatorSpeedSequence : SpeedAnimationSequence
    {
        private readonly Action<float> _animation;

        private TransformAnimatorSpeedSequence(float speed, float distance, Action<float> animation) : base(speed,
            distance)
        {
            _animation = animation;
        }

        /// <summary>
        /// Create a new Move speed animation
        /// </summary>
        /// <param name="transformAnimator">The TransformAnimator of the transform to animate</param>
        /// <param name="startPosition">The start position</param>
        /// <param name="endPosition">The end position</param>
        /// <param name="speed">The speed of the animation. When the distance is 1 unity unit with the speed 1 it will take 1 seconds to animate.</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public static TransformAnimatorSpeedSequence CreateMoveAnimation(TransformAnimator transformAnimator,
            Vector3 startPosition, Vector3 endPosition, float speed, Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            var dist = Vector3.Distance(startPosition, endPosition);

            return new TransformAnimatorSpeedSequence(speed, dist,
                t => transformAnimator.Transform.position = Vector3.Lerp(startPosition, endPosition, easeFunction(t)));
        }

        /// <summary>
        /// Create a new Move speed animation which uses the local position of the transform.
        /// </summary>
        /// <param name="transformAnimator">The TransformAnimator of the transform to animate</param>
        /// <param name="startPosition">The start position</param>
        /// <param name="endPosition">The end position</param>
        /// <param name="speed">The speed of the animation. When the distance is 1 unity unit with the speed 1 it will take 1 seconds to animate.</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public static TransformAnimatorSpeedSequence CreateLocalMoveAnimation(TransformAnimator transformAnimator,
            Vector3 startPosition, Vector3 endPosition, float speed, Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            var dist = Vector3.Distance(startPosition, endPosition);

            return new TransformAnimatorSpeedSequence(speed, dist,
                t => transformAnimator.Transform.localPosition =
                    Vector3.Lerp(startPosition, endPosition, easeFunction(t)));
        }

        /// <summary>
        /// Create a new scale animation.
        /// </summary>
        /// <param name="transformAnimator">The TransformAnimator of the transform to animate</param>
        /// <param name="startScale">The start scale</param>
        /// <param name="endScale">The end scale</param>
        /// <param name="speed">The speed of the animation. When the scale has a difference of 1 unit with the speed of 1 it takes 1 second to animate </param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public static TransformAnimatorSpeedSequence CreateScaleAnimation(TransformAnimator transformAnimator,
            Vector3 startScale, Vector3 endScale, float speed, Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            var dist = Vector3.Distance(startScale, endScale);

            return new TransformAnimatorSpeedSequence(speed, dist,
                t => transformAnimator.Transform.localScale = Vector3.Lerp(startScale, endScale, easeFunction(t)));
        }

        /// <summary>
        /// Create a new rotation animation.
        /// </summary>
        /// <param name="transformAnimator">The TransformAnimator of the transform to animate</param>
        /// <param name="startRotation">The start rotation</param>
        /// <param name="endRotation">The end rotation</param>
        /// <param name="speed">The speed of the animation. When the rotation distance is 1 Euler angle and the speed is 1 second the animation will take 1 second.</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
        public static TransformAnimatorSpeedSequence CreateRotationAnimation(TransformAnimator transformAnimator,
            Quaternion startRotation, Quaternion endRotation, float speed, Func<float, float> easeFunction = null)
        {
            easeFunction = WhenNullUseDefault(easeFunction);
            var dist = Quaternion.Angle(startRotation, endRotation);

            return new TransformAnimatorSpeedSequence(speed, dist,
                t => transformAnimator.Transform.rotation =
                    Quaternion.Lerp(startRotation, endRotation, easeFunction(t)));
        }

        /// <summary>
        /// Create a new rotation animation which uses the local rotation of the transform.
        /// </summary>
        /// <param name="transformAnimator">The TransformAnimator of the transform to animate</param>
        /// <param name="startRotation">The start rotation</param>
        /// <param name="endRotation">The end rotation</param>
        /// <param name="speed">The speed of the animation. When the rotation distance is 1 Euler angle and the speed is 1 second the animation will take 1 second.</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
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

        /// <summary>
        /// Create a custom animation.
        /// <remarks>
        /// The t value of the animation is calculated as follow: <code>travelTime * speed / travelDistance</code>
        /// </remarks>
        /// </summary>
        /// <param name="transformAnimator">The TransformAnimator of the transform to animate</param>
        /// <param name="animation">The action to call every step. The step will get the t [0,1] value as a parameter.</param>
        /// <param name="speed">The speed of the animation.</param>
        /// <param name="distance">The distance the animation needs to travel.</param>
        /// <param name="easeFunction">The ease functions to use for the interpolation</param>
        /// <returns>it self</returns>
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