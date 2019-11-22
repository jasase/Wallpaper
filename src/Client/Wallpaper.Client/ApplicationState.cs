using System.Threading.Tasks;
using Plugin.Application.Wallpaper.Client.Mangers;
using Plugin.Application.Wallpaper.Client.Views;
using WpfUi.Common.Helper;

namespace Plugin.Application.Wallpaper.Client
{
    public abstract class ApplicationState : ViewModel
    {
        protected readonly WallpaperOrchestrator _orchestrator;
        protected readonly TaskBarIconViewModel _parent;

        private bool _isLoggedIn;
        private ViewModel _popUpViewModel;

        public string Username => _orchestrator.AuthenticationManager.Username;
        public abstract bool IsLoggedIn { get; }
        public abstract ViewModel PopupViewModel { get; }

        public ApplicationState(WallpaperOrchestrator orchestrator, TaskBarIconViewModel parent)
        {
            _orchestrator = orchestrator;
            _parent = parent;
        }

        public abstract Task OnLogin();
    }

    public class Connected : ApplicationState
    {
        private readonly PopupViewModel _popupViewModel;

        public Connected(WallpaperOrchestrator orchestrator, TaskBarIconViewModel parent)
            : base(orchestrator, parent)
        {
            _popupViewModel = new PopupViewModel(orchestrator);
        }

        public override bool IsLoggedIn => true;
        public override ViewModel PopupViewModel => _popupViewModel;

        public override async Task OnLogin()
        {
            await _orchestrator.AuthenticationManager.Logout();
            _parent.State = new NoConnection(_orchestrator, _parent);
        }

    }

    public class NoConnection : ApplicationState
    {
        private readonly NotConnectedPopupViewModel _popupViewModel;

        public NoConnection(WallpaperOrchestrator orchestrator, TaskBarIconViewModel parent)
            : base(orchestrator, parent)
        {
            _popupViewModel = new NotConnectedPopupViewModel();
        }

        public override bool IsLoggedIn => false;
        public override ViewModel PopupViewModel => _popupViewModel;

        public override async Task OnLogin()
        {
            var result = await _orchestrator.AuthenticationManager.Login();
            _parent.State = new Connected(_orchestrator, _parent);

            _orchestrator.Start();
        }
    }
}
