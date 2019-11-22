using System;
using System.Threading.Tasks;
using AutoUpdate.Abstraction;
using Framework.Contracts.Extension;
using Framework.Contracts.IocContainer;
using Microsoft.Win32;
using Plugin.Application.Wallpaper.Client.Mangers;
using WpfUi.Common.Helper;
using WpfUi.Contracts.Services;

namespace Plugin.Application.Wallpaper.Client
{
    public class TaskBarIconViewModel : ViewModel, IUserInteraction
    {
        private const string APPLICATION_NAME = "WallpaperClient";
        private readonly IUiApplication _uiApplication;
        private readonly IEnvironmentParameters _environmentParameters;
        private readonly WallpaperClientVersionService _wallpaperClientVersionService;
        private readonly RegistryKey _regKeyAutostart;
        private readonly WallpaperOrchestrator _orchestrator;
        private IUpdateVersionHandle _updateHandle;

        public ApplicationState _state;

        public RelayCommand ExitCommand { get; set; }
        public AsyncRelayCommand ExecuteUpdateCommand { get; set; }
        public RelayCommand ChangeAutostartCommand { get; set; }
        public RelayCommand ResetViewCommand { get; set; }
        public AsyncRelayCommand LoginCommand { get; set; }


        public ApplicationState State
        {
            get { return _state; }
            set { SetValue(() => State, value, ref _state); }
        }

        public string VersionNumber => _wallpaperClientVersionService.DetermineCurrentVersionNumber().ToString();

        public string UpdateMenuEntry => _updateHandle != null && _updateHandle.HasNewVersion ? "Update: New Version " + _updateHandle.NewVersion.VersionNumber : "Update: N/A";
        public bool IsAutostartActive => _regKeyAutostart.GetValue(APPLICATION_NAME) != null;

        public TaskBarIconViewModel(IDependencyResolver resolver,
                                    IUiApplication uiApplication,
                                    IDispatcher dispatcher,
                                    IEnvironmentParameters environmentParameters,
                                    WallpaperClientVersionService wallpaperClientVersionService)
        {
            _regKeyAutostart = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            _orchestrator = new WallpaperOrchestrator(resolver);
            _state = new NoConnection(_orchestrator, this);

            _uiApplication = uiApplication;
            _environmentParameters = environmentParameters;
            _wallpaperClientVersionService = wallpaperClientVersionService;
            ExitCommand = new RelayCommand(x => OnExit());
            LoginCommand = new AsyncRelayCommand(() => _state.OnLogin());
            ChangeAutostartCommand = new RelayCommand(x => OnChangeAutostart());
            ResetViewCommand = new RelayCommand(x => OnResetView());
            ExecuteUpdateCommand = new AsyncRelayCommand(() => OnExecuteUpdateCommand(), x => _updateHandle != null && _updateHandle.HasNewVersion);

            dispatcher.Dispatch(() =>
            {
                _state.OnLogin();
            });
        }

        private void OnResetView()
            => _orchestrator.ResetMonitors();

        private void OnExit()
            => _uiApplication.Exit();

        private void OnChangeAutostart()
        {
            if (IsAutostartActive)
            {
                _regKeyAutostart.DeleteValue(APPLICATION_NAME, false);
            }
            else
            {
                _regKeyAutostart.SetValue(APPLICATION_NAME, _environmentParameters.ExecutablePath);
            }

            OnPropertyChanged(nameof(IsAutostartActive));
        }

        private async Task OnExecuteUpdateCommand()
        {
            if (_updateHandle == null || !_updateHandle.HasNewVersion)
            {
                return;
            }

            await Task.Factory.StartNew(() => _updateHandle.UpdateToNewVersion());
        }

        public void NewVersionAvailable(IUpdateVersionHandle handle)
        {
            _updateHandle = handle;
            OnPropertyChanged(nameof(UpdateMenuEntry));
        }
    }
}
