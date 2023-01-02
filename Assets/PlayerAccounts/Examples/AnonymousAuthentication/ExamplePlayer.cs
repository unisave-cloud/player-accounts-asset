namespace PlayerAccounts.Examples.AnonymousAuthentication
{
    /// <summary>
    /// Represents a player of your game
    ///
    /// Add fields for data you want to store for each player.
    ///
    /// You can rename this to "Player" or "GamePlayer" or maybe "PacmanPlayer",
    /// whatever makes sense in your game.
    /// </summary>
    public class ExamplePlayer
    {
        /// <summary>
        /// What the player wants to call himself
        /// </summary>
        public string nickname;
        
        /// <summary>
        /// How many coins does the player have
        /// </summary>
        public int coins;
    }
}