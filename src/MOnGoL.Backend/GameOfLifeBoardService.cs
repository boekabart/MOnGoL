using MOnGoL.Common;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MOnGoL.Backend
{
    public class GameOfLifeBoardService : IBoardService
    {
        private Board _theBoard;
        private SemaphoreSlim _lock = new SemaphoreSlim(1);
        private readonly IPlayersService playersService;

        public GameOfLifeBoardService(IPlayersService playersService)
        {
            _theBoard = Board.Create(21, 21);
            LifeStep();
            this.playersService = playersService;
        }

        public EventHandler<ChangeSet> OnBoardChanged { get; set; }

        public async Task<Board> GetBoard()
        {
            await _lock.WaitAsync();
            try
            {
                return _theBoard;
            }
            finally
            {
                _lock.Release();
            }
        }

        private async void LifeStep()
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            using var _ = await _lock.DisposableEnter();
            var changes = GameOfLife.NextGenerationChanges(_theBoard);
            ApplyChanges(changes);
            LifeStep();
        }

        public async Task<bool> TryPlaceToken(Coordinate where, Token token)
        {
            await _lock.WaitAsync();
            try
            {
                if (!_theBoard.IsValidCoordinate(where))
                    throw new ArgumentException(nameof(where));

                var currentToken = _theBoard.TokenAt(where);
                if (currentToken is not null)
                {
                    if (currentToken.Equals(token))
                        return true; // Goal met
                    return false; // Occupado
                }

                var change = new Change(where, token);
                ApplyChange(change);

                return true;
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Call from within lock
        /// </summary>
        /// <param name="change">The change to apply; pre-valided</param>
        private void ApplyChange(Change change)
        {
            var changes = new ChangeSet(ImmutableList<Change>.Empty.Add(change));
            ApplyChanges(changes);
        }

        /// <summary>
        /// Call from within lock
        /// </summary>
        /// <param name="change">The change to apply; pre-valided</param>
        private void ApplyChanges(ChangeSet changes)
        {
            if (!changes.Changes.Any())
                return;

            _theBoard = _theBoard.WithChanges(changes);
            OnBoardChanged?.Invoke(this, changes);

            Score();
        }

        private void Score()
        {
            var (scoringChanges, scores) = FindFiveInARows(_theBoard);
            if (scoringChanges.Changes.Any())
            {
                _theBoard = _theBoard.WithChanges(scoringChanges);
                OnBoardChanged?.Invoke(this, scoringChanges);
            }

            foreach (var score in scores)
                playersService.Score(score);
        }

        public static (ChangeSet, ImmutableList<Token> ScoringTokens) FindFiveInARows(Board board)
        {
            var Rows = board.Height;
            var Columns = board.Width;

            var minToScore = 5;

            var hits = board.GetRowsAndColumns()
                .SelectMany(rowOrColumn => rowOrColumn.Window((t1, t2) => t1.Token != t2.Token)
               .Where(segment => segment[0].Token is not null)
               .Where(segment => segment.Count >= minToScore));

            var resets = hits.SelectMany(segment => segment).Select(pair => new Change(pair.Coordinate, null)).ToImmutableList();
            var scores = hits.SelectMany(segment => segment.Skip(minToScore - 1).Select(pair => pair.Token)).ToImmutableList();
            return (new ChangeSet(resets), scores);
        }

    }
}
