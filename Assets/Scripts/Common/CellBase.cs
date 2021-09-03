using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// セルの基底クラス
/// </summary>
/// <typeparam name="T1">状態の型</typeparam>
/// <typeparam name="T2">設定の型</typeparam>
public abstract class CellBase<T1, T2> : MonoBehaviourArray2DValue
    where T1 : struct
    where T2 : ScriptableObject
{
    /// <summary>
    /// 状態
    /// </summary>
    [SerializeField]
    protected T1 state;

    /// <summary>
    /// 状態
    /// </summary>
    public T1 State
    {
        get => state;
        set
        {
            state = value;
            OnCellStateSet();
        }
    }

    /// <summary>
    /// 設定
    /// </summary>
    [SerializeField]
    protected T2 settings;

    protected virtual void OnValidate()
    {
        OnCellStateSet();
    }

    /// <summary>
    /// セルの状態が設定されたときの処理
    /// </summary>
    protected virtual void OnCellStateSet() { }
}
