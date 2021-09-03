using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シングルトンパターンのMonoBehaviour
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// インスタンス
    /// </summary>
    public static T Instance { get; private set; } = null;

    protected virtual void Awake()
    {
        SetInstance();
    }

    /// <summary>
    /// このクラスのインスタンスを設定する
    /// </summary>
    private void SetInstance()
    {
        if (Instance == null)
        {
            Instance = (T)FindObjectOfType(typeof(T));
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
