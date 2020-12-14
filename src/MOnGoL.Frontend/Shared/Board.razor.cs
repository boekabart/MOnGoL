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

        private Common.Board? board;

        private IEnumerable<IEnumerable<Coordinate>> Rows
            => Enumerable.Range(0, board.Height).Select(y => Enumerable.Range(0, board.Width).Select(x => new Coordinate(x, y)));

        private PlacedToken? Coor(Coordinate coordinate) => board?.TokenAt(coordinate);

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            PlayerService.OnBoardChanged += OnBoardChanged;
            board = await PlayerService.GetBoard();
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

        public void Dispose()
        {
            PlayerService.OnBoardChanged -= OnBoardChanged;
        }

        public static string DummyEmoji { get; } = "╳\uFE0F";
    }
}
