using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper
{
    /// <summary>
    /// マインスイーパー
    /// </summary>
    public class Minesweeper : GameBase<GameState, CellArray2D, Cell, CellState, CellSettings>
    {
        /// <summary>
        /// 地雷の数
        /// </summary>
        [SerializeField]
        int mineCount = 5;

        /// <summary>
        /// 地雷でないセルを開けたときに得られるコインの枚数
        /// </summary>
        [SerializeField]
        int coinEarnedWhenSafeCellOpened = 100;

        /// <summary>
        /// 連続開放ボーナス
        /// </summary>
        [SerializeField]
        int continusOpenBounus = 10;

        /// <summary>
        /// コインボーナス倍率
        /// </summary>
        [SerializeField]
        int coinBonusMultiplier = 2;

        /// <summary>
        /// タイムボーナス倍率
        /// </summary>
        [SerializeField]
        float timeBonusMultiplier = 0.5f;

        /// <summary>
        /// ボーナスが何回続いたか
        /// </summary>
        int bonusCount = 0;

        /// <summary>
        /// 地雷探査のために必要なコスト
        /// </summary>
        [SerializeField]
        IntGameVariable mineDetectionCost = null;

        /// <summary>
        /// ライフ増加のために必要なコスト
        /// </summary>
        [SerializeField]
        IntGameVariable increasingLifeCost = null;

        /// <summary>
        /// プレイヤーの残機
        /// </summary>
        [SerializeField]
        IntGameVariable playerLife = null;

        /// <summary>
        /// 所有しているコインの枚数
        /// </summary>
        [SerializeField]
        IntGameVariable coins = null;

        /// <summary>
        /// 残り時間
        /// </summary>
        [SerializeField]
        FloatGameVariable remainingTime = null;

        /// <summary>
        /// コインボーナス
        /// </summary>
        [SerializeField]
        IntGameVariable coinBonus = null;

        /// <summary>
        /// タイムボーナス
        /// </summary>
        [SerializeField]
        IntGameVariable timeBonus = null;

        /// <summary>
        /// スコア
        /// </summary>
        [SerializeField]
        IntGameVariable score = null;

        /// <summary>
        /// まいんの台詞
        /// </summary>
        [SerializeField]
        Message message = null;

        /// <summary>
        /// 汎用台詞に戻るまでの秒数
        /// </summary>
        [SerializeField]
        float secondsToCommonMessage = 3.0f;

        /// <summary>
        /// 台詞戻し用タイムカウント
        /// </summary>
        float messageTimeCount = 0.0f;

        /// <summary>
        /// 地雷探査機ボタン
        /// </summary>
        [SerializeField]
        Button mineDetectorButton = null;

        /// <summary>
        /// ライフ増加ボタン
        /// </summary>
        [SerializeField]
        Button increasingLifeButton = null;

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

        /// <summary>
        /// クリア状態になったときに実行するイベント
        /// </summary>
        [SerializeField]
        GameEvent onClear = null;

        /// <summary>
        /// ゲームオーバー状態になったときに実行するイベント
        /// </summary>
        [SerializeField]
        GameEvent onGameOver = null;

        /// <summary>
        /// 最後に開けられたマス
        /// </summary>
        Cell lastOpenedCell = null;

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
                // 自身を最後に開かれたセルとして設定する
                cell.SafeCellOpened += (x) => lastOpenedCell = x;

                // プレイ中であればクリックを受け付ける
                cell.AcceptClick += () => State == GameState.Play;
            }
        }


        protected override void OnGameStateSet()
        {
            switch (State)
            {
                case GameState.None:
                    break;

                case GameState.Ready:

                    // Ready状態のイベントを実行する
                    onReady.Raise();

                    // セルを初期化する
                    cells.Init();

                    // 地雷を設置する
                    BuryMines();

                    // セルの状態を更新する
                    UpdateCellsStatus();

                    // ゲーム変数を初期化する
                    InitGameVariables();

                    // メッセージを通常状態にする
                    message.CurrentState = Message.State.Normal;

                    break;

                case GameState.Play:

                    // Play状態のイベントを実行する
                    onPlay.Raise();

                    break;

                case GameState.Clear:

                    // Clear状態のイベントを実行する
                    onClear.Raise();

                    // セルを全て開く(イベントなし)
                    OpenCellsWithoutRaisingEvents();

                    // スコアを設定する
                    SetScore();
                    break;

                case GameState.GameOver:

                    // GameOver状態のイベントを実行する
                    onGameOver.Raise();

                    // セルを全て開く(イベントなし)
                    OpenCellsWithoutRaisingEvents();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// スコアを設定する
        /// </summary>
        private void SetScore()
        {
            int coinBonusValue = coins.RuntimeValue * coinBonusMultiplier;
            coinBonus.RuntimeValue = coinBonusValue;

            int timeBonusValue = Mathf.FloorToInt(remainingTime.RuntimeValue * timeBonusMultiplier);
            timeBonus.RuntimeValue = timeBonusValue;

            int scoreValue = coinBonusValue + timeBonusValue;
            score.RuntimeValue = scoreValue;
        }

        /// <summary>
        /// イベントを発生させずに全てのセルを開く
        /// </summary>
        private void OpenCellsWithoutRaisingEvents()
        {
            foreach (var cell in cells.Values)
            {
                cell.OpenWithoutRaisingEvents();
            }
        }

        private void Update()
        {
            switch (State)
            {
                case GameState.None:
                    break;

                case GameState.Ready:
                    break;

                case GameState.Play:
                    UpdateOnPlay();
                    break;

                case GameState.Clear:
                    break;

                case GameState.GameOver:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// プレイ状態のときの更新処理
        /// </summary>
        private void UpdateOnPlay()
        {
            // 地雷探査ボタンの状態を更新する
            if (coins.RuntimeValue >= mineDetectionCost.RuntimeValue)
            {
                mineDetectorButton.interactable = true;
            }
            else
            {
                mineDetectorButton.interactable = false;
            }

            // ライフ増加ボタンの状態を更新する
            if (coins.RuntimeValue >= increasingLifeCost.RuntimeValue)
            {
                increasingLifeButton.interactable = true;
            }
            else
            {
                increasingLifeButton.interactable = false;
            }
        }

        private void LateUpdate()
        {
            switch (State)
            {
                case GameState.None:
                    break;

                case GameState.Ready:
                    break;

                case GameState.Play:
                    LateUpdateOnPlay();
                    break;

                case GameState.Clear:
                    break;

                case GameState.GameOver:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// プレイ状態時の最終更新処理
        /// </summary>
        private void LateUpdateOnPlay()
        {
            // メッセージタイムカウントが1秒以上設定されている場合
            if (messageTimeCount > 0.0f)
            {
                // カウントを減らす
                messageTimeCount -= Time.deltaTime;

                // ゼロ以下になったら
                if (messageTimeCount <= 0.0f)
                {
                    // メッセージを通常状態にする
                    message.CurrentState = Message.State.Normal;
                }
            }

            // 制限時間減少
            SubtructRemainingTime();

            // 制限時間がゼロになったら
            if (remainingTime.RuntimeValue == 0.0f)
            {
                // ゲームオーバー
                State = GameState.GameOver;
            }
        }

        /// <summary>
        /// 制限時間を減算する
        /// </summary>
        private void SubtructRemainingTime()
        {
            remainingTime.RuntimeValue -= Time.deltaTime;
        }

        /// <summary>
        /// ゲーム変数を初期化する
        /// </summary>
        protected override void InitGameVariables()
        {
            IInitializable[] initializables =
            {
                coins,
                playerLife,
                mineDetectionCost,
                increasingLifeCost,
                remainingTime,
                coinBonus,
                timeBonus,
                score
            };

            foreach (var var in initializables)
            {
                var.Init();
            }
        }

        /// <summary>
        /// 地雷を埋める
        /// </summary>
        private void BuryMines()
        {
            // セルが地雷の数以上なら中止する
            int cellCount = cells.Values.Length;
            if (mineCount >= cellCount)
            {
                Debug.LogError("地雷の数がセルの数以上になっています");
                return;
            }

            // 各セルに地雷を埋め込むかどうか判断する配列
            bool[] isMine = RandomUtility.GetRandomBoolArray(cellCount, mineCount);

            // 判断する配列でtrueになっている位置と同じ位置のセルを地雷状態にする
            for (int i = 0; i < cellCount; i++)
            {
                if (isMine[i])
                {
                    Cell cell = cells.Values[i];
                    cell.State = CellState.Mine;
                }
            }
        }

        /// <summary>
        /// セルの状態を更新する
        /// </summary>
        private void UpdateCellsStatus()
        {
            for (int y = 0; y < cells.Rows; y++)
            {
                for (int x = 0; x < cells.Columns; x++)
                {
                    Cell cell = cells.GetValue(x, y);

                    // 地雷のセルは更新する必要なし
                    if (cell.State == CellState.Mine)
                    {
                        continue;
                    }

                    int surroundingMines = CountSurroundingMines(x, y);
                    cell.State = (CellState)surroundingMines;
                }
            }
        }

        /// <summary>
        /// 周囲の地雷の数を調べる
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <returns>地雷の数</returns>
        private int CountSurroundingMines(int x, int y)
        {
            int count = 0;

            Cell[] surroundings = cells.GetSurroundingValues(x, y);
            foreach (var cell in surroundings)
            {
                if (cell.State == CellState.Mine)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// 地雷以外の全てのセルが開かれたか
        /// </summary>
        /// <returns></returns>
        private bool AllCellsHasOpenedExceptMine()
        {
            bool result = true;

            foreach (var cell in cells.Values)
            {
                if (!cell.IsOpened &&
                    cell.State != CellState.Mine)
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// ゲーム開始ボタンが押されたときのイベント
        /// </summary>
        public void OnStartButtonPressed()
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

        /// <summary>
        /// 地雷探査機ボタンが押されたときのイベント
        /// </summary>
        public void OnMineDetectorButtonPressed()
        {
            if (!CanPay(mineDetectionCost))
            {
                return;
            }

            // ランダムに旗を建てられていない地雷マスを配列にして取得する
            // 要素数がゼロなら中止する
            Cell[] mineCells = GetMines(true, true);
            if (mineCells.Length == 0)
            {
                return;
            }

            // 配列の中からランダムにセルを1つ選び、それに旗を立てる
            int targetIndex = Random.Range(0, mineCells.Length);
            Cell targetCell = mineCells[targetIndex];
            targetCell.Flagged = true;

            // この処理によって選ばれたセルは確実に地雷であるため、立てた旗を取り除けないようにする
            targetCell.IsMineDefinitely = true;

            // コストを支払う
            int cost = mineDetectionCost.RuntimeValue;
            coins.RuntimeValue -= cost;
        }

        /// <summary>
        /// ライフ増加ボタンが押されたときのイベント
        /// </summary>
        public void OnIncreasingLifeButtonPressed()
        {
            if (!CanPay(increasingLifeCost))
            {
                return;
            }

            // ライフを1増やす
            playerLife.RuntimeValue++;

            // コストを支払う
            int cost = increasingLifeCost.RuntimeValue;
            coins.RuntimeValue -= cost;
        }

        /// <summary>
        /// コストを支払うことができるか
        /// </summary>
        /// <param name="cost">費用</param>
        /// <returns></returns>
        public bool CanPay(IntGameVariable cost)
        {
            // プレイ中でなければ不可
            if (State != GameState.Play)
            {
                return false;
            }

            bool result = false;
            if (coins.RuntimeValue >= cost.RuntimeValue)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// 地雷セルを取得する
        /// </summary>
        /// <param name="exceptOpened">開放されているセルを除外するか</param>
        /// <param name="exceptFlagRaised">旗が立てられているセルを除外するか</param>
        /// <returns>セルの配列</returns>
        private Cell[] GetMines(bool exceptOpened, bool exceptFlagRaised)
        {
            List<Cell> mineCellList = new List<Cell>();

            foreach (var cell in cells.Values)
            {
                if (cell.State == CellState.Mine)
                {
                    if (exceptOpened && cell.IsOpened)
                    {
                        continue;
                    }

                    if (exceptFlagRaised && cell.Flagged)
                    {
                        continue;
                    }

                    mineCellList.Add(cell);
                }
            }

            Cell[] cellArray = mineCellList.ToArray();
            return cellArray;
        }

        /// <summary>
        /// 安全なセルがクリックされたときのイベント
        /// </summary>
        public void OnSafeCellClicked()
        {
            // メッセージを獲得状態にする
            message.CurrentState = Message.State.Obtaining;

            // メッセージタイムカウントを設定する
            messageTimeCount = secondsToCommonMessage;
        }

        /// <summary>
        /// 安全なセルが開かれたときのイベント
        /// </summary>
        public void OnSafeCellOpened()
        {
            // お金の計算
            int reward = coinEarnedWhenSafeCellOpened;

            // 連続開放ボーナス
            reward += continusOpenBounus * bonusCount;

            // お金を増やす
            coins.RuntimeValue += reward;

            // ボーナスカウントを増やす
            bonusCount++;

            if (AllCellsHasOpenedExceptMine())
            {
                Debug.Log("クリア！");

                State = GameState.Clear;
            }
            else
            {
                Debug.Log("セーフ！");

                // 開いたセルが空白なら、周囲8セルに地雷がないことが確定するため、それらのセルも開く
                if (lastOpenedCell.State == CellState.None)
                {
                    OpenSurroundingCells(lastOpenedCell.Position);
                }
                // 開いたセルが8なら、周囲8セルが地雷だと確定するため、それら全てに旗を立てる
                else if (lastOpenedCell.State == CellState.Eight)
                {
                    FlagSurroundingMines(lastOpenedCell.Position);
                }
            }
        }

        /// <summary>
        /// 周囲のセルを開く
        /// </summary>
        /// <param name="position">基準となる座標</param>
        private void OpenSurroundingCells(Vector2Int position)
        {
            Cell[] surroundings = cells.GetSurroundingValues(position);
            foreach (var cell in surroundings)
            {
                // 地雷でない場合
                if (!cell.IsOpened && cell.State != CellState.Mine)
                {
                    // 開く
                    cell.Open();
                }
            }
        }

        /// <summary>
        /// 周囲の地雷に旗を立てる
        /// </summary>
        /// <param name="position">基準となる座標</param>
        private void FlagSurroundingMines(Vector2Int position)
        {
            Cell[] surroundings = cells.GetSurroundingValues(position);
            foreach (var cell in surroundings)
            {
                // 旗を立てる
                cell.Flagged = true;

                // 旗を取り除けないようにする
                cell.IsMineDefinitely = true;
            }
        }

        /// <summary>
        /// 地雷マスが開かれたときのイベント
        /// </summary>
        public void OnMineCellOpened()
        {
            // メッセージをダメージ状態にする
            message.CurrentState = Message.State.Damage;

            // メッセージタイムカウントを設定する
            messageTimeCount = secondsToCommonMessage;

            // ライフ減少
            playerLife.RuntimeValue--;

            // 連続開放ボーナスカウントをゼロにする
            bonusCount = 0;

            // ダメージを受ける
            if (playerLife.RuntimeValue <= 0)
            {
                GameOver();
            }
        }

        /// <summary>
        /// ゲームオーバー
        /// </summary>
        public void GameOver()
        {
            State = GameState.GameOver;
        }
    }
}
