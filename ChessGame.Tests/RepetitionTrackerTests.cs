using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class RepetitionTrackerTests
{
    [Fact]
    public void NewTracker_ShouldHaveZeroCount()
    {
        var board = new Board();

        var tracker =
            new RepetitionTracker();

        Assert.Equal(
            0,
            tracker.GetPositionCount(board)
        );
    }

    [Fact]
    public void RecordingPositionOnce_ShouldHaveCountOne()
    {
        var board = new Board();

        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        var tracker =
            new RepetitionTracker();

        tracker.RecordPosition(board);

        Assert.Equal(
            1,
            tracker.GetPositionCount(board)
        );
    }

    [Fact]
    public void RecordingSamePositionTwice_ShouldHaveCountTwo()
    {
        var board = new Board();

        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        var tracker =
            new RepetitionTracker();

        tracker.RecordPosition(board);
        tracker.RecordPosition(board);

        Assert.Equal(
            2,
            tracker.GetPositionCount(board)
        );
    }

    [Fact]
    public void RecordingSamePositionThreeTimes_ShouldBeThreefoldRepetition()
    {
        var board = new Board();

        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        var tracker =
            new RepetitionTracker();

        tracker.RecordPosition(board);
        tracker.RecordPosition(board);
        tracker.RecordPosition(board);

        Assert.Equal(
            3,
            tracker.GetPositionCount(board)
        );

        Assert.True(
            tracker.IsThreefoldRepetition(board)
        );
    }

    [Fact]
    public void DifferentPositions_ShouldHaveDifferentCounts()
    {
        var board1 = new Board();
        var board2 = new Board();

        board1.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board2.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        var tracker =
            new RepetitionTracker();

        tracker.RecordPosition(board1);
        tracker.RecordPosition(board1);
        tracker.RecordPosition(board2);

        Assert.Equal(
            2,
            tracker.GetPositionCount(board1)
        );

        Assert.Equal(
            1,
            tracker.GetPositionCount(board2)
        );

        Assert.False(
            tracker.IsThreefoldRepetition(board2)
        );
    }

    [Fact]
    public void Clear_ShouldRemoveAllRecordedPositions()
    {
        var board = new Board();

        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        var tracker =
            new RepetitionTracker();

        tracker.RecordPosition(board);
        tracker.RecordPosition(board);
        tracker.RecordPosition(board);

        Assert.True(
            tracker.IsThreefoldRepetition(board)
        );

        tracker.Clear();

        Assert.Equal(
            0,
            tracker.GetPositionCount(board)
        );

        Assert.False(
            tracker.IsThreefoldRepetition(board)
        );
    }
}