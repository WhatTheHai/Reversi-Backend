using Microsoft.AspNetCore.Mvc;
using ReversiRestApi.Interfaces;
using ReversiRestApi.Models;
using ReversiRestApi.Models.Apis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReversiRestApi.Controllers
{
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
        public ActionResult AddNewGame([FromBody] ApiGame game) {
            iRepository.AddGame(new Game() {
                Description = game.Description,
                Player1Token = game.Player1Token,
                Token = (Guid.NewGuid().ToString())
            });
            return Ok();
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


        // GET api/game/{token}/turn
        [HttpGet("{token}/turn")]
        public ActionResult<string> Turn(string token) {
            var game = iRepository.GetGame(token);
            if (game == null) {
                return NotFound();
            }
            return ApiGame.ConvertGameToApiGameData(game).IsTurn;
        }
        
        // PUT api/game/surrender
        [HttpPut("surrender")]
        public ActionResult<bool> Surrender([FromBody] ApiPlayerGameData surrenderGame) {
            var game = iRepository.GetGame(surrenderGame.GameToken);

            if (game == null) {
                return NotFound();
            }

            return game.Surrender(surrenderGame.PlayerToken);
        }

        // PUT api/game/move
        [HttpPut("move")]
        public ActionResult<bool> DoMove([FromBody] ApiPlayerMove move) {
            var game = iRepository.GetGames().FirstOrDefault(g => g.Token == move.GameToken);

            if (game == null) {
                return NotFound();
            }

            if (game.IsTurn != game.GetPlayerColour(move.PlayerToken)) {
                return Unauthorized();
            }

            return game.DoMove(move.Y, move.X);
        }

        // PUT api/game/joingame
        [HttpPut("joingame")]
        public ActionResult<bool> JoinGame([FromBody] ApiPlayerGameData joinGame) {
            var game = iRepository.GetGame(joinGame.GameToken);

            if (game == null) {
                return NotFound();
            }

            if (game.Player2Token == null) {
                game.Player2Token = joinGame.PlayerToken;
                return true;
            }

            return false;
        }


    }
}
