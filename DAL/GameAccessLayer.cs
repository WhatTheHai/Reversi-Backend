using System.Collections.Generic;
using System.Linq;
using ReversiRestApi.Interfaces;
using ReversiRestApi.Models;

namespace ReversiRestApi.DAL
{
    public class GameAccessLayer : IGameRepository
    {
        private ReversiDbContext _context;
        public GameAccessLayer(ReversiDbContext context)
        {
            _context = context;
        }

        public void AddGame(Game game) {
            _context.Games.Add(game);
            _context.SaveChanges();
        }

        public List<Game> GetGames() {
            return _context.Games.ToList();
        }

        public Game GetGame(string gameToken) {
            Game game = _context.Games.FirstOrDefault(g => g.Token == gameToken);
            if (game != null) {
                game.GameFinished();
            }
            return game;
        }

        public void RemoveGame(Game game) {
            _context.Games.Remove(game);
            _context.SaveChanges();
        }

        public void Save() {
            _context.SaveChanges();
        }
    }
}
