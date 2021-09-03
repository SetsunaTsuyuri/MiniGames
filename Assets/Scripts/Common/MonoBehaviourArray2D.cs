using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviourの2次元配列
/// </summary>
/// <typeparam name="T1">値の型</typeparam>
public class MonoBehaviourArray2D<T> : MonoBehaviour, IInitializable where T : MonoBehaviourArray2DValue
{
    /// <summary>
    /// 行
    /// </summary>
    [field: SerializeField]
    public int Rows { get; protected set; } = 0;

    /// <summary>
    /// 列
    /// </summary>
    [field: SerializeField]
    public int Columns { get; protected set; } = 0;

    /// <summary>
    /// 配列
    /// </summary>
    public T[] Values { get; protected set; } = null;

    public void Init()
    {
        // 値の配列が存在しない場合
        if (Values == null)
        {
            // 配列を宣言する
            Values = new T[Rows * Columns];
        }
        else
        {
            // 値をそれぞれ初期化する
            foreach (var value in Values)
            {
                value.Init();
            }
        }
    }

    /// <summary>
    /// 値のゲームオブジェクトを生成する
    /// </summary>
    /// <param name="prefab">プレファブ</param>
    /// <param name="parent">親</param>
    public void InstantiateValues(T prefab, Transform parent)
    {
        for (int i = 0; i < Values.Length; i++)
        {
            T value = Instantiate(prefab, parent);
            SetValue(value, i);
        }
    }

    /// <summary>
    /// 値を取得する
    /// </summary>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    /// <returns></returns>
    public T GetValue(int x, int y)
    {
        T value = null;

        if (!IsOutOfRange(x, y))
        {
            value = Values[y * Rows + x];
        }

        return value;
    }

    /// <summary>
    /// 値を取得する
    /// </summary>
    /// <param name="position">座標</param>
    /// <returns></returns>
    public T GetValue(Vector2Int position)
    {
        return GetValue(position.x, position.y);
    }

    /// <summary>
    /// 値を代入する
    /// </summary>
    /// <param name="value">値</param>
    /// <param name="index">座標</param>
    /// <returns></returns>
    public bool SetValue(T value, int index)
    {
        bool result = false;

        if (!IsOutOfRange(index))
        {
            Values[index] = value;
            value.Position = ToPosition(index);
            result = true;
        }

        return result;
    }

    /// <summary>
    /// 値を代入する
    /// </summary>
    /// <param name="value">値</param>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    /// <returns>代入に成功したか</returns>
    public bool SetValue(T value, int x, int y)
    {
        bool result = false;

        if (!IsOutOfRange(x, y))
        {
            Values[y * Rows + x] = value;
            value.Position = new Vector2Int(x, y);
            result = true;
        }

        return result;
    }

    /// <summary>
    /// 値を代入する
    /// </summary>
    /// <param name="value">値</param>
    /// <param name="position">座標</param>
    /// <returns>代入に成功したか</returns>
    public bool SetValue(T value, Vector2Int position)
    {
        return SetValue(value, position.x, position.y);
    }

    /// <summary>
    /// 配列の領域外か
    /// </summary>
    /// <param name="index">配列の添え字</param>
    /// <returns></returns>
    private bool IsOutOfRange(int index)
    {
        bool result = false;

        if (index < 0 || index >= Values.Length)
        {
            result = true;
        }

        return result;
    }

    /// <summary>
    /// 配列の領域外か
    /// </summary>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    /// <returns></returns>
    private bool IsOutOfRange(int x, int y)
    {
        bool result = false;

        if (x < 0 || x >= Columns ||
            y < 0 || y >= Rows)
        {
            result = true;
        }

        return result;
    }

    /// <summary>
    ///  指定した座標の周囲の値を配列にして取得する
    /// </summary>
    /// <param name="x">X座標</param>
    /// <param name="column">Y座標</param>
    /// <returns></returns>
    public T[] GetSurroundingValues(int x, int y)
    {
        List<T> valueList = new List<T>();

        for (int row = y - 1; row <= y + 1; row++)
        {
            for (int column = x - 1; column <= x + 1; column++)
            {
                // 自身または配列範囲外は取得しない
                if (column == x && row == y || IsOutOfRange(column, row))
                {
                    continue;
                }

                T value = GetValue(column, row);
                valueList.Add(value);
            }
        }

        T[] valueArray = valueList.ToArray();
        return valueArray;
    }

    /// <summary>
    /// 指定した座標の周囲の値を配列にして取得する
    /// </summary>
    /// <param name="position">座標</param>
    /// <returns></returns>
    public T[] GetSurroundingValues(Vector2Int position)
    {
        return GetSurroundingValues(position.x, position.y);
    }

    /// <summary>
    /// 指定したX座標の値を全て取得する
    /// </summary>
    /// <param name="x">X座標</param>
    /// <returns>値の配列</returns>
    public T[] GetXValues(int x)
    {
        T[] valueArray = new T[Rows];
        for (int i = 0; i < Rows; i++)
        {
            valueArray[i] = GetValue(x, i);
        }

        return valueArray;
    }

    /// <summary>
    /// 指定したY座標の値を全て取得する
    /// </summary>
    /// <param name="y">Y座標</param>
    /// <returns>値の配列</returns>
    public T[] GetYValues(int y)
    {
        T[] valueArray = new T[Columns];
        for (int i = 0; i < Columns; i++)
        {
            valueArray[i] = GetValue(i, y);
        }
        return valueArray;
    }

    /// <summary>
    /// 配列の添え字を座標に変換する
    /// </summary>
    /// <param name="index">添え字</param>
    /// <returns>座標</returns>
    public Vector2Int ToPosition(int index)
    {
        Vector2Int position = Vector2Int.zero;
        position.x = index % Columns;
        position.y = index / Rows;

        return position;
    }
}
