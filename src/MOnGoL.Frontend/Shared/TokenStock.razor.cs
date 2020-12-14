using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MOnGoL.Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOnGoL.Frontend.Shared
{
    public partial class TokenStock : IDisposable
    {
        [Inject] private IPlayerService PlayerService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            PlayerService.OnTokenStockChanged += OnTokenStockChanged;
            await SetTokenStock(await PlayerService.GetTokenStock());
        }

        private async void OnTokenStockChanged(object sender, int newStockValue)
        {
            await SetTokenStock(newStockValue);
        }

        private async Task SetTokenStock(int newStockValue)
        {
            var myInfo = await PlayerService.GetMyInfo();
            Stock = myInfo is null
                ? Array.Empty<string>()
                : Enumerable.Repeat(myInfo.Token.Emoji, newStockValue).ToArray();
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            PlayerService.OnTokenStockChanged -= OnTokenStockChanged;
        }

        public string[] Stock { get; private set; } = Array.Empty<string>();
    }
}
