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
    public partial class Board : IDisposable
    {
        [Inject] private IPlayerService PlayerService { get; set; }
        [Inject] private ILogger<PlayerList> Logger { get; set; }

        private Common.Board? board;

        private IEnumerable<IEnumerable<Coordinate>> Rows
            => Enumerable.Range(0, board.Height).Select(y => Enumerable.Range(0, board.Width).Select(x => new Coordinate(x, y)));

        private PlacedToken? Coor(Coordinate coordinate) => board?.TokenAt(coordinate);

        private int Countdown { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            PlayerService.OnBoardChanged += OnBoardChanged;
            PlayerService.OnCountdownChanged += OnCountdownChanged;
            PlayerService.OnTokenStockChanged += OnTokenStockChanged;
            board = await PlayerService.GetBoard();
            await SetTokenStock(await PlayerService.GetTokenStock());
        }

        private async Task Put(Coordinate coor)
        {
            var success = await PlayerService.TryPlaceToken(coor);
        }

        private async void OnBoardChanged(object sender, ChangeSet changes)
        {
            board = board.WithChanges(changes);
            await InvokeAsync(StateHasChanged);
        }

        private async void OnCountdownChanged(object sender, int countDown)
        {
            Countdown = countDown;
            await InvokeAsync(StateHasChanged);
        }

        private async void OnTokenStockChanged(object sender, int newStockValue)
        {
            await SetTokenStock(newStockValue);
        }

        private async Task SetTokenStock(int newStockValue)
        {
            var myInfo = await PlayerService.GetMyInfo();
            TokenStock = myInfo is null
                ? Array.Empty<string>()
                : Enumerable.Repeat(myInfo.Token.Emoji, newStockValue).ToArray();
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            PlayerService.OnBoardChanged -= OnBoardChanged;
            PlayerService.OnCountdownChanged -= OnCountdownChanged;
            PlayerService.OnTokenStockChanged -= OnTokenStockChanged;
        }

        public static string DummyEmoji { get; } = "╳\uFE0F";
        public string[] TokenStock { get; private set; } = Array.Empty<string>();
    }
}
