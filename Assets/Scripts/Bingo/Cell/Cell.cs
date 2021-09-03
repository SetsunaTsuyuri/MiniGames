using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bingo
{
    /// <summary>
    /// セル
    /// </summary>
    public class Cell : CellBase<CellState, CellSettings>
    {
        /// <summary>
        /// 数字
        /// </summary>
        [SerializeField]
        int number = 0;

        /// <summary>
        /// 数字
        /// </summary>
        public int Number
        {
            get => number;
            set
            {
                number = value;
                UpdateText();
            }
        }

        /// <summary>
        /// セルの画像
        /// </summary>
        [SerializeField]
        Image cellImage = null;

        /// <summary>
        /// 数字のテキスト
        /// </summary>
        [SerializeField]
        Text numberText = null;

        protected override void OnCellStateSet()
        {
            base.OnCellStateSet();

            UpdateColor();
            UpdateText();
        }

        /// <summary>
        /// セルの色を更新する
        /// </summary>
        private void UpdateColor()
        {
            Color newColor;

            if (State == CellState.Valid ||
                State == CellState.Free)
            {
                newColor = settings.ValidCellColor;
            }
            else
            {
                newColor = settings.DefaultColor;
            }

            cellImage.color = newColor;
        }

        /// <summary>
        /// 数字のテキストを更新する
        /// </summary>
        private void UpdateText()
        {
            string newText;
            if (State == CellState.Free)
            {
                newText = settings.FreeCellText;
            }
            else
            {
                newText = Number.ToString();
            }

            numberText.text = newText;
        }

        public override void Init()
        {
            base.Init();

            State = CellState.Invalid;
            Number = 0;
        }

        /// <summary>
        /// クリックされたときのイベント(デバッグ用)
        /// </summary>
        public void OnClicked()
        {
            Debug.Log(Position);

            if (State == CellState.Invalid)
            {
                State = CellState.Valid;
            }
            else if (State == CellState.Valid)
            {
                State = CellState.Invalid;
            }
        }
    }
}
