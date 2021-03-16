using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Xml;

namespace ReversiRestApi.Models
{
    public class Game : IGame {
        public int ID { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public string Player1Token { get; set; }
        public string Player2Token { get; set; }
        public Colour[,] Board { get; set; }
        public Colour IsTurn { get; set; }
        private const int BoardSize = 8;
        public bool Finished { get; private set; }
        public string Winner { get; private set; }
        public string Loser { get; private set; }
        public int GetBoardSize => BoardSize;

        public Game() {
            Board = new Colour[GetBoardSize, GetBoardSize];
            BeginPosition();
            IsTurn = Colour.Black;
        }

        public bool Pass() {

            var emptySquares = AllEmptySquares();

            if (emptySquares.Any(emptySquare => PossibleMove(emptySquare[1], emptySquare[0]))) {
                return false;
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
            int white;
            var black = white = 0;
            for (int i = 0; i < GetBoardSize; i++) {
                for (int j = 0; j < GetBoardSize; j++) {
                    switch (Board[i, j]) {
                        case Colour.None:
                            break;
                        case Colour.Black:
                            black++;
                            break;
                        case Colour.White:
                            white++;
                            break;
                    }
                }
            }

            if (black > white)
                return Colour.Black;
            return white > black ? Colour.White : Colour.None;

        }


        public bool PossibleMove(int rowMove, int columnMove) {
            if (!InBounds(rowMove, columnMove))
                return false;

            var vectors = new List<int> {-1, 0, 1};
            foreach (int col in vectors) {
                foreach (int row in vectors) {
                    if (col == row && col == 0) continue;

                    int currentRow = rowMove;
                    int currentColumn = columnMove;

                    currentRow += row;
                    currentColumn += col;

                    if (!InBounds(currentRow, currentColumn)) continue;
                    if (Board[currentRow, currentColumn] == IsTurn) continue;

                    while (InBounds(currentRow + row, currentColumn + col)) {
                        currentRow += row;
                        currentColumn += col;
                        if (Board[currentRow, currentColumn] == Colour.None)
                            break;
                        if (Board[currentRow, currentColumn] == IsTurn)
                            return true;
                    }
                }
            }

            return false;
        }

        public bool InBounds(int rowMove, int columnMove) {
            return rowMove >= 0 && rowMove <= 7 && columnMove >= 0 && columnMove <= 7;
        }

        public bool DoMove(int rowMove, int columnMove) {
            if (!InBounds(rowMove, columnMove))
                return false;

            var directions = new List<int[]>();

            var vectors = new List<int> {-1, 0, 1};
            foreach (int col in vectors) {
                foreach (int row in vectors) {
                    if (col == row && col == 0) continue;
                    int currentRow = rowMove;
                    int currentColumn = columnMove;
                    currentRow += row;
                    currentColumn += col;
                    if (!InBounds(currentRow, currentColumn)) continue;
                    if (Board[currentRow, currentColumn] == IsTurn) continue;

                    while (InBounds(currentRow + row, currentColumn + col)) {
                        currentRow += row;
                        currentColumn += col;
                        if (Board[currentRow, currentColumn] == Colour.None)
                            break;
                        if (Board[currentRow, currentColumn] == IsTurn) {
                            directions.Add(new int[] {row, col});
                            break;
                        }
                    }
                }
            }

            if (directions.Any()) {
                Board[rowMove, columnMove] = IsTurn;
                ProcessMoves(rowMove, columnMove, directions);
                ChangeTurns();
                return true;
            }

            return false;
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
                        emptySquares.Add(new[] {i, j});
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
            }
        }
    }
}
