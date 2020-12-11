using Microsoft.AspNetCore.Components;
using MOnGoL.Common;
using System;
using System.Threading.Tasks;

namespace MOnGoL.Frontend.Shared
{
    public partial class LoginButton : IDisposable
    {
        [Inject] private IPlayerService PlayerService { get; set; }

        private bool IsLoggedIn { get; set; }

        protected override async Task OnInitializedAsync()
        {
            PlayerService.OnMyInfoChanged += OnMyInfoChanged;
            IsLoggedIn = await PlayerService.GetMyInfo() is not null;
        }

        private async void OnMyInfoChanged(object sender, PlayerInfo? e)
        {
            IsLoggedIn = e is not null;
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            PlayerService.OnMyInfoChanged -= OnMyInfoChanged;
        }
    }
}
