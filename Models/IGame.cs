using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReversiRestApi.Models
{
    public interface IGame
    {
        int ID { get; set; }
        string Description { get; set; }

        //het unieke token van het spel
        string Token { get; set; }
        string Player1Token { get; set; }
        string Player2Token { get; set; }
        Colour[,] Board { get; set; }
        Colour IsTurn { get; set; }
        bool Pass();
        bool GameFinished();

        //welke kleur het meest voorkomend op het speelbord
        Colour WinningColour();

        //controle of op bepaalde positie een zet mogelijk is
        bool PossibleMove(int rowMove, int columnMove);
        bool DoMove(int rowMove, int columnMove);
    }
}
