using System;
using System.Diagnostics;
using System.IO;
using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public sealed class StockfishEngine : IDisposable
{
    private readonly Process process;
    private readonly StreamWriter input;
    private readonly StreamReader output;

    // =========================================================
    // LAST PROMOTION
    // =========================================================

    public PieceType? LastPromotion { get; private set; }

    public string LastBestMove { get; private set; } = "";

    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public StockfishEngine(
        string enginePath)
    {
        if (string.IsNullOrWhiteSpace(enginePath))
        {
            throw new ArgumentException(
                "Stockfish engine path is empty.",
                nameof(enginePath)
            );
        }

        if (!File.Exists(enginePath))
        {
            throw new FileNotFoundException(
                "Stockfish executable was not found.",
                enginePath
            );
        }

        process =
            new Process();

        process.StartInfo =
            new ProcessStartInfo
            {
                FileName = enginePath,

                UseShellExecute = false,

                RedirectStandardInput = true,

                RedirectStandardOutput = true,

                RedirectStandardError = true,

                CreateNoWindow = true
            };

        process.Start();

        input =
            process.StandardInput;

        output =
            process.StandardOutput;

        input.AutoFlush = true;
    }

    // =========================================================
    // RUNNING
    // =========================================================

    public bool IsRunning =>
        !process.HasExited;

    // =========================================================
    // SEND COMMAND
    // =========================================================

    public void SendCommand(
        string command)
    {
        if (!IsRunning)
        {
            return;
        }

        input.WriteLine(
            command
        );
    }

    // =========================================================
    // WAIT FOR RESPONSE
    // =========================================================

    public string WaitFor(
        string expected)
    {
        string result = "";

        while (!output.EndOfStream)
        {
            string? line =
                output.ReadLine();

            if (line == null)
            {
                break;
            }

            result +=
                line +
                Environment.NewLine;

            if (line.Trim() ==
                expected)
            {
                break;
            }
        }

        return result;
    }

    // =========================================================
    // INITIALIZE UCI
    // =========================================================

    public string Initialize()
    {
        SendCommand(
            "uci"
        );

        return WaitFor(
            "uciok"
        );
    }

    // =========================================================
    // READY
    // =========================================================

    public string WaitUntilReady()
    {
        SendCommand(
            "isready"
        );

        return WaitFor(
            "readyok"
        );
    }

    // =========================================================
    // GET BEST MOVE
    // =========================================================

    public Move? GetBestMove(
        ChessGame game,
        int thinkTimeMilliseconds = 2000)
    {
        if (!IsRunning)
        {
            return null;
        }

        // -----------------------------------------------------
        // Reset promotion information.
        // -----------------------------------------------------

        LastPromotion = null;
        LastBestMove = "";

        // -----------------------------------------------------
        // Make sure Stockfish is ready.
        // -----------------------------------------------------

        WaitUntilReady();

        // -----------------------------------------------------
        // Generate current FEN.
        // -----------------------------------------------------

        string fen =
            FenGenerator.Generate(
                game.Board,
                game.SideToMove,
                game.LastMove
            );

        // -----------------------------------------------------
        // Tell Stockfish current position.
        // -----------------------------------------------------

        SendCommand(
            $"position fen {fen}"
        );

        // -----------------------------------------------------
        // Ask Stockfish to think.
        // -----------------------------------------------------

        thinkTimeMilliseconds =
            Math.Max(
                100,
                thinkTimeMilliseconds
            );

        SendCommand(
            $"go movetime {thinkTimeMilliseconds}"
        );

        // -----------------------------------------------------
        // Wait for bestmove.
        // -----------------------------------------------------

        while (!output.EndOfStream)
        {
            string? line =
                output.ReadLine();

            if (line == null)
            {
                break;
            }

            line =
                line.Trim();

            if (!line.StartsWith(
                    "bestmove ",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string[] parts =
                line.Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries
                );

            if (parts.Length < 2)
            {
                return null;
            }

            string bestMove =
                parts[1].Trim();

            LastBestMove = bestMove;

            Move? move =
                ParseBestMove(
                    bestMove,
                    out PieceType? promotion
                );

            LastPromotion = promotion;

            return move;
        }

        return null;
    }

    // =========================================================
    // PARSE STOCKFISH MOVE
    // =========================================================

    public static Move? ParseBestMove(
        string notation,
        out PieceType? promotion)
    {
        // =========================================================
        // RESET PROMOTION
        // =========================================================

        promotion = null;

        // =========================================================
        // INVALID
        // =========================================================

        if (string.IsNullOrWhiteSpace(notation))
        {
            return null;
        }

        // =========================================================
        // NO MOVE
        // =========================================================

        if (notation == "0000")
        {
            return null;
        }

        // =========================================================
        // UCI MOVE
        // =========================================================

        if (notation.Length < 4)
        {
            return null;
        }

        string fromSquare =
            notation.Substring(
                0,
                2
            );

        string toSquare =
            notation.Substring(
                2,
                2
            );

        Position from;

        Position to;

        if (!TryParseSquare(
                fromSquare,
                out from))
        {
            return null;
        }

        if (!TryParseSquare(
                toSquare,
                out to))
        {
            return null;
        }

        // =========================================================
        // PROMOTION
        // =========================================================

        if (notation.Length >= 5)
        {
            char promotionChar =
                char.ToLowerInvariant(
                    notation[4]
                );

            promotion =
                promotionChar switch
                {
                    'q' => PieceType.Queen,
                    'r' => PieceType.Rook,
                    'b' => PieceType.Bishop,
                    'n' => PieceType.Knight,

                    _ => null
                };
        }

        // =========================================================
        // CREATE MOVE
        // =========================================================

        return new Move(
            from,
            to
        );
    }

    // =========================================================
    // PARSE ALGEBRAIC SQUARE
    // =========================================================

    private static bool TryParseSquare(
        string square,
        out Position position)
    {
        position =
            default;

        if (square.Length != 2)
        {
            return false;
        }

        char file =
            char.ToLowerInvariant(
                square[0]
            );

        char rank =
            square[1];

        if (file < 'a' ||
            file > 'h')
        {
            return false;
        }

        if (rank < '1' ||
            rank > '8')
        {
            return false;
        }

        int column =
            file - 'a';

        int row =
            '8' - rank;

        position =
            new Position(
                row,
                column
            );

        return true;
    }

    // =========================================================
    // DISPOSE
    // =========================================================

    public void Dispose()
    {
        try
        {
            if (!process.HasExited)
            {
                SendCommand(
                    "quit"
                );

                if (!process.WaitForExit(
                        1000))
                {
                    process.Kill();
                }
            }
        }
        catch
        {
            // Ignore shutdown errors.
        }

        input.Dispose();

        output.Dispose();

        process.Dispose();
    }
}