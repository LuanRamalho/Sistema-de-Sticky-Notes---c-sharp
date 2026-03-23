using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

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
            this.Text = "Meus Sticky Notes";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Botão Superior
            var btnAdd = new Button {
                Text = "+ Nova Nota",
                Dock = DockStyle.Top,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightGreen,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAdd.Click += (s, e) => CreateNewNote();

            // Painel onde as notas ficarão "presas"
            _pnlContainer = new FlowLayoutPanel {
                Dock = DockStyle.Fill,
                AutoScroll = true, // Adiciona barra de rolagem se houver muitas notas
                Padding = new Padding(20),
                BackColor = Color.Transparent
            };

            this.Controls.Add(_pnlContainer);
            this.Controls.Add(btnAdd);
        }

        private void CreateNewNote()
        {
            string[] colors = { "#FFF740", "#FF7EB9", "#7AFCFF", "#98FF98", "#DDA0DD" };
            var random = new Random();
            var note = new Note { Content = "", Color = colors[random.Next(colors.Length)] };
            
            DatabaseHelper.SaveNote(note); // Salva no banco para gerar o ID
            AddNoteToUI(note);
        }

        private void LoadExistingNotes()
        {
            _pnlContainer.Controls.Clear();
            var notes = DatabaseHelper.GetAllNotes();
            foreach (var note in notes)
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