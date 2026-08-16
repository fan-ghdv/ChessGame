using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public class Board
{
    public const int Size = 8;

    private readonly Piece?[,] squares = new Piece?[Size, Size];

    public Piece? GetPiece(Position position)
    {
        return squares[position.Row, position.Column];
    }

    public void SetPiece(Position position, Piece? piece)
    {
        squares[position.Row, position.Column] = piece;
    }

    public void Clear()
    {
        Array.Clear(squares, 0, squares.Length);
    }
}