using MOnGoL.Common;
using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace MOnGoL.Backend
{
    public class GameOfLifeBoardService : IBoardService
    {
        private Board _theBoard;
        private SemaphoreSlim _lock = new SemaphoreSlim(1);

        public GameOfLifeBoardService()
        {
            _theBoard = new Board(21, 21);
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
            _theBoard = _theBoard.WithChanges(changes);
            OnBoardChanged?.Invoke(this, changes);
        }
    }
}
