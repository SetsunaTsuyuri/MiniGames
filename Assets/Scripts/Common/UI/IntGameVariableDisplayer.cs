using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Int型ゲーム変数を表示する
/// </summary>
public class IntGameVariableDisplayer : GameVariableDisplayerBase<IntGameVariable, int>
{
    /// <summary>
    /// 変数の値を表示するテキスト
    /// </summary>
    [SerializeField]
    protected Text view = null;

    /// <summary>
    /// プラスマイナスを逆に表示するか
    /// </summary>
    [SerializeField]
    protected bool reverse = false;

    protected override void UpdateDisplay()
    {
        base.UpdateDisplay();

        // テキストを更新する
        int newValue = gameVariable.RuntimeValue;
        if (reverse)
        {
            newValue = -newValue;
        }
        view.text = newValue.ToString();
    }
}
