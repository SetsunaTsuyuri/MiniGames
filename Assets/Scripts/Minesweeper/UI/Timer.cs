using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper
{
    /// <summary>
    /// 制限時間を表示するUIテキスト
    /// </summary>
    public class Timer : GameVariableDisplayerBase<FloatGameVariable, float>
    {
        /// <summary>
        /// 表示するテキスト
        /// </summary>
        [SerializeField]
        Text view = null;

        protected override void UpdateDisplay()
        {
            base.UpdateDisplay();

            // 変数の値を(分:秒)の形にしてテキストに反映させる
            float time = gameVariable.RuntimeValue;
            int minutes = Mathf.FloorToInt(time / 60.0f);
            int seconds = Mathf.FloorToInt(time - minutes * 60);
            //int tenMilliSeconds = Mathf.FloorToInt((time - minutes * 60 - seconds) * 100);
            //string timeText = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, tenMilliSeconds);
            string timeText = string.Format("{0:00}:{1:00}", minutes, seconds);
            view.text = timeText;
        }
    }
}
