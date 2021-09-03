using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2次元配列の値
/// </summary>
public abstract class MonoBehaviourArray2DValue : MonoBehaviour, IInitializable
{
    /// <summary>
    /// 座標
    /// </summary>
    public Vector2Int Position { get; set; } = Vector2Int.zero;

    public virtual void Init() { }
}
