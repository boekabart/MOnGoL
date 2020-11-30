﻿using System.Collections.Immutable;

namespace MOnGoL.Common
{
    public record Coordinate(int X, int Y) { }
    public record Token(char Emoji) { }
    public record Change(Coordinate Coordinate, Token? Token) { }
    public record ChangeSet(IImmutableList<Change> Changes) { }
    public record Board(int Width, int Height, Token[,] Tokens)
    {
        public Board(int width, int height) : this(width, height, new Token[width, height]) { }
    }
    public record PlayerInfo(string Name, Token Token) {}
}
