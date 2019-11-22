namespace Plugin.Application.Wallpaper.Common.Azure.Authentication
{
    public interface IRefreshTokenStore
    {
        void StoreRefreshToken(string newRefreshToken);
    }
}
