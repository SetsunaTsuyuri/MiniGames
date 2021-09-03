using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameOfLife
{
    /// <summary>
    /// セル
    /// </summary>
    public class Cell : CellBase<CellState, CellSettings>, IClickable
    {
        /// <summary>
        /// 生きているか
        /// </summary>
        [SerializeField]
        bool isAlive = false;

        /// <summary>
        /// 生きているか
        /// </summary>
        public bool IsAlive
        {
            get => isAlive;
            set
            {
                isAlive = value;
                OnDeadOrAlive();
            }
        }

        /// <summary>
        /// 画像
        /// </summary>
        [SerializeField]
        Image image = null;

        /// <summary>
        /// クリック可能か
        /// </summary>
        public event Func<bool> AcceptClick;

        protected override void OnValidate()
        {
            base.OnValidate();

            OnDeadOrAlive();
        }

        public override void Init()
        {
            base.Init();

            IsAlive = false;
            State = CellState.None;

        }

        /// <summary>
        /// 生死状態を更新する
        /// </summary>
        public void UpdateDeadOrAlive()
        {
            // 自身の状態によって生死を設定する
            switch (State)
            {
                case CellState.None:
                    break;

                case CellState.Birth:
                    IsAlive = true;
                    break;

                case CellState.Survival:
                    IsAlive = true;
                    break;

                case CellState.Underpopulation:
                    IsAlive = false;
                    break;

                case CellState.Overpopulation:
                    IsAlive = false;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 生死が設定されたときの処理
        /// </summary>
        private void OnDeadOrAlive()
        {
            if (IsAlive)
            {
                image.color = settings.Alive;
            }
            else
            {
                image.color = settings.Dead;
            }
        }

        public void OnClicked()
        {
            // イベント未登録またはクリックを受け付けない状態なら中止する
            if (AcceptClick == null || !AcceptClick())
            {
                return;
            }

            // 生死を切り替える
            SwitchDeadOrAlive();
        }

        /// <summary>
        /// 生死を切り替える
        /// </summary>
        private void SwitchDeadOrAlive()
        {
            IsAlive = !IsAlive;
        }
    }
}
