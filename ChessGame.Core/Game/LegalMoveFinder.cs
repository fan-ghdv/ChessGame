using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class LegalMoveFinder
{
    public static bool HasAnyLegalMove(
        Board board,
        PieceColor color)
    {
        for (int row = 0;
             row < Board.Size;
             row++)
        {
            for (int column = 0;
                 column < Board.Size;
                 column++)
            {
                Position from =
                    new Position(row, column);

                Piece? piece =
                    board.GetPiece(from);

                if (piece == null ||
                    piece.Color != color)
                {
                    continue;
                }

                for (int targetRow = 0;
                     targetRow < Board.Size;
                     targetRow++)
                {
                    for (int targetColumn = 0;
                         targetColumn < Board.Size;
                         targetColumn++)
                    {
                        Position to =
                            new Position(
                                targetRow,
                                targetColumn
                            );

                        Move move =
                            new Move(
                                from,
                                to
                            );

                        if (MoveValidator.IsLegalMove(
                                board,
                                move,
                                color))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
}