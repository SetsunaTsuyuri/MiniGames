using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper
{
    /// <summary>
    /// ゲームの台詞設定
    /// </summary>
    [CreateAssetMenu(menuName = "Minesweeper/Settings/Message")]
    public class MessageSettings : ScriptableObject
    {
        /// <summary>
        /// 汎用 1行目
        /// </summary>
        [SerializeField]
        string common1st = "";

        /// <summary>
        /// 汎用 2行目
        /// </summary>
        [SerializeField]
        string common2nd = "";

        /// <summary>
        /// 獲得 1行目
        /// </summary>
        [SerializeField]
        string obtaining1st = "";

        /// <summary>
        /// 獲得 2行目
        /// </summary>
        [SerializeField]
        string obtaining2nd = "";

        /// <summary>
        /// ダメージ 1行目
        /// </summary>
        [SerializeField]
        string damage1st = "";

        /// <summary>
        /// ダメージ 2行目
        /// </summary>
        [SerializeField]
        string damage2nd = "";

        /// <summary>
        /// 汎用台詞を取得する
        /// </summary>
        /// <returns></returns>
        public string GetCommonMessage()
        {
            return GetMessage(common1st, common2nd);
        }

        /// <summary>
        /// 獲得台詞を取得する
        /// </summary>
        /// <returns></returns>
        public string GetObtaningMessage()
        {
            return GetMessage(obtaining1st, obtaining2nd);
        }

        /// <summary>
        /// 損傷台詞を取得する
        /// </summary>
        /// <returns></returns>
        public string GetDamageMessage()
        {
            return GetMessage(damage1st, damage2nd);
        }

        /// <summary>
        /// 台詞を取得する
        /// </summary>
        /// <param name="row1st">1行目</param>
        /// <param name="row2nd">2行目</param>
        /// <returns></returns>
        private string GetMessage(string row1st, string row2nd)
        {
            string message = row1st + "\n" + row2nd;
            return message;
        }
    }
}
