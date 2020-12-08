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
    public partial class PlayerList : IDisposable
    {
        [Inject] private IPlayerService PlayerService { get; set; }
        [Inject] private ILogger<PlayerList> Logger { get; set; }

        private IImmutableList<PlayerState>? playerList = null;
        private PlayerInfo? MyInfo { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Logger.LogDebug("Reading my info from PlayerService");
            PlayerService.OnMyInfoChanged += OnMyInfoChanged;
            MyInfo = await PlayerService.GetMyInfo();

            Logger.LogDebug("Reading playerList from PlayerService");
            PlayerService.OnPlayerlistChanged += OnNewPlayerlist;
            playerList = await PlayerService.GetPlayerlist();

            Logger.LogDebug("PlayerList retrieved with {0} entries", playerList.Count);
        }

        private async void OnMyInfoChanged(object sender, PlayerInfo? e)
        {
            MyInfo = e;
            await InvokeAsync(StateHasChanged);
        }

        private bool IsMe(PlayerInfo playerInfo) => MyInfo is not null && playerInfo.Equals(MyInfo);

        private async void OnNewPlayerlist(object sender, IImmutableList<PlayerState> newValue)
        {
            playerList = newValue;
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            PlayerService.OnPlayerlistChanged -= OnNewPlayerlist;
        }
    }
}
