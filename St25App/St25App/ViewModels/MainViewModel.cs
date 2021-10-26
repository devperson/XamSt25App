using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using PropertyChanged;
using St25App.Models;
using St25App.Pages;
using St25App.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace St25App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INfcSettings nfcSettingsService;

        public MainViewModel(INavigationService navigationService, IPageDialogService pageDialogService, INfcSettings nfcSettingsService) : base(navigationService, pageDialogService)
        {
            this.EnableNfcCommand = new DelegateCommand(OnEnableNfcCommand);
            this.ShowReadWriteMemPageCommand = new DelegateCommand(OnShowReadWriteMemPageCommand);
            this.nfcSettingsService = nfcSettingsService;
        }

        public TagInfo TagInfo { get; set; }

        [DependsOn(nameof(TagInfo))]
        public bool HasTag => TagInfo != null;

        public bool IsNfcDisabled { get; set; }

        public DelegateCommand EnableNfcCommand { get; set; }
        public DelegateCommand ShowReadWriteMemPageCommand { get; set; }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            App.TagDiscoveredAction = OnTagDiscovered;
            App.NfcDisabledAction = OnNfcDisabled;
        }

        private void OnNfcDisabled()
        {
            this.IsNfcDisabled = true;
            this.TagInfo = null;
        }

        private void OnTagDiscovered(TagInfo tagInfo)
        {
            this.TagInfo = tagInfo;
        }

        private void OnEnableNfcCommand()
        {
            this.IsNfcDisabled = false;
            nfcSettingsService.ShowNfcSettings();
        }

        private void OnShowReadWriteMemPageCommand()
        {
            NavigationService.NavigateAsync(nameof(MemoryListViewPage));
        }
    }
}
