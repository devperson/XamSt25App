using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using St25App.Models;
using St25App.Pages;
using St25App.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace St25App.ViewModels
{
    public class MemoryListViewModel : ViewModelBase
    {
        private readonly ITagReadWriteMemory tagReadWriteMemService;

        public MemoryListViewModel(INavigationService navigationService, IPageDialogService pageDialogService, ITagReadWriteMemory tagReadWriteMemService) : base(navigationService, pageDialogService)
        {
            this.ReadMemoryCommand = new DelegateCommand(OnReadMemoryCommand);
            this.ClearMemoryCommand = new DelegateCommand(OnClearMemoryCommand);
            this.tagReadWriteMemService = tagReadWriteMemService;
        }

        public List<TagMemoryRow> Rows { get; set; }
        public TagMemoryRow SelectedRow { get; set; }

        public int Start { get; set; } = 0;
        public int NumberOfBytes { get; set; } = 64;
        public DelegateCommand ReadMemoryCommand { get; set; }
        public DelegateCommand ClearMemoryCommand { get; set; }
        public bool ShowEditHint { get; set; }

        private async void OnReadMemoryCommand()
        {                        
            this.Rows = await tagReadWriteMemService.GetMemoryRowsAsync(Start, NumberOfBytes);
            this.ShowEditHint = true;
        }

        public void EditRow(TagMemoryRow row)
        {
            var param = new NavigationParameters();
            param.Add("StartAddress", Start);
            param.Add("SeelctedRow", row);
            this.NavigationService.NavigateAsync(nameof(EditMemoryRowPage), param);
        }

        private async void OnClearMemoryCommand()
        {
            var res = await PageDialogService.DisplayAlertAsync("Confirmation needed", "Do you want to erase the tag's memory?", "Yes", "Cancel");

            if (res)
                await tagReadWriteMemService.ClearMemoryAsync();
        }
    }
}
