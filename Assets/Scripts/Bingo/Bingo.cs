using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Bingo
{
    /// <summary>
    /// ビンゴ
    /// </summary>
    public class Bingo : GameBase<GameState, CellArray2D, Cell, CellState, CellSettings>
    {
        /// <summary>
        /// 1列目の最小値
        /// </summary>
        [SerializeField]
        int startNumber = 1;

        /// <summary>
        /// 1列目の最大値
        /// </summary>
        [SerializeField]
        int endNumber = 15;

        /// <summary>
        /// フリーなセルの位置
        /// </summary>
        [SerializeField]
        Vector2Int freeCellPosition = Vector2Int.zero;

        /// <summary>
        /// 抽選機
        /// </summary>
        [SerializeField]
        LotteryMachine lotteryMachine = null;

        /// <summary>
        /// ビンゴテキスト
        /// </summary>
        [SerializeField]
        Text bingo = null;

        /// <summary>
        /// プレイ状態になったときに実行するイベント
        /// </summary>
        [SerializeField]
        GameEvent onPlay = null;

        /// <summary>
        /// セルに割り当てられる数字の候補配列
        /// </summary>
        int[] cellNumbers = null;

        public override void Init()
        {
            base.Init();

            // 抽選機にビンゴカードの穴開けイベントを登録する
            lotteryMachine.Stopped += Punch;

            // 抽選機に再抽選可能か判断するイベントを登録する
            lotteryMachine.CanRestart += () => State != GameState.Clear;

            // 準備状態になる
            State = GameState.Ready;
        }

        /// <summary>
        /// ビンゴカードの穴を開ける
        /// </summary>
        /// <param name="number"></param>
        public void Punch(int number)
        {
            Cell cell = cells.Values.FirstOrDefault(n => n.Number == number);
            if (cell != null && cell.State == CellState.Invalid)
            {
                cell.State = CellState.Valid;
            }

            if (cells.IsBingo())
            {
                // ビンゴしたことを示すテキストを表示する
                bingo.enabled = true;

                // クリア状態になる
                State = GameState.Clear;
            }
        }

        /// <summary>
        /// セルにランダムな数値を割り当てる
        /// </summary>
        /// <param name="cellArray2D"></param>
        private void AssignRandomNumber(CellArray2D cellArray2D)
        {
            int start = startNumber;
            int end = endNumber;

            for (int i = 0; i < cellArray2D.Columns; i++)
            {
                cellNumbers = RandomUtility.GetRandomIntArrayWithoutDuplication(start, end, cellArray2D.Rows);
                Cell[] columns = cellArray2D.GetXValues(i);
                for (int j = 0; j < columns.Length; j++)
                {
                    columns[j].Number = cellNumbers[j];
                }
                start += endNumber;
                end += endNumber;
            }
        }

        /// <summary>
        /// フリーのセルを設定する
        /// </summary>
        /// <param name="position">座標</param>
        private void SetFreeCell(Vector2Int position)
        {
            Cell cell = cells.GetValue(position);
            cell.State = CellState.Free;
        }

        protected override void OnGameStateSet()
        {
            switch (State)
            {
                case GameState.Ready:

                    // ゲーム変数初期化
                    InitGameVariables();

                    // ビンゴテキストを非表示にする
                    bingo.enabled = false;

                    // ビンゴカードを初期化する
                    cells.Init();

                    // セルにランダムな数値を割り当てる
                    AssignRandomNumber(cells);

                    // フリーのセルを設定する
                    SetFreeCell(freeCellPosition);

                    // 抽選機を初期化する
                    lotteryMachine.Init();
                    break;

                case GameState.Play:

                    // イベント実行
                    onPlay.Raise();

                    // ルーレットを回す
                    lotteryMachine.State = LotteryMachineState.Run;
                    break;
                case GameState.Clear:
                    break;
                default:
                    break;
            }
        }

        protected override void InitGameVariables()
        {
            lotteryMachine.NumbersCount.Init();
        }

        /// <summary>
        /// ゲーム開始ボタンが押されたときの処理
        /// </summary>
        public void OnGameStartButtonPressed()
        {
            State = GameState.Play;
        }

        /// <summary>
        /// やり直しボタンが押されたときのイベント
        /// </summary>
        public void OnRestartButtonPressed()
        {
            State = GameState.Ready;
        }
    }
}
