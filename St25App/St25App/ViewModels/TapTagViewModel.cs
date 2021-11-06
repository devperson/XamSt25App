using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace St25App.ViewModels
{
    public class TapTagViewModel : ViewModelBase
    {
        public TapTagViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
        }

        Func<Task<bool>> func;
        public override void Initialize(INavigationParameters parameters)
        {
            App.TagDiscoveredEvent += App_TagDiscoveredEvent;

            if (parameters.ContainsKey("Function"))
            {
                func = (Func<Task<bool>>)parameters["Function"];
                LoadingText = parameters["LoadingText"].ToString();
            }
        }

        private async void App_TagDiscoveredEvent(object sender, Models.TagInfo e)
        {
            if (func != null)
            {
                this.IsBusy = true;
                var res = await func();
                this.IsBusy = false;

                if (res)
                {
                    await this.NavigationService.GoBackAsync();
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            App.TagDiscoveredEvent -= App_TagDiscoveredEvent;
        }
    }
}
