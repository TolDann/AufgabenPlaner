using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace AufgabenPlaner.Classes
{
    internal class Todo_Transfer
    {

        public void Save_NewTodo(TodoItem todo)
        {
            // Ordner auf dem Desktop erstellen, falls nicht vorhanden
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "AufgabenPlaner");
            Directory.CreateDirectory(folderPath);

            // Ungültige Dateizeichen aus dem Titel entfernen
            string safeTitle = Regex.Replace(todo.Title, @"[<>:""/\\|?*]", "_");

            // Dateiname erstellen
            string newFileName = $"{todo.Date:yyyyMMdd}_{safeTitle}.txt";

            // Voller Pfad
            string filePath = Path.Combine(folderPath, newFileName);

            // Prüfen, ob Datei existiert
            if (File.Exists(filePath))
            {
                string message = "Für diesen Tag existiert bereits eine Aufgabe mit diesem Titel.\nMöchten Sie die bestehende Aufgabe überschreiben?";
                string caption = "Aufgabe bereits vorhanden";
                MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    // Abbrechen, wenn der Benutzer Nein wählt
                    return;
                }
            }
            // Datei schreiben
            string[] filetext = { todo.Description, todo.Date.ToShortDateString(), todo.Priority };
            File.WriteAllLines(filePath, filetext);
        }
    }
}
