using Microsoft.AspNetCore.Components;
using MOnGoL.Common;
using System;
using System.Threading.Tasks;

namespace MOnGoL.Frontend.Shared
{
    public partial class Countdown : IDisposable
    {
        [Inject] private IPlayerService PlayerService { get; set; }

        private int CountdownCount { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            PlayerService.OnCountdownChanged += OnCountdownChanged;
        }

        private async void OnCountdownChanged(object sender, int countDown)
        {
            CountdownCount = countDown;
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            PlayerService.OnCountdownChanged -= OnCountdownChanged;
        }
    }
}
