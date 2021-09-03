using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム変数の基底クラス
/// </summary>
public abstract class GameVariableBase<T> : ScriptableObject, IInitializable where T : IComparable
{
    /// <summary>
    /// 初期値
    /// </summary>
    [SerializeField]
    protected T initialValue;

    /// <summary>
    /// 下限値を設けるか
    /// </summary>
    [SerializeField]
    protected bool hasLowerLimit = false;

    /// <summary>
    /// 最小値
    /// </summary>
    [SerializeField]
    protected T minValue;

    /// <summary>
    /// 上限値を設けるか
    /// </summary>
    [SerializeField]
    protected bool hasUpperLimit = false;

    /// <summary>
    /// 最大値
    /// </summary>
    [SerializeField]
    protected T maxValue;

    /// <summary>
    /// 値が設定されたときのイベント
    /// </summary>
    [SerializeField]
    protected GameEvent onValueSet = null;

    /// <summary>
    /// 実行中に用いる値
    /// </summary>
    //[NonSerialized]
    T runtimeValue;

    /// <summary>
    /// 実行中に用いる値
    /// </summary>
    public T RuntimeValue
    {
        get => runtimeValue;
        set
        {
            runtimeValue = value;
            AdjustValue();
            RaiseEvent();
        }
    }

    public virtual void Init()
    {
        RuntimeValue = initialValue;
    }

    /// <summary>
    /// 値を修正する
    /// </summary>
    protected void AdjustValue()
    {
        if (hasUpperLimit && RuntimeValue.CompareTo(maxValue) > 0)
        {
            RuntimeValue = maxValue;
        }
        if (hasLowerLimit && RuntimeValue.CompareTo(minValue) < 0)
        {
            RuntimeValue = minValue;
        }
    }

    /// <summary>
    /// イベントを実行する
    /// </summary>
    protected virtual void RaiseEvent()
    {
        if (!onValueSet)
        {
            return;
        }

        onValueSet.Raise();
    }
}
