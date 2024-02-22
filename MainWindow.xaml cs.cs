

using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ezhednevnic
{

    public class Note : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        private bool _isEditing;
        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                _isEditing = value;
                NotifyPropertyChanged("IsEditing");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
    public class RelayCommand : ICommand
    {
        private Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
    public static class JsonHelper
    {
        public static void Serialize<T>(string filePath, T obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            File.WriteAllText(filePath, json);
        }

        public static T Deserialize<T>(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
   

    public partial class MainWindow : Window
    {
        public ObservableCollection<Note> Notes { get; set; }
        public DateTime SelectedDate { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Note _selectedNote;
        public Note SelectedNote
        {
            get { return _selectedNote; }
            set
            {
                if (_selectedNote != value)
                {
                    _selectedNote = value;
                    NotifyPropertyChanged(nameof(SelectedNote));
                }
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyPropertyChanged(nameof(Title));
                }
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged(nameof(Description));
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            SelectedDate = DateTime.Today;
            LoadNotes();
            DataContext = this;

            UpdateNoteCommand = new RelayCommand(UpdateSelectedNote);
            AddNoteCommand = new RelayCommand(AddNote);
            DeleteNoteCommand = new RelayCommand(DeleteNote);

            foreach (var note in Notes)
            {
                note.PropertyChanged += (sender, e) => SaveNotes();
            }
        }
        public ICommand UpdateNoteCommand { get; set; }
        public ICommand AddNoteCommand { get; set; }
        public ICommand DeleteNoteCommand { get; set; }

        private void LoadNotes()
        {
            string filePath = "notes.json";
            if (File.Exists(filePath))
            {
                Notes = JsonHelper.Deserialize<ObservableCollection<Note>>(filePath);
            }
            else
            {
                Notes = new ObservableCollection<Note>();
            }
        }
        public void SaveNotes()
        {
            string filePath = "notes.json";
            JsonHelper.Serialize(filePath, Notes);
             
        }
        public void UpdateSelectedNote()
        {
            if (SelectedNote != null)
            {
                SelectedNote.Title = TitleTextBox.Text; 
                SelectedNote.Description = DescriptionTextBox.Text; 
                SelectedNote.Date = SelectedDate; 
                SaveNotes();
            }
        }
        public void AddNote()
        {
            SelectedNote = new Note { Date = SelectedDate };
            Notes.Add(SelectedNote);
            SaveNotes();
            SelectedNote = Notes.Last();
            UpdateSelectedNote();
        }
        public void DeleteNote()
        {
            if (SelectedNote != null)
            {
                Notes.Remove(SelectedNote);
                SaveNotes();
            }
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is Note selectedNote)
            {
                SelectedNote = selectedNote;
                TitleTextBox.Text = SelectedNote.Title;
                DescriptionTextBox.Text = SelectedNote.Description;
            }
        }
    }
}