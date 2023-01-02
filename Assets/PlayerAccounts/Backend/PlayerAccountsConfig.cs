namespace PlayerAccounts
{
    public class PlayerAccountsConfig
    {
        /// <summary>
        /// Name of the database collection containing player documents
        /// </summary>
        public virtual string PlayersCollection => "players";

        public virtual string[] NonWritableFieldsByMyself => new[] {
            "_key", "_id", "_rev",
            "email", "password", "bannedUntil"
        };

        public static PlayerAccountsConfig GetConfigInstance()
        {
            // TODO: do some searching around and return the most-specific config type instance
            return new PlayerAccountsConfig();
        }
    }
}