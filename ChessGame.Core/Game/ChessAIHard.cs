using System.Diagnostics;
using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class ChessAIHard
{
    private static readonly Random Random =
        new Random();
    private static long NodesSearched = 0;
    private static long TranspositionHits = 0;
    private static int CompletedDepth = 0;

    // =========================================================
    // TRANSPOSITION TABLE
    // =========================================================

    private static readonly Dictionary<
        string,
        TranspositionEntry
    > TranspositionTable =
        new Dictionary<
            string,
            TranspositionEntry
        >();

    // =========================================================
    // SEARCH DEPTH
    // =========================================================

    private const int SearchDepth = 4;

    private const int QuiescenceDepth = 2;

    private const int SearchTimeLimitMs = 2000;

    private const int CheckmateScore = 1000000;


    // =========================================================
    // TRANSPOSITION ENTRY
    // =========================================================

    private sealed class TranspositionEntry
    {
        public int Depth { get; }

        public int Score { get; }

        public TranspositionBound Bound { get; }

        public TranspositionEntry(
            int depth,
            int score,
            TranspositionBound bound)
        {
            Depth = depth;
            Score = score;
            Bound = bound;
        }
    }


// =========================================================
// TRANSPOSITION BOUND
// =========================================================

private enum TranspositionBound
{
    Exact,
    LowerBound,
    UpperBound
}

// =========================================================
// SEARCH TIMEOUT
// =========================================================

private sealed class SearchTimeoutException : Exception
{
}


    // =========================================================
    // GET BEST MOVE
    // =========================================================

    public static Move? GetBestMove(
        ChessGame game,
        PieceColor color)
    {
        if (game.SideToMove != color)
        {
            return null;
        }

    // =========================================================
    // RESET TRANSPOSITION TABLE
    // =========================================================

        TranspositionTable.Clear();

        NodesSearched = 0;
        TranspositionHits = 0;
        CompletedDepth = 0;

        Stopwatch stopwatch =
            Stopwatch.StartNew();

        long moveGenerationStart =
            stopwatch.ElapsedMilliseconds;

        List<Move> legalMoves =
            GetLegalMoves(
                game,
                color
            );

        Console.WriteLine(
            $"Hard AI legal move generation: " +
            $"{stopwatch.ElapsedMilliseconds - moveGenerationStart} ms"
        );

        if (legalMoves.Count == 0)
        {
            return null;
        }

        long orderingStart =
            stopwatch.ElapsedMilliseconds;

        legalMoves =
            OrderMoves(
                game,
                legalMoves,
                color
            );

        Console.WriteLine(
            $"Hard AI move ordering: " +
            $"{stopwatch.ElapsedMilliseconds - orderingStart} ms"
        );

        Move? bestMove = null;

        int bestScore = int.MinValue;


        // =========================================================
        // ITERATIVE DEEPENING
        // =========================================================

        Move? previousBestMove = null;

        for (
            int currentDepth = 1;
            currentDepth <= SearchDepth;
            currentDepth++)
        {
            if (stopwatch.ElapsedMilliseconds >=
                SearchTimeLimitMs)
            {
                break;
            }

            Move? currentBestMove = null;

            int currentBestScore =
                int.MinValue;


            // =====================================================
            // TRY PREVIOUS ITERATION BEST MOVE FIRST
            // =====================================================

            if (previousBestMove != null)
            {
                Move? pvMove =
                    legalMoves.FirstOrDefault(
                        m =>
                            m.From == previousBestMove.From &&
                            m.To == previousBestMove.To
                    );

                if (pvMove != null)
                {
                    legalMoves.Remove(pvMove);
                    legalMoves.Insert(0, pvMove);
                }
            }

            // -----------------------------------------------------
            // SEARCH EVERY MOVE
            // -----------------------------------------------------

            foreach (Move move in legalMoves)
            {
                if (stopwatch.ElapsedMilliseconds >=
                    SearchTimeLimitMs)
                {
                    break;
                }

                Piece? movingPiece =
                    game.Board.GetPiece(
                        move.From
                    );

                if (movingPiece == null)
                {
                    continue;
                }


                bool isPromotion =
                    movingPiece.Type == PieceType.Pawn &&
                    PawnPromotion.CanPromote(
                        move.To,
                        color
                    );


                bool success =
                    game.TryMoveForSearch(
                        move,
                        color,
                        isPromotion
                            ? PieceType.Queen
                            : null
                    );

                if (!success)
                {
                    continue;
                }


                PieceColor opponent =
                    color == PieceColor.White
                        ? PieceColor.Black
                        : PieceColor.White;


                    int score;

                    try
                    {
                        score =
                            Minimax(
                                game,
                                currentDepth - 1,
                                int.MinValue + 1,
                                int.MaxValue - 1,
                                opponent,
                                color,
                                stopwatch
                            );
                    }
                    catch (SearchTimeoutException)
                    {
                        game.UndoSearchMove();

                        Console.WriteLine(
                            $"Hard AI time limit reached at depth {currentDepth}."
                        );

                        break;
                    }

                    game.UndoSearchMove();

                score +=
                    Random.Next(
                        0,
                        5
                    );


                if (score > currentBestScore)
                {
                    currentBestScore =
                        score;

                    currentBestMove =
                        move;
                }
            }


            // -----------------------------------------------------
            // ONLY ACCEPT A COMPLETED ITERATION
            // -----------------------------------------------------

            if (currentBestMove != null)
            {
                bestMove =
                    currentBestMove;

                bestScore =
                    currentBestScore;

                previousBestMove =
                    currentBestMove;

                CompletedDepth =
                    currentDepth;
            }


            Console.WriteLine(
                $"Hard AI Depth {currentDepth}: " +
                $"{bestMove?.From} -> {bestMove?.To}, " +
                $"Score = {bestScore}"
            );
        }

        Console.WriteLine(
            $"Hard AI total search: " +
            $"{stopwatch.ElapsedMilliseconds} ms"
        );

        return bestMove;
    }


    // =========================================================
    // MINIMAX
    // =========================================================

    private static int Minimax(
        ChessGame game,
        int depth,
        int alpha,
        int beta,
        PieceColor sideToMove,
        PieceColor aiColor,
        Stopwatch stopwatch)
    {
        NodesSearched++;
        if (stopwatch.ElapsedMilliseconds >=
            SearchTimeLimitMs)
        {
            throw new SearchTimeoutException();
        }

        // =====================================================
        // SAVE ORIGINAL ALPHA / BETA
        // =====================================================

        int originalAlpha =
            alpha;

        int originalBeta =
            beta;


        // =====================================================
        // POSITION KEY
        // =====================================================

        string positionKey =
            PositionKeyGenerator.Generate(
                game.Board,
                sideToMove,
                game.LastMove
            );


        // =====================================================
        // TRANSPOSITION TABLE LOOKUP
        // =====================================================

        if (TranspositionTable.TryGetValue(
                positionKey,
                out TranspositionEntry? entry))
        {
            TranspositionHits++;
            if (entry.Depth >= depth)
            {
                if (entry.Bound ==
                    TranspositionBound.Exact)
                {
                    return entry.Score;
                }

                if (entry.Bound ==
                    TranspositionBound.LowerBound)
                {
                    alpha =
                        Math.Max(
                            alpha,
                            entry.Score
                        );
                }

                if (entry.Bound ==
                    TranspositionBound.UpperBound)
                {
                    beta =
                        Math.Min(
                            beta,
                            entry.Score
                        );
                }

                if (alpha >= beta)
                {
                    return entry.Score;
                }
            }
        }


        // =====================================================
        // GAME RESULT
        // =====================================================

        GameResult result =
            game.GetGameResult(
                sideToMove
            );


        if (result != GameResult.Ongoing)
        {
            return EvaluateGameResult(
                result,
                aiColor,
                depth
            );
        }


        // =====================================================
        // DEPTH LIMIT
        // =====================================================

        if (depth <= 0)
        {
            return QuiescenceSearch(
                game,
                int.MinValue + 1,
                int.MaxValue - 1,
                sideToMove,
                aiColor,
                QuiescenceDepth,
                stopwatch
            );
        }


        // =====================================================
        // LEGAL MOVES
        // =====================================================

        List<Move> legalMoves =
            GetLegalMoves(
                game,
                sideToMove
            );


        if (legalMoves.Count == 0)
        {
            return EvaluatePosition(
                game,
                aiColor
            );
        }

        legalMoves =
            OrderMoves(
                game,
                legalMoves,
                sideToMove
            );


        bool maximizing =
            sideToMove == aiColor;


        // =====================================================
        // MAXIMIZING
        // =====================================================

        if (maximizing)
        {
            int bestScore =
                int.MinValue;


            foreach (Move move in legalMoves)
            {
                Piece? movingPiece =
                    game.Board.GetPiece(
                        move.From
                    );

                if (movingPiece == null)
                {
                    continue;
                }


                bool isPromotion =
                    movingPiece.Type == PieceType.Pawn &&
                    PawnPromotion.CanPromote(
                        move.To,
                        sideToMove
                    );


                bool success =
                    game.TryMoveForSearch(
                        move,
                        sideToMove,
                        isPromotion
                            ? PieceType.Queen
                            : null
                    );


                if (!success)
                {
                    continue;
                }


                PieceColor nextSide =
                    sideToMove ==
                        PieceColor.White
                            ? PieceColor.Black
                            : PieceColor.White;


                            int score;

                            try
                            {
                                score =
                                    Minimax(
                                        game,
                                        depth - 1,
                                        alpha,
                                        beta,
                                        nextSide,
                                        aiColor,
                                        stopwatch
                                    );
                            }
                            finally
                            {
                                game.UndoSearchMove();
                            }


                bestScore =
                    Math.Max(
                        bestScore,
                        score
                    );


                alpha =
                    Math.Max(
                        alpha,
                        bestScore
                    );


                if (beta <= alpha)
                {
                    break;
                }
            }

            TranspositionBound bound;

            if (bestScore <= originalAlpha)
            {
                bound =
                    TranspositionBound.UpperBound;
            }
            else if (bestScore >= originalBeta)
            {
                bound =
                    TranspositionBound.LowerBound;
            }
            else
            {
                bound =
                    TranspositionBound.Exact;
            }

            TranspositionTable[positionKey] =
                new TranspositionEntry(
                    depth,
                    bestScore,
                    bound
                );

            return bestScore;
        }


        // =====================================================
        // MINIMIZING
        // =====================================================

        else
        {
            int bestScore =
                int.MaxValue;


            foreach (Move move in legalMoves)
            {
                Piece? movingPiece =
                    game.Board.GetPiece(
                        move.From
                    );

                if (movingPiece == null)
                {
                    continue;
                }


                bool isPromotion =
                    movingPiece.Type == PieceType.Pawn &&
                    PawnPromotion.CanPromote(
                        move.To,
                        sideToMove
                    );


                bool success =
                    game.TryMoveForSearch(
                        move,
                        sideToMove,
                        isPromotion
                            ? PieceType.Queen
                            : null
                    );


                if (!success)
                {
                    continue;
                }


                PieceColor nextSide =
                    sideToMove ==
                        PieceColor.White
                            ? PieceColor.Black
                            : PieceColor.White;


                            int score;

                            try
                            {
                                score =
                                    Minimax(
                                        game,
                                        depth - 1,
                                        alpha,
                                        beta,
                                        nextSide,
                                        aiColor,
                                        stopwatch
                                    );
                            }
                            finally
                            {
                                game.UndoSearchMove();
                            }


                bestScore =
                    Math.Min(
                        bestScore,
                        score
                    );


                beta =
                    Math.Min(
                        beta,
                        bestScore
                    );


                if (beta <= alpha)
                {
                    break;
                }
            }

            TranspositionBound bound;

            if (bestScore <= originalAlpha)
            {
                bound =
                    TranspositionBound.UpperBound;
            }
            else if (bestScore >= originalBeta)
            {
                bound =
                    TranspositionBound.LowerBound;
            }
            else
            {
                bound =
                    TranspositionBound.Exact;
            }

            TranspositionTable[positionKey] =
                new TranspositionEntry(
                    depth,
                    bestScore,
                    bound
                );

            return bestScore;
        }
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

        for (int fromRow = 0; fromRow < Board.Size; fromRow++)
        {
            for (int fromColumn = 0; fromColumn < Board.Size; fromColumn++)
            {
                Position from =
                    new Position(
                        fromRow,
                        fromColumn
                    );

                Piece? piece =
                    game.Board.GetPiece(from);

                if (piece == null ||
                    piece.Color != color)
                {
                    continue;
                }

                // =====================================================
                // ONLY CHECK SQUARES THAT THIS PIECE CAN ACTUALLY REACH
                // =====================================================

                switch (piece.Type)
                {
                    case PieceType.Pawn:
                        AddPawnMoves(
                            game,
                            color,
                            from,
                            legalMoves
                        );
                        break;

                    case PieceType.Knight:
                        AddKnightMoves(
                            game,
                            color,
                            from,
                            legalMoves
                        );
                        break;

                    case PieceType.Bishop:
                        AddSlidingMoves(
                            game,
                            color,
                            from,
                            legalMoves,
                            true,
                            false
                        );
                        break;

                    case PieceType.Rook:
                        AddSlidingMoves(
                            game,
                            color,
                            from,
                            legalMoves,
                            false,
                            true
                        );
                        break;

                    case PieceType.Queen:
                        AddSlidingMoves(
                            game,
                            color,
                            from,
                            legalMoves,
                            true,
                            true
                        );
                        break;

                    case PieceType.King:
                        AddKingMoves(
                            game,
                            color,
                            from,
                            legalMoves
                        );
                        break;
                }
            }
        }

        return legalMoves;
    }

    private static void AddPawnMoves(
        ChessGame game,
        PieceColor color,
        Position from,
        List<Move> legalMoves)
    {
        int direction =
            color == PieceColor.White
                ? -1
                : 1;

        // Forward one
        AddCandidateMove(
            game,
            color,
            from,
            new Position(
                from.Row + direction,
                from.Column
            ),
            legalMoves
        );

        // Forward two
        AddCandidateMove(
            game,
            color,
            from,
            new Position(
                from.Row + direction * 2,
                from.Column
            ),
            legalMoves
        );

        // Capture left
        AddCandidateMove(
            game,
            color,
            from,
            new Position(
                from.Row + direction,
                from.Column - 1
            ),
            legalMoves
        );

        // Capture right
        AddCandidateMove(
            game,
            color,
            from,
            new Position(
                from.Row + direction,
                from.Column + 1
            ),
            legalMoves
        );
    }

    private static void AddKnightMoves(
        ChessGame game,
        PieceColor color,
        Position from,
        List<Move> legalMoves)
    {
        int[,] offsets =
        {
            { -2, -1 },
            { -2,  1 },
            { -1, -2 },
            { -1,  2 },
            {  1, -2 },
            {  1,  2 },
            {  2, -1 },
            {  2,  1 }
        };

        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            AddCandidateMove(
                game,
                color,
                from,
                new Position(
                    from.Row + offsets[i, 0],
                    from.Column + offsets[i, 1]
                ),
                legalMoves
            );
        }
    }

    private static void AddKingMoves(
        ChessGame game,
        PieceColor color,
        Position from,
        List<Move> legalMoves)
    {
        for (int row = -1; row <= 1; row++)
        {
            for (int column = -1; column <= 1; column++)
            {
                if (row == 0 && column == 0)
                {
                    continue;
                }

                AddCandidateMove(
                    game,
                    color,
                    from,
                    new Position(
                        from.Row + row,
                        from.Column + column
                    ),
                    legalMoves
                );
            }
        }

        // Castling
        AddCandidateMove(
            game,
            color,
            from,
            new Position(
                from.Row,
                from.Column + 2
            ),
            legalMoves
        );

        AddCandidateMove(
            game,
            color,
            from,
            new Position(
                from.Row,
                from.Column - 2
            ),
            legalMoves
        );
    }

    private static void AddSlidingMoves(
        ChessGame game,
        PieceColor color,
        Position from,
        List<Move> legalMoves,
        bool diagonal,
        bool straight)
    {
        if (straight)
        {
            AddDirection(
                game,
                color,
                from,
                1,
                0,
                legalMoves
            );

            AddDirection(
                game,
                color,
                from,
                -1,
                0,
                legalMoves
            );

            AddDirection(
                game,
                color,
                from,
                0,
                1,
                legalMoves
            );

            AddDirection(
                game,
                color,
                from,
                0,
                -1,
                legalMoves
            );
        }

        if (diagonal)
        {
            AddDirection(
                game,
                color,
                from,
                1,
                1,
                legalMoves
            );

            AddDirection(
                game,
                color,
                from,
                1,
                -1,
                legalMoves
            );

            AddDirection(
                game,
                color,
                from,
                -1,
                1,
                legalMoves
            );

            AddDirection(
                game,
                color,
                from,
                -1,
                -1,
                legalMoves
            );
        }
    }

    private static void AddDirection(
        ChessGame game,
        PieceColor color,
        Position from,
        int rowDirection,
        int columnDirection,
        List<Move> legalMoves)
    {
        int row =
            from.Row + rowDirection;

        int column =
            from.Column + columnDirection;

        while (
            row >= 0 &&
            row < Board.Size &&
            column >= 0 &&
            column < Board.Size)
        {
            Position to =
                new Position(
                    row,
                    column
                );

            Piece? target =
                game.Board.GetPiece(to);

            if (target != null &&
                target.Color == color)
            {
                break;
            }

            AddCandidateMove(
                game,
                color,
                from,
                to,
                legalMoves
            );

            if (target != null)
            {
                break;
            }

            row += rowDirection;
            column += columnDirection;
        }
    }

    private static void AddCandidateMove(
        ChessGame game,
        PieceColor color,
        Position from,
        Position to,
        List<Move> legalMoves)
    {
        if (to.Row < 0 ||
            to.Row >= Board.Size ||
            to.Column < 0 ||
            to.Column >= Board.Size)
        {
            return;
        }

        Move move =
            new Move(
                from,
                to
            );

        Piece? piece =
            game.Board.GetPiece(from);

        if (piece == null)
        {
            return;
        }

        Piece? target =
            game.Board.GetPiece(to);

        if (target != null &&
            target.Color == color)
        {
            return;
        }

        bool isPromotion =
            piece.Type == PieceType.Pawn &&
            PawnPromotion.CanPromote(
                to,
                color
            );

        bool success =
            game.TryMoveForSearch(
                move,
                color,
                isPromotion
                    ? PieceType.Queen
                    : null
            );

        if (!success)
        {
            return;
        }

        game.UndoSearchMove();

        legalMoves.Add(move);
    }

    private static List<Move> OrderMoves(
        ChessGame game,
        List<Move> moves,
        PieceColor color)
    {
        List<(Move Move, int Score)> scoredMoves =
            new List<(Move Move, int Score)>();

        foreach (Move move in moves)
        {
            int score = 0;

            Piece? movingPiece =
                game.Board.GetPiece(move.From);

            Piece? capturedPiece =
                game.Board.GetPiece(move.To);

            if (movingPiece == null)
            {
                continue;
            }

            // =====================================================
            // CAPTURE VALUE
            // =====================================================

            if (capturedPiece != null)
            {
                int victimValue =
                    GetPieceValue(capturedPiece.Type);

                int attackerValue =
                    GetPieceValue(movingPiece.Type);

                // MVV-LVA:
                // Most Valuable Victim - Least Valuable Attacker

                score +=
                    victimValue * 10 -
                    attackerValue;
            }

            // =====================================================
            // PROMOTION
            // =====================================================

            if (movingPiece.Type == PieceType.Pawn &&
                PawnPromotion.CanPromote(
                    move.To,
                    color))
            {
                score += 9000;
            }

            // =====================================================
            // TRY MOVE
            // =====================================================

            bool success =
                game.TryMove(
                    move,
                    color,
                    movingPiece.Type == PieceType.Pawn &&
                    PawnPromotion.CanPromote(
                        move.To,
                        color)
                        ? PieceType.Queen
                        : null);

            if (!success)
            {
                continue;
            }

            PieceColor opponent =
                color == PieceColor.White
                    ? PieceColor.Black
                    : PieceColor.White;

            // =====================================================
            // CHECK
            // =====================================================

            if (CheckmateDetector.IsCheckmate(
                    game.Board,
                    opponent))
            {
                score += 1000000;
            }
            else if (CheckDetector.IsInCheck(
                        game.Board,
                        opponent))
            {
                score += 5000;
            }

            // =====================================================
            // UNDO
            // =====================================================

            game.UndoMove();

            scoredMoves.Add(
                (move, score)
            );
        }

        scoredMoves.Sort(
            (a, b) =>
                b.Score.CompareTo(a.Score)
        );

        return scoredMoves
            .Select(x => x.Move)
            .ToList();
    }


    // =========================================================
    // POSITION EVALUATION
    // =========================================================

    private static int EvaluatePosition(
        ChessGame game,
        PieceColor aiColor)
    {
        PieceColor opponent =
            aiColor == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

        int score = 0;


        // =========================================================
        // MATERIAL
        // =========================================================

        for (
            int row = 0;
            row < Board.Size;
            row++)
        {
            for (
                int column = 0;
                column < Board.Size;
                column++)
            {
                Piece? piece =
                    game.Board.GetPiece(
                        new Position(
                            row,
                            column
                        )
                    );

                if (piece == null)
                {
                    continue;
                }

                int value =
                    GetPieceValue(
                        piece.Type
                    );

                if (piece.Color == aiColor)
                {
                    score += value;
                }
                else
                {
                    score -= value;
                }
            }
        }


        // =========================================================
        // CENTER CONTROL
        // =========================================================

        score +=
            EvaluateCenterControl(
                game,
                aiColor
            );


        // =========================================================
        // PIECE ACTIVITY
        // =========================================================

        score +=
            EvaluatePieceActivity(
                game,
                aiColor
            );


        // =========================================================
        // PAWN STRUCTURE
        // =========================================================

        score +=
            EvaluatePawnStructure(
                game,
                aiColor
            );


        // =========================================================
        // PASSED PAWNS
        // =========================================================

        score +=
            EvaluatePassedPawns(
                game,
                aiColor
            );


        // =========================================================
        // KING SAFETY
        // =========================================================

        score +=
            EvaluateKingSafety(
                game,
                aiColor
            );


        // =========================================================
        // CHECK
        // =========================================================

        if (CheckDetector.IsInCheck(
                game.Board,
                aiColor))
        {
            score -= 80;
        }

        if (CheckDetector.IsInCheck(
                game.Board,
                opponent))
        {
            score += 80;
        }


        return score;
    }

    private static int EvaluateCenterControl(
        ChessGame game,
        PieceColor aiColor)
    {
        PieceColor opponent =
            aiColor == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

        Position[] centerSquares =
        {
            new Position(3, 3), // d5
            new Position(3, 4), // e5
            new Position(4, 3), // d4
            new Position(4, 4)  // e4
        };

        int score = 0;

        foreach (Position position in centerSquares)
        {
            Piece? piece =
                game.Board.GetPiece(
                    position
                );

            if (piece == null)
            {
                continue;
            }

            if (piece.Color == aiColor)
            {
                score += 25;
            }
            else
            {
                score -= 25;
            }
        }

        return score;
    }

    private static int EvaluatePieceActivity(
        ChessGame game,
        PieceColor aiColor)
    {
        PieceColor opponent =
            aiColor == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

        int score = 0;

        for (
            int row = 0;
            row < Board.Size;
            row++)
        {
            for (
                int column = 0;
                column < Board.Size;
                column++)
            {
                Position position =
                    new Position(
                        row,
                        column
                    );

                Piece? piece =
                    game.Board.GetPiece(
                        position
                    );

                if (piece == null)
                {
                    continue;
                }

                int activity =
                    GetActivityBonus(
                        piece.Type,
                        row,
                        column
                    );

                if (piece.Color == aiColor)
                {
                    score += activity;
                }
                else
                {
                    score -= activity;
                }
            }
        }

        return score;
    }

    private static int GetActivityBonus(
        PieceType type,
        int row,
        int column)
    {
        int centerDistance =
            Math.Abs(
                row - 3
            )
            +
            Math.Abs(
                column - 3
            );

        return type switch
        {
            PieceType.Knight =>
                Math.Max(
                    0,
                    35 - centerDistance * 6
                ),

            PieceType.Bishop =>
                Math.Max(
                    0,
                    25 - centerDistance * 4
                ),

            PieceType.Queen =>
                Math.Max(
                    0,
                    12 - centerDistance * 2
                ),

            PieceType.Rook =>
                column == 0 ||
                column == 7
                    ? 5
                    : 10,

            _ => 0
        };
    }

    private static int EvaluatePawnStructure(
        ChessGame game,
        PieceColor aiColor)
    {
        int score = 0;

        score +=
            EvaluatePawnStructureForColor(
                game,
                aiColor
            );

        PieceColor opponent =
            aiColor == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

        score -=
            EvaluatePawnStructureForColor(
                game,
                opponent
            );

        return score;
    }

    private static int EvaluatePawnStructureForColor(
        ChessGame game,
        PieceColor color)
    {
        int score = 0;

        int[] fileCounts =
            new int[Board.Size];


        for (
            int row = 0;
            row < Board.Size;
            row++)
        {
            for (
                int column = 0;
                column < Board.Size;
                column++)
            {
                Piece? piece =
                    game.Board.GetPiece(
                        new Position(
                            row,
                            column
                        )
                    );

                if (piece != null &&
                    piece.Type == PieceType.Pawn &&
                    piece.Color == color)
                {
                    fileCounts[column]++;
                }
            }
        }


        // =========================================================
        // DOUBLED PAWNS
        // =========================================================

        for (
            int column = 0;
            column < Board.Size;
            column++)
        {
            if (fileCounts[column] > 1)
            {
                score -=
                    (fileCounts[column] - 1)
                    * 20;
            }
        }


        // =========================================================
        // ISOLATED PAWNS
        // =========================================================

        for (
            int column = 0;
            column < Board.Size;
            column++)
        {
            if (fileCounts[column] == 0)
            {
                continue;
            }

            bool hasLeftPawn =
                column > 0 &&
                fileCounts[column - 1] > 0;

            bool hasRightPawn =
                column < Board.Size - 1 &&
                fileCounts[column + 1] > 0;

            if (!hasLeftPawn &&
                !hasRightPawn)
            {
                score -= 15;
            }
        }


        return score;
    }

    private static int EvaluatePassedPawns(
        ChessGame game,
        PieceColor aiColor)
    {
        int score = 0;

        PieceColor opponent =
            aiColor == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

        score +=
            EvaluatePassedPawnsForColor(
                game,
                aiColor
            );

        score -=
            EvaluatePassedPawnsForColor(
                game,
                opponent
            );

        return score;
    }

    private static int EvaluatePassedPawnsForColor(
        ChessGame game,
        PieceColor color)
    {
        int score = 0;

        for (
            int row = 0;
            row < Board.Size;
            row++)
        {
            for (
                int column = 0;
                column < Board.Size;
                column++)
            {
                Piece? pawn =
                    game.Board.GetPiece(
                        new Position(
                            row,
                            column
                        )
                    );

                if (pawn == null ||
                    pawn.Type != PieceType.Pawn ||
                    pawn.Color != color)
                {
                    continue;
                }

                if (!IsPassedPawn(
                        game,
                        row,
                        column,
                        color))
                {
                    continue;
                }

                int advancement =
                    color == PieceColor.White
                        ? 6 - row
                        : row - 1;

                score +=
                    40 +
                    advancement * 15;
            }
        }

        return score;
    }

    private static bool IsPassedPawn(
        ChessGame game,
        int row,
        int column,
        PieceColor color)
    {
        PieceColor opponent =
            color == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

        int direction =
            color == PieceColor.White
                ? -1
                : 1;

        int currentRow =
            row + direction;


        while (
            currentRow >= 0 &&
            currentRow < Board.Size)
        {
            for (
                int file =
                    Math.Max(0, column - 1);

                file <=
                    Math.Min(
                        Board.Size - 1,
                        column + 1
                    );

                file++)
            {
                Piece? piece =
                    game.Board.GetPiece(
                        new Position(
                            currentRow,
                            file
                        )
                    );

                if (piece != null &&
                    piece.Type == PieceType.Pawn &&
                    piece.Color == opponent)
                {
                    return false;
                }
            }

            currentRow += direction;
        }

        return true;
    }

    private static int EvaluateKingSafety(
        ChessGame game,
        PieceColor aiColor)
    {
        PieceColor opponent =
            aiColor == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

        int score = 0;

        Position? aiKing =
            FindKing(
                game,
                aiColor
            );

        Position? opponentKing =
            FindKing(
                game,
                opponent
            );


        // =========================================================
        // KING EXISTS
        // =========================================================

        if (aiKing == null)
        {
            score -= 10000;
        }

        if (opponentKing == null)
        {
            score += 10000;
        }


        // =========================================================
        // KING ACTIVITY
        // =========================================================

        if (aiKing != null)
        {
            if (aiKing.Value.Row >= 2 &&
                aiKing.Value.Row <= 5 &&
                aiKing.Value.Column >= 2 &&
                aiKing.Value.Column <= 5)
            {
                if (game.MoveHistory.Count < 20)
                {
                    score -= 25;
                }
            }
        }


        return score;
    }

    private static Position? FindKing(
        ChessGame game,
        PieceColor color)
    {
        for (
            int row = 0;
            row < Board.Size;
            row++)
        {
            for (
                int column = 0;
                column < Board.Size;
                column++)
            {
                Piece? piece =
                    game.Board.GetPiece(
                        new Position(
                            row,
                            column
                        )
                    );

                if (piece != null &&
                    piece.Type == PieceType.King &&
                    piece.Color == color)
                {
                    return new Position(
                        row,
                        column
                    );
                }
            }
        }

        return null;
    }


    // =========================================================
    // GAME RESULT EVALUATION
    // =========================================================

    private static int EvaluateGameResult(
        GameResult result,
        PieceColor aiColor,
        int depth)
    {
        if (result == GameResult.WhiteWins)
        {
            return aiColor == PieceColor.White
                ? CheckmateScore + depth
                : -CheckmateScore - depth;
        }


        if (result == GameResult.BlackWins)
        {
            return aiColor == PieceColor.Black
                ? CheckmateScore + depth
                : -CheckmateScore - depth;
        }

        return 0;
    }


    // =========================================================
    // PIECE VALUE
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
    // CENTER BONUS
    // =========================================================

    private static int GetCenterBonus(
        int row,
        int column)
    {
        int distance =
            Math.Abs(
                row - 3
            )
            +
            Math.Abs(
                column - 3
            );


        return Math.Max(
            0,
            20 - distance * 5
        );
    }

    private static int QuiescenceSearch(
        ChessGame game,
        int alpha,
        int beta,
        PieceColor sideToMove,
        PieceColor aiColor,
        int depth,
        Stopwatch stopwatch)
    {
        if (stopwatch.ElapsedMilliseconds >=
            SearchTimeLimitMs)
        {
            throw new SearchTimeoutException();
        }
        int standPat =
            EvaluatePosition(
                game,
                aiColor
            );

        // =====================================================
        // DEPTH LIMIT
        // =====================================================

        if (depth <= 0)
        {
            return standPat;
        }

        // =====================================================
        // MAXIMIZING SIDE
        // =====================================================

        bool maximizing =
            sideToMove == aiColor;

        if (maximizing)
        {
            if (standPat >= beta)
            {
                return standPat;
            }

            alpha =
                Math.Max(
                    alpha,
                    standPat
                );
        }
        else
        {
            if (standPat <= alpha)
            {
                return standPat;
            }

            beta =
                Math.Min(
                    beta,
                    standPat
                );
        }

        // =====================================================
        // ONLY TACTICAL MOVES
        // =====================================================

        List<Move> tacticalMoves =
            GetTacticalMoves(
                game,
                sideToMove
            );

        tacticalMoves =
            OrderMoves(
                game,
                tacticalMoves,
                sideToMove
            );

        // =====================================================
        // SEARCH
        // =====================================================

        foreach (Move move in tacticalMoves)
        {
            Piece? movingPiece =
                game.Board.GetPiece(
                    move.From
                );

            if (movingPiece == null)
            {
                continue;
            }

            bool isPromotion =
                movingPiece.Type == PieceType.Pawn &&
                PawnPromotion.CanPromote(
                    move.To,
                    sideToMove
                );

            bool success =
                game.TryMoveForSearch(
                    move,
                    sideToMove,
                    isPromotion
                        ? PieceType.Queen
                        : null
                );

            if (!success)
            {
                continue;
            }

            PieceColor nextSide =
                sideToMove == PieceColor.White
                    ? PieceColor.Black
                    : PieceColor.White;

            int score;

            try
            {
                score =
                    QuiescenceSearch(
                        game,
                        alpha,
                        beta,
                        nextSide,
                        aiColor,
                        depth - 1,
                        stopwatch
                    );
            }
            finally
            {
                game.UndoSearchMove();
            }

            if (maximizing)
            {
                if (score > standPat)
                {
                    standPat = score;
                }

                alpha =
                    Math.Max(
                        alpha,
                        standPat
                    );

                if (alpha >= beta)
                {
                    break;
                }
            }
            else
            {
                if (score < standPat)
                {
                    standPat = score;
                }

                beta =
                    Math.Min(
                        beta,
                        standPat
                    );

                if (alpha >= beta)
                {
                    break;
                }
            }
        }

        return standPat;
    }

    private static List<Move> GetTacticalMoves(
        ChessGame game,
        PieceColor color)
    {
        List<Move> tacticalMoves =
            new List<Move>();

        List<Move> legalMoves =
            GetLegalMoves(
                game,
                color
            );

        foreach (Move move in legalMoves)
        {
            Piece? movingPiece =
                game.Board.GetPiece(
                    move.From
                );

            if (movingPiece == null)
            {
                continue;
            }

            Piece? capturedPiece =
                game.Board.GetPiece(
                    move.To
                );

            // =====================================================
            // CAPTURE
            // =====================================================

            if (capturedPiece != null)
            {
                tacticalMoves.Add(move);
                continue;
            }

            // =====================================================
            // PROMOTION
            // =====================================================

            if (movingPiece.Type == PieceType.Pawn &&
                PawnPromotion.CanPromote(
                    move.To,
                    color
                ))
            {
                tacticalMoves.Add(move);
                continue;
            }

            // =====================================================
            // CHECK
            // =====================================================

            bool success =
                game.TryMove(
                    move,
                    color,
                    movingPiece.Type == PieceType.Pawn &&
                    PawnPromotion.CanPromote(
                        move.To,
                        color)
                        ? PieceType.Queen
                        : null
                );

            if (!success)
            {
                continue;
            }

            PieceColor opponent =
                color == PieceColor.White
                    ? PieceColor.Black
                    : PieceColor.White;

            bool givesCheck =
                CheckDetector.IsInCheck(
                    game.Board,
                    opponent
                );

            game.UndoMove();

            if (givesCheck)
            {
                tacticalMoves.Add(move);
            }
        }

        return tacticalMoves;
    }
    public static string GetSearchStatistics()
    {
        return
            $"Depth: {CompletedDepth} | " +
            $"Nodes: {NodesSearched:N0} | " +
            $"TT Hits: {TranspositionHits:N0}";
    }
}