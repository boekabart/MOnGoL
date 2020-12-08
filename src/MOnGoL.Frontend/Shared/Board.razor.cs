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
        [Inject] private IPlayerBoardService BoardService { get; set; }
        [Inject] private ILogger<PlayerList> Logger { get; set; }

        private Common.Board? board;

        private int Height => board?.Height ?? 20;
        private int Width => board?.Width ?? 20;

        private IEnumerable<IEnumerable<Coordinate>> Rows
            => Enumerable.Range(0, board.Height).Select(y => Enumerable.Range(0, board.Width).Select(x => new Coordinate(x, y)));

        private PlacedToken? Coor(Coordinate coordinate) => board?.TokenAt(coordinate);

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            BoardService.OnBoardChanged += OnBoardChanged;
            board = await BoardService.GetBoard();
        }

        private async Task Put(Coordinate coor)
        {
            var success = await BoardService.TryPlaceToken(coor);
        }

        private async void OnBoardChanged(object sender, ChangeSet changes)
        {
            board = board.WithChanges(changes);
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            BoardService.OnBoardChanged -= OnBoardChanged;
        }
    }
}
