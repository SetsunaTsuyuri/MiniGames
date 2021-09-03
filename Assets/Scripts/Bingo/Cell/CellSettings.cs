using UnityEngine;
using UnityEngine.UI;

namespace Bingo
{
    /// <summary>
    /// セルの設定
    /// </summary>
    [CreateAssetMenu(menuName = "Bingo/Settings/Cell")]
    public class CellSettings : ScriptableObject
    {
        /// <summary>
        /// セルの色(初期設定)
        /// </summary>
        [field: SerializeField]
        public Color DefaultColor { get; private set; } = Color.white;

        /// <summary>
        /// 有効なセルの色
        /// </summary>
        [field: SerializeField]
        public Color ValidCellColor { get; private set; } = Color.yellow;
 
        /// <summary>
        /// フリーセルに表示する文字
        /// </summary>
        [field: SerializeField]
        public string FreeCellText { get; private set; } = "F";
    }
}
