using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using St25App.Models;
using St25App.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace St25App.ViewModels
{
    public class EditMemoryRowViewModel : ViewModelBase
    {
        private readonly ITagReadWriteMemory tagReadWriteMemService;

        public EditMemoryRowViewModel(INavigationService navigationService, IPageDialogService pageDialogService, ITagReadWriteMemory tagReadWriteMemService) : base(navigationService, pageDialogService)
        {
            this.ApplyChangesCommand = new DelegateCommand(OnApplyChangesCommand);
            this.tagReadWriteMemService = tagReadWriteMemService;
        }

        public TagMemoryRow SelectedRow { get; set; }
        public int StartAddress { get; set; }

        public DelegateCommand ApplyChangesCommand { get; set; }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if(parameters.ContainsKey("SeelctedRow"))
            {
                SelectedRow = parameters["SeelctedRow"] as TagMemoryRow;
                StartAddress = (int)parameters["StartAddress"];
            }
        }

        public async void OnApplyChangesCommand()
        {
            await tagReadWriteMemService.UpdateMemoryRowAsync(StartAddress, SelectedRow);
            await this.NavigationService.GoBackAsync();
        }
    }
}
