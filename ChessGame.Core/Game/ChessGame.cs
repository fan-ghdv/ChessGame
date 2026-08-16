using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public class ChessGame
{
    public Board Board { get; }

    public ChessGame()
    {
        Board = new Board();
        SetupInitialPosition();
    }

    private void SetupInitialPosition()
    {
        Board.Clear();

        SetupBackRank(PieceColor.White, 7);
        SetupPawns(PieceColor.White, 6);

        SetupBackRank(PieceColor.Black, 0);
        SetupPawns(PieceColor.Black, 1);
    }

    private void SetupBackRank(PieceColor color, int row)
    {
        Board.SetPiece(
            new Position(row, 0),
            new Piece(PieceType.Rook, color)
        );

        Board.SetPiece(
            new Position(row, 1),
            new Piece(PieceType.Knight, color)
        );

        Board.SetPiece(
            new Position(row, 2),
            new Piece(PieceType.Bishop, color)
        );

        Board.SetPiece(
            new Position(row, 3),
            new Piece(PieceType.Queen, color)
        );

        Board.SetPiece(
            new Position(row, 4),
            new Piece(PieceType.King, color)
        );

        Board.SetPiece(
            new Position(row, 5),
            new Piece(PieceType.Bishop, color)
        );

        Board.SetPiece(
            new Position(row, 6),
            new Piece(PieceType.Knight, color)
        );

        Board.SetPiece(
            new Position(row, 7),
            new Piece(PieceType.Rook, color)
        );
    }

    private void SetupPawns(PieceColor color, int row)
    {
        for (int column = 0; column < Board.Size; column++)
        {
            Board.SetPiece(
                new Position(row, column),
                new Piece(PieceType.Pawn, color)
            );
        }
    }

    public int CountPieces()
    {
        int count = 0;

        for (int row = 0; row < Board.Size; row++)
        {
            for (int column = 0; column < Board.Size; column++)
            {
                if (Board.GetPiece(new Position(row, column)) != null)
                {
                    count++;
                }
            }
        }

        return count;
    }
}