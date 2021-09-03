using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper
{
    /// <summary>
    /// ゲーム中のメッセージ
    /// </summary>
    public class Message : MonoBehaviour
    {
        /// <summary>
        /// 状態
        /// </summary>
        public enum State
        {
            None,
            Normal,
            Obtaining,
            Damage
        }

        /// <summary>
        /// 現在の状態
        /// </summary>
        [SerializeField]
        State currentState = State.None;

        /// <summary>
        /// 現在の状態
        /// </summary>
        public State CurrentState
        {
            get => currentState;
            set
            {
                currentState = value;
                OnStateChanged();
            }
        }

        /// <summary>
        /// 表示するテキスト
        /// </summary>
        [SerializeField]
        Text view = null;

        /// <summary>
        /// 設定
        /// </summary>
        [SerializeField]
        MessageSettings settings = null;

        private void OnValidate()
        {
            OnStateChanged();
        }

        /// <summary>
        /// 状態変化時の処理
        /// </summary>
        private void OnStateChanged()
        {
            string newText = "";
            switch (CurrentState)
            {
                case State.None:
                    break;

                case State.Normal:

                    newText = settings.GetCommonMessage();
                    break;

                case State.Obtaining:

                    newText = settings.GetObtaningMessage();
                    break;

                case State.Damage:

                    newText = settings.GetDamageMessage();
                    break;
                default:
                    break;
            }

            UpdateText(newText);
        }

        /// <summary>
        /// テキストを更新する
        /// </summary>
        private void UpdateText(string newText)
        {
            view.text = newText;   
        }
    }
}
