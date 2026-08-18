using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class CheckDetector
{
    public static bool IsInCheck(
        Board board,
        PieceColor color)
    {
        Position? kingPosition = FindKing(
            board,
            color
        );

        if (kingPosition == null)
        {
            return false;
        }

        PieceColor enemyColor =
            color == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

        return AttackDetector.IsSquareAttacked(
            board,
            kingPosition.Value,
            enemyColor
        );
    }

    private static Position? FindKing(
        Board board,
        PieceColor color)
    {
        for (int row = 0; row < Board.Size; row++)
        {
            for (int column = 0; column < Board.Size; column++)
            {
                Position position =
                    new Position(row, column);

                Piece? piece =
                    board.GetPiece(position);

                if (piece != null &&
                    piece.Type == PieceType.King &&
                    piece.Color == color)
                {
                    return position;
                }
            }
        }

        return null;
    }
}