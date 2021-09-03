using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ゲームイベントリスナー
/// </summary>
public class GameEventListener : MonoBehaviour
{
    [SerializeField]
    GameEvent gameEvent = null;

    [SerializeField]
    UnityEvent response = null;

    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        gameEvent.UnregisterListener(this);
    }

    /// <summary>
    /// イベント実行時に行う処理
    /// </summary>
    public void OnEventRaised()
    {
        response.Invoke();
    }
}
