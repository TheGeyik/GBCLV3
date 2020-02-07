using GBCLV3.Models;
using GBCLV3.Models.Installation;
using GBCLV3.Services;
using GBCLV3.Services.Authentication;
using GBCLV3.Services.Launch;
using GBCLV3.Utils;
using GBCLV3.ViewModels.Pages;
using Stylet;
using StyletIoC;
using System.Windows;

namespace GBCLV3.ViewModels.Windows
{
    class MainViewModel : Conductor<IScreen>.Collection.OneActive
    {
        #region Private Members

        // IoC
        private readonly Config _config;
        private readonly VersionService _versionService;
        private readonly AuthService _authService;

        private readonly Screen[] _pages;

        private readonly IWindowManager _windowManager;

        #endregion

        #region Constructor

        [Inject]
        public MainViewModel(
            ConfigService configService,
            VersionService versionService,
            AuthService authService,
            ThemeService themeService,

            LaunchViewModel launchVM,
            SettingsRootViewModel settingsVM,
            VersionsRootViewModel versionsVM,
            AuxiliariesRootViewModel auxVM,
            AboutViewModel aboutVM,

            IWindowManager windowManager)
        {
            _windowManager = windowManager;
            _config = configService.Entries;
            _versionService = versionService;
            _authService = authService;

            _pages = new Screen[]
            {
                launchVM, settingsVM, versionsVM, auxVM, aboutVM,
            };

            // Start up with LaunchView
            ActivePageIndex = 0;

            // Bind background image service
            ThemeService = themeService;

            //Set Title
            this.DisplayName = "GBCL v" + AssemblyUtil.Version;

            // Register usernameChanged event
            authService.UsernameChanged += username => Username = username;
        }

        #endregion

        #region Bindings

        public ThemeService ThemeService { get; private set; }

        public string Username { get; set; }

        public int ActivePageIndex { get; set; }

        public void ChangeActivePage()
        {
            this.ActivateItem(_pages[ActivePageIndex]);
        }

        #endregion

        #region Override Methods

        protected override void OnInitialActivate()
        {
            // If no game version exists, navigate to version install view on user's discretion
            if (!_versionService.LoadAll())
            {
                if (_windowManager.ShowMessageBox("${WhetherInstallNewVersion}", "${NoVersionFound}",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ActivePageIndex = 2;
                    (_pages[2] as VersionsRootViewModel).OnNavigateInstallView(null, InstallType.Version);
                }
            }
        }

        #endregion
    }
}
