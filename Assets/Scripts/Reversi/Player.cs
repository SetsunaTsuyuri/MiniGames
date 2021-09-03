using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reversi
{
    /// <summary>
    /// プレイヤー
    /// </summary>
    public class Player : MonoBehaviour
    {
        /// <summary>
        /// 石の色
        /// </summary>
        [field: SerializeField]
        public CellState StoneColor { get; private set; } = CellState.Black;

        /// <summary>
        /// 盤面にある石の数
        /// </summary>
        [field: SerializeField]
        public IntGameVariable Stones { get; private set; } = null;

        /// <summary>
        /// 直前のターンに石を置いたか
        /// </summary>
        public bool PutStoneInPreviousTurn { get; set; } = false;
    }
}
