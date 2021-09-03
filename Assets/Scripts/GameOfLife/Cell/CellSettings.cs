using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameOfLife
{
    /// <summary>
    /// セルの設定
    /// </summary>
    [CreateAssetMenu(menuName = "GameOfLife/Settings/Cell")]
    public class CellSettings : ScriptableObject
    {
        /// <summary>
        /// 死亡時の色
        /// </summary>
        [field: SerializeField]
        public Color Dead { get; private set; } = Color.white;

        /// <summary>
        /// 生存時の色
        /// </summary>
        [field: SerializeField]
        public Color Alive { get; private set; } = Color.black;
    }
}
