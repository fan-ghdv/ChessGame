using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class FenParserTests
{
    [Fact]
    public void ParseBoard_ShouldLoadInitialPosition()
    {
        string fen =
            "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        Board board =
            FenParser.ParseBoard(fen);

        Assert.Equal(
            PieceType.Rook,
            board.GetPiece(
                new Position(0, 0)
            )!.Type
        );

        Assert.Equal(
            PieceColor.Black,
            board.GetPiece(
                new Position(0, 0)
            )!.Color
        );

        Assert.Equal(
            PieceType.King,
            board.GetPiece(
                new Position(7, 4)
            )!.Type
        );

        Assert.Equal(
            PieceColor.White,
            board.GetPiece(
                new Position(7, 4)
            )!.Color
        );
    }

    [Fact]
    public void ParseBoard_ShouldLoadEmptyBoard()
    {
        string fen =
            "8/8/8/8/8/8/8/8 w - - 0 1";

        Board board =
            FenParser.ParseBoard(fen);

        Assert.Equal(
            0,
            CountPieces(board)
        );
    }

    [Fact]
    public void ParseBoard_ShouldLoadMixedPieces()
    {
        string fen =
            "rnbqkBNR/8/8/8/8/8/8/8 w - - 0 1";

        Board board =
            FenParser.ParseBoard(fen);

        Assert.Equal(
            PieceType.Rook,
            board.GetPiece(
                new Position(0, 0)
            )!.Type
        );

        Assert.Equal(
            PieceColor.White,
            board.GetPiece(
                new Position(0, 5)
            )!.Color
        );

        Assert.Equal(
            PieceType.Knight,
            board.GetPiece(
                new Position(0, 6)
            )!.Type
        );
    }

    [Fact]
    public void ParseBoard_ShouldRejectInvalidRankCount()
    {
        string fen =
            "8/8/8/8/8/8/8 w - - 0 1";

        Assert.Throws<ArgumentException>(
            () => FenParser.ParseBoard(fen)
        );
    }

    [Fact]
    public void ParseBoard_ShouldRejectInvalidPiece()
    {
        string fen =
            "8/8/8/8/8/8/8/7X w - - 0 1";

        Assert.Throws<ArgumentException>(
            () => FenParser.ParseBoard(fen)
        );
    }

    private static int CountPieces(
        Board board)
    {
        int count = 0;

        for (int row = 0; row < Board.Size; row++)
        {
            for (
                int column = 0;
                column < Board.Size;
                column++
            )
            {
                if (
                    board.GetPiece(
                        new Position(row, column)
                    ) != null
                )
                {
                    count++;
                }
            }
        }

        return count;
    }
}