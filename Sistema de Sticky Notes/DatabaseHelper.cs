using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

public class Note
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Color { get; set; }
}

public static class DatabaseHelper
{
    private const string ConnectionString = "Data Source=notes.db";

    public static void Initialize()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Notes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Content TEXT,
                Color TEXT
            );";
        command.ExecuteNonQuery();
    }

    public static void SaveNote(Note note)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var command = connection.CreateCommand();
        if (note.Id == 0) {
            command.CommandText = "INSERT INTO Notes (Content, Color) VALUES ($content, $color); SELECT last_insert_rowid();";
        } else {
            command.CommandText = "UPDATE Notes SET Content = $content, Color = $color WHERE Id = $id";
            command.Parameters.AddWithValue("$id", note.Id);
        }
        command.Parameters.AddWithValue("$content", note.Content);
        command.Parameters.AddWithValue("$color", note.Color);
        
        if (note.Id == 0) note.Id = Convert.ToInt32(command.ExecuteScalar());
        else command.ExecuteNonQuery();
    }

    public static List<Note> GetAllNotes()
    {
        var notes = new List<Note>();
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Notes";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            notes.Add(new Note {
                Id = reader.GetInt32(0),
                Content = reader.GetString(1),
                Color = reader.GetString(2)
            });
        }
        return notes;
    }

    public static void DeleteNote(int id)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Notes WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}