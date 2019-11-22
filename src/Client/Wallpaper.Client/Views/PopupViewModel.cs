using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Application.Wallpaper.Client.Mangers;
using WpfUi.Common.Helper;

namespace Plugin.Application.Wallpaper.Client.Views
{
    public class PopupViewModel : ViewModel
    {
        private readonly WallpaperOrchestrator _wallpaperOrchestrator;

        public AsyncRelayCommand ForwardCommand { get; set; }
        public AsyncRelayCommand BackwardCommand { get; set; }

        public IEnumerable<WallpaperViewModel> Past => _wallpaperOrchestrator.PlaylistLast.Select(x => new WallpaperViewModel(x));
        public IEnumerable<WallpaperViewModel> Current => _wallpaperOrchestrator.PlaylistCurrent.Select(x => new WallpaperViewModel(x));
        public IEnumerable<WallpaperViewModel> Future => _wallpaperOrchestrator.Playlist.Select(x => new WallpaperViewModel(x));

        public PopupViewModel(WallpaperOrchestrator wallpaperOrchestrator)
        {
            _wallpaperOrchestrator = wallpaperOrchestrator;

            _wallpaperOrchestrator.WallpaperChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(Past));
                OnPropertyChanged(nameof(Current));
                OnPropertyChanged(nameof(Future));
            };

            ForwardCommand = new AsyncRelayCommand(OnForward, x => _wallpaperOrchestrator.Playlist.Any());
            BackwardCommand = new AsyncRelayCommand(OnBackward, x => _wallpaperOrchestrator.PlaylistLast.Any());
        }

        private Task OnBackward()
        {
            _wallpaperOrchestrator.RotatePictureBackward();
            return Task.CompletedTask;
        }

        private Task OnForward()
        {
            _wallpaperOrchestrator.RotatePictureForward();
            return Task.CompletedTask;
        }
    }
}
