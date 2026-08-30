using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class DrawDetector
{
    public static bool IsInsufficientMaterial(
        Board board)
    {
        int bishopCount = 0;
        int knightCount = 0;
        int otherPieceCount = 0;

        Position? firstBishopPosition = null;
        Position? secondBishopPosition = null;

        for (int row = 0; row < Board.Size; row++)
        {
            for (int column = 0; column < Board.Size; column++)
            {
                Position position =
                    new Position(row, column);

                Piece? piece =
                    board.GetPiece(position);

                if (piece == null)
                {
                    continue;
                }

                switch (piece.Type)
                {
                    case PieceType.King:
                        break;

                    case PieceType.Bishop:
                        bishopCount++;

                        if (firstBishopPosition == null)
                        {
                            firstBishopPosition = position;
                        }
                        else if (secondBishopPosition == null)
                        {
                            secondBishopPosition = position;
                        }

                        break;

                    case PieceType.Knight:
                        knightCount++;
                        break;

                    default:
                        otherPieceCount++;
                        break;
                }
            }
        }

        if (otherPieceCount > 0)
        {
            return false;
        }

        if (bishopCount == 0 &&
            knightCount == 0)
        {
            return true;
        }

        if (bishopCount == 1 &&
            knightCount == 0)
        {
            return true;
        }

        if (bishopCount == 0 &&
            knightCount == 1)
        {
            return true;
        }

        if (bishopCount == 2 &&
            knightCount == 0 &&
            firstBishopPosition != null &&
            secondBishopPosition != null)
        {
            bool firstSquareColor =
                IsDarkSquare(
                    firstBishopPosition.Value
                );

            bool secondSquareColor =
                IsDarkSquare(
                    secondBishopPosition.Value
                );

            return firstSquareColor ==
                   secondSquareColor;
        }

        return false;
    }

    public static bool IsFiftyMoveDraw(
        Board board)
    {
        return board.HalfmoveClock >= 100;
    }

    private static bool IsDarkSquare(
        Position position)
    {
        return
            (position.Row + position.Column) % 2 == 1;
    }
}