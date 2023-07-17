using ReversiRestApi.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReversiRestApi.Models.Apis
{
    public class ApiGameStatus
    {
        public GameStatus GameStatus { get; set; }
        public bool Finished { get; set; }
        public string Winner { get; set; } 
        public string Loser { get; set; }

        public static ApiGameStatus ConvertGameToApiGameStatus(Game game) {
            return new ApiGameStatus {
                GameStatus = game.GameStatus,
                Finished = game.Finished,
                Winner = game.Winner,
                Loser = game.Loser,
            };
        }
    }
}
