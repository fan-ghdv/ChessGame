using System.Collections.Generic;

namespace ChessGame.Core.Game;

public class MoveHistory
{
    private readonly List<MoveRecord> records =
        new List<MoveRecord>();

    public int Count =>
        records.Count;

    public bool CanUndo =>
        records.Count > 0;

    public void Clear()
    {
        records.Clear();
    }

    public void Add(
        MoveRecord record)
    {
        records.Add(record);
    }

    public MoveRecord? GetLast()
    {
        if (records.Count == 0)
        {
            return null;
        }

        return records[^1];
    }

    public MoveRecord? RemoveLast()
    {
        if (records.Count == 0)
        {
            return null;
        }

        int lastIndex =
            records.Count - 1;

        MoveRecord record =
            records[lastIndex];

        records.RemoveAt(lastIndex);

        return record;
    }

    public IReadOnlyList<MoveRecord> GetAll()
    {
        return records.AsReadOnly();
    }
}