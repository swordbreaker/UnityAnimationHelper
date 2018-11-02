using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace AnimationHelpers
{
    public static class TmpProExtention
    {
        public static TextAnimator TypeWriteText(this TMP_Text text, float typeSpeed, float waitTimeSenteceEnd, bool animateWords = false)
        {
            return TextAnimator.Create(text, typeSpeed, waitTimeSenteceEnd, animateWords);
        }
    }

    public class TextAnimator : MonoBehaviour, IDisposable
    {
        public delegate void TextAnimationAction(string element, int index);

        private bool _disposed = false;

        private TMP_Text _text;
        private float _speed;
        private float _waitTimeSenteceEnd;
        private bool _animateWords;
        private List<TextAnimationAction> _actions = new List<TextAnimationAction>();

        public Coroutine CurrentCoroutine { get; private set; }

        private TextAnimator()
        { }

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

        public void AddAction(TextAnimationAction action)
        {
            _actions.Add(action);
        }

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
                _text.maxVisibleWords = i+1;
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
                _text.maxVisibleCharacters = i+1;
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

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Destroy(gameObject);
        }
    }
}