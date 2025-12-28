namespace BancoSimulado.Security
{
    public static class TokenStore
    {
        // token -> expiración
        public static Dictionary<string, DateTime> Tokens = new();

        // refreshToken -> token
        public static Dictionary<string, string> RefreshTokens = new();
    }
}
