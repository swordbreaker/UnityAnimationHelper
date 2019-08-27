using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace AnimationHelpers
{
    /// <summary>
    /// Typewriter extension method for Text Mesh
    /// </summary>
    public static class TmpProExtention
    {
        /// <summary>
        /// Create a new TypeWrite animation
        /// </summary>
        /// <param name="text">The Text Mesh Pro text to animate</param>
        /// <param name="typeSpeed">The pause between each character/word</param>
        /// <param name="waitTimeSenteceEnd">The pause when sentence is finished (only recognizes . as an end of a sentence)</param>
        /// <param name="animateWords">When true while words will appear else single characters will appear</param>
        /// <returns>A instance of a TextAnimator</returns>
        public static TextAnimator TypeWriteText(this TMP_Text text, float typeSpeed, float waitTimeSenteceEnd, bool animateWords = false)
        {
            return TextAnimator.Create(text, typeSpeed, waitTimeSenteceEnd, animateWords);
        }
    }

    /// <summary>
    /// Animator for animating text in a type writer stlye
    /// </summary>
    public class TextAnimator : MonoBehaviour, IDisposable
    {
        /// <summary>
        /// Delegate for a action executed each animation step
        /// </summary>
        /// <param name="element">The current element which will appear (can be a character or a word)</param>
        /// <param name="index">The index in the string of the character (if word the index of the word without spaces)</param>
        public delegate void TextAnimationAction(string element, int index);

        private bool _disposed = false;

        private TMP_Text _text;
        private float _speed;
        private float _waitTimeSenteceEnd;
        private bool _animateWords;
        private List<TextAnimationAction> _actions = new List<TextAnimationAction>();

        public bool IsRunning => !_disposed;
        
        /// <inheritdoc cref="CurrentCoroutine"/>
        public Coroutine CurrentCoroutine { get; private set; }

        public TextAnimator() { }

        /// <summary>
        /// Create a new instance of TextAnimator
        /// </summary>
        /// <param name="text">The Text Mesh Pro text to animate</param>
        /// <param name="speed">The pause between each character/word</param>
        /// <param name="waitTimeSenteceEnd">The pause when sentence is finished (only recognizes . as an end of a sentence)</param>
        /// <param name="animateWords">When true while words will appear else single characters will appear</param>
        /// <returns></returns>
        public static TextAnimator Create(TMP_Text text, float speed, float waitTimeSenteceEnd, bool animateWords)
        {
            var go = new GameObject("TransformAnimator", typeof(TextAnimator));
            var textAnimator = go.GetComponent<TextAnimator>();
            textAnimator._text = text;
            textAnimator._speed = speed;
            textAnimator._waitTimeSenteceEnd = waitTimeSenteceEnd;
            textAnimator._animateWords = animateWords;

            text.ForceMeshUpdate();

            if (animateWords)
            {
                text.maxVisibleWords = 0;
            }
            else
            {
                text.maxVisibleCharacters = 0;
            }

            return textAnimator;
        }

        /// <summary>
        /// Add actions between each animation step.
        /// </summary>
        /// <param name="action">The action <see cref="TextAnimationAction"/></param>
        public void AddAction(TextAnimationAction action)
        {
            _actions.Add(action);
        }

        /// <summary>
        /// Execute the animation.
        /// </summary>
        /// <returns>It self</returns>
        public TextAnimator Execute()
        {
            if (_disposed) throw new ObjectDisposedException(this.name);
            CurrentCoroutine = StartCoroutine(_animateWords ? _ExecuteWord() : _ExecuteCharacter());
            return this;
        }

        private IEnumerator _ExecuteWord()
        {
            for (var i = 0; i < _text.textInfo.wordCount; i++)
            {
                _text.maxVisibleWords = i + 1;
                var wordInfo = _text.textInfo.wordInfo[i];

                _actions.ForEach(action => action?.Invoke(wordInfo.GetWord(), i));

                if (wordInfo.GetWord().Contains('.'))
                {
                    yield return new WaitForSeconds(_waitTimeSenteceEnd);
                }
                else
                {
                    yield return new WaitForSeconds(_speed);
                }
            }
            Dispose();
        }

        private IEnumerator _ExecuteCharacter()
        {
            bool lastWasFullstop = false;

            for (var i = 0; i < _text.textInfo.characterCount; i++)
            {
                _text.maxVisibleCharacters = i + 1;
                var charInfo = _text.textInfo.characterInfo[i];

                _actions.ForEach(action => action?.Invoke(charInfo.character.ToString(), i));

                if (lastWasFullstop)
                {
                    yield return new WaitForSeconds(_waitTimeSenteceEnd);
                }
                else
                {
                    yield return new WaitForSeconds(_speed);
                }
                lastWasFullstop = charInfo.character == '.';
            }
            Dispose();
        }

        /// <summary>
        /// Stop the animation
        /// </summary>
        public void Stop()
        {
            Dispose();
        }

        /// <summary>
        /// Same as Stop <see cref="Stop"/>
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Destroy(gameObject);
        }
    }
}