using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public class ChessGame
{
    // =========================================================
    // BOARD
    // =========================================================

    public Board Board { get; }

    // =========================================================
    // LAST MOVE
    // =========================================================

    public Move? LastMove { get; private set; }

    public MoveHistory MoveHistory { get; }

    // =========================================================
    // SIDE TO MOVE
    // =========================================================

    public PieceColor SideToMove
    {
        get;
        private set;
    }

    // =========================================================
    // THREEFOLD REPETITION
    // =========================================================

    private readonly Dictionary<string, int> positionCounts =
        new Dictionary<string, int>();

    private readonly Stack<SearchUndoState> searchUndoStack =
        new Stack<SearchUndoState>();

    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public ChessGame()
    {
        Board = new Board();

        LastMove = null;

        MoveHistory = new MoveHistory();

        SideToMove = PieceColor.White;

        SetupInitialPosition();

        // Record the initial position.
        RecordCurrentPosition();
    }

    // =========================================================
    // LOAD FROM FEN
    // =========================================================

    public static ChessGame FromFen(string fen)
    {
        // =========================================================
        // PARSE FEN
        // =========================================================

        FenState state =
            FenParser.Parse(fen);

        // =========================================================
        // CREATE GAME
        // =========================================================

        ChessGame game =
            new ChessGame();

        // =========================================================
        // RESTORE BOARD
        // =========================================================

        game.Board.Clear();

        for (int row = 0; row < Board.Size; row++)
        {
            for (int column = 0; column < Board.Size; column++)
            {
                Position position =
                    new Position(
                        row,
                        column
                    );

                Piece? piece =
                    state.Board.GetPiece(position);

                if (piece != null)
                {
                    game.Board.SetPiece(
                        position,
                        piece
                    );
                }
            }
        }

        // =========================================================
        // RESTORE SIDE TO MOVE
        // =========================================================

        game.SideToMove =
            state.SideToMove;

        // =========================================================
        // RESTORE HALF-MOVE CLOCK
        // =========================================================

        game.Board.RestoreHalfmoveClock(
            state.HalfmoveClock
        );

        // =========================================================
        // RESTORE FULL-MOVE NUMBER
        // =========================================================

        game.Board.RestoreFullmoveNumber(
            state.FullmoveNumber
        );

        // =========================================================
        // RESTORE LAST MOVE
        // =========================================================

        if (state.EnPassantTarget == null)
        {
            game.LastMove = null;
        }
        else
        {
            string enPassantTarget =
                PositionToFenSquare(
                    state.EnPassantTarget.Value
                );

            game.LastMove =
                CreateLastMoveFromEnPassant(
                    enPassantTarget,
                    game.SideToMove
                );
        }

        // =========================================================
        // RESET REPETITION STATE
        // =========================================================

        game.positionCounts.Clear();

        game.RecordCurrentPosition();

        return game;
    }

    private static string PositionToFenSquare(
        Position position)
    {
        char file =
            (char)('a' + position.Column);

        char rank =
            (char)('8' - position.Row);

        return $"{file}{rank}";
    }

    // =========================================================
    // SET SIDE TO MOVE
    // =========================================================

    public void SetSideToMove(
        PieceColor color)
    {
        SideToMove = color;
    }

    // =========================================================
    // INITIAL POSITION
    // =========================================================

    private void SetupInitialPosition()
    {
        Board.Clear();

        SetupBackRank(
            PieceColor.White,
            7
        );

        SetupPawns(
            PieceColor.White,
            6
        );

        SetupBackRank(
            PieceColor.Black,
            0
        );

        SetupPawns(
            PieceColor.Black,
            1
        );
    }

    // =========================================================
    // SETUP BACK RANK
    // =========================================================

    private void SetupBackRank(
        PieceColor color,
        int row)
    {
        // Rook
        Board.SetPiece(
            new Position(row, 0),
            new Piece(
                PieceType.Rook,
                color
            )
        );

        // Knight
        Board.SetPiece(
            new Position(row, 1),
            new Piece(
                PieceType.Knight,
                color
            )
        );

        // Bishop
        Board.SetPiece(
            new Position(row, 2),
            new Piece(
                PieceType.Bishop,
                color
            )
        );

        // Queen
        Board.SetPiece(
            new Position(row, 3),
            new Piece(
                PieceType.Queen,
                color
            )
        );

        // King
        Board.SetPiece(
            new Position(row, 4),
            new Piece(
                PieceType.King,
                color
            )
        );

        // Bishop
        Board.SetPiece(
            new Position(row, 5),
            new Piece(
                PieceType.Bishop,
                color
            )
        );

        // Knight
        Board.SetPiece(
            new Position(row, 6),
            new Piece(
                PieceType.Knight,
                color
            )
        );

        // Rook
        Board.SetPiece(
            new Position(row, 7),
            new Piece(
                PieceType.Rook,
                color
            )
        );
    }

    // =========================================================
    // SETUP PAWNS
    // =========================================================

    private void SetupPawns(
        PieceColor color,
        int row)
    {
        for (
            int column = 0;
            column < Board.Size;
            column++
        )
        {
            Board.SetPiece(
                new Position(row, column),
                new Piece(
                    PieceType.Pawn,
                    color
                )
            );
        }
    }

    public bool TryMoveForSearch(
        Move move,
        PieceColor color,
        PieceType? promotionType = null)
    {
        if (color != SideToMove)
        {
            return false;
        }

        Piece? movedPiece =
            Board.GetPiece(move.From);

        if (movedPiece == null ||
            movedPiece.Color != color)
        {
            return false;
        }

        Piece? capturedPiece =
            Board.GetPiece(move.To);

        Position? capturedPosition =
            capturedPiece != null
                ? move.To
                : null;

        bool previousMovedState =
            movedPiece.HasMoved;

        int previousHalfmoveClock =
            Board.HalfmoveClock;

        int previousFullmoveNumber =
            Board.FullmoveNumber;

        PieceColor previousSideToMove =
            SideToMove;

        Move? previousLastMove =
            LastMove;

        bool wasCastling = false;

        Position? rookFrom = null;
        Position? rookTo = null;
        Piece? rook = null;

        bool rookPreviousMovedState = false;

        // =========================================================
        // EN PASSANT
        // =========================================================

        if (movedPiece.Type == PieceType.Pawn &&
            capturedPiece == null &&
            move.From.Column != move.To.Column)
        {
            Position possibleCapturedPosition =
                new Position(
                    move.From.Row,
                    move.To.Column
                );

            Piece? possibleCapturedPawn =
                Board.GetPiece(
                    possibleCapturedPosition
                );

            if (possibleCapturedPawn != null &&
                possibleCapturedPawn.Type == PieceType.Pawn &&
                possibleCapturedPawn.Color != color)
            {
                capturedPiece =
                    possibleCapturedPawn;

                capturedPosition =
                    possibleCapturedPosition;
            }
        }

        // =========================================================
        // CASTLING
        // =========================================================

        if (movedPiece.Type == PieceType.King)
        {
            int row =
                color == PieceColor.White
                    ? 7
                    : 0;

            if (move.From == new Position(row, 4) &&
                move.To == new Position(row, 6))
            {
                Piece? possibleRook =
                    Board.GetPiece(
                        new Position(row, 7)
                    );

                if (possibleRook != null &&
                    possibleRook.Type == PieceType.Rook &&
                    possibleRook.Color == color)
                {
                    wasCastling = true;

                    rookFrom =
                        new Position(row, 7);

                    rookTo =
                        new Position(row, 5);

                    rook =
                        possibleRook;

                    rookPreviousMovedState =
                        possibleRook.HasMoved;
                }
            }
            else if (
                move.From == new Position(row, 4) &&
                move.To == new Position(row, 2))
            {
                Piece? possibleRook =
                    Board.GetPiece(
                        new Position(row, 0)
                    );

                if (possibleRook != null &&
                    possibleRook.Type == PieceType.Rook &&
                    possibleRook.Color == color)
                {
                    wasCastling = true;

                    rookFrom =
                        new Position(row, 0);

                    rookTo =
                        new Position(row, 3);

                    rook =
                        possibleRook;

                    rookPreviousMovedState =
                        possibleRook.HasMoved;
                }
            }
        }

        // =========================================================
        // PROMOTION
        // =========================================================

        bool isPromotion =
            movedPiece.Type == PieceType.Pawn &&
            PawnPromotion.CanPromote(
                move.To,
                color
            );

        if (!isPromotion &&
            promotionType != null)
        {
            return false;
        }

        // =========================================================
        // EXECUTE
        // =========================================================

        bool success =
            MoveExecutor.TryExecuteMove(
                Board,
                move,
                color,
                previousLastMove,
                promotionType
            );

        if (!success)
        {
            return false;
        }

        // =========================================================
        // UPDATE STATE
        // =========================================================

        LastMove = move;

        SideToMove =
            SideToMove == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

        if (color == PieceColor.Black)
        {
            Board.IncrementFullmoveNumber();
        }

        // =========================================================
        // STORE SEARCH UNDO STATE
        // =========================================================

        searchUndoStack.Push(
            new SearchUndoState(
                move,
                movedPiece,
                previousMovedState,
                capturedPiece,
                capturedPosition,
                wasCastling,
                rookFrom,
                rookTo,
                rook,
                rookPreviousMovedState,
                previousHalfmoveClock,
                previousFullmoveNumber,
                previousSideToMove,
                previousLastMove
            )
        );

        return true;
    }

    // =========================================================
    // MOVE
    // =========================================================

    public bool TryMove(
        Move move,
        PieceColor color,
        PieceType? promotionType = null)
    {
        // =========================================================
        // CHECK SIDE TO MOVE
        // =========================================================

        if (color != SideToMove)
        {
            return false;
        }

        // =========================================================
        // GET MOVED PIECE
        // =========================================================

        Piece? movedPiece =
            Board.GetPiece(move.From);

        if (movedPiece == null)
        {
            return false;
        }

        // =========================================================
        // SAVE PREVIOUS STATE
        // =========================================================

        bool previousMovedState =
            movedPiece.HasMoved;

        int previousHalfmoveClock =
            Board.HalfmoveClock;

        int previousFullmoveNumber =
            Board.FullmoveNumber;

        PieceColor previousSideToMove =
            SideToMove;

        Move? previousLastMove =
            LastMove;

        // =========================================================
        // DETECT CAPTURE
        // =========================================================

        Piece? capturedPiece =
            Board.GetPiece(move.To);

        Position? capturedPosition =
            capturedPiece != null
                ? move.To
                : null;

        // =========================================================
        // DETECT EN PASSANT
        // =========================================================

        if (movedPiece.Type == PieceType.Pawn &&
            capturedPiece == null &&
            move.From.Column != move.To.Column)
        {
            Position possibleCapturedPosition =
                new Position(
                    move.From.Row,
                    move.To.Column
                );

            Piece? possibleCapturedPawn =
                Board.GetPiece(
                    possibleCapturedPosition
                );

            if (possibleCapturedPawn != null &&
                possibleCapturedPawn.Type == PieceType.Pawn &&
                possibleCapturedPawn.Color != color)
            {
                capturedPiece =
                    possibleCapturedPawn;

                capturedPosition =
                    possibleCapturedPosition;
            }
        }

        // =========================================================
        // DETECT CASTLING
        // =========================================================

        bool wasCastling = false;

        Position? rookFrom = null;
        Position? rookTo = null;
        Piece? rook = null;

        bool rookPreviousMovedState = false;

        if (movedPiece.Type == PieceType.King)
        {
            int row =
                color == PieceColor.White
                    ? 7
                    : 0;

            // -----------------------------------------------------
            // KING-SIDE CASTLING
            // -----------------------------------------------------

            if (move.From == new Position(row, 4) &&
                move.To == new Position(row, 6))
            {
                Position possibleRookFrom =
                    new Position(row, 7);

                Piece? possibleRook =
                    Board.GetPiece(
                        possibleRookFrom
                    );

                if (possibleRook != null &&
                    possibleRook.Type == PieceType.Rook &&
                    possibleRook.Color == color)
                {
                    wasCastling = true;

                    rookFrom =
                        possibleRookFrom;

                    rookTo =
                        new Position(row, 5);

                    rook =
                        possibleRook;

                    rookPreviousMovedState =
                        possibleRook.HasMoved;
                }
            }

            // -----------------------------------------------------
            // QUEEN-SIDE CASTLING
            // -----------------------------------------------------

            else if (
                move.From == new Position(row, 4) &&
                move.To == new Position(row, 2))
            {
                Position possibleRookFrom =
                    new Position(row, 0);

                Piece? possibleRook =
                    Board.GetPiece(
                        possibleRookFrom
                    );

                if (possibleRook != null &&
                    possibleRook.Type == PieceType.Rook &&
                    possibleRook.Color == color)
                {
                    wasCastling = true;

                    rookFrom =
                        possibleRookFrom;

                    rookTo =
                        new Position(row, 3);

                    rook =
                        possibleRook;

                    rookPreviousMovedState =
                        possibleRook.HasMoved;
                }
            }
        }

        // =========================================================
        // DETECT PROMOTION
        // =========================================================

        bool isPromotion =
            movedPiece.Type == PieceType.Pawn &&
            PawnPromotion.CanPromote(
                move.To,
                color
            );

        // A promotion type is only valid when
        // the move is actually a promotion.
        if (!isPromotion &&
            promotionType != null)
        {
            return false;
        }

        // =========================================================
        // EXECUTE MOVE
        // =========================================================

        bool success =
            MoveExecutor.TryExecuteMove(
                Board,
                move,
                color,
                LastMove,
                promotionType
            );

        // =========================================================
        // ILLEGAL MOVE
        // =========================================================

        if (!success)
        {
            return false;
        }

        // =========================================================
        // UPDATE LAST MOVE
        // =========================================================

        LastMove = move;

        // =========================================================
        // CHANGE SIDE
        // =========================================================

        SideToMove =
            SideToMove == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

        if (color == PieceColor.Black)
        {
            Board.IncrementFullmoveNumber();
        }

        // =========================================================
        // CHECK AFTER MOVE
        // =========================================================

        bool isCheck =
            CheckDetector.IsInCheck(
                Board,
                SideToMove
            );

        bool isCheckmate =
            CheckmateDetector.IsCheckmate(
                Board,
                SideToMove
            );

        // =========================================================
        // GENERATE SAN
        // =========================================================

        string sanNotation =
            SanNotationGenerator.Generate(
                Board,
                move,
                movedPiece,
                capturedPiece,
                promotionType,
                isCheck,
                isCheckmate,
                wasCastling
            );

        // =========================================================
        // DEBUG SAN
        // =========================================================

        Console.WriteLine(
            $"SAN: {sanNotation}"
        );

        // =========================================================
        // CREATE MOVE RECORD
        // =========================================================

        MoveRecord record =
            new MoveRecord(
                move,
                sanNotation,
                color,
                movedPiece,
                previousMovedState,
                capturedPiece,
                capturedPosition,
                wasCastling,
                rookFrom,
                rookTo,
                rook,
                rookPreviousMovedState,
                isPromotion,
                promotionType,
                previousHalfmoveClock,
                previousFullmoveNumber,
                previousSideToMove,
                previousLastMove
            );

        // =========================================================
        // ADD TO MOVE HISTORY
        // =========================================================

        MoveHistory.Add(
            record
        );

        // =========================================================
        // RECORD POSITION
        // =========================================================

        RecordCurrentPosition();

        return true;
    }

    public bool UndoMove()
    {
        // =========================================================
        // GET LAST MOVE RECORD
        // =========================================================

        MoveRecord? record =
            MoveHistory.RemoveLast();

        if (record == null)
        {
            return false;
        }

        if (record.MovedPiece == null)
        {
            return false;
        }

        // =========================================================
        // REMOVE CURRENT POSITION FROM REPETITION COUNTER
        // =========================================================

        RemoveCurrentPosition();

        Move move =
            record.Move;

        // =========================================================
        // RESTORE MOVED PIECE
        // =========================================================

        Board.SetPiece(
            move.From,
            record.MovedPiece
        );

        // Clear destination square.
        Board.SetPiece(
            move.To,
            null
        );

        // =========================================================
        // RESTORE CAPTURED PIECE
        // =========================================================

        if (record.CapturedPiece != null &&
            record.CapturedPosition != null)
        {
            Board.SetPiece(
                record.CapturedPosition.Value,
                record.CapturedPiece
            );
        }

        // =========================================================
        // RESTORE CASTLING ROOK
        // =========================================================

        if (record.WasCastling &&
            record.Rook != null &&
            record.RookFrom != null &&
            record.RookTo != null)
        {
            Board.SetPiece(
                record.RookTo.Value,
                null
            );

            Board.SetPiece(
                record.RookFrom.Value,
                record.Rook
            );

            record.Rook.RestoreMovedState(
                record.RookPreviousMovedState
            );
        }

        // =========================================================
        // RESTORE MOVED PIECE STATE
        // =========================================================

        record.MovedPiece.RestoreMovedState(
            record.PreviousMovedState
        );

        // =========================================================
        // RESTORE HALF-MOVE CLOCK
        // =========================================================

        Board.RestoreHalfmoveClock(
            record.PreviousHalfmoveClock
        );

        Board.RestoreFullmoveNumber(
            record.PreviousFullmoveNumber
        );

        // =========================================================
        // RESTORE GAME STATE
        // =========================================================

        SideToMove =
            record.PreviousSideToMove;

        LastMove =
            record.PreviousLastMove;

        return true;
    }

    // =========================================================
    // THREEFOLD REPETITION
    // =========================================================

    private void RecordCurrentPosition()
    {
        string positionKey =
            PositionKeyGenerator.Generate(
                Board,
                SideToMove,
                LastMove
            );

        if (positionCounts.ContainsKey(
                positionKey))
        {
            positionCounts[positionKey]++;
        }
        else
        {
            positionCounts[positionKey] = 1;
        }
    }

    // =========================================================
    // GET CURRENT POSITION COUNT
    // =========================================================

    public int GetCurrentPositionCount()
    {
        string positionKey =
            PositionKeyGenerator.Generate(
                Board,
                SideToMove,
                LastMove
            );

        if (positionCounts.TryGetValue(
                positionKey,
                out int count))
        {
            return count;
        }

        return 0;
    }

    // =========================================================
    // THREEFOLD REPETITION CHECK
    // =========================================================

    public bool IsThreefoldRepetition()
    {
        string positionKey =
            PositionKeyGenerator.Generate(
                Board,
                SideToMove,
                LastMove
            );

        return
            positionCounts.TryGetValue(
                positionKey,
                out int count
            )
            && count >= 3;
    }

    // =========================================================
    // GAME RESULT
    // =========================================================

    public GameResult GetGameResult(
        PieceColor sideToMove)
    {
        // -----------------------------------------------------
        // CHECKMATE
        // -----------------------------------------------------

        if (CheckmateDetector.IsCheckmate(
                Board,
                sideToMove))
        {
            return sideToMove == PieceColor.White
                ? GameResult.BlackWins
                : GameResult.WhiteWins;
        }

        // -----------------------------------------------------
        // STALEMATE
        // -----------------------------------------------------

        if (StalemateDetector.IsStalemate(
                Board,
                sideToMove))
        {
            return GameResult.Stalemate;
        }

        // -----------------------------------------------------
        // THREEFOLD REPETITION
        // -----------------------------------------------------

        if (IsThreefoldRepetition())
        {
            return GameResult.ThreefoldRepetition;
        }

        // -----------------------------------------------------
        // FIFTY-MOVE RULE
        // -----------------------------------------------------

        if (DrawDetector.IsFiftyMoveDraw(
                Board))
        {
            return GameResult.FiftyMoveDraw;
        }

        // -----------------------------------------------------
        // INSUFFICIENT MATERIAL
        // -----------------------------------------------------

        if (DrawDetector.IsInsufficientMaterial(
                Board))
        {
            return GameResult.InsufficientMaterial;
        }

        // -----------------------------------------------------
        // GAME CONTINUES
        // -----------------------------------------------------

        return GameResult.Ongoing;
    }

    // =========================================================
    // COUNT PIECES
    // =========================================================

    public int CountPieces()
    {
        int count = 0;

        for (
            int row = 0;
            row < Board.Size;
            row++
        )
        {
            for (
                int column = 0;
                column < Board.Size;
                column++
            )
            {
                Position position =
                    new Position(
                        row,
                        column
                    );

                if (
                    Board.GetPiece(
                        position
                    ) != null
                )
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static Piece CreatePieceFromFenCharacter(
        char character)
    {
        PieceColor color =
            char.IsUpper(character)
                ? PieceColor.White
                : PieceColor.Black;

        char lower =
            char.ToLowerInvariant(character);

        PieceType type =
            lower switch
            {
                'p' => PieceType.Pawn,
                'r' => PieceType.Rook,
                'n' => PieceType.Knight,
                'b' => PieceType.Bishop,
                'q' => PieceType.Queen,
                'k' => PieceType.King,

                _ => throw new ArgumentException(
                    $"Invalid FEN piece character: {character}"
                )
            };

        return new Piece(
            type,
            color
        );
    }

    private static void ApplyCastlingRights(
        Board board,
        string castlingRights)
    {
        // Default:
        // Pieces loaded from FEN are considered
        // to have moved unless the FEN explicitly
        // gives them castling rights.

        for (int row = 0; row < Board.Size; row++)
        {
            for (int column = 0; column < Board.Size; column++)
            {
                Piece? piece =
                    board.GetPiece(
                        new Position(row, column)
                    );

                if (piece != null)
                {
                    piece.RestoreMovedState(true);
                }
            }
        }

        if (castlingRights == "-")
        {
            return;
        }

        // White king
        Piece? whiteKing =
            board.GetPiece(
                new Position(7, 4)
            );

        // Black king
        Piece? blackKing =
            board.GetPiece(
                new Position(0, 4)
            );

        if (castlingRights.Contains('K') ||
            castlingRights.Contains('Q'))
        {
            if (whiteKing != null &&
                whiteKing.Type == PieceType.King &&
                whiteKing.Color == PieceColor.White)
            {
                whiteKing.RestoreMovedState(false);
            }
        }

        if (castlingRights.Contains('k') ||
            castlingRights.Contains('q'))
        {
            if (blackKing != null &&
                blackKing.Type == PieceType.King &&
                blackKing.Color == PieceColor.Black)
            {
                blackKing.RestoreMovedState(false);
            }
        }

        // White king-side rook
        if (castlingRights.Contains('K'))
        {
            Piece? rook =
                board.GetPiece(
                    new Position(7, 7)
                );

            if (rook != null &&
                rook.Type == PieceType.Rook &&
                rook.Color == PieceColor.White)
            {
                rook.RestoreMovedState(false);
            }
        }

        // White queen-side rook
        if (castlingRights.Contains('Q'))
        {
            Piece? rook =
                board.GetPiece(
                    new Position(7, 0)
                );

            if (rook != null &&
                rook.Type == PieceType.Rook &&
                rook.Color == PieceColor.White)
            {
                rook.RestoreMovedState(false);
            }
        }

        // Black king-side rook
        if (castlingRights.Contains('k'))
        {
            Piece? rook =
                board.GetPiece(
                    new Position(0, 7)
                );

            if (rook != null &&
                rook.Type == PieceType.Rook &&
                rook.Color == PieceColor.Black)
            {
                rook.RestoreMovedState(false);
            }
        }

        // Black queen-side rook
        if (castlingRights.Contains('q'))
        {
            Piece? rook =
                board.GetPiece(
                    new Position(0, 0)
                );

            if (rook != null &&
                rook.Type == PieceType.Rook &&
                rook.Color == PieceColor.Black)
            {
                rook.RestoreMovedState(false);
            }
        }
    }

    private static Move? CreateLastMoveFromEnPassant(
        string enPassantTarget,
        PieceColor sideToMove)
    {
        if (enPassantTarget == "-")
        {
            return null;
        }

        if (enPassantTarget.Length != 2)
        {
            throw new ArgumentException(
                "Invalid FEN en passant target."
            );
        }

        char file = enPassantTarget[0];
        char rank = enPassantTarget[1];

        if (file < 'a' || file > 'h' ||
            rank < '1' || rank > '8')
        {
            throw new ArgumentException(
                "Invalid FEN en passant target."
            );
        }

        int targetColumn =
            file - 'a';

        int targetRow =
            '8' - rank;

        // The previous mover is the opposite color.
        PieceColor previousMover =
            sideToMove == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

        int direction =
            previousMover == PieceColor.White
                ? -1
                : 1;

        Position to =
            new Position(
                targetRow - direction,
                targetColumn
            );

        Position from =
            new Position(
                targetRow + direction,
                targetColumn
            );

        return new Move(
            from,
            to
        );
    }

    private void RemoveCurrentPosition()
    {
        string positionKey =
            PositionKeyGenerator.Generate(
                Board,
                SideToMove,
                LastMove
            );

        if (positionCounts.TryGetValue(
                positionKey,
                out int count))
        {
            if (count <= 1)
            {
                positionCounts.Remove(positionKey);
            }
            else
            {
                positionCounts[positionKey] = count - 1;
            }
        }
    }

    private sealed class SearchUndoState
    {
        public Move Move { get; }

        public Piece MovedPiece { get; }

        public bool PreviousMovedState { get; }

        public Piece? CapturedPiece { get; }

        public Position? CapturedPosition { get; }

        public bool WasCastling { get; }

        public Position? RookFrom { get; }

        public Position? RookTo { get; }

        public Piece? Rook { get; }

        public bool RookPreviousMovedState { get; }

        public int PreviousHalfmoveClock { get; }

        public int PreviousFullmoveNumber { get; }

        public PieceColor PreviousSideToMove { get; }

        public Move? PreviousLastMove { get; }

        public SearchUndoState(
            Move move,
            Piece movedPiece,
            bool previousMovedState,
            Piece? capturedPiece,
            Position? capturedPosition,
            bool wasCastling,
            Position? rookFrom,
            Position? rookTo,
            Piece? rook,
            bool rookPreviousMovedState,
            int previousHalfmoveClock,
            int previousFullmoveNumber,
            PieceColor previousSideToMove,
            Move? previousLastMove)
        {
            Move = move;
            MovedPiece = movedPiece;
            PreviousMovedState = previousMovedState;

            CapturedPiece = capturedPiece;
            CapturedPosition = capturedPosition;

            WasCastling = wasCastling;
            RookFrom = rookFrom;
            RookTo = rookTo;
            Rook = rook;
            RookPreviousMovedState = rookPreviousMovedState;

            PreviousHalfmoveClock = previousHalfmoveClock;
            PreviousFullmoveNumber = previousFullmoveNumber;
            PreviousSideToMove = previousSideToMove;
            PreviousLastMove = previousLastMove;
        }
    }

    public bool UndoSearchMove()
    {
        if (searchUndoStack.Count == 0)
        {
            return false;
        }

        SearchUndoState state =
            searchUndoStack.Pop();

        Move move =
            state.Move;

        // =========================================================
        // RESTORE MOVED PIECE
        // =========================================================

        Board.SetPiece(
            move.From,
            state.MovedPiece
        );

        // =========================================================
        // CLEAR DESTINATION
        // =========================================================

        Board.SetPiece(
            move.To,
            null
        );

        // =========================================================
        // RESTORE CAPTURED PIECE
        // =========================================================

        if (state.CapturedPiece != null &&
            state.CapturedPosition != null)
        {
            Board.SetPiece(
                state.CapturedPosition.Value,
                state.CapturedPiece
            );
        }

        // =========================================================
        // RESTORE CASTLING ROOK
        // =========================================================

        if (state.WasCastling &&
            state.Rook != null &&
            state.RookFrom != null &&
            state.RookTo != null)
        {
            Board.SetPiece(
                state.RookTo.Value,
                null
            );

            Board.SetPiece(
                state.RookFrom.Value,
                state.Rook
            );

            state.Rook.RestoreMovedState(
                state.RookPreviousMovedState
            );
        }

        // =========================================================
        // RESTORE MOVED PIECE STATE
        // =========================================================

        state.MovedPiece.RestoreMovedState(
            state.PreviousMovedState
        );

        // =========================================================
        // RESTORE BOARD STATE
        // =========================================================

        Board.RestoreHalfmoveClock(
            state.PreviousHalfmoveClock
        );

        Board.RestoreFullmoveNumber(
            state.PreviousFullmoveNumber
        );

        // =========================================================
        // RESTORE GAME STATE
        // =========================================================

        SideToMove =
            state.PreviousSideToMove;

        LastMove =
            state.PreviousLastMove;

        return true;
    }
}