using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReversiRestApi.Models {
    public interface IGameRepository {
        void AddGame(Game game);

        public List<Game> GetGames();

        Game GetGame(string gameToken);
    }
}
