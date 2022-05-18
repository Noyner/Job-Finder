namespace CRM.DAL.Models.ResponseModels.Auth
{
    public class TokenResponse
    {
        /// <summary>
        /// Токен аутентификации
        /// </summary>
        public string Token { get; }
        

        public TokenResponse(string token)
        {
            Token = token;
        }
    }
}