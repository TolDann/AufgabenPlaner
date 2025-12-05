using AufgabenPlaner.Classes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AufgabenPlaner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Show_Todos(true);
            Test_data();
        }

        private void Show_Todos(bool show) 
        {
            dg_todos.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            lbl_todos_Buttons.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            stack_newTodo.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
            lbl_newTodo_Buttons.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
            lbl_headline.Content = show ? "Aufgaben" : "Aufgabe";
        }

        private void MainButtons_Click(object sender, RoutedEventArgs e)
        {
            // Button aus sender holen
            Button btn = (Button)sender;
            if (btn == null)
                return;

            // Tag auswerten
            string tag = (string)btn.Tag;
            if (string.IsNullOrEmpty(tag))
                return;

            switch (tag)
            {
                case "todos": Show_Todos(true); break;
                case "newTodo": Show_Todos(false); break;
                case "end": Close(); break;
            }
        }

        private void Btn_NewTodo_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string tag = (string)btn.Tag;

            switch (tag)
            {
                case "clear": Clear_NewTodo_InputFields(); break;
                case "save":
                    TodoItem newItem = Get_New_Todo_From_UserInput();
                    Try_Save_New_Todo(newItem);
                    break;
            }
        } 

        private TodoItem Get_New_Todo_From_UserInput()
        {
            // Neue Aufgabe erstellen
            TodoItem newItem = new TodoItem
            {
                Title = string.IsNullOrWhiteSpace(tbx_newTodo_title.Text)
                    ? "Unbenannte Aufgabe"
                    : tbx_newTodo_title.Text.Trim(),

                Description = string.IsNullOrWhiteSpace(tbx_newTodo_description.Text)
                    ? "(Keine Beschreibung angegeben)"
                    : tbx_newTodo_description.Text.Trim(),

                Priority = Get_NewTodo_Selected_Radiobutton()
            };

            // Datum setzen
            Set_NewTodo_Date(newItem);
            return newItem;
        }

        private void Set_NewTodo_Date(TodoItem newItem)
        {
            // Datum prüfen: wenn kein Datum ausgewählt wurde, Standardwert ist das aktuelle Datum
            // Es wird zusätzlich ein Vermerk in der Beschreibung der Aufgabe geschrieben
            if (dp_newTodo_datePicker.SelectedDate.HasValue)
            {
                newItem.Date = dp_newTodo_datePicker.SelectedDate.Value;
            }
            else
            {
                newItem.Date = DateTime.Today;
                newItem.Description += " Hinweis: Da kein Datum angegeben wurde, wurde das Erstellungsdatum dieser Aufgabe automatisch gesetzt.";
            }
        }

        private void Try_Save_New_Todo(TodoItem newItem)
        {
            try
            {
                // Aufgabe speichern
                Todo_Transfer todo_Transfer = new Todo_Transfer();
                todo_Transfer.Save_NewTodo(newItem);
                MessageBox.Show($"Aufgabe \"{newItem.Title}\" wurde erfolgreich erstellt.",
                    "Aufgabe gespeichert",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung beim Speichern
                MessageBox.Show(
                    $"Fehler beim Speichern der Aufgabe:\n{ex.Message}",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private string Get_NewTodo_Selected_Radiobutton()
        {
            RadioButton[] radios = { rb_newToDo_Green, rb_newToDo_Orange, rb_newToDo_Red };

            foreach (var rb in radios)
                if (rb.IsChecked == true)
                    return rb.Tag.ToString();

            return string.Empty;
        }

        private void Clear_NewTodo_InputFields()
        {
            tbx_newTodo_title.Text = string.Empty;
            tbx_newTodo_description.Text = string.Empty;
            dp_newTodo_datePicker.Text = string.Empty;
            rb_newToDo_Green.IsChecked = true;
        }

        // ACHTUNG!!! Später entfernen
        // Das ist zum testen
        private void Test_data()
        {
            dg_todos.ItemsSource = Get_Testdata();
        }

        private List<TodoItem> Get_Testdata()
        {
            return new List<TodoItem>()
            {
                new TodoItem { IsSelected = false,  Title = "Test 1", Description = "Beispieltext 1", Priority  ="Mittel", Date = DateTime.Now },
                new TodoItem { IsSelected = false, Title = "Test 2", Description = "Beispieltext 2",Priority = "Hoch", Date = DateTime.Now.AddDays(-1) },
            };
        }

      
    }
}
