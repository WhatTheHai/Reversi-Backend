using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Xml;

namespace ReversiRestApi.Models
{
    public class Game : IGame
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public string Player1Token { get; set; }
        public string Player2Token { get; set; }
        public Colour[,] Board { get; set; }
        public Colour IsTurn { get; set; }
        public const int BoardSize = 8;

        public Game() {
            Board = new Colour[BoardSize, BoardSize];
            BeginPosition();
        }

        public bool Pass() {
            throw new NotImplementedException();
        }

        public bool Finished() {
            throw new NotImplementedException();
        }

        public Colour WinningColour()
        {
            int black, white;
            black = white = 0;
            for (int i = 0; i < BoardSize; i++) {
                for (int j = 0; j < BoardSize; j++) {
                    switch (Board[i, j])
                    {
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
            if (rowMove < 0 || rowMove > 7 || columnMove < 0 || columnMove > 7)
            {
                return false;
            }

            var vectors = new List<int> {-1, 0, 1};
            foreach (int col in vectors) {
                foreach (int row in vectors) {

                }
            }

            return true;
        }

        public bool DoMove(int rowMove, int columnMove) {
            throw new NotImplementedException();
        }

        public void BeginPosition()
        {
            int halfSz = BoardSize / 2;
            for (int row = 0; row < BoardSize; row++) { // X-axis
                for (int col = 0; col < BoardSize; col++) { // Y-axis

                    Board[row, col] = Colour.None; //Gets overwritten if it has to be black/white

                    if (row == col && (row == halfSz - 1 || row == halfSz))
                    {
                        Board[row, col] = Colour.Black;
                    }

                    if ((row == halfSz - 1 && col == halfSz) || (row == halfSz && col == halfSz - 1))
                    {
                        Board[row, col] = Colour.White;
                    }

                }
            }
        }

        public void ChangeTurns()
        {
            IsTurn = (IsTurn == Colour.White) ? Colour.Black : Colour.White;
        }

        public Colour OppositeColour(Colour colour)
        {
            return (colour == Colour.White) ? Colour.Black : Colour.White;
        }
    }
}
