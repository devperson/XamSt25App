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
using System.Threading.Tasks;
using Xamarin.Forms;

namespace St25App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INfcSettings nfcSettingsService;
        private readonly ITagReadWriteMemory tagReadWriteMemService;
        private bool ignorTagDiscovery = false;

        public MainViewModel(INavigationService navigationService, IPageDialogService pageDialogService, INfcSettings nfcSettingsService, ITagReadWriteMemory tagReadWriteMemService) : base(navigationService, pageDialogService)
        {
            this.EnableNfcCommand = new DelegateCommand(OnEnableNfcCommand);
            this.ClearMemoryCommand = new DelegateCommand(OnClearMemoryCommand);
            this.SaveMemoryCommand = new DelegateCommand(OnSaveMemoryCommand);
            this.nfcSettingsService = nfcSettingsService;
            this.tagReadWriteMemService = tagReadWriteMemService;
        }

        public TagInfo TagInfo { get; set; }

        [DependsOn(nameof(TagInfo))]
        public bool HasTag => TagInfo != null;

        public bool IsNfcDisabled { get; set; }

        public DelegateCommand EnableNfcCommand { get; set; }                
        public DelegateCommand ClearMemoryCommand { get; set; }
        public DelegateCommand SaveMemoryCommand { get; set; }
        public List<TagMemoryRow> Rows { get; set; }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            App.TagDiscoveredEvent += App_TagDiscoveredEvent;
            App.NfcDisabledAction = OnNfcDisabled;
        }        

        private async void App_TagDiscoveredEvent(object sender, TagInfo e)
        {
            if (ignorTagDiscovery)
                return;

            LoadingText = "Reading memory...";
            this.IsBusy = true;

            this.TagInfo = e;
            this.Rows = await tagReadWriteMemService.GetMemoryRowsAsync(0, e.SizeInBytes);

            this.IsBusy = false;            
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            ignorTagDiscovery = false;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            ignorTagDiscovery = true;
        }

        private void OnNfcDisabled()
        {
            this.IsNfcDisabled = true;
            this.TagInfo = null;
        }

        private void OnEnableNfcCommand()
        {
            this.IsNfcDisabled = false;
            nfcSettingsService.ShowNfcSettings();
        }
        
        private async void OnClearMemoryCommand()
        {
            var res = await PageDialogService.DisplayAlertAsync("Confirmation needed", "Do you want to erase the tag's memory?", "Yes", "Cancel");

            if (res)
            {
                Func<Task<bool>> func = async () =>
                {
                    var result = await tagReadWriteMemService.ClearMemoryAsync();
                    this.Rows = await tagReadWriteMemService.GetMemoryRowsAsync(0, TagInfo.SizeInBytes);

                    return result;
                };

                LoadingText = "Clearing memory...";
                this.IsBusy = true;
                var res2 = await func();
                this.IsBusy = false;

                if (res2 == false)//could not execute operation with tag
                {
                    NavigateToTapTag(func);
                }
            }
        }


        private async void OnSaveMemoryCommand()
        {
            Func<Task<bool>> func = async () =>
            {
               return await tagReadWriteMemService.UpdateMemoryRowsAsync(0, Rows);
            };

            LoadingText = "Saving memory...";
            this.IsBusy = true;
            var res = await func();
            this.IsBusy = false;

            if (res == false)//could not execute operation with tag
            {
                NavigateToTapTag(func);
            }
        }


        private async void NavigateToTapTag(Func<Task<bool>> func)
        {            
            var param = new NavigationParameters();
            param.Add("Function", func);
            param.Add("LoadingText", LoadingText);
            await this.NavigationService.NavigateAsync(nameof(TapTagPage), param);
        }
    }
}
