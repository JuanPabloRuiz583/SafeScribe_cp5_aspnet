namespace cp5.Services
{
    public class InMemoryTokenBlacklistService : ITokenBlacklistService
    {
        private readonly HashSet<string> _blacklistedTokens = new();

        public Task AddToBlacklistAsync(string jti)
        {
            _blacklistedTokens.Add(jti);
            return Task.CompletedTask;
        }

        public Task<bool> IsBlacklistedAsync(string jti)
        {
            var isBlacklisted = _blacklistedTokens.Contains(jti);
            return Task.FromResult(isBlacklisted);
        }
    }
}
