using System;
using System.IO;
using System.Runtime.InteropServices;
using ChessGame.Core.Models;

namespace ChessGame.Avalonia;

public static class ChessSoundPlayer
{
    // =========================================================
    // VOLUME
    // =========================================================

    private const float PawnVolume = 0.70f;
    private const float KnightVolume = 0.50f;
    private const float BishopVolume = 0.50f;
    private const float RookVolume = 0.20f;
    private const float QueenVolume = 0.90f;
    private const float KingVolume = 0.60f;

    // =========================================================
    // WINDOWS SOUND API
    // =========================================================

    [DllImport(
        "winmm.dll",
        SetLastError = true,
        CharSet = CharSet.Unicode)]
    private static extern bool PlaySound(
        string pszSound,
        IntPtr hmod,
        uint fdwSound);

    private const uint SND_FILENAME = 0x00020000;
    private const uint SND_ASYNC = 0x0001;
    private const uint SND_NODEFAULT = 0x0002;

    // =========================================================
    // PLAY
    // =========================================================

    public static void Play(
        PieceType pieceType)
    {
        string fileName =
            pieceType switch
            {
                PieceType.Pawn => "pawn.wav",
                PieceType.Knight => "knight.wav",
                PieceType.Bishop => "bishop.wav",
                PieceType.Rook => "rook.wav",
                PieceType.Queen => "queen.wav",
                PieceType.King => "king.wav",

                _ => string.Empty
            };

        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }

        float volume =
            pieceType switch
            {
                PieceType.Pawn => PawnVolume,
                PieceType.Knight => KnightVolume,
                PieceType.Bishop => BishopVolume,
                PieceType.Rook => RookVolume,
                PieceType.Queen => QueenVolume,
                PieceType.King => KingVolume,

                _ => 1.0f
            };

        string sourcePath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Assets",
                "Sounds",
                fileName
            );

        if (!File.Exists(sourcePath))
        {
            return;
        }

        string adjustedPath =
            Path.Combine(
                Path.GetTempPath(),
                $"ChessGame_{fileName}"
            );

        try
        {
            if (!CreateVolumeAdjustedWav(
                    sourcePath,
                    adjustedPath,
                    volume))
            {
                return;
            }

            PlaySound(
                adjustedPath,
                IntPtr.Zero,
                SND_FILENAME |
                SND_ASYNC |
                SND_NODEFAULT
            );
        }
        catch
        {
            // Sound must never crash the chess game.
        }
    }

    // =========================================================
    // VOLUME ADJUSTMENT
    // =========================================================

    private static bool CreateVolumeAdjustedWav(
        string sourcePath,
        string destinationPath,
        float volume)
    {
        try
        {
            byte[] data =
                File.ReadAllBytes(
                    sourcePath
                );

            if (data.Length < 44)
            {
                return false;
            }

            ushort audioFormat =
                BitConverter.ToUInt16(
                    data,
                    20
                );

            if (audioFormat != 1)
            {
                return false;
            }

            ushort bitsPerSample =
                BitConverter.ToUInt16(
                    data,
                    34
                );

            int dataPosition =
                FindDataChunk(data);

            if (dataPosition < 0)
            {
                return false;
            }

            int dataSize =
                BitConverter.ToInt32(
                    data,
                    dataPosition + 4
                );

            int audioStart =
                dataPosition + 8;

            if (dataSize < 0 ||
                audioStart < 0 ||
                audioStart + dataSize > data.Length)
            {
                return false;
            }

            volume =
                Math.Clamp(
                    volume,
                    0.0f,
                    1.0f
                );

            byte[] output =
                (byte[])data.Clone();

            // -------------------------------------------------
            // 16-BIT PCM
            // -------------------------------------------------

            if (bitsPerSample == 16)
            {
                for (
                    int i = audioStart;
                    i + 1 < audioStart + dataSize;
                    i += 2)
                {
                    short sample =
                        BitConverter.ToInt16(
                            output,
                            i
                        );

                    int adjusted =
                        (int)(
                            sample * volume
                        );

                    adjusted =
                        Math.Clamp(
                            adjusted,
                            short.MinValue,
                            short.MaxValue
                        );

                    output[i] =
                        (byte)(
                            adjusted & 0xFF
                        );

                    output[i + 1] =
                        (byte)(
                            (adjusted >> 8) & 0xFF
                        );
                }
            }

            // -------------------------------------------------
            // 8-BIT PCM
            // -------------------------------------------------

            else if (bitsPerSample == 8)
            {
                for (
                    int i = audioStart;
                    i < audioStart + dataSize;
                    i++)
                {
                    int sample =
                        output[i] - 128;

                    int adjusted =
                        (int)(
                            sample * volume
                        );

                    adjusted =
                        Math.Clamp(
                            adjusted,
                            -128,
                            127
                        );

                    output[i] =
                        (byte)(
                            adjusted + 128
                        );
                }
            }
            else
            {
                return false;
            }

            File.WriteAllBytes(
                destinationPath,
                output
            );

            return true;
        }
        catch
        {
            return false;
        }
    }

    // =========================================================
    // FIND WAV DATA CHUNK
    // =========================================================

    private static int FindDataChunk(
        byte[] wav)
    {
        int position = 12;

        while (
            position + 8 <= wav.Length)
        {
            string chunkId =
                System.Text.Encoding.ASCII.GetString(
                    wav,
                    position,
                    4
                );

            int chunkSize =
                BitConverter.ToInt32(
                    wav,
                    position + 4
                );

            if (chunkId == "data")
            {
                return position;
            }

            if (chunkSize < 0)
            {
                return -1;
            }

            position +=
                8 + chunkSize;

            if (chunkSize % 2 != 0)
            {
                position++;
            }
        }

        return -1;
    }
}