using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Reversi
{
    /// <summary>
    /// メッセージのデータ集
    /// </summary>
    public class MessageDataCollections : MonoBehaviour
    {
        /// <summary>
        /// データ
        /// </summary>
        [field: SerializeField]
        public MessageData[] Data { get; private set; }

        /// <summary>
        /// データ取得
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public string GetMessage(MessageAttribute attribute)
        {
            string message = null;
            MessageData data = Data.FirstOrDefault(d => d.Attribute == attribute);
            if (data != null)
            {
                message = data.Message;
            }
            return message;
        }
    }

}
