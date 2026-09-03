using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class ChessAINormal
{
    private static readonly Random Random = new Random();

    // =========================================================
    // PIECE VALUES
    // =========================================================

    private static int GetPieceValue(
        PieceType type)
    {
        return type switch
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
    // GET NORMAL MOVE
    // =========================================================

    public static Move? GetMove(
        ChessGame game,
        PieceColor color)
    {
        if (game.SideToMove != color)
        {
            return null;
        }

        List<Move> legalMoves =
            GetLegalMoves(
                game,
                color
            );

        if (legalMoves.Count == 0)
        {
            return null;
        }


        int bestScore =
            int.MinValue;

        List<Move> bestMoves =
            new List<Move>();


        foreach (Move move in legalMoves)
        {
            int score =
                EvaluateMove(
                    game,
                    move,
                    color
                );


            if (score > bestScore)
            {
                bestScore = score;

                bestMoves.Clear();

                bestMoves.Add(move);
            }
            else if (score == bestScore)
            {
                bestMoves.Add(move);
            }
        }


        if (bestMoves.Count == 0)
        {
            return legalMoves[
                Random.Next(
                    legalMoves.Count
                )
            ];
        }


        return bestMoves[
            Random.Next(
                bestMoves.Count
            )
        ];
    }


    // =========================================================
    // GET LEGAL MOVES
    // =========================================================

    private static List<Move> GetLegalMoves(
        ChessGame game,
        PieceColor color)
    {
        List<Move> legalMoves =
            new List<Move>();


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
                    game.Board.GetPiece(
                        from
                    );


                if (piece == null ||
                    piece.Color != color)
                {
                    continue;
                }


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
                                color
                            ))
                        {
                            legalMoves.Add(
                                move
                            );

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
                                color
                            ))
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
                                        promotionType
                                    ))
                                {
                                    legalMoves.Add(
                                        move
                                    );

                                    game.UndoMove();

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }


        return legalMoves;
    }


    // =========================================================
    // EVALUATE MOVE
    // =========================================================

    private static int EvaluateMove(
        ChessGame game,
        Move move,
        PieceColor color)
    {
        Piece? movingPiece =
            game.Board.GetPiece(
                move.From
            );


        if (movingPiece == null)
        {
            return int.MinValue;
        }


        Piece? capturedPiece =
            game.Board.GetPiece(
                move.To
            );


        int score = 0;


        // =========================================================
        // DIRECT CAPTURE VALUE
        // =========================================================

        if (capturedPiece != null)
        {
            score +=
                GetPieceValue(
                    capturedPiece.Type
                );
        }


        // =========================================================
        // PROMOTION VALUE
        // =========================================================

        if (movingPiece.Type ==
                PieceType.Pawn &&
            PawnPromotion.CanPromote(
                move.To,
                color
            ))
        {
            score += 900;
        }


        // =========================================================
        // MAKE MOVE
        // =========================================================

        if (!game.TryMove(
                move,
                color
            ))
        {
            return int.MinValue;
        }


        PieceColor opponent =
            color == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;


        // =========================================================
        // CHECK
        // =========================================================

        bool givesCheck =
            CheckDetector.IsInCheck(
                game.Board,
                opponent
            );


        if (givesCheck)
        {
            score += 500;
        }


        // =========================================================
        // CHECKMATE
        // =========================================================

        bool givesCheckmate =
            CheckmateDetector.IsCheckmate(
                game.Board,
                opponent
            );


        if (givesCheckmate)
        {
            score += 10000;
        }


        // =========================================================
        // PIECE SAFETY
        // =========================================================

        if (IsPieceImmediatelyCapturable(
            game,
            move.To,
            opponent
        ))
        {
            int movingPieceValue =
                GetPieceValue(
                    movingPiece.Type
                );

            int capturedPieceValue =
                capturedPiece != null
                    ? GetPieceValue(
                        capturedPiece.Type
                    )
                    : 0;


            // ---------------------------------------------------------
            // NORMAL EXCHANGE
            // ---------------------------------------------------------

            if (capturedPieceValue >=
                movingPieceValue * 70 / 100)
            {
                score -=
                    movingPieceValue / 10;
            }

            // ---------------------------------------------------------
            // BAD TRADE
            // ---------------------------------------------------------

            else
            {
                score -=
                    movingPieceValue / 2;
            }
        }


        // =========================================================
        // UNDO TEST MOVE
        // =========================================================

        game.UndoMove();


        // =========================================================
        // SMALL RANDOM FACTOR
        // =========================================================

        score +=
            Random.Next(
                0,
                10
            );


        return score;
    }


    // =========================================================
    // GET OPPONENT BEST CAPTURE
    // =========================================================

    private static int GetOpponentBestCaptureValue(
        ChessGame game,
        PieceColor opponent)
    {
        int bestCapture =
            0;


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


                Piece? attackingPiece =
                    game.Board.GetPiece(
                        from
                    );


                if (attackingPiece == null ||
                    attackingPiece.Color != opponent)
                {
                    continue;
                }


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


                        Piece? target =
                            game.Board.GetPiece(
                                to
                            );


                        if (target == null ||
                            target.Color == opponent)
                        {
                            continue;
                        }


                        Move captureMove =
                            new Move(
                                from,
                                to
                            );


                        if (game.TryMove(
                                captureMove,
                                opponent
                            ))
                        {
                            int value =
                                GetPieceValue(
                                    target.Type
                                );


                            if (value > bestCapture)
                            {
                                bestCapture =
                                    value;
                            }


                            game.UndoMove();
                        }
                    }
                }
            }
        }


        return bestCapture;
    }


    // =========================================================
    // CHECK IMMEDIATE CAPTURE
    // =========================================================

    private static bool IsPieceImmediatelyCapturable(
        ChessGame game,
        Position target,
        PieceColor opponent)
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
                    new Position(
                        row,
                        column
                    );


                Piece? piece =
                    game.Board.GetPiece(
                        from
                    );


                if (piece == null ||
                    piece.Color != opponent)
                {
                    continue;
                }


                Move captureAttempt =
                    new Move(
                        from,
                        target
                    );


                if (game.TryMove(
                        captureAttempt,
                        opponent
                    ))
                {
                    game.UndoMove();

                    return true;
                }
            }
        }


        return false;
    }
}