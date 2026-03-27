using System;
using System.Drawing;
using System.Windows.Forms;

namespace StickyNotesApp
{
    public class MainForm : Form
    {
        private FlowLayoutPanel _pnlContainer;

        public MainForm()
        {
            DatabaseHelper.Initialize();
            InitializeComponent();
            LoadExistingNotes();
        }

        private void InitializeComponent()
        {
            this.Text = "Sticky Notes - NoSQL Style";
            this.Size = new Size(960, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);

            var btnAdd = new Button {
                Text = "+ Nova Nota",
                Dock = DockStyle.Top,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 150, 136),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => CreateNewNote();

            _pnlContainer = new FlowLayoutPanel {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20),
                BackColor = Color.Transparent
            };

            this.Controls.Add(_pnlContainer);
            this.Controls.Add(btnAdd);
        }

        private void CreateNewNote()
        {
            string[] colors = { "#FFF740", "#FF7EB9", "#7AFCFF", "#98FF98", "#DDA0DD", "#FFB347" };
            var random = new Random();
            var note = new Note { Content = "", Color = colors[random.Next(colors.Length)] };
            
            // Salva na lista e no arquivo
            DatabaseHelper.SaveNote(note); 
            AddNoteToUI(note);
        }

        private void LoadExistingNotes()
        {
            _pnlContainer.Controls.Clear();
            foreach (var note in DatabaseHelper.GetAllNotes())
            {
                AddNoteToUI(note);
            }
        }

        private void AddNoteToUI(Note note)
        {
            var noteControl = new StickyNoteControl(note);
            noteControl.OnDeleteRequested += (control) => {
                _pnlContainer.Controls.Remove(control);
                control.Dispose();
            };
            _pnlContainer.Controls.Add(noteControl);
        }
    }
}