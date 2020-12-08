using MOnGoL.Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;

namespace MOnGoL.Backend
{
    public static class GameOfLife
    {
        public static ChangeSet NextGenerationChanges(Board currentGrid)
        {

            var changes = GetChanges(currentGrid);
            return new ChangeSet(changes.ToImmutableList());

            // Using a private function so we can use yield return.
            // Private because processing as IEnumerable is fatal.
            IEnumerable<Change> GetChanges(Board currentGrid)
            {
                var Rows = currentGrid.Height;
                var Columns = currentGrid.Width;
                // Loop through every cell 
                for (var row = 0; row < Rows; row++)
                {
                    for (var column = 0; column < Columns; column++)
                    {
                        var coor = new Coordinate(row, column);
                        // find your alive neighbors
                        var aliveNeighbors = new List<PlacedToken>();
                        for (var i = -1; i <= 1; i++)
                        {
                            for (var j = -1; j <= 1; j++)
                            {
                                if (i == 0 && j == 0)
                                    continue;
                                var neighbor = currentGrid.TokenAt(new Coordinate((row + i + Rows) % Rows, (column + j + Columns) % Columns));
                                if (neighbor is null)
                                    continue;
                                aliveNeighbors.Add(neighbor);
                            }
                        }

                        var currentCell = currentGrid.TokenAt(coor);

                        if (currentCell.IsLocked())
                            continue;

                        // Implementing the Rules of Life 

                        // Cell is lonely and dies 
                        if (currentCell.IsAlive() && aliveNeighbors.Count < 2)
                        {
                            yield return new Change(coor, Dead);
                        }

                        // Cell dies due to over population 
                        else if (currentCell.IsAlive() && aliveNeighbors.Count > 3)
                        {
                            yield return new Change(coor, Dead);
                        }

                        // A new cell is born 
                        else if (currentCell.IsDead() && aliveNeighbors.Count == 3)
                        {
                            yield return new Change(coor, aliveNeighbors[RandomNumberGenerator.GetInt32(0, 3)].Unlocked());
                        }
                    }
                }
            }
        }
        private static PlacedToken Dead => null;
        private static PlacedToken Unlocked(this PlacedToken token) => token.IsLocked() ? new PlacedToken(token.Emoji, false) : token;
        private static bool IsDead(this PlacedToken token) => token is null;
        private static bool IsAlive(this PlacedToken token) => token is not null && !token.IsLocked();
        private static bool IsLocked(this PlacedToken token) => token is not null && token.Scored;

    }
}
