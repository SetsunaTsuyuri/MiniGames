using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reversi
{
    /// <summary>
    /// リバーシ
    /// </summary>
    public class Reversi : GameBase<GameState, CellArray2D, Cell, CellState, CellSettings>
    {
        /// <summary>
        /// プレイヤー
        /// </summary>
        [SerializeField]
        Player player = null;

        /// <summary>
        /// 敵
        /// </summary>
        [SerializeField]
        Player enemy = null;

        /// <summary>
        /// 黒石の初期配置
        /// </summary>
        [SerializeField]
        Vector2Int[] blackStonesInitialPositions = null;

        /// <summary>
        /// 白石の初期配置
        /// </summary>
        [SerializeField]
        Vector2Int[] whiteStonesInitialPositions = null;

        /// <summary>
        /// 黒石の数
        /// </summary>
        [SerializeField]
        IntGameVariable blackStones = null;

        /// <summary>
        /// 白石の数
        /// </summary>
        [SerializeField]
        IntGameVariable whiteStones = null;

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
        /// 敵のターンか
        /// </summary>
        bool isEnemysTurn = false;

        /// <summary>
        /// 石置き演出の待機時間
        /// </summary>
        [SerializeField]
        float waitTimeOnStonePut = 0.2f;

        /// <summary>
        /// 敵のターンになったときの待機時間
        /// </summary>
        [SerializeField]
        float waitTimeOnEnemyTurn = 0.5f;

        /// <summary>
        /// 石置きまたはひっくり返している最中である
        /// </summary>
        bool puttingOrTuneOverStones = false;

        /// <summary>
        /// メッセージのデータ集
        /// </summary>
        [SerializeField]
        MessageDataCollections messageDataCollections = null;

        /// <summary>
        /// メッセージテキスト
        /// </summary>
        [SerializeField]
        Text message = null;

        /// <summary>
        /// 敵のターンか
        /// </summary>
        bool IsEnemysTurn
        {
            get => isEnemysTurn;
            set
            {
                isEnemysTurn = value;
                OnTurnSwitched();
            }
        }

        /// <summary>
        /// 最後にクリックされたセル
        /// </summary>
        Cell lastClickedCell = null;

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
                // 自身を最後に石が置かれたセルとして設定する
                cell.PutStone += (x) => lastClickedCell = x;

                // プレイ状態であればクリックを受け付ける
                cell.AcceptClick += () => State == GameState.Play;
            }
        }

        protected override void OnGameStateSet()
        {
            switch (State)
            {
                case GameState.Ready:

                    // Ready状態のイベントを実行する
                    onReady.Raise();

                    // メッセージ変更
                    ChangeMessage(MessageAttribute.Ready);

                    // セルを初期化する
                    cells.Init();

                    // 初期の石を置く
                    PutInitialStones();

                    // ゲーム変数を初期化する
                    InitGameVariables();

                    // 各セルを更新する
                    UpdateCells();

                    // 石の数(ゲーム変数)を更新する
                    UpdateStoneCounts();

                    break;

                case GameState.Play:

                    // Play状態のイベントを実行する
                    onPlay.Raise();

                    ChangeMessage(MessageAttribute.PlayersTurn);

                    break;

                case GameState.Clear:

                    // メッセージ更新
                    ChangeMessage(MessageAttribute.Clear);

                    break;
                case GameState.GameOver:

                    // メッセージ更新
                    ChangeMessage(MessageAttribute.GameOver);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 初期の石を置く
        /// </summary>
        private void PutInitialStones()
        {
            // 黒石を置く
            foreach (var position in blackStonesInitialPositions)
            {
                Cell cell = cells.GetValue(position);
                cell.State = CellState.Black;
            }

            // 白石を置く
            foreach (var position in whiteStonesInitialPositions)
            {
                Cell cell = cells.GetValue(position);
                cell.State = CellState.White;
            }
        }

        protected override void InitGameVariables()
        {
            IInitializable[] initializables =
{
                blackStones,
                whiteStones
            };

            foreach (var val in initializables)
            {
                val.Init();
            }
        }

        /// <summary>
        /// 各セルを更新する
        /// </summary>
        private void UpdateCells()
        {
            foreach (var cell in cells.Values)
            {
                UpdateStoneCount(cell);
            }
        }

        /// <summary>
        /// 増やせる石の数を更新する
        /// </summary>
        /// <param name="cell">セル</param>
        private void UpdateStoneCount(Cell cell)
        {
            // 石を置けない場合
            if (!CanPutStone(cell))
            {
                // 増やせる石の数をゼロにして終了する
                cell.StoneCount = 0;
                return;
            }

            // 増やせる石の数
            int count = 1;

            // 周囲の非ターンプレイヤーの石
            Cell[] nonTurnPlayerStones = GetSurroundingNonTurnPlayersStones(cell);
            foreach (var stone in nonTurnPlayerStones)
            {
                //石が存在する方向
                Vector2Int direction = stone.Position - cell.Position;

                // その方向にターンプレイヤーの石が存在する場合
                if (ExsistsTurnPlyersStone(stone, direction))
                {
                    // 増やせる石の数に裏返せる石の数を足す
                    count += CountStonesCanBeTurnedOver(stone, direction);
                }
            }

            // 増やせる石の数を更新する
            cell.StoneCount = count;
        }

        /// <summary>
        /// 裏返せる石の数を数える
        /// </summary>
        /// <param name="cell">セル</param>
        /// <param name="direction">方向</param>
        private int CountStonesCanBeTurnedOver(Cell cell, Vector2Int direction, int count = 0)
        {
            count++;

            // 隣のセルを取得する
            Cell nextCell = cells.GetValue(cell.Position + direction);
            if (nextCell != null)
            {
                // それが非ターンプレイヤーの石である場合
                if (nextCell.State == GetNonTurnPlayer().StoneColor)
                {
                    // 再帰処理
                    count = CountStonesCanBeTurnedOver(nextCell, direction, count);
                }
            }

            return count;
        }

        /// <summary>
        /// 石を置くことができるか
        /// </summary>
        /// <param name="cell">対象のセル</param>
        /// <returns></returns>
        private bool CanPutStone(Cell cell)
        {
            // 既に石が存在するセルには置けない
            if (cell.State != CellState.Empty)
            {
                return false;
            }

            bool result = false;

            // 周囲の非ターンプレイヤーの石を取得する
            Cell[] nonTurnPlayerStones = GetSurroundingNonTurnPlayersStones(cell);

            // 相手の石が存在する場合、そこに石を置ける可能性がある。
            foreach (var stone in nonTurnPlayerStones)
            {
                //石が存在する方向
                Vector2Int direction = stone.Position - cell.Position;

                // その方向にターンプレイヤーの石が存在するか
                if (ExsistsTurnPlyersStone(stone, direction))
                {
                    // 石置き可能
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// ターンプレイヤーを取得する
        /// </summary>
        /// <returns></returns>
        private Player GetTurnPlayer()
        {
            Player turnPlayer = player;
            if (IsEnemysTurn)
            {
                turnPlayer = enemy;
            }

            return turnPlayer;
        }

        /// <summary>
        /// 非ターンプレイヤーを取得する
        /// </summary>
        /// <returns></returns>
        private Player GetNonTurnPlayer()
        {
            Player nonTurnPlayer = enemy;
            if (IsEnemysTurn)
            {
                nonTurnPlayer = player;
            }

            return nonTurnPlayer;
        }

        /// <summary>
        /// 周囲の非ターンプレイヤーの石を配列にして取得する
        /// </summary>
        /// <param name="cell">中心のセル</param>
        /// <returns></returns>
        private Cell[] GetSurroundingNonTurnPlayersStones(Cell cell)
        {
            CellState nonTurnPlayerStoneState = GetNonTurnPlayer().StoneColor;
            Cell[] surroundings = cells.GetSurroundingValues(cell.Position, nonTurnPlayerStoneState);
            return surroundings;
        }

        /// <summary>
        /// ターンプレイヤーの石が存在するか
        /// </summary>
        /// <param name="cell">基準となるセル</param>
        /// <param name="direction">探す方向</param>
        /// <returns></returns>
        private bool ExsistsTurnPlyersStone(Cell cell, Vector2Int direction)
        {
            bool result = false;

            // 隣のセルを取得する
            Cell nextCell = cells.GetValue(cell.Position + direction);
            if (nextCell != null)
            {
                // それがターンプレイヤーの石である場合
                if (nextCell.State == GetTurnPlayer().StoneColor)
                {
                    // 挟まれている
                    result = true;
                }
                // それが非ターンプレイヤーの石である場合
                else if (nextCell.State == GetNonTurnPlayer().StoneColor)
                {
                    // 再帰処理
                    result = ExsistsTurnPlyersStone(nextCell, direction);
                }
            }

            return result;
        }

        /// <summary>
        /// セルに石が置かれたときの処理
        /// </summary>
        public void OnStonePut()
        {
            if (puttingOrTuneOverStones)
            {
                return;
            }

            StartCoroutine(OnStonePutCoroutine());
        }

        public IEnumerator OnStonePutCoroutine()
        {
            puttingOrTuneOverStones = true;

            // 石を置く
            CellState turnPlayersStone = GetTurnPlayer().StoneColor;
            lastClickedCell.State = turnPlayersStone;

            yield return new WaitForSeconds(waitTimeOnStonePut);

            // 挟まれている石を裏返す
            yield return TurnOverSandwichedStones();

            // 石の数カウントを更新する
            UpdateStoneCounts();

            // ターンプレイヤーの石置き成功フラグをONにする
            Player turnPlayer = GetTurnPlayer();
            turnPlayer.PutStoneInPreviousTurn = true;

            // ターンを切り替える
            SwitchTurn();

            puttingOrTuneOverStones = false;
        }

        /// <summary>
        /// 挟まれた石を裏返す
        /// </summary>
        private IEnumerator TurnOverSandwichedStones()
        {
            // 置かれた石の周囲にある非ターンプレイヤーの石を取得する
            Cell[] nonTurnPlayerStones = GetSurroundingNonTurnPlayersStones(lastClickedCell);
            foreach (var stone in nonTurnPlayerStones)
            {
                //石が存在する方向
                Vector2Int direction = stone.Position - lastClickedCell.Position;

                // その方向にターンプレイヤーの石が存在する場合
                if (ExsistsTurnPlyersStone(stone, direction))
                {
                    // 石を裏返す
                    yield return TurnOverStones(stone, direction);
                }
            }
        }

        /// <summary>
        /// 石を裏返す
        /// </summary>
        /// <param name="cell">セル</param>
        /// <param name="direction">方向</param>
        private IEnumerator TurnOverStones(Cell cell, Vector2Int direction)
        {
            // 裏返る
            cell.TurnOver();

            yield return new WaitForSeconds(waitTimeOnStonePut);

            // 隣のセルを取得する
            Cell nextCell = cells.GetValue(cell.Position + direction);
            if (nextCell != null)
            {
                // それが非ターンプレイヤーの石である場合
                if (nextCell.State == GetNonTurnPlayer().StoneColor)
                {
                    // 再帰処理
                    yield return TurnOverStones(nextCell, direction);
                }
            }
        }

        /// <summary>
        /// 石の数(ゲーム変数)を更新する
        /// </summary>
        private void UpdateStoneCounts()
        {
            // セルの配列
            Cell[] values = cells.Values;

            // 黒石の数
            blackStones.RuntimeValue = values.Count(x => x.State == CellState.Black);

            // 白石の数
            whiteStones.RuntimeValue = values.Count(x => x.State == CellState.White);
        }

        /// <summary>
        /// ターンを切り替える
        /// </summary>
        private void SwitchTurn()
        {
            if (IsEnemysTurn)
            {
                ChangeMessage(MessageAttribute.PlayersTurn);
            }
            else
            {
                ChangeMessage(MessageAttribute.EnemysTurn);
            }

            IsEnemysTurn = !IsEnemysTurn;
        }

        /// <summary>
        /// ターン開始時の処理
        /// </summary>
        private void OnTurnSwitched()
        {
            // メッセージ更新
            MessageAttribute messageAttribute = MessageAttribute.PlayersTurn;
            if (IsEnemysTurn)
            {
                messageAttribute = MessageAttribute.EnemysTurn;
            }
            ChangeMessage(messageAttribute);

            // セルを更新する
            UpdateCells();

            // 裏返せる石が存在する場合
            int puttableCells = CountPuttableCells();
            if (puttableCells > 0)
            {
                // 敵のターンの場合
                if (IsEnemysTurn)
                {
                    // どのセルに石を置くか選ぶ
                    Cell toBePut = ChooseCell();
                    toBePut.Invoke("OnClicked", waitTimeOnEnemyTurn);
                }
            }
            else
            {
                // 非ターンプレイヤーも石を置けなかった場合
                Player nonTurnPlayer = GetNonTurnPlayer();
                if (!nonTurnPlayer.PutStoneInPreviousTurn)
                {
                    // 勝敗判定
                    if (PlayerHasWon())
                    {
                        State = GameState.Clear;
                    }
                    else
                    {
                        State = GameState.GameOver;
                    }
                }
                else
                {
                    // ターンプレイヤーの石置き成功フラグをOFFにする
                    Player turnPlayer = GetTurnPlayer();
                    turnPlayer.PutStoneInPreviousTurn = false;

                    // ターンを切り替える
                    SwitchTurn();
                }
            }
        }

        /// <summary>
        /// 石を置くセルを選ぶ
        /// </summary>
        /// <returns></returns>
        private Cell ChooseCell()
        {
            // 石置き可能なセルを集める
            List<Cell> cellList = new List<Cell>();
            foreach (var cell in cells.Values)
            {
                if (cell.CanBePutStone())
                {
                    cellList.Add(cell);
                }
            }

            // 最も多く石を裏返せるセルを選ぶ
            Cell maxStoneCountCell = cellList[0];
            foreach (var cell in cells.Values)
            {
                if (cell.StoneCount > maxStoneCountCell.StoneCount)
                {
                    maxStoneCountCell = cell;
                }
            }

            return maxStoneCountCell;
        }

        /// <summary>
        /// プレイヤーが勝った
        /// </summary>
        /// <returns></returns>
        private bool PlayerHasWon()
        {
            int playersStones;
            int enemysStones;
            if (player.StoneColor == CellState.Black)
            {
                playersStones = blackStones.RuntimeValue;
                enemysStones = whiteStones.RuntimeValue;
            }
            else
            {
                playersStones = whiteStones.RuntimeValue;
                enemysStones = blackStones.RuntimeValue;
            }

            bool result = playersStones > enemysStones;
            return result;

        }

        /// <summary>
        /// 石置き可能なセルの数を数える
        /// </summary>
        /// <returns></returns>
        private int CountPuttableCells()
        {
            int count = 0;
            foreach (var cell in cells.Values)
            {
                if (cell.CanBePutStone())
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// メッセージを変更する
        /// </summary>
        /// <param name="attribute">属性</param>
        private void ChangeMessage(MessageAttribute attribute)
        {
            string newText = messageDataCollections.GetMessage(attribute);
            if (newText != null)
            {
                message.text = newText;
            }
        }

        /// <summary>
        /// ゲーム開始ボタンが押されたときの処理
        /// </summary>
        public void OnGameStartButtonPressed()
        {
            State = GameState.Play;
        }

        /// <summary>
        /// ゲーム開始ボタンが押されたときの処理
        /// </summary>
        public void OnRestartButtonPressed()
        {
            State = GameState.Ready;
        }
    }
}
