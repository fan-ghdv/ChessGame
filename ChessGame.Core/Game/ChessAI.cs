using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class ChessAI
{
private static readonly Random Random =
new Random();

// =========================================================
// PIECE VALUES
// =========================================================

private static int GetPieceValue(
    PieceType pieceType)
{
    return pieceType switch
    {
        PieceType.Pawn => 100,
        PieceType.Knight => 320,
        PieceType.Bishop => 330,
        PieceType.Rook => 500,
        PieceType.Queen => 900,
        PieceType.King => 20000,

        _ => 0
    };
}

// =========================================================
// GET EASY MOVE
// =========================================================

public static Move? GetRandomMove(
    ChessGame game,
    PieceColor color)
{
    // =====================================================
    // CHECK TURN
    // =====================================================

    if (game.SideToMove != color)
    {
        return null;
    }

    List<Move> legalMoves =
        new List<Move>();

    // =====================================================
    // CHECK EVERY SQUARE
    // =====================================================

    for (int fromRow = 0;
         fromRow < Board.Size;
         fromRow++)
    {
        for (int fromColumn = 0;
             fromColumn < Board.Size;
             fromColumn++)
        {
            Position from =
                new Position(
                    fromRow,
                    fromColumn
                );

            Piece? piece =
                game.Board.GetPiece(from);

            // -------------------------------------------------
            // EMPTY
            // -------------------------------------------------

            if (piece == null)
            {
                continue;
            }

            // -------------------------------------------------
            // WRONG COLOR
            // -------------------------------------------------

            if (piece.Color != color)
            {
                continue;
            }

            // =================================================
            // TRY EVERY DESTINATION
            // =================================================

            for (int toRow = 0;
                 toRow < Board.Size;
                 toRow++)
            {
                for (int toColumn = 0;
                     toColumn < Board.Size;
                     toColumn++)
                {
                    Position to =
                        new Position(
                            toRow,
                            toColumn
                        );

                    if (from == to)
                    {
                        continue;
                    }

                    Move move =
                        new Move(
                            from,
                            to
                        );

                    // =================================================
                    // NORMAL MOVE
                    // =================================================

                    if (game.TryMove(
                            move,
                            color))
                    {
                        legalMoves.Add(move);

                        game.UndoMove();

                        continue;
                    }

                    // =================================================
                    // PROMOTION
                    // =================================================

                    if (piece.Type ==
                        PieceType.Pawn &&
                        PawnPromotion.CanPromote(
                            to,
                            color))
                    {
                        PieceType[] promotionTypes =
                        {
                            PieceType.Queen,
                            PieceType.Rook,
                            PieceType.Bishop,
                            PieceType.Knight
                        };

                        foreach (
                            PieceType promotionType
                            in promotionTypes)
                        {
                            if (game.TryMove(
                                    move,
                                    color,
                                    promotionType))
                            {
                                legalMoves.Add(move);

                                game.UndoMove();

                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    // =========================================================
    // NO LEGAL MOVES
    // =========================================================

    if (legalMoves.Count == 0)
    {
        return null;
    }

    // =========================================================
    // EASY AI
    // =========================================================

    if (Random.NextDouble() < 0.25)
    {
        return legalMoves[
            Random.Next(
                legalMoves.Count
            )
        ];
    }

    // =========================================================
    // FIND CAPTURES
    // =========================================================

    List<(Move move, int value)> captures =
        new List<(Move move, int value)>();

    foreach (Move move in legalMoves)
    {
        Piece? target =
            game.Board.GetPiece(
                move.To
            );

        if (target == null)
        {
            continue;
        }

        if (target.Color == color)
        {
            continue;
        }

        captures.Add(
            (
                move,
                GetPieceValue(
                    target.Type
                )
            )
        );
    }

    // =========================================================
    // IF THERE ARE CAPTURES
    // =========================================================

    if (captures.Count > 0)
    {
        // -----------------------------------------------------
        // Sort by captured piece value.
        // -----------------------------------------------------

        captures =
            captures
                .OrderByDescending(
                    x => x.value
                )
                .ToList();

        // -----------------------------------------------------
        // Easy AI does NOT always take the most valuable piece.
        //
        // Pick from the top few captures.
        // -----------------------------------------------------

        int candidateCount =
            Math.Min(
                3,
                captures.Count
            );

        int index =
            Random.Next(
                candidateCount
            );

        return captures[index].move;
    }

    // =========================================================
    // NO CAPTURE
    //
    // Pick a random legal move.
    // =========================================================

    return legalMoves[
        Random.Next(
            legalMoves.Count
        )
    ];
}
}