namespace PlayerAccounts
{
    public class PlayerAccount
    {
        /// <summary>
        /// ID of the database document that represents the player
        /// </summary>
        public string DocumentId => _id;
        private string _id;

        public string anonymousToken;
    }
}