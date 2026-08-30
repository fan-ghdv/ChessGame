using ChessGame.Core.Game;
using ChessGame.Core.Models;
using ChessGameGame = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class FenStateTests
{
    // =========================================================
    // FEN GENERATOR STATE TESTS
    // =========================================================

    [Fact]
    public void InitialPosition_ShouldHaveCorrectFullmoveNumber()
    {
        var game = new ChessGameGame();

        string fen =
            FenGenerator.Generate(
                game.Board,
                game.SideToMove,
                game.LastMove
            );

        Assert.EndsWith(
            " - 0 1",
            fen
        );
    }

    [Fact]
    public void AfterWhiteMove_FullmoveNumberShouldRemainOne()
    {
        var game = new ChessGameGame();

        Assert.True(
            game.TryMove(
                new Move(
                    new Position(6, 4),
                    new Position(5, 4)
                ),
                PieceColor.White
            )
        );

        string fen =
            FenGenerator.Generate(
                game.Board,
                game.SideToMove,
                game.LastMove
            );

        Assert.EndsWith(
            " - 0 1",
            fen
        );
    }

    [Fact]
    public void AfterBlackMove_FullmoveNumberShouldBecomeTwo()
    {
        var game = new ChessGameGame();

        Assert.True(
            game.TryMove(
                new Move(
                    new Position(6, 4),
                    new Position(5, 4)
                ),
                PieceColor.White
            )
        );

        Assert.True(
            game.TryMove(
                new Move(
                    new Position(1, 4),
                    new Position(3, 4)
                ),
                PieceColor.Black
            )
        );

        string fen =
            FenGenerator.Generate(
                game.Board,
                game.SideToMove,
                game.LastMove
            );

        Assert.EndsWith(
            " e6 0 2",
            fen
        );
    }

    [Fact]
    public void BlackDoublePawnMove_ShouldSetEnPassantTarget()
    {
        var game = new ChessGameGame();

        Assert.True(
            game.TryMove(
                new Move(
                    new Position(6, 4),
                    new Position(5, 4)
                ),
                PieceColor.White
            )
        );

        Assert.True(
            game.TryMove(
                new Move(
                    new Position(1, 4),
                    new Position(3, 4)
                ),
                PieceColor.Black
            )
        );

        string fen =
            FenGenerator.Generate(
                game.Board,
                game.SideToMove,
                game.LastMove
            );

        Assert.Contains(
            " e6 ",
            fen
        );
    }

    [Fact]
    public void SinglePawnMove_ShouldNotSetEnPassantTarget()
    {
        var game = new ChessGameGame();

        Assert.True(
            game.TryMove(
                new Move(
                    new Position(6, 4),
                    new Position(5, 4)
                ),
                PieceColor.White
            )
        );

        string fen =
            FenGenerator.Generate(
                game.Board,
                game.SideToMove,
                game.LastMove
            );

        Assert.Contains(
            " - ",
            fen
        );
    }

    [Fact]
    public void NonPawnDoubleMove_ShouldNotSetEnPassantTarget()
    {
        var board = new Board();

        board.Clear();

        board.SetPiece(
            new Position(4, 0),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        var move =
            new Move(
                new Position(4, 0),
                new Position(2, 0)
            );

        string fen =
            FenGenerator.Generate(
                board,
                PieceColor.Black,
                move
            );

        Assert.Contains(
            " - ",
            fen
        );
    }

    // =========================================================
    // FEN PARSER STATE TESTS
    // =========================================================

    [Fact]
    public void Parse_ShouldReadSideToMove()
    {
        string fen =
            "8/8/8/8/8/8/8/4K2k b - - 0 1";

        FenState state =
            FenParser.Parse(fen);

        Assert.Equal(
            PieceColor.Black,
            state.SideToMove
        );
    }

    [Fact]
    public void Parse_ShouldReadCastlingRights()
    {
        string fen =
            "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1";

        FenState state =
            FenParser.Parse(fen);

        Assert.True(
            state.WhiteKingSideCastling
        );

        Assert.True(
            state.WhiteQueenSideCastling
        );

        Assert.True(
            state.BlackKingSideCastling
        );

        Assert.True(
            state.BlackQueenSideCastling
        );
    }

    [Fact]
    public void Parse_ShouldReadNoCastlingRights()
    {
        string fen =
            "4k3/8/8/8/8/8/8/4K3 w - - 0 1";

        FenState state =
            FenParser.Parse(fen);

        Assert.False(
            state.WhiteKingSideCastling
        );

        Assert.False(
            state.WhiteQueenSideCastling
        );

        Assert.False(
            state.BlackKingSideCastling
        );

        Assert.False(
            state.BlackQueenSideCastling
        );
    }

    [Fact]
    public void Parse_ShouldReadEnPassantTarget()
    {
        string fen =
            "8/8/8/3pP3/8/8/8/4K2k w - d6 0 1";

        FenState state =
            FenParser.Parse(fen);

        Assert.NotNull(
            state.EnPassantTarget
        );

        Assert.Equal(
            new Position(2, 3),
            state.EnPassantTarget!.Value
        );
    }

    [Fact]
    public void Parse_ShouldReadHalfmoveClock()
    {
        string fen =
            "8/8/8/8/8/8/8/4K2k w - - 37 12";

        FenState state =
            FenParser.Parse(fen);

        Assert.Equal(
            37,
            state.HalfmoveClock
        );
    }

    [Fact]
    public void Parse_ShouldReadFullmoveNumber()
    {
        string fen =
            "8/8/8/8/8/8/8/4K2k w - - 37 12";

        FenState state =
            FenParser.Parse(fen);

        Assert.Equal(
            12,
            state.FullmoveNumber
        );
    }

    [Fact]
    public void Parse_ShouldApplyWhiteCastlingRightsToPieces()
    {
        string fen =
            "4k3/8/8/8/8/8/8/R3K2R w K - 0 1";

        FenState state =
            FenParser.Parse(fen);

        Piece whiteKing =
            state.Board.GetPiece(
                new Position(7, 4)
            )!;

        Piece whiteQueenSideRook =
            state.Board.GetPiece(
                new Position(7, 0)
            )!;

        Piece whiteKingSideRook =
            state.Board.GetPiece(
                new Position(7, 7)
            )!;

        Assert.False(
            whiteKing.HasMoved
        );

        Assert.True(
            whiteQueenSideRook.HasMoved
        );

        Assert.False(
            whiteKingSideRook.HasMoved
        );
    }

    [Fact]
    public void Parse_ShouldApplyBlackCastlingRightsToPieces()
    {
        string fen =
            "r3k2r/8/8/8/8/8/8/4K3 b k - 0 1";

        FenState state =
            FenParser.Parse(fen);

        Piece blackKing =
            state.Board.GetPiece(
                new Position(0, 4)
            )!;

        Piece blackQueenSideRook =
            state.Board.GetPiece(
                new Position(0, 0)
            )!;

        Piece blackKingSideRook =
            state.Board.GetPiece(
                new Position(0, 7)
            )!;

        Assert.False(
            blackKing.HasMoved
        );

        Assert.True(
            blackQueenSideRook.HasMoved
        );

        Assert.False(
            blackKingSideRook.HasMoved
        );
    }
}