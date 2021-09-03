using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameOfLife
{
    /// <summary>
    /// ライフゲーム
    /// </summary>
    public class GameOfLife : GameBase<GameState, CellArray2D, Cell, CellState, CellSettings>
    {
        /// <summary>
        /// 世代を更新する処理を行う間隔
        /// </summary>
        [SerializeField]
        float updateInterval = 0.2f;

        /// <summary>
        /// 世代更新カウント
        /// </summary>
        float updateIntervalCount = 0.0f;

        /// <summary>
        /// 生きているセルの数(初期状態)
        /// </summary>
        [SerializeField]
        int initialLivingCells = 0;

        /// <summary>
        /// 誕生
        /// </summary>
        [SerializeField, Range(0, 8)]
        int birth = 3;

        /// <summary>
        /// 過疎
        /// </summary>
        [SerializeField, Range(0, 3)]
        int underpopulation = 1;

        /// <summary>
        /// 過密
        /// </summary>
        [SerializeField, Range(4, 8)]
        int overpopulation = 4;

        /// <summary>
        /// 世代
        /// </summary>
        [SerializeField]
        IntGameVariable steps = null;

        /// <summary>
        /// 死んでいるセルの数
        /// </summary>
        [SerializeField]
        IntGameVariable deadCells = null;

        /// <summary>
        /// 生きているセルの数
        /// </summary>
        [SerializeField]
        IntGameVariable livingCells = null;

        /// <summary>
        /// 誕生したセルの数
        /// </summary>
        [SerializeField]
        IntGameVariable bornCells = null;

        /// <summary>
        /// 生存したセルの数
        /// </summary>
        [SerializeField]
        IntGameVariable survivalCells = null;

        /// <summary>
        /// 過疎により死亡したのセルの数
        /// </summary>
        [SerializeField]
        IntGameVariable underpopulationCells = null;

        /// <summary>
        /// 過密により死亡したセルの数
        /// </summary>
        [SerializeField]
        IntGameVariable overpopulationCells = null;

        /// <summary>
        /// 準備状態になったときに実行するイベント
        /// </summary>
        [SerializeField]
        GameEvent onReady = null;

        /// <summary>
        /// プレイ状態になったときに実行するイベント
        /// </summary>
        [SerializeField]
        GameEvent onPlay = null;

        public override void Init()
        {
            base.Init();

            // 各セルにイベントを登録する
            RegisterCellsEvents();

            // 準備状態になる
            State = GameState.Ready;
        }

        /// <summary>
        /// 各セルにイベントを登録する
        /// </summary>
        private void RegisterCellsEvents()
        {
            foreach (var cell in cells.Values)
            {
                // 準備状態であればクリックを受け付ける
                cell.AcceptClick += () => State == GameState.Ready;
            }
        }

        protected override void OnGameStateSet()
        {
            switch (State)
            {
                case GameState.None:
                    break;

                case GameState.Ready:

                    // Ready状態のイベント実行
                    onReady.Raise();

                    // セルを初期化する
                    cells.Init();

                    // ゲーム変数を初期化する
                    InitGameVariables();

                    // 世代更新カウントをリセットする
                    InitUpdateInterValCount();

                    // 生きたセルをランダムに配置する
                    SetLivingCellsAtRandomPositions(initialLivingCells);
                    break;

                case GameState.Play:

                    // プレイ状態のイベントを実行する
                    onPlay.Raise();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 生きたセルをランダムな位置に設定する
        /// </summary>
        /// <param name="aliveCount">生きたセルの数</param>
        private void SetLivingCellsAtRandomPositions(int aliveCount)
        {
            int cellCount = cells.Values.Length;

            // 生きたセルの数が全体の数より多く設定されている場合は中止する
            if (aliveCount > cellCount)
            {
                Debug.LogError("生きたセルの数が全体の数より多く設定されています");
                return;
            }

            // どのセルを生きた状態にするか判断する配列
            bool[] isAlive = RandomUtility.GetRandomBoolArray(cellCount, aliveCount);

            // 判断する配列でtrueになっている位置と同じ位置のセルを生きた状態にする
            for (int i = 0; i < cellCount; i++)
            {
                if (isAlive[i])
                {
                    Cell cell = cells.Values[i];
                    cell.IsAlive = true;
                }
            }
        }

        protected override void InitGameVariables()
        {
            IInitializable[] initializables =
            {
                steps,
                deadCells,
                livingCells,
                bornCells,
                survivalCells,
                underpopulationCells,
                overpopulationCells
            };

            foreach (var val in initializables)
            {
                val.Init();
            }
        }

        private void Update()
        {
            // プレイ中の場合
            if (State == GameState.Play)
            {
                // 世代更新カウントが0以下の場合
                if (updateIntervalCount <= 0.0f)
                {
                    // 世代更新カウントをリセットする
                    InitUpdateInterValCount();

                    // 次世代へ移行する
                    ToTheNextStep();
                }
                else
                {
                    // 経過時間分カウントを減らす
                    updateIntervalCount -= Time.deltaTime;
                }
            }
        }

        /// <summary>
        /// 世代更新カウントをリセットする
        /// </summary>
        private void InitUpdateInterValCount()
        {
            updateIntervalCount = updateInterval;
        }

        /// <summary>
        /// 次の世代へ移行する
        /// </summary>
        private void ToTheNextStep()
        {
            // セルの状態を更新する
            UpdateCellsState();

            // セルの生死を更新する
            UpdateCellsDeadOrAlive();

            // ゲーム変数を更新する
            UpdateGameVariables();
        }

        /// <summary>
        /// セルの状態を更新する
        /// </summary>
        private void UpdateCellsState()
        {
            foreach (var cell in cells.Values)
            {
                // 周囲の生きているセルを数える
                int aliveCount = CountSurroundingAlives(cell.Position);

                // 次世代におけるセルの状態を取得する
                CellState nextStepState = GetNextStepCellState(cell, aliveCount);

                // セルの状態を設定する
                cell.State = nextStepState;

            }
        }

        /// <summary>
        /// 周囲の生きているセルを数える
        /// </summary>
        /// <param name="position">座標</param>
        /// <returns></returns>
        private int CountSurroundingAlives(Vector2Int position)
        {
            // 周囲の生きているセルを取得する
            Cell[] surroundingCells = cells.GetSurroundingValues(position, true);
            
            // 生きているセルの数を数える
            int livings = surroundingCells.Length;
            return livings;
        }

        /// <summary>
        /// 次世代の状態を取得する
        /// </summary>
        /// <param name="cell">セル</param>
        /// <param name="surroundingAlives">周囲の生きているセルの数</param>
        /// <returns></returns>
        private CellState GetNextStepCellState(Cell cell, int surroundingAlives)
        {
            CellState nextStep = CellState.None;

            // セルが生きている場合
            if (cell.IsAlive)
            {
                // 過疎
                if (surroundingAlives <= underpopulation)
                {
                    nextStep = CellState.Underpopulation;
                }
                // 過密
                else if (surroundingAlives >= overpopulation)
                {
                    nextStep = CellState.Overpopulation;
                }
                // 生存
                else
                {
                    nextStep = CellState.Survival;
                }
            }
            // セルが死んでいる場合
            else
            {
                // 誕生
                if (surroundingAlives == birth)
                {
                    nextStep = CellState.Birth;
                }
            }

            return nextStep;
        }

        /// <summary>
        /// セルの生死状態を更新する
        /// </summary>
        private void UpdateCellsDeadOrAlive()
        {
            foreach (var cell in cells.Values)
            {
                cell.UpdateDeadOrAlive();
            }
        }

        /// <summary>
        /// ゲーム変数を更新する
        /// </summary>
        private void UpdateGameVariables()
        {
            // 世代
            steps.RuntimeValue++;

            // セルの配列
            Cell[] values = cells.Values;

            // 死んでいるセルの数
            deadCells.RuntimeValue = values.Count(x => !x.IsAlive);

            // 生きているセルの数
            livingCells.RuntimeValue = values.Count(x => x.IsAlive);

            // 誕生したセルの数
            bornCells.RuntimeValue = values.Count(x => x.State == CellState.Birth);

            // 生存したセルの数
            survivalCells.RuntimeValue = values.Count(x => x.State == CellState.Survival);

            // 過疎により死亡したセルの数
            underpopulationCells.RuntimeValue = values.Count(x => x.State == CellState.Underpopulation);

            // 過密により死亡したセルの数
            overpopulationCells.RuntimeValue = values.Count(x => x.State == CellState.Overpopulation);

        }

        /// <summary>
        /// ゲーム開始ボタンが押されたときの処理
        /// </summary>
        public void OnGameStartButtonPressed()
        {
            State = GameState.Play;
        }

        /// <summary>
        /// やり直しボタンが押されたときの処理
        /// </summary>
        public void OnRestartButtonPressed()
        {
            State = GameState.Ready;
        }
    }
}
