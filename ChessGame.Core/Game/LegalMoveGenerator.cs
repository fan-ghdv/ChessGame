using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class LegalMoveGenerator
{
    public static List<Move> GetLegalMoves(
        Board board,
        PieceColor color)
    {
        var legalMoves = new List<Move>();

        for (int row = 0; row < Board.Size; row++)
        {
            for (int column = 0; column < Board.Size; column++)
            {
                var from = new Position(row, column);

                Piece? piece = board.GetPiece(from);

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
                        var to =
                            new Position(
                                targetRow,
                                targetColumn
                            );

                        if (from == to)
                        {
                            continue;
                        }

                        var move = new Move(
                            from,
                            to
                        );

                        if (MoveValidator.IsLegalMove(
                                board,
                                move,
                                color))
                        {
                            legalMoves.Add(move);
                        }
                    }
                }
            }
        }

        return legalMoves;
    }
}