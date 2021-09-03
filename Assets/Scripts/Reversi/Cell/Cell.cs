using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reversi
{
    /// <summary>
    /// セル
    /// </summary>
    public class Cell : CellBase<CellState, CellSettings>, IClickable
    {
        /// <summary>
        /// 裏返せる石の数
        /// </summary>
        [SerializeField]
        int stoneCount = 0;

        /// <summary>
        /// 裏返せる石の数
        /// </summary>
        public int StoneCount
        {
            get => stoneCount;
            set
            {
                stoneCount = value;
                OnStoneCountSet();
            }
        }

        /// <summary>
        /// 石の画像
        /// </summary>
        [SerializeField]
        Image stone = null;

        /// <summary>
        /// 石置き可能なセルを示すマークの画像
        /// </summary>
        [SerializeField]
        Image puttableMark = null;

        /// <summary>
        /// 増やせる石の数を示すテキスト
        /// </summary>
        [SerializeField]
        Text countText = null;

        /// <summary>
        /// 石が置かれたときのイベント(ゲームイベント)
        /// </summary>
        [SerializeField]
        GameEvent onStonePut = null;

        /// <summary>
        /// 石が置かれたときのイベント(アクション)
        /// </summary>
        public event Action<Cell> PutStone;

        /// <summary>
        /// クリックを受け付けるか
        /// </summary>
        public event Func<bool> AcceptClick;

        protected override void OnValidate()
        {
            base.OnValidate();

            OnStoneCountSet();
        }

        /// <summary>
        /// 裏返せる石の数が設定されたときの処理
        /// </summary>
        private void OnStoneCountSet()
        {
            if (CanBePutStone())
            {
                puttableMark.enabled = true;
            }
            else
            {
                puttableMark.enabled = false;
            }

            string newText = "";
            if (CanBePutStone())
            {
                newText = stoneCount.ToString();
            }

            countText.text = newText;
        }

        protected override void OnCellStateSet()
        {
            base.OnCellStateSet();

            switch (State)
            {
                case CellState.Empty:
                    stone.enabled = false;
                    break;

                case CellState.Black:
                    stone.enabled = true;
                    stone.color = Color.black;
                    break;

                case CellState.White:
                    stone.enabled = true;
                    stone.color = Color.white;
                    break;

                default:
                    break;
            }
        }

        public override void Init()
        {
            base.Init();

            State = CellState.Empty;
        }

        public void OnClicked()
        {
            // イベント未登録またはクリックを受け付けない状態なら中止する
            if (AcceptClick == null || !AcceptClick())
            {
                Debug.Log(Position + "このセルはクリックを受け付けません");
                return;
            }

            // 石置き不可の場合中止する
            if (!CanBePutStone())
            {
                Debug.Log(Position + "このセルに石を置くことはできません");
                return;
            }

            // イベント実行
            PutStone?.Invoke(this);
            onStonePut.Raise();
        }

        /// <summary>
        /// 裏返る
        /// </summary>
        public void TurnOver()
        {
            switch (State)
            {
                case CellState.Empty:
                    break;

                case CellState.Black:
                    State = CellState.White;
                    break;

                case CellState.White:
                    State = CellState.Black;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 石を置くことができるか
        /// </summary>
        /// <returns></returns>
        public bool CanBePutStone()
        {
            return StoneCount > 0;
        }
    }
}
