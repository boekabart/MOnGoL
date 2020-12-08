using System.Collections.Immutable;

namespace MOnGoL.Common
{
    public record Coordinate(int X, int Y) { }
    public record Token(string Emoji) { }
    public record PlacedToken(string Emoji, bool Scored) : Token(Emoji) { }
    public record Change(Coordinate Coordinate, PlacedToken? Token) { }
    public record ChangeSet(IImmutableList<Change> Changes) { }
    public record Board(int Width, int Height, PlacedToken[] Tokens)
    {
        public static Board Create(int width, int height) => new Board(width, height, new PlacedToken[height * width]);
    }
    public record PlayerInfo(string Name, Token Token) {}
    public record PlayerState(PlayerInfo PlayerInfo, int Score) { }
}
