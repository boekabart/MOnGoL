using System;
using System.Collections.Generic;
using System.Linq;

namespace MOnGoL.Common
{
    public static class BoardExtensions
    {
        public static bool IsValidCoordinate(this Board aBoard, Coordinate coordinate) => coordinate.X >= 0 && coordinate.Y >= 0 && coordinate.X < aBoard.Width && coordinate.Y < aBoard.Height;
        public static Token? TokenAt(this Board aBoard, Coordinate coordinate) => aBoard.Tokens[coordinate.X + aBoard.Width * coordinate.Y];
        public static Board WithChanges(this Board aBoard, ChangeSet changes)
        {
            var newBoard = new Board(aBoard.Width, aBoard.Height, (Token[])aBoard.Tokens.Clone());
            foreach (var change in changes.Changes)
                newBoard.Tokens[change.Coordinate.X + aBoard.Width * change.Coordinate.Y] = change.Token;
            return newBoard;
        }

        public static IEnumerable<(Coordinate Coordinate, Token? Token)> GetRow(this Board aBoard, int rowNumber)
            => aBoard.Tokens[(rowNumber*aBoard.Width)..((rowNumber+1)*aBoard.Width)]
            .Select((t,i) => (new Coordinate(i, rowNumber),t));
        public static IEnumerable<(Coordinate Coordinate, Token? Token)> GetColumn(this Board aBoard, int columnNumber)
            => Enumerable.Range(0, aBoard.Height).Select(rowNumber => new Coordinate(columnNumber, rowNumber)).Select(coor => (coor, aBoard.TokenAt(coor)));
        public static IEnumerable<IEnumerable<(Coordinate Coordinate, Token? Token)>> GetRows(this Board aBoard)
            => Enumerable.Range(0, aBoard.Height).Select(rowNumber => aBoard.GetRow(rowNumber));
        public static IEnumerable<IEnumerable<(Coordinate Coordinate, Token? Token)>> GetColumns(this Board aBoard)
            => Enumerable.Range(0, aBoard.Width).Select(columnNumber => aBoard.GetColumn(columnNumber));
        public static IEnumerable<IEnumerable<(Coordinate Coordinate, Token? Token)>> GetRowsAndColumns(this Board aBoard)
            => aBoard.GetRows().Concat(aBoard.GetColumns());
        public static IEnumerable<(Coordinate Coordinate, Token? Token)> GetSlash(this Board aBoard, int startColumnNumber)
            => Enumerable.Range(0, aBoard.Height)
            .Select(index => new Coordinate(index + startColumnNumber, aBoard.Height - (index + 1)))
            .Where(coor => aBoard.IsValidCoordinate(coor))
            .Select(coor => (coor, aBoard.TokenAt(coor)));
        public static IEnumerable<IEnumerable<(Coordinate Coordinate, Token? Token)>> GetSlashes(this Board aBoard)
            => Enumerable.Range(1 - aBoard.Height, aBoard.Width + aBoard.Height - 1)
            .Select(startColumnNumber => aBoard.GetSlash(startColumnNumber));
        public static IEnumerable<(Coordinate Coordinate, Token? Token)> GetBackslash(this Board aBoard, int startColumnNumber)
            => Enumerable.Range(0, aBoard.Height)
            .Select(index =>new Coordinate(index + startColumnNumber, index))
            .Where(coor => aBoard.IsValidCoordinate(coor))
            .Select(coor =>(coor, aBoard.TokenAt(coor)));
        public static IEnumerable<IEnumerable<(Coordinate Coordinate, Token? Token)>> GetBackslashes(this Board aBoard)
            => Enumerable.Range(1-aBoard.Height, aBoard.Width + aBoard.Height - 1)
            .Select(startColumnNumber => aBoard.GetBackslash(startColumnNumber)); 
        public static IEnumerable<IEnumerable<(Coordinate Coordinate, Token? Token)>> GetRowsColumnsAndDiagonals(this Board aBoard)
            => aBoard.GetRowsAndColumns().Concat(aBoard.GetSlashes()).Concat(aBoard.GetBackslashes());

        public static IEnumerable<ArraySegment<T>> Window<T>(this IEnumerable<T> src, Func<T, T, bool> shouldStartNewWindow)
        {
            var array = src.ToArray();
            int start = 0;
            int index = 0;
            T prev = default;
            foreach (var item in array)
            {
                if (index > 0 && shouldStartNewWindow(item, prev))
                {
                    yield return array[start..index];
                    start = index;
                }
                prev = item;
                index++;
            }
            yield return array[start..];
        }

    }
}

