using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bingo
{
    /// <summary>
    /// 抽選機
    /// </summary>
    public class LotteryMachine : MonoBehaviour, IInitializable
    {
        /// <summary>
        /// 最小値
        /// </summary>
        [SerializeField]
        int min = 1;

        /// <summary>
        /// 最大値
        /// </summary>
        [SerializeField]
        int max = 75;

        /// <summary>
        /// 更新する間隔
        /// </summary>
        [SerializeField]
        float updateInterval = 0.1f;

        /// <summary>
        /// 更新カウント
        /// </summary>
        float updateIntervalCount = 0.0f;

        /// <summary>
        /// 状態
        /// </summary>
        public LotteryMachineState State { get; set; } = LotteryMachineState.Stop;

        /// <summary>
        /// 数字のリスト
        /// </summary>
        public List<int> Numbers { get; set; } = null;

        /// <summary>
        /// リスト何番目が選ばれているか
        /// </summary>
        int selectedIndex = 0;

        /// <summary>
        /// 抽選される数字の数
        /// </summary>
        [field: SerializeField]
        public IntGameVariable NumbersCount { get; private set; } = null;

        /// <summary>
        /// 抽選機が止められたときのイベント
        /// </summary>
        public event Action<int> Stopped;

        /// <summary>
        /// 1度止まった後、抽選を再開可能か
        /// </summary>
        public event Func<bool> CanRestart;

        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                UpdateText();
            }
        }

        /// <summary>
        /// 数字のテキスト
        /// </summary>
        [SerializeField]
        Text text = null;

        /// <summary>
        /// 最初に表示する文字列
        /// </summary>
        [SerializeField]
        string initialText = "?";

        public void Init()
        {
            State = LotteryMachineState.Stop;

            Numbers = new List<int>();
            for (int i = min; i <= max; i++)
            {
                Numbers.Add(i);
            }

            text.text = initialText;
        }

        private void Update()
        {
            switch (State)
            {
                case LotteryMachineState.Stop:
                    break;

                case LotteryMachineState.Run:
                    UpdateOnRun();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Run状態のときの更新処理
        /// </summary>
        private void UpdateOnRun()
        {
            if (updateIntervalCount <= 0.0f)
            {
                // 更新カウントを初期化する
                updateIntervalCount = updateInterval;

                // リスト選択を更新する
                UpdateSelecetedIndex();
            }
            else
            {
                // カウントを経過時間分減らす
                updateIntervalCount -= Time.deltaTime;

            }
        }

        /// <summary>
        /// リスト何番目を選ぶかを更新する
        /// </summary>
        private void UpdateSelecetedIndex()
        {
            if (Numbers == null || Numbers.Count == 0)
            {
                return;
            }

            if (SelectedIndex == Numbers.Count - 1 || Numbers.Count == 1)
            {
                SelectedIndex = 0;
            }
            else
            {
                SelectedIndex++;
            }
        }

        /// <summary>
        /// テキストを更新する
        /// </summary>
        private void UpdateText()
        {
            if (Numbers == null || Numbers.Count == 0)
            {
                return;
            }

            text.text = Numbers[SelectedIndex].ToString();
        }

        /// <summary>
        /// 停止ボタンが押されたときの処理
        /// </summary>
        public void OnStopButtonPressed()
        {
            if (Numbers == null || Numbers.Count == 0)
            {
                return;
            }

            // イベント未登録なら中止する
            if (Stopped == null || CanRestart == null)
            {
                return;
            }

            // 抽選を再開できないなら中止する
            if (!CanRestart())
            {
                return;
            }

            // 停止しているなら抽選を再開する
            if (State == LotteryMachineState.Stop)
            {
                State = LotteryMachineState.Run;
                return;
            }


            // 停止する
            State = LotteryMachineState.Stop;

            // 選ばれた数字
            int selectedNumber = Numbers[SelectedIndex];

            // 停止したときのイベント実行
            Stopped(selectedNumber);

            // 数字リストカウントを減らす
            NumbersCount.RuntimeValue--;

            // 同じ数字が選ばれないよう、数字リストから今選ばれている数字を取り除く
            // ※稀にArgumentOutOfRangeExceptionが発生する。未解決。
            // Numbers.RemoveAt(SelectedIndex);
        }
    }
}
