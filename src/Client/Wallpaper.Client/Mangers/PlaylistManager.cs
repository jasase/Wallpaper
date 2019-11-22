using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Contracts.Extension;
using Plugin.Application.Wallpaper.Client.Model;
using Web.Wallpapers;

namespace Plugin.Application.Wallpaper.Client.Mangers
{
    public class PlaylistManager
    {
        private readonly Queue<PlaylistElement> _currentPlaylist;
        private readonly ILogger _logger;
        private readonly WallpaperApiClient _wallpaperApiClient;

        public PlaylistManager(ILogger logger, WallpaperApiClient wallpaperApiClient)
        {
            _logger = logger;
            _wallpaperApiClient = wallpaperApiClient;
            _currentPlaylist = new Queue<PlaylistElement>();
        }

        public IEnumerable<PlaylistElement> GetNext()
        {
            _logger.Debug("Getting next elements from playlist");
            lock (_currentPlaylist)
            {
                for (var i = 0; i < 5; i++)
                {
                    if (!_currentPlaylist.Any())
                    {
                        _logger.Debug("No elements in playlist available. Getting new elements from server");
                        LoadPlaylistFromServer();
                    }

                    if (_currentPlaylist.Any())
                    {
                        yield return _currentPlaylist.Dequeue();
                    }
                }
            }
        }

        private void LoadPlaylistFromServer()
        {
            var result = _wallpaperApiClient.ExecuteApiCall<PlaylistEntry[]>("wallpaper/playlist").Result;
            foreach (var cur in result)
            {
                _currentPlaylist.Enqueue(new PlaylistElement
                {
                    WallpaperId = cur.WallpaperId
                });
            }
        }

        public void ElementVisited(Guid wallpaperId)
        {

        }
    }
}
