using MOnGoL.Common;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
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
            await Task.Delay(TimeSpan.FromSeconds(2));
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

                var change = new Change(where, new PlacedToken(token.Emoji, false));
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
                ScheduleClearScore(scoringChanges);
            }

            foreach (var score in scores)
                playersService.Score(score);
        }

        private async void ScheduleClearScore(ChangeSet changes)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            var resetChanges = new ChangeSet(changes.Changes.Select(change => new Change(change.Coordinate, null)).ToImmutableList());
            using var _ = await _lock.DisposableEnter();
            ApplyChanges(resetChanges);
        }

        public static (ChangeSet, ImmutableList<PlacedToken> ScoringTokens) FindFiveInARows(Board board)
        {
            var Rows = board.Height;
            var Columns = board.Width;

            var minToScore = 5;

            var hits = board
                .GetRowsColumnsAndDiagonals()
                .SelectMany(rowOrColumn => rowOrColumn.Window((t1, t2) => t1.Token != t2.Token)
               .Where(segment => segment[0].Token is not null)
               .Where(segment => !IsScoreToken(segment[0].Token))
               .Where(segment => segment.Count >= minToScore));

            var resets = hits.SelectMany(segment => segment).Select(pair => new Change(pair.Coordinate, ScoreToken(pair.Token))).ToImmutableList();
            var scores = hits.SelectMany(segment => segment.Skip(minToScore - 1).Select(pair => pair.Token)).ToImmutableList();
            return (new ChangeSet(resets), scores);
        }

        private static PlacedToken ScoreToken(PlacedToken src) => new PlacedToken(src.Emoji, true);
        private static bool IsScoreToken(PlacedToken token) => token.Scored;
    }
}
