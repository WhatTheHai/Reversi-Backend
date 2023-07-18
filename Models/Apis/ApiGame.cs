using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReversiRestApi.Models {

    // Stringify everything for the api
    public class ApiGame {
        public string Description { get; set; }
        public string Token { get; set; }
        public string Player1Token { get; set; }
        public string Player2Token { get; set; }
        public string Board { get; set; }
        public string IsTurn { get; set; }
        public string BoardSize { get; set; }
        public string Finished { get; set; }
        public string Winner { get; set; }
        public string Loser { get; set; }
        public string UpdatedScores { get; set; }

        public static ApiGame ConvertGameToApiGameData(Game game) {
            return new ApiGame() {
                Description = game.Description,
                Token = game.Token,
                Player1Token = game.Player1Token,
                Player2Token = game.Player2Token,
                // Function directly translates object into a string
                Board = JsonConvert.SerializeObject(game.Board),
                IsTurn = game.IsTurn.ToString(),
                BoardSize = game.GetBoardSize.ToString(),
                Finished = game.Finished.ToString(),
                Winner = game.Winner,
                Loser = game.Loser,
                UpdatedScores = game.UpdatedScores.ToString(),
            };
        }
    }
}
