using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using CoreTail.Shared;
using CoreTail.Uwp.Platform;
using FileInfo = CoreTail.Uwp.Platform.FileInfo;
using SystemPlatformService = CoreTail.Uwp.Platform.SystemPlatformService;

namespace CoreTail.Uwp
{
    sealed partial class App
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (e.PrelaunchActivated == false)
                Initialize(e);
        }

        protected override async void OnActivated(IActivatedEventArgs e)
        {
            var vm = Initialize(e);

            if (e.Kind == ActivationKind.Protocol)
            {
                var storageFile = 
                    await ((ProtocolActivatedEventArgs) e)
                        .Data.Values.OfType<string>()
                        .Select(SharedStorageAccessManager.RedeemTokenForFileAsync)
                        .FirstOrDefault();

                if (storageFile != null)
                    await vm.OpenAndWatchFileAsync(new FileInfo(storageFile));
            }
        }

        private static FileReaderViewModel<FileInfo> Initialize(IActivatedEventArgs e)
        {
            FileReaderViewModel<FileInfo> vm;

            var mainPage = Window.Current.Content as MainPage;

            var createPage = mainPage == null;

            if (createPage)
            {
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                mainPage = new MainPage();
            }

            if (mainPage.DataContext == null)
            {
                vm = CreateViewModel();
                mainPage.DataContext = vm;
            }
            else
                vm = mainPage.DataContext as FileReaderViewModel<FileInfo>;

            if (createPage)
            {
                Window.Current.Content = mainPage;
                Window.Current.Activate();
            }

            return vm;
        }
        
        // TODO: hide UI on sharing
        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs e)
        {
            e.ShareOperation.ReportStarted();

            if (e.ShareOperation.Data.Contains(StandardDataFormats.StorageItems))
            {
                var sharedStorageFiles = 
                    (await e.ShareOperation.Data.GetStorageItemsAsync())
                    .OfType<StorageFile>()
                    .ToArray();

                if (sharedStorageFiles.Length > 0)
                {
                    var inputData = new ValueSet();

                    for (var i = 0; i < sharedStorageFiles.Length; i++)
                        inputData.Add($"file{i}", SharedStorageAccessManager.AddFile(sharedStorageFiles[i]));

                    if (await Launcher.LaunchUriAsync(
                        new Uri("coretail-app2app:?x=" + Guid.NewGuid()),
                        new LauncherOptions { TargetApplicationPackageFamilyName = "5a3cae1b-7b4c-4ed2-be89-40911aa7f6ae_n6fhnfby9ccnm" },
                        inputData))
                    {
                        e.ShareOperation.ReportDataRetrieved();
                        e.ShareOperation.ReportCompleted();
                        return;
                    }                    
                }
            }

            e.ShareOperation.ReportError("Error opening file(s)");
        }

        private static void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            var mainPage = Window.Current.Content as MainPage;

            if (mainPage?.DataContext != null)
            {
                (mainPage.DataContext as IDisposable)?.Dispose();
                mainPage.DataContext = null;
            }

            deferral.Complete();
        }

        private static FileReaderViewModel<FileInfo> CreateViewModel()
        {
            return new ViewModelFactory<FileInfo>(
                    new Dispatcher(Window.Current.Dispatcher),
                    new UIPlatformService(),
                    new SystemPlatformService())
                .CreateMainWindowViewModel();
        }
    }
}