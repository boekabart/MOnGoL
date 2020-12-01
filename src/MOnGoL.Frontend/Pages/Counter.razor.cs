using Microsoft.AspNetCore.Components;
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

        private string Name = String.Empty;
        private string Emoji => EmojiData.Emoji.TryGetEmojiByShortcode(EmojiText.Replace(":","").Trim(), out var emoji)?emoji.ToString():null;
        private string EmojiText = String.Empty;
        private PlayerInfo? MyInfo { get; set; }

        private IImmutableList<PlayerInfo>? playerList = null;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            PlayerService.OnPlayerlistChanged += OnNewValue;
            MyInfo = await PlayerService.GetMyInfo();
            playerList = await PlayerService.GetPlayerlist();
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

            MyInfo = await PlayerService.Register(
            new PlayerInfo(Name,
                new Token(Rune.GetRuneAt(Emoji, 0))
                )
            );
        }

        public async ValueTask DisposeAsync()
        {
            PlayerService.OnPlayerlistChanged -= OnNewValue;
            await PlayerService.Leave();
        }
    }
}
