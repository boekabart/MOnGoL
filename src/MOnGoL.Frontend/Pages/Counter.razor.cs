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

namespace MOnGoL.Frontend.Pages
{
    public partial class Counter : IAsyncDisposable
    {
        [Inject] private IPlayerService PlayerService { get; set; }
        [Inject] private ILocalStorageService LocalStorage { get; set; }
        [Inject] private ILogger<Counter> Logger { get; set; }

        private string Name = String.Empty;
        private string Emoji => EmojiData.Emoji.TryGetEmojiByShortcode(EmojiText.Replace(":","").Trim(), out var emoji)?emoji.ToString():null;
        private string EmojiText = String.Empty;
        private PlayerInfo? MyInfo { get; set; }

        private IImmutableList<PlayerInfo>? playerList = null;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Logger.LogDebug("Reading my info from PlayerService");
            MyInfo = await PlayerService.GetMyInfo();
            Logger.LogDebug("Got this: '{0}'", MyInfo);
            PlayerService.OnPlayerlistChanged += OnNewValue;
            playerList = await PlayerService.GetPlayerlist();
            Logger.LogDebug("PlayerList retrieved with {0} entries", playerList.Count);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            if (MyInfo is not null)
                return;

            Logger.LogDebug("Seeing if I have stored 'PlayerInfo' in localStorage...");
            var storedInfo = await LocalStorage.GetItemAsync<PlayerInfo>("PlayerInfo");
            Logger.LogDebug("Got this: '{0}'", storedInfo);
            if (storedInfo is not null)
            {
                Logger.LogDebug("Trying to register with stored playerInfo");
                MyInfo = await PlayerService.Register(storedInfo);
                if (MyInfo is null)
                {
                    Logger.LogDebug("Server refused, apparently that would be a conflict with an already registered player");
                    Name = storedInfo.Name;
                    EmojiText = storedInfo.Token.Emoji.ToString();
                }
                this.StateHasChanged();
            }
        }

        private async void OnNewValue(object sender, IImmutableList<PlayerInfo> newValue)
        {
            playerList = newValue;
            await InvokeAsync(StateHasChanged);
        }

        private bool CanRegister => !string.IsNullOrEmpty(Name) && Emoji is not null;

        private async Task Register()
        {
            if (!CanRegister)
                return;
            if (MyInfo is not null)
                return;

            Logger.LogDebug("Trying to register playerInfo");
            MyInfo = await PlayerService.Register(
            new PlayerInfo(Name,
                new Token(Rune.GetRuneAt(Emoji, 0).ToString())
                )
            );
            if (MyInfo is not null)
            {
                Logger.LogDebug("Server accepted. Storing this data in LocalStorage now");
                await LocalStorage.SetItemAsync("PlayerInfo", MyInfo);
                Logger.LogDebug("Stored");
            }
            else
                Logger.LogDebug("Server refused, apparently that would be a conflict with an already registered player");
        }

        public ValueTask DisposeAsync()
        {
            PlayerService.OnPlayerlistChanged -= OnNewValue;
            //await PlayerService.Leave();
            return ValueTask.CompletedTask;
        }
    }
}
