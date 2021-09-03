using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameOfLife
{
    /// <summary>
    /// 世代を表示するUI
    /// </summary>
    public class StepsDisplayer : GameVariableDisplayerBase<IntGameVariable, int>
    {
        /// <summary>
        /// 表示するテキスト
        /// </summary>
        [SerializeField]
        Text view = null;

        /// <summary>
        /// 接頭辞
        /// </summary>
        [SerializeField]
        string prefix = "第";

        /// <summary>
        /// 接尾辞
        /// </summary>
        [SerializeField]
        string suffix = "世代";

        protected override void UpdateDisplay()
        {
            base.UpdateDisplay();

            // 新しい値を文字列に変換する
            string newText = gameVariable.RuntimeValue.ToString();

            // テキストを更新する
            view.text = prefix + newText + suffix;
        }
    }

}
