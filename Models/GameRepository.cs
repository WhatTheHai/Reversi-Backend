using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReversiRestApi.Interfaces;

namespace ReversiRestApi.Models
{
    public class GameRepository : IGameRepository {
        //List of temporary games
        public List<Game> Games { get; set; }

        public GameRepository() {
            Game game1 = new Game {
                Token = "a123a",
                Player1Token = "abcdef",
                Description = "Potje snel reversi, dus niet lang nadenken",
            };
            Game game2 = new Game {
                Token = "b123b",
                Player1Token = "ghjkl",
                Player2Token = "mnopqr",
                Description = "Ik zoek een gevorderde tegenspeler!"
            };
            Game game3 = new Game() {
                Token = "c123c",
                Player1Token = "stuvwx",
                Description = "Na dit spel wil ik er nog een paar spelen tegen zelfde tegenstander"
            };

            Games = new List<Game> {game1, game2, game3};
        }
        public void AddGame(Game game) {
            Games.Add(game);
        }

        public List<Game> GetGames() {
            return Games;
        }

        public Game GetGame(string gameToken) {
            return Games.FirstOrDefault(game => game.Token == gameToken);
        }
    }
}
