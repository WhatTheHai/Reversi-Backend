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
        public ActionResult<List<ApiAwaitingGame>> GetGameDescAwaitingPlayers() {
            List<ApiAwaitingGame> awaitingGames = iRepository.GetGames()
                .Where(game => game.Player2Token == null)
                .Select(game => ApiAwaitingGame.ConvertGameToApiAwaitingGame(game))
                .ToList();

            return awaitingGames;
        }

        // GET api/game/{token}/anygames
        [HttpGet("{token}/anygames")]
        public ActionResult<List<ApiGame>> GetGamePlayerToken(string token) {
            List<ApiGame> awaitingGames = iRepository.GetGames()
                .Where(game => game.Player1Token == token || game.Player2Token == token)
                .Select(game => ApiGame.ConvertGameToApiGameData(game))
                .ToList();
            return awaitingGames.Any() ? awaitingGames : null;
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

            var surrender = game.Surrender(surrenderGame.PlayerToken);

            if (surrender == false) {
                return BadRequest("You are not allowed to surrender!");
            }

            iRepository.Save();
            return Ok();
        }

        // PUT api/game/move
        [HttpPut("move")]
        public ActionResult<bool> DoMove([FromBody] ApiPlayerMove move) {
            var game = iRepository.GetGames().FirstOrDefault(g => g.Token == move.GameToken);

            if (game == null) {
                return NotFound();
            }

            if (game.IsTurn != game.GetPlayerColour(move.PlayerToken) || move.PlayerToken == null) {
                return Unauthorized("This is not your turn");
            }

            var doMove = game.DoMove(move.Y, move.X);

            if (doMove == false) {
                return BadRequest("Not a valid move!");
            }
            iRepository.Save();
            return true;
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
                iRepository.Save();
                return true;
            }
            return false;
        }

        // DELETE api/game/{token}
        [HttpDelete("{token}")]
        public ActionResult RemoveGame([FromBody] ApiPlayerGameData removeGame)
        {
            var game = iRepository.GetGame(removeGame.GameToken);

            if (game == null)
            {
                return NotFound();
            }

            // Check if the player1token matches the player1token of the game
            if (game.Player1Token != removeGame.PlayerToken)
            {
                return Unauthorized("You are not the host of the game, you can't delete.");
            }

            iRepository.RemoveGame(game);
            iRepository.Save();

            return Ok();
        }
    }
}
