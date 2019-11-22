using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Application.Wallpaper.Client.Model;
using WpfUi.Common.Helper;

namespace Plugin.Application.Wallpaper.Client
{
    public class WallpaperViewModel : ViewModel
    {
        private LocalWallpaper _wallpaper;

        public string Caption => _wallpaper.Caption;
        public string WallpaperPath => _wallpaper.Thumbnail.Exists ? _wallpaper.Thumbnail.FullName : "na.png";
        public AsyncRelayCommand DownVoteCommand { get; }
        public AsyncRelayCommand UpVoteCommand { get; }
        public RelayCommand InfoCommand { get; }

        public WallpaperViewModel(LocalWallpaper wallpaper)
        {
            _wallpaper = wallpaper ?? throw new ArgumentNullException(nameof(wallpaper));

            DownVoteCommand = new AsyncRelayCommand(OnDownVoteCommand, x => false);
            UpVoteCommand = new AsyncRelayCommand(OnUpVoteCommand, x => false);
            InfoCommand = new RelayCommand(x => OnInfo());
        }

        private void OnInfo()
        {
            var uri = new Uri($"https://web.sternheim.eu/wallpaper/details/{_wallpaper.Id.ToString()}");
            Process.Start(new ProcessStartInfo(uri.AbsoluteUri));
        }

        private async Task OnDownVoteCommand()
        { }

        private async Task OnUpVoteCommand()
        { }
    }
}
