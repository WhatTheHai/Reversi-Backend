namespace ReversiRestApi.Models.Apis
{
    public class ApiAwaitingGame
    {
        public string Description { get; set; }
        public string Token { get; set; }
        public string Player1Token { get; set; }

        public static ApiAwaitingGame ConvertGameToApiAwaitingGame(Game game) {
            return new ApiAwaitingGame() {
                Description = game.Description,
                Token = game.Token,
                Player1Token = game.Player1Token,
            };
        }

    }
}
