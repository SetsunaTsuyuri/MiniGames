using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Reversi
{
    /// <summary>
    /// セルの2次元配列
    /// </summary>
    public class CellArray2D : MonoBehaviourArray2D<Cell>
    {
        /// <summary>
        /// 周囲のセルを取得する
        /// </summary>
        /// <param name="position">座標</param>
        /// <param name="state">セルのステート</param>
        /// <returns></returns>
        public Cell[] GetSurroundingValues(Vector2Int position, CellState state)
        {
            Cell[] surrondings = GetSurroundingValues(position);
            Cell[] stoneCells = surrondings.Where(x => x.State == state).ToArray();
            return stoneCells;
        }
    }
}
