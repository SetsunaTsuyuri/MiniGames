using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reversi
{
    /// <summary>
    /// メッセージデータ
    /// </summary>
    [System.Serializable]
    public class MessageData
    {
        [field: SerializeField]
        public MessageAttribute Attribute { get; private set; }

        [field: SerializeField]
        public string Message { get; private set; }
    }
}
