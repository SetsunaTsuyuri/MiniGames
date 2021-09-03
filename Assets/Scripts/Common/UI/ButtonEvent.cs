using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタンイベント
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonEvent : MonoBehaviour, IInitializable, ICacheableRequiredComponents
{
    /// <summary>
    /// ゲームイベント
    /// </summary>
    [SerializeField]
    GameEvent gameEvent = null;

    /// <summary>
    /// 1度しか押せないか
    /// </summary>
    [SerializeField]
    bool canOnlyBePressedOnce = false;

    /// <summary>
    /// ボタン
    /// </summary>
    Button selfButton = null;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        CacheRequiredComponents();
    }

    public void CacheRequiredComponents()
    {
        selfButton = GetComponent<Button>();
    }

    /// <summary>
    /// ポインターが入ったときに起こる処理
    /// </summary>
    public void OnPointerEnter()
    {
        // 自身をフォーカスする
        selfButton.Select();
    }

    /// <summary>
    /// ボタンが押されたときに起こる処理
    /// </summary>
    public void OnClicked()
    {
        UpdateInteractable();

        if (gameEvent)
        {
            gameEvent.Raise();
        }
    }

    /// <summary>
    /// ボタンの選択可否状態を更新する
    /// </summary>
    private void UpdateInteractable()
    {
        // 1度しか押せないボタンの場合
        if (selfButton.interactable && canOnlyBePressedOnce)
        {
            // ボタンを無効にする
            selfButton.interactable = false;
        }
    }
}
