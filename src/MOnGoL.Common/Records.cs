using System.Collections.Immutable;
using System.Text;

namespace MOnGoL.Common
{
    public record Coordinate(int X, int Y) { }
    public record Token(string Emoji) { }
    public record Change(Coordinate Coordinate, Token? Token) { }
    public record ChangeSet(IImmutableList<Change> Changes) { }
    public record Board(int Width, int Height, Token[] Tokens)
    {
        public static Board Create(int width, int height) => new Board(width, height, new Token[height * width]);
    }
    public record PlayerInfo(string Name, Token Token) {}
}
