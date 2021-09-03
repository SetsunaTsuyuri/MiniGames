using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ランダム関連の便利なクラス
/// </summary>
public static class RandomUtility
{
    /// <summary>
    /// tureの位置がランダムなbool型の配列を取得する
    /// </summary>
    /// <param name="arrayCount">配列の要素数</param>
    /// <param name="trueCount">trueの数</param>
    /// <returns></returns>
    public static bool[] GetRandomBoolArray(int arrayCount, int trueCount)
    {
        bool[] array = new bool[arrayCount];
        for (int i = 0; i < trueCount; i++)
        {
            array[i] = true;
        }

        bool[] randomArray = array.Shuffle().ToArray();
        return randomArray;
    }

    /// <summary>
    /// ランダムな値のInt型配列を取得する(重複なし)
    /// </summary>
    /// <param name="min">最小値</param>
    /// <param name="max">最大値</param>
    /// <returns></returns>
    public static int[] GetRandomIntArrayWithoutDuplication(int min, int max)
    {
        if (min > max)
        {
            Debug.LogError("最小値が最大値より高くなっています");
            return null;
        }

        int numbers = max - min + 1;
        if (numbers <= 0)
        {
            Debug.LogError("要素数に対して乱数の候補が足りません");
            return null;
        }

        int[] array = new int[numbers];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = min + i;
        }

        int[] randomArray = array.Shuffle().ToArray();
        return randomArray;
    }


    /// <summary>
    /// ランダムな値のInt型配列を取得する(重複なし)
    /// </summary>
    /// <param name="min">最小値</param>
    /// <param name="max">最大値</param>
    /// <param name="length">要素数</param>
    /// <returns></returns>
    public static int[] GetRandomIntArrayWithoutDuplication(int min, int max, int length)
    {
        int[] array = GetRandomIntArrayWithoutDuplication(min, max);
        if (array == null)
        {
            return null;
        }

        int[] randomArray = array.Take(length).ToArray();
        return randomArray;
    }
}
