using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム変数表示の基底クラス
/// </summary>
/// <typeparam name="T1">表示対象の型</typeparam>
/// <typeparam name="T2">直前の値の型</typeparam>
public abstract class GameVariableDisplayerBase<T1, T2> : MonoBehaviour where T1 : GameVariableBase<T2> where T2 : IComparable
{
    /// <summary>
    /// ゲーム変数
    /// </summary>
    [SerializeField]
    protected T1 gameVariable;

    /// <summary>
    /// 直前の値
    /// </summary>
    protected T2 previousValue;

    protected virtual void Start()
    {
        UpdateDisplay();
    }

    protected virtual void Update()
    {
        if (ValueHasChanged())
        {
            UpdateDisplay();
        }
    }

    /// <summary>
    /// 値が変わったか
    /// </summary>
    /// <returns></returns>
    protected bool ValueHasChanged()
    {
        return gameVariable.RuntimeValue.CompareTo(previousValue) != 0;
    }

    /// <summary>
    /// 表示を更新する
    /// </summary>
    protected virtual void UpdateDisplay()
    {
        // 直前の値を現在値と同じにする
        previousValue = gameVariable.RuntimeValue;
    }
}
