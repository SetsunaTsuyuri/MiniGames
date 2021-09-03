using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bingo
{
    /// <summary>
    /// セルの2次元配列
    /// </summary>
    public class CellArray2D : MonoBehaviourArray2D<Cell>
    {
        public bool IsBingo()
        {
            // 横のビンゴ
            for (int i = 0; i < Columns; i++)
            {
                Cell[] column = GetXValues(i);
                if (!ContainInvalid(column))
                {
                    return true;
                }
            }

            // 縦のビンゴ
            for (int i = 0; i < Rows; i++)
            {
                Cell[] row = GetYValues(i);
                if (!ContainInvalid(row))
                {
                    return true;
                }
            }

            // 斜め右下のビンゴ
            List<Cell> diagonallyRightDown = new List<Cell>();
            GetCellLine(diagonallyRightDown, Vector2Int.zero, Vector2Int.one);
            if (!ContainInvalid(diagonallyRightDown))
            {
                return true;
            }

            // 斜め右上のビンゴ
            List<Cell> diagonallyRightUp = new List<Cell>();
            GetCellLine(diagonallyRightUp, new Vector2Int(0, Rows - 1), new Vector2Int(1, -1));
            if (!ContainInvalid(diagonallyRightUp))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// セルを一直線に取得する
        /// </summary>
        /// <param name="cells">セルの配列</param>
        /// <param name="pivot">基準</param>
        /// <param name="direction">方向</param>
        private void GetCellLine(List<Cell> cells, Vector2Int pivot, Vector2Int direction)
        {
            Cell cell = GetValue(pivot);
            if (cell != null)
            {
                cells.Add(cell);
                GetCellLine(cells, pivot + direction, direction);
            }
        }

        /// <summary>
        /// 無効なマスが含まれている
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        private bool ContainInvalid(Cell[] cells)
        {
            return cells.Any(c => c.State == CellState.Invalid);

        }

        /// <summary>
        /// 無効なマスが含まれている
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        private bool ContainInvalid(List<Cell> cells)
        {
            return cells.Any(c => c.State == CellState.Invalid);
        }
    }
}
