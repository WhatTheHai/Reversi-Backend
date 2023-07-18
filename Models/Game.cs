using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using ReversiRestApi.Interfaces;
using ReversiRestApi.Models.Enums;

namespace ReversiRestApi.Models {
    public class Game : IGame {
        public int ID { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public string Player1Token { get; set; }
        public string Player2Token { get; set; }
        [NotMapped] public Colour[,] Board { get; set; }

        [Column("Board")]
        public string BoardString {
            get => ConvertBoardToString(Board);
            set => Board = ConvertStringToBoard(value);
        }

        public Colour IsTurn { get; set; }
        private const int BoardSize = 8;
        [NotMapped] public bool Finished { get; private set; }
        [NotMapped] public string Winner { get; private set; }
        [NotMapped] public string Loser { get; private set; }
        public int GetBoardSize => BoardSize;

        [NotMapped]
        public GameStatus GameStatus => Player2Token == null
            ? GameStatus.Awaiting
            : (GameFinished() ? GameStatus.Finished : GameStatus.Busy);

        public bool UpdatedScores { get; set; } = false;


        public Game() {
            Board = new Colour[GetBoardSize, GetBoardSize];
            BeginPosition();
            IsTurn = Colour.Black;
        }

        public bool Pass() {
            for (int row = 0; row < GetBoardSize; row++)
            {
                for (int col = 0; col < GetBoardSize; col++)
                {
                    if (PossibleMove(row, col))
                    {
                        return false;
                    }
                }
            }

            ChangeTurns();
            return true;
        }

        public bool GameFinished() {
            var emptySquares = AllEmptySquares();
            if (emptySquares == null) {
                return false;
            }

            var rememberTurn = IsTurn;
            if (Pass() && Pass()) {
                Finished = true;
                DetermineWinnerAndLoser(WinningColour());
                return true;
            }

            IsTurn = rememberTurn;
            return false;
        }

        public Colour WinningColour() {
            int blackCount = Board.Cast<Colour>().Count(cell => cell == Colour.Black);
            int whiteCount = Board.Cast<Colour>().Count(cell => cell == Colour.White);

            if (blackCount > whiteCount)
                return Colour.Black;
            if (whiteCount > blackCount)
                return Colour.White;
            return Colour.None;
        }

        private bool IsValidMove(int rowMove, int columnMove, out List<int[]> directions) {
            directions = new List<int[]>();

            var vectors = new List<int> { -1, 0, 1 };
            foreach (int col in vectors)
            {
                foreach (int row in vectors)
                {
                    if (col == 0 && row == 0) continue;

                    int currentRow = rowMove + row;
                    int currentColumn = columnMove + col;

                    if (!InBounds(currentRow, currentColumn) || Board[currentRow, currentColumn] != OppositeColour(IsTurn))
                        continue;

                    bool validDirection = false;
                    while (InBounds(currentRow + row, currentColumn + col))
                    {
                        currentRow += row;
                        currentColumn += col;
                        if (Board[currentRow, currentColumn] == Colour.None)
                            break;
                        if (Board[currentRow, currentColumn] == IsTurn)
                        {
                            validDirection = true;
                            break;
                        }
                    }

                    if (validDirection)
                    {
                        directions.Add(new int[] { row, col });
                    }
                }
            }

            return directions.Count > 0;
        }

        public bool PossibleMove(int rowMove, int columnMove) {
            if (!InBounds(rowMove, columnMove) || Board[rowMove, columnMove] != Colour.None)
                return false;

            List<int[]> directions;
            return IsValidMove(rowMove, columnMove, out directions);
        }

        public bool DoMove(int rowMove, int columnMove) {
            if (!InBounds(rowMove, columnMove) || Board[rowMove, columnMove] != Colour.None)
                return false;

            List<int[]> directions;
            if (IsValidMove(rowMove, columnMove, out directions)) {
                Board[rowMove, columnMove] = IsTurn;
                ProcessMoves(rowMove, columnMove, directions);
                ChangeTurns();
                return true;
            }

            return false;
        }


        public bool InBounds(int rowMove, int columnMove) {
            return rowMove >= 0 && rowMove < GetBoardSize && columnMove >= 0 && columnMove < GetBoardSize;
        }

        public void ProcessMoves(int rowMove, int columnMove, List<int[]> directions) {
            foreach (var direction in directions) {
                int currentRow = rowMove + direction[0];
                int currentCol = columnMove + direction[1];
                while (Board[currentRow, currentCol] == OppositeColour(IsTurn)) {
                    Board[currentRow, currentCol] = IsTurn;
                    currentRow += direction[0];
                    currentCol += direction[1];
                }
            }
        }


        public void BeginPosition() {
            int halfSz = GetBoardSize / 2;
            for (int row = 0; row < GetBoardSize; row++) { // X-axis
                for (int col = 0; col < GetBoardSize; col++) { // Y-axis

                    Board[row, col] = Colour.None; //Gets overwritten if it has to be black/white

                    if (row == col && (row == halfSz - 1 || row == halfSz)) {
                        Board[row, col] = Colour.White;
                    }

                    if ((row == halfSz - 1 && col == halfSz) || (row == halfSz && col == halfSz - 1)) {
                        Board[row, col] = Colour.Black;
                    }
                }
            }
        }

        public void ChangeTurns() {
            IsTurn = (IsTurn == Colour.White) ? Colour.Black : Colour.White;
        }

        public Colour OppositeColour(Colour colour) {
            return (colour == Colour.White) ? Colour.Black : Colour.White;
        }

        public Colour GetPlayerColour(string PlayerToken) {
            if (PlayerToken == Player1Token) {
                return Colour.White;
            }

            return PlayerToken == Player2Token ? Colour.Black : Colour.None;
        }

        public List<int[]> AllEmptySquares() {
            var emptySquares = new List<int[]>();
            for (int i = 0; i < GetBoardSize; i++) {
                for (int j = 0; j < GetBoardSize; j++) {
                    if (Board[i, j] == Colour.None)
                        emptySquares.Add(new[] { i, j });
                }
            }

            return emptySquares;
        }

        public bool Surrender(string playerToken) {
            if (Player1Token == null || Player2Token == null) {
                return false;
            }

            if (playerToken.Equals(Player1Token)) {
                Winner = Player2Token;
                Loser = Player1Token;
                Finished = true;
            }
            else if (playerToken.Equals(Player2Token)) {
                Winner = Player1Token;
                Loser = Player2Token;
                Finished = true;
            }
            else {
                return false;
            }

            return true;
        }

        public void DetermineWinnerAndLoser(Colour WinningColour) {
            switch (WinningColour) {
                case Colour.Black:
                    Winner = Player1Token;
                    Loser = Player2Token;
                    break;
                case Colour.White:
                    Winner = Player2Token;
                    Loser = Player1Token;
                    break;
                case Colour.None:
                    Winner = "Draw";
                    Loser = "Draw";
                    break;
            }
        }

        private string ConvertBoardToString(Colour[,] board) {
            var oneDimensionBoard = board.Cast<Colour>().ToArray();
            return JsonConvert.SerializeObject(oneDimensionBoard);
        }

        private Colour[,] ConvertStringToBoard(string boardString) {
            Colour[] oneDimensionBoard = JsonConvert.DeserializeObject<Colour[]>(boardString);

            // 1d -> 2d
            int size = (int)Math.Sqrt(oneDimensionBoard.Length);
            Colour[,] board = new Colour[size, size];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    // i * size to go one more "down"
                    board[i, j] = oneDimensionBoard[i * size + j];
                }
            }

            return board;
        }
    }
}