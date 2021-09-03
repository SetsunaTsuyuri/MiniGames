using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// シーン遷移の管理者
/// </summary>
public class SceneTransition : MonoBehaviour
{
    /// <summary>
    /// フェード時間
    /// </summary>
    [SerializeField]
    float fadeDuration = 0.5f;

    /// <summary>
    /// フェードインマテリアル
    /// </summary>
    [SerializeField]
    Material fadeIn = null;

    /// <summary>
    /// フェードアウトマテリアル
    /// </summary>
    [SerializeField]
    Material fadeOut = null;

    /// <summary>
    /// フェードイン・アウトで表示する画像
    /// </summary>
    [SerializeField]
    Image view = null;

    private void Start()
    {
        StartCoroutine(DoFade(fadeIn, fadeDuration));
    }

    /// <summary>
    /// シーンを変更する
    /// </summary>
    /// <param name="sceneName">新しいシーンの名前</param>
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(ChangeSceneCoroutine(sceneName));
    }

    /// <summary>
    /// シーン変更のコルーチン
    /// </summary>
    /// <param name="sceneName">新しいシーンの名前</param>
    /// <returns></returns>
    private IEnumerator ChangeSceneCoroutine(string sceneName)
    {
        yield return DoFade(fadeOut, fadeDuration);
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// フェード処理を行う
    /// </summary>
    /// <param name="material">マテリアル</param>
    /// <param name="duration">時間</param>
    /// <returns></returns>
    private IEnumerator DoFade(Material material, float duration)
    {
        // 参考記事
        // https://blog.cfm-art.net/archives/963

        view.enabled = true;
        view.material = material;

        float current = 0;
        while (current < duration)
        {
            material.SetFloat("_Alpha", current / duration);
            yield return new WaitForEndOfFrame();
            current += Time.deltaTime;
        }
        material.SetFloat("_Alpha", 1);
    }
}
