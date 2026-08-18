namespace ChessGame.Core.Models;

public class Piece
{
    public PieceType Type { get; }

    public PieceColor Color { get; }

    public bool HasMoved { get; private set; }

    public Piece(
        PieceType type,
        PieceColor color)
    {
        Type = type;
        Color = color;
        HasMoved = false;
    }

    public void MarkAsMoved()
    {
        HasMoved = true;
    }
}