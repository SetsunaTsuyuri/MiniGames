using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームの基底クラス
/// </summary>
/// <typeparam name="T1">ゲームの状態の型</typeparam>
/// <typeparam name="T2">セルの2次元配列の型</typeparam>
/// <typeparam name="T3">セルの型</typeparam>
/// <typeparam name="T4">セルの状態の型</typeparam>
/// <typeparam name="T5">セルの設定の型</typeparam>
public abstract class GameBase<T1, T2, T3, T4, T5> : MonoBehaviour, IInitializable
    where T1 : struct
    where T2 : MonoBehaviourArray2D<T3>
    where T3 : CellBase<T4, T5>
    where T4 : struct
    where T5 : ScriptableObject
{
    /// <summary>
    /// 現在のゲームステート
    /// </summary>
    T1 state;

    /// <summary>
    /// 現在のゲームステート
    /// </summary>
    public T1 State
    {
        get => state;
        set
        {
            state = value;
            OnGameStateSet();
        }
    }

    /// <summary>
    /// グリッドレイアウトグループ
    /// </summary>
    [SerializeField]
    protected GridLayoutGroup container = null;

    /// <summary>
    /// セルの2次元配列
    /// </summary>
    [SerializeField]
    protected T2 cells = null;

    /// <summary>
    /// セルのプレファブ
    /// </summary>
    [SerializeField]
    protected T3 cellPrefab = null;

    protected virtual void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        // セルを初期化する
        cells.Init();

        // セルのゲームオブジェクトを生成する
        cells.InstantiateValues(cellPrefab, container.transform);
    }

    /// <summary>
    /// ゲームステートが設定されたときの処理
    /// </summary>
    protected abstract void OnGameStateSet();

    /// <summary>
    /// ゲーム変数を初期化する
    /// </summary>
    protected abstract void InitGameVariables();
}
