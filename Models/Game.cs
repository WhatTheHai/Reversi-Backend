using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

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

        public Colour WinningColour() {
            throw new NotImplementedException();
        }

        public bool PossibleMove(int rowMove, int columnMove) {
            throw new NotImplementedException();
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

                    if (row == col && (row == halfSz - 1 || col == halfSz - 1))
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
    }
}
