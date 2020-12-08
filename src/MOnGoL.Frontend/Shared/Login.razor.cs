using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MOnGoL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MOnGoL.Frontend.Shared
{
    public partial class Login : IDisposable
    {
        [Inject] private IPlayerService PlayerService { get; set; }
        [Inject] private ILocalStorageService LocalStorage { get; set; }
        [Inject] private ILogger<PlayerList> Logger { get; set; }

        private string Name = String.Empty;
        private string Emoji => Emojis.Contains(EmojiText)
            ? EmojiText
            : EmojiData.Emoji.TryGetEmojiByShortcode(EmojiText.Replace(":","").Trim(), out var emoji)?emoji.ToString():null;
        private string EmojiText = String.Empty;
        private PlayerInfo? MyInfo { get; set; }

        private HashSet<string> Emojis = new HashSet<string>(EmojiData.Emoji.All.Select(emo => emo.ToString()));

        public Login()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Logger.LogDebug("Reading my info from PlayerService");
            PlayerService.OnMyInfoChanged += OnMyInfoChanged;
            MyInfo = await PlayerService.GetMyInfo();
            Logger.LogDebug("Got this: '{0}'", MyInfo);
        }

        private async void OnMyInfoChanged(object sender, PlayerInfo? e)
        {
            MyInfo = e;
            await InvokeAsync(StateHasChanged);
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

        private bool CanRegister => !string.IsNullOrEmpty(Name) && Emoji is not null;
        private bool CanRandom => RandomTask is null;
        private Task RandomTask { get; set; }

        private CancellationTokenSource disposalCts = new CancellationTokenSource();

        private async void RandomEmoji()
        {
            RandomTask = SpinTheWheel();
            await RandomTask;
            RandomTask = null;
            await InvokeAsync(StateHasChanged);
        }

        private async Task SpinTheWheel()
        {
            var max = EmojiData.Emoji.All.Length;
            foreach (var delay in Enumerable.Range(1, 50).Select(i => TimeSpan.FromMilliseconds(i * i * i / 125)))
            {
                await Task.Delay(delay, disposalCts.Token);
                var emojiNo = new Random().Next(0, max);
                EmojiText = EmojiData.Emoji.All[emojiNo].ToString();
                await InvokeAsync(StateHasChanged);
                if (MyInfo is not null)
                    break;
            }
        }

        private async Task Leave()
        {
            if (MyInfo is not null)
            {
                await PlayerService.Leave();
                MyInfo = null;
            }
        }

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

        public void Dispose()
        {
            disposalCts.Cancel();
            PlayerService.OnMyInfoChanged -= OnMyInfoChanged;
        }
    }
}
