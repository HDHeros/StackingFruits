using System;
using System.Collections;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;

namespace Extensions
{
    public static class TMProExtensions
    {
        //=====  USE EXAMPLE  ======
        //private Coroutine _counterCoroutine;
        //private TextMeshProUGUI _text;
        // private void Start()
        // {
        //     _counterCoroutine = StartCoroutine(_text.DOCustomCounter(200, 300, 1, 
        //         f => Mathf.Round(f).ToString(CultureInfo.InvariantCulture)));//start count
        //     StopCoroutine(_counterCoroutine);//break count
        // }
        /// <summary>
        /// Extension method consistently changes text of TMPro from "from" to "to" value.
        /// </summary>
        /// <param name="from">Start value</param>
        /// <param name="to">End value</param>
        /// <param name="duration">Time for which "from" becomes "to"</param>
        /// <param name="valueHandler">This function processes current value before it will be set to TMP</param>
        /// <param name="completeCallback">Complete callback if need</param>

        /// <returns></returns>
        public static IEnumerator DOCustomCounter(this TextMeshProUGUI tmp, double from, double to, float duration,
            Func<double, string> valueHandler = null, Action completeCallback = null)
        {
            var timeFromStart = 0f;
            do
            {
                timeFromStart += Time.deltaTime;
                var currentValue = duration == 0f ? to : MathA.Lerp(from, to, timeFromStart / duration);
                tmp.SetText(valueHandler != null ? valueHandler(currentValue) : MathA.Round(currentValue).ToString(CultureInfo.InvariantCulture));
                yield return null;
            } while (timeFromStart < duration);
            completeCallback?.Invoke();
        }

        public static IEnumerator DOCustomCounter(this TextMeshPro tmp, double from, double to, float duration,
            Func<double, string> valueHandler = null, Action completeCallback = null)
        {
            var timeFromStart = 0f;
            do
            {
                timeFromStart += Time.deltaTime;
                var currentValue = duration == 0f ? to : MathA.Lerp(from, to, timeFromStart / duration);
                tmp.SetText(valueHandler != null ? valueHandler(currentValue) : MathA.Round(currentValue).ToString(CultureInfo.InvariantCulture));
                yield return null;
            } while (timeFromStart < duration);
            completeCallback?.Invoke();
        }
        
        //=====  USE EXAMPLE  ======
        // private TextMeshProUGUI _tmp;
        // private Coroutine _timerCoroutine;
        //
        // private void Start()
        // {
        //     _timerCoroutine = StartCoroutine(_tmp.DOCustomCountdownTimer(TimeSpan.FromSeconds(10), 0.01f, 
        //         @"ss\:ff", s => $"Remaining  {s}", 
        //         () => Debug.Log("Countdown complete"), () => Debug.Log("Countdown update")));
        // }
        /// <summary>
        /// Extension method to implement the countdown time
        /// </summary>
        /// <param name="countDownFrom"></param>
        /// <param name="updateRate">How often tmp will be update</param>
        /// <param name="timeFormat">String to format time</param>
        /// <param name="stringFormatFunc">Func to format string which will apply to TMP</param>
        /// <param name="completeCallback">Func will be invoke after complete countdown</param>
        /// <param name="updateCallback">Func will be invoke after each update</param>
        /// <returns></returns>
        public static IEnumerator DOSimpleCountdownTimer(this TextMeshProUGUI tmp, TimeSpan countDownFrom, 
            float updateRate = 1, string timeFormat = @"hh\:mm\:ss", Func<string, string> stringFormatFunc = null, 
            Action completeCallback = null, Action updateCallback = null)
        {
            var delayWFS = new WaitForSeconds(updateRate);
            var delayTS = TimeSpan.FromSeconds(updateRate);
            for (var remainingTime = countDownFrom + delayTS; remainingTime.TotalMilliseconds >= 0; remainingTime -= delayTS)
            {
                var text = remainingTime.ToString(timeFormat);
                tmp.SetText(stringFormatFunc == null ? text : stringFormatFunc(text));
                updateCallback?.Invoke();
                yield return delayWFS;
            }
            completeCallback?.Invoke();
        }

        public static IEnumerator DoPrintAnimation(this TextMeshProUGUI tmp, string text, float betweenCharacterDelay,
            Action onTypingStopped)
        {
            StringBuilder visibleString = new StringBuilder("", text.Length);
            tmp.SetText(visibleString.ToString());
            var delay = new WaitForSeconds(betweenCharacterDelay);
            foreach (var c in text)
            {
                visibleString.Append(c);
                tmp.SetText(visibleString.ToString());
                yield return delay;
            }
            onTypingStopped?.Invoke();
        }
    }
}