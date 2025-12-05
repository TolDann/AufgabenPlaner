using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.WindowsAPICodePack.Dialogs;



namespace AufgabenPlaner.Classes
{
    internal class Todo_Transfer
    {
        public string FolderPath { get; set; }

        /* ------------------------------
         * Zuweisung des Verzeichnisses
         ------------------------------ */
        public Todo_Transfer() 
        {
            FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "AufgabenPlaner");
        }

        /* ------------------------------
         * Neue Aufgaben Datei erzeugen
         ------------------------------ */
        public void Save_NewTodo(TodoItem todo)
        {
            // Ordner auf dem Desktop erstellen, falls nicht vorhanden      
            Directory.CreateDirectory(FolderPath);

            // Ungültige Dateizeichen aus dem Titel entfernen
            string safeTitle = Regex.Replace(todo.Title, @"[<>:""/\\|?*]", "_");

            // Dateiname erstellen
            string newFileName = $"{todo.Date:yyyyMMdd}_{safeTitle}.txt";

            // Voller Pfad
            string filePath = Path.Combine(FolderPath, newFileName);

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

        /* ------------------------------
         * Aufgaben aus dem Ordner lesen
         ------------------------------ */
        public List<TodoItem> Get_TodoList()
        {
            if (string.IsNullOrWhiteSpace(FolderPath))
            {
                FolderPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "AufgabenPlaner");
            }

            var todos = new List<TodoItem>();

            // Prüfen ob der Ordner existiert
            if (!Directory.Exists(FolderPath))
            {
                ShowFolderErrorDialog(FolderPath);

                // Benutzer kann neuen Ordner auswählen
                if (TrySelectNewFolder(out string newPath))
                {
                    FolderPath = newPath;
                    return Get_TodoList(); 
                }

                return todos;
            }

            // Alle .txt-Dateien im Ordner durchgehen
            foreach (string file in Directory.EnumerateFiles(FolderPath, "*.txt"))
            {
                try
                {
                    string[] lines = File.ReadAllLines(file);

                    if (lines.Length < 3)
                        continue;

                    //  Titel anhand des Dateinamen ohne Datum vergeben
                    string title = Path.GetFileNameWithoutExtension(file);
                    title = title.Substring(9); 

                    // Datum parsen
                    if (!DateTime.TryParseExact(lines[1], "dd.MM.yyyy", CultureInfo.InvariantCulture,
                                                DateTimeStyles.None, out DateTime date))
                    {
                        continue;
                    }

                    todos.Add(new TodoItem
                    {
                        Title = title,
                        Description = lines[0],
                        Priority = lines[2],
                        Date = date
                    });
                }
                catch (Exception ex)
                {
                    // Fehler wenn keine "Todo-Datei" gefunden wurde 
                    ShowFileErrorDialog(file, ex.Message);
                }
            }

            return todos;
        }

        private void ShowFolderErrorDialog(string expectedPath)
        {
            MessageBox.Show(
                $"Der erwartete Ordner wurde nicht gefunden:\n\n{expectedPath}",
                "Ordner nicht gefunden",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        private void ShowFileErrorDialog(string file, string error)
        {
            MessageBox.Show(
                $"Die Datei konnte nicht verarbeitet werden:\n\n{file}\n\nFehler:\n{error}",
                "Fehler beim Lesen der Datei",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private bool TrySelectNewFolder(out string selectedFolder)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Bitte wählen Sie den Ordner mit den Aufgaben aus"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                selectedFolder = dialog.FileName;
                return true;
            }

            selectedFolder = null;
            return false;
        }

    }

}
