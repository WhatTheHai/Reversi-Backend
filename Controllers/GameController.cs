using Microsoft.AspNetCore.Mvc;
using ReversiRestApi.Models;
using ReversiRestApi.Models.Apis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReversiRestApi.Controllers {
    [Route("api/game")]
    [ApiController]
    public class GameController : ControllerBase {
        private readonly IGameRepository iRepository;

        public GameController(IGameRepository repository) {
            iRepository = repository;
        }

        // GET api/game
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetGameDescAwaitingPlayers() {
            return iRepository.GetGames().Where(game => game.Player2Token == null).Select(game => game.Description).ToList();
        }

        // POST api/game
        [HttpPost]
        public void AddNewGame([FromBody] ApiGame game) {
            iRepository.AddGame(new Game() {
                Description = game.Description,
                Player1Token = game.Player1Token,
                Token = (new Guid()).ToString()
            });
        }

        // GET api/game/{token}
        [HttpGet("{token}")]
        public ActionResult<ApiGame> GetGameWithToken(string token) {
            var game = iRepository.GetGame(token);
            if (game == null) {
                return NotFound();
            }
            return ApiGame.ConvertGameToApiGameData(game);
        }

        [HttpGet("{token}/turn")]
        public ActionResult<string> Turn(string token) {
            var game = iRepository.GetGame(token);
            if (game == null) {
                return NotFound();
            }
            return ApiGame.ConvertGameToApiGameData(game).IsTurn;
        }

        [HttpPut("surrender")]
        public ActionResult<bool> Surrender([FromBody] ApiSurrender surrenderGame) {
            var game = iRepository.GetGames().First(g => g.Token == surrenderGame.GameToken);

            if (game == null) {
                return NotFound();
            }

            return game.Surrender(surrenderGame.PlayerToken);
        }

        [HttpPut("move")]
        public ActionResult<bool> DoMove([FromBody] ApiPlayerMove move) {
            var game = iRepository.GetGames().First(g => g.Token == move.GameToken);

            if (game == null) {
                return NotFound();
            }

            if (game.IsTurn != game.GetPlayerColour(move.PlayerToken)) {
                return Unauthorized();
            }

            return game.DoMove(move.Y, move.X);
        } 
    }
}
