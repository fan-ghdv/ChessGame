namespace ChessGame.Core.Models;

public class Board
{
    public const int Size = 8;

    private readonly Piece?[,] squares =
        new Piece?[Size, Size];

    public int HalfmoveClock { get; private set; }

    public int FullmoveNumber { get; private set; }

    public Board()
    {
        HalfmoveClock = 0;
        FullmoveNumber = 1;
    }

    public Piece? GetPiece(
        Position position)
    {
        return squares[
            position.Row,
            position.Column
        ];
    }

    public void SetPiece(
        Position position,
        Piece? piece)
    {
        squares[
            position.Row,
            position.Column
        ] = piece;
    }

    public void Clear()
    {
        for (int row = 0; row < Size; row++)
        {
            for (int column = 0; column < Size; column++)
            {
                squares[row, column] = null;
            }
        }

        HalfmoveClock = 0;
        FullmoveNumber = 1;
    }

    public void ResetHalfmoveClock()
    {
        HalfmoveClock = 0;
    }

    public void IncrementHalfmoveClock()
    {
        HalfmoveClock++;
    }

    public void RestoreHalfmoveClock(
        int value)
    {
        HalfmoveClock = value;
    }

    public void IncrementFullmoveNumber()
    {
        FullmoveNumber++;
    }

    public void RestoreFullmoveNumber(
        int value)
    {
        FullmoveNumber = value;
    }
}