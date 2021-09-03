using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper
{
    /// <summary>
    /// セルの設定
    /// </summary>
    [CreateAssetMenu(menuName = "Minesweeper/Settings/Cell")]
    public class CellSettings : ScriptableObject
    {
        /// <summary>
        /// 色の配列
        /// </summary>
        [field: SerializeField]
        public Color[] ColorArray { get; private set; } = null;
    }
}
