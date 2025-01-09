using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace C971_MobileApp
{
    internal class Term : INotifyPropertyChanged
    {
        private bool _isExpanded;

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.MinValue;
        public DateTime EndDate { get; set; } = DateTime.MinValue;

        public string ExpandCollapseIcon => IsExpanded ? "\ue5c7" : "\ue5c5"; // Collapse (up) or Expand (down)

        [Ignore] // Prevents this property from being stored in the database
        public List<Course> Courses { get; set; } = new List<Course>();

        [Ignore]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                    OnPropertyChanged(nameof(ExpandCollapseIcon));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Term() { }
    }
}
