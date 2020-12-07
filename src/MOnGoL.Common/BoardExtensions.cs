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
    }
}

