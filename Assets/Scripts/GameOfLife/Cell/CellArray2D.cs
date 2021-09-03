using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameOfLife
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
        /// <param name="deadOrAlive">生死</param>
        /// <returns></returns>
        public Cell[] GetSurroundingValues(Vector2Int position, bool deadOrAlive)
        {
            Cell[] surrondings = GetSurroundingValues(position);
            Cell[] cells = surrondings.Where(x => x.IsAlive == deadOrAlive).ToArray();
            return cells;
        }
    }
}
