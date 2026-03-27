using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class Note
{
    public string Content { get; set; } = "";
    public string Color { get; set; } = "#FFF740";
}

public static class DatabaseHelper
{
    private static readonly string FilePath = "notes.json";
    private static List<Note> _notes = new();

    public static void Initialize()
    {
        if (File.Exists(FilePath))
        {
            try
            {
                string json = File.ReadAllText(FilePath);
                _notes = JsonSerializer.Deserialize<List<Note>>(json) ?? new List<Note>();
            }
            catch { _notes = new List<Note>(); }
        }
    }

    private static void SaveToFile()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(FilePath, JsonSerializer.Serialize(_notes, options));
    }

    public static void SaveNote(Note note)
    {
        if (!_notes.Contains(note)) _notes.Add(note);
        SaveToFile();
    }

    public static List<Note> GetAllNotes() => _notes;

    public static void DeleteNote(Note note)
    {
        if (_notes.Remove(note)) SaveToFile();
    }
}