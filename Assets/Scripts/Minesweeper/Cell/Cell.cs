using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper
{
    /// <summary>
    /// セル
    /// </summary>
    public class Cell : CellBase<CellState, CellSettings>, IClickable
    {
        /// <summary>
        /// 開かれたか
        /// </summary>
        [SerializeField]
        bool isOpened = false;

        /// <summary>
        /// 開かれたか
        /// </summary>
        public bool IsOpened
        {
            get => isOpened;
            set
            {
                isOpened = value;
                OnCellOpendOrClosed();
            }
        }

        /// <summary>
        /// 旗が立てられたか
        /// </summary>
        [SerializeField]
        bool flagged = false;

        /// <summary>
        /// 旗が立てられたか
        /// </summary>
        public bool Flagged
        {
            get => flagged;
            set
            {
                flagged = value;
                OnFlagRaisedOrRemoved();
            }
        }

        /// <summary>
        /// 地雷であることが確定しているか
        /// </summary>
        public bool IsMineDefinitely { get; set; } = false;

        /// <summary>
        /// セルに表示される文字
        /// </summary>
        [SerializeField]
        private Text mineCount = null;

        /// <summary>
        /// セルを隠す画像
        /// </summary>
        [SerializeField]
        private Image concealer = null;

        /// <summary>
        /// モンスター画像
        /// </summary>
        [SerializeField]
        private Image monster = null;

        /// <summary>
        /// 旗の画像
        /// </summary>
        [SerializeField]
        Image flag = null;

        /// <summary>
        /// 通常セルがクリックされたときのイベント
        /// </summary>
        [SerializeField]
        GameEvent onSafeCellClicked = null;

        /// <summary>
        /// 通常セルが開かれたときのイベント
        /// </summary>
        [SerializeField]
        GameEvent onSafeCellOpened = null;

        /// <summary>
        /// 地雷セルが開かれたときのイベント
        /// </summary>
        [SerializeField]
        GameEvent onMineCellOpened = null;

        /// <summary>
        /// 安全なセルが開かれたときのイベント
        /// </summary>
        public event Action<Cell> SafeCellOpened;

        /// <summary>
        /// クリック可能か
        /// </summary>
        public event Func<bool> AcceptClick;

        protected override void OnValidate()
        {
            base.OnValidate();

            OnCellOpendOrClosed();
            OnFlagRaisedOrRemoved();
        }

        public override void Init()
        {
            base.Init();

            IsOpened = false;
            Flagged = false;
            IsMineDefinitely = false;
            State = CellState.None;
        }

        protected override void OnCellStateSet()
        {
            base.OnCellStateSet();

            if (mineCount == null)
            {
                return;
            }

            if (State == CellState.None)
            {
                mineCount.text = "";
                monster.enabled = false;
            }
            else if (State == CellState.Mine)
            {
                mineCount.text = "";
                monster.enabled = true;
            }
            else
            {
                int mines = (int)State;
                mineCount.text = mines.ToString();
                Color newColor = settings.ColorArray[mines - 1];
                mineCount.color = newColor;

                monster.enabled = false;
            }
        }

        /// <summary>
        /// セルが開かれたまたは閉じられた場合の処理
        /// </summary>
        private void OnCellOpendOrClosed()
        {
            if (IsOpened)
            {
                concealer.gameObject.SetActive(false);
            }
            else
            {
                concealer.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 旗が揚げられたまたは降ろされた場合の処理
        /// </summary>
        private void OnFlagRaisedOrRemoved()
        {
            if (Flagged)
            {
                flag.gameObject.SetActive(true);
            }
            else
            {
                flag.gameObject.SetActive(false);
            }
        }

        public void OnClicked()
        {
            // イベント未登録またはクリックを受け付けない状態なら中止する
            if (AcceptClick == null || !AcceptClick())
            {
                return;
            }

            // 左クリック
            if (Input.GetMouseButtonUp(0))
            {
                if (!Flagged && !IsOpened)
                {
                    if (State != CellState.Mine)
                    {
                        // イベント実行
                        onSafeCellClicked.Raise();
                    }

                    Open();
                }
            }
            // 右クリック
            else if (Input.GetMouseButtonUp(1))
            {
                if (!IsOpened)
                {
                    // 旗を立てる、または撤去する
                    RaiseOrRemoveFlag();
                }
            }
        }

        /// <summary>
        /// セルを開く
        /// </summary>
        public void Open()
        {
            if (Flagged)
            {
                Flagged = false;
            }

            IsOpened = true;

            if (State == CellState.Mine)
            {
                // イベント実行
                onMineCellOpened.Raise();
            }
            else
            {
                // イベント実行
                SafeCellOpened?.Invoke(this);
                onSafeCellOpened.Raise();
            }
        }

        /// <summary>
        /// イベントを発生させずにセルを開く
        /// </summary>
        public void OpenWithoutRaisingEvents()
        {
            if (Flagged)
            {
                Flagged = false;
            }

            if (!IsOpened)
            {
                IsOpened = true;
            }
        }

        /// <summary>
        /// 旗を立てるまたは撤去する
        /// </summary>
        public void RaiseOrRemoveFlag()
        {
            // 地雷であることが確定している場合、旗は撤去できない
            if (Flagged && IsMineDefinitely)
            {
                return;
            }

            Flagged = !Flagged;
        }
    }
}
