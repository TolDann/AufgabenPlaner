using System;
using System.Windows.Controls;

namespace AufgabenPlaner.Classes
{
    internal class TodoItem
    {
        public bool IsSelected { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public DateTime Date { get; set; }


        public string RowColor { get { return Set_RowColor(); } }

        private string Set_RowColor()
        {
            if (Priority == "Hoch")
                return "Red";
            else if (Priority == "Mittel")
                return "Orange";
            else return "Green";
        }
    }
}
