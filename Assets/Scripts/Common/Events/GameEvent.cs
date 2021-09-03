using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームイベント
/// </summary>
[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    /// <summary>
    /// リスナーのリスト
    /// </summary>
    List<GameEventListener> listeners = new List<GameEventListener>();

    /// <summary>
    /// イベントを実行する
    /// </summary>
    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }

    /// <summary>
    /// イベントリスナーを登録する
    /// </summary>
    /// <param name="listener">リスナー</param>
    public void RegisterListener(GameEventListener listener)
    {
        listeners.Add(listener);
    }

    /// <summary>
    /// イベントリスナーを登録解除する
    /// </summary>
    /// <param name="listener">リスナー</param>
    public void UnregisterListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }

}
