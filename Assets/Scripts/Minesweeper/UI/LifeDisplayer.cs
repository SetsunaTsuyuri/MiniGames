using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper
{
    /// <summary>
    /// ライフを表示する
    /// </summary>
    public class LifeDisplayer : GameVariableDisplayerBase<IntGameVariable, int>
    {
        /// <summary>
        /// ライフのプレファブ
        /// </summary>
        [SerializeField]
        Image imagePrefab = null;

        /// <summary>
        /// ライフのリスト
        /// </summary>
        [SerializeField]
        List<Image> lifeImageList = new List<Image>();

        protected override void UpdateDisplay()
        {
            base.UpdateDisplay();

            // 現在のライフ
            int currentLife = gameVariable.RuntimeValue;

            // アクティブな画像の数
            int activeImageCount = lifeImageList.Count(n => n.isActiveAndEnabled);

            if (currentLife > activeImageCount)
            {
                // 生成すべき数
                int toBeGenerated = currentLife - activeImageCount;

                // 非アクティブの画像を抽出する
                Image[] nonActiveImages = lifeImageList.Where(n => !n.isActiveAndEnabled).ToArray();

                // 可能な限りアクティブ化する
                foreach (var image in nonActiveImages)
                {
                    if (toBeGenerated <= 0)
                    {
                        break;
                    }

                    image.gameObject.SetActive(true);
                    toBeGenerated--;
                }

                // それでも足りなければ画像を生成する
                GenerateImages(toBeGenerated);
            }
            else if (currentLife < activeImageCount)
            {
                int toBeDeactivated = activeImageCount - currentLife;
                DeactivateImages(toBeDeactivated);
            }
        }

        /// <summary>
        /// ライフ画像を生成する
        /// </summary>
        /// <param name="number">生成する数</param>
        private void GenerateImages(int number)
        {
            for (int i = 0; i < number; i++)
            {
                // 画像を生成する
                Image image = Instantiate(imagePrefab, transform);

                // リストに加える
                lifeImageList.Add(image);
            }
        }

        /// <summary>
        /// ライフ画像を非アクティブにする
        /// </summary>
        /// <param name="number">非アクティブにする数</param>
        private void DeactivateImages(int number)
        {
            // アクティブな画像を抽出する
            Image[] activeImages = lifeImageList.Where(n => n.isActiveAndEnabled).ToArray();

            // 指定した数だけ非アクティブ化する
            foreach (var image in activeImages)
            {
                if (number <= 0)
                {
                    break;
                }

                image.gameObject.SetActive(false);
                number--;
            }
        }
    }
}
