using System;
using System.Drawing;
using System.Windows.Forms;

namespace StickyNotesApp
{
    public class StickyNoteControl : UserControl
    {
        private Note _note;
        private RichTextBox _txtContent;
        private Button _btnDelete;
        private Button _btnColor; // Novo botão para trocar a cor
        public event Action<StickyNoteControl>? OnDeleteRequested;

        public StickyNoteControl(Note note)
        {
            _note = note;
            InitializeComponent();
            ApplyColor(ColorTranslator.FromHtml(_note.Color));
            _txtContent.Text = _note.Content;
        }

        private void InitializeComponent()
        {
            this.Size = new Size(220, 220);
            this.Margin = new Padding(10);
            this.BorderStyle = BorderStyle.FixedSingle; // Leve borda para separar as notas

            // Botão de Excluir (X)
            _btnDelete = new Button {
                Text = "X",
                Width = 25,
                Height = 25,
                Location = new Point(190, 2),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
            _btnDelete.FlatAppearance.BorderSize = 0;
            _btnDelete.Click += (s, e) => {
                var result = MessageBox.Show("Excluir esta nota?", "Confirmar", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes) {
                    DatabaseHelper.DeleteNote(_note.Id);
                    OnDeleteRequested?.Invoke(this);
                }
            };

            // Botão de Cor (Ícone de paleta simplificado)
            _btnColor = new Button {
                Text = "🎨",
                Width = 25,
                Height = 25,
                Location = new Point(160, 2),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnColor.FlatAppearance.BorderSize = 0;
            _btnColor.Click += (s, e) => ChangeColor();

            // Campo de Texto
            _txtContent = new RichTextBox {
                Location = new Point(5, 30),
                Size = new Size(210, 185),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe Print", 11),
                ScrollBars = RichTextBoxScrollBars.None
            };
            
            _txtContent.TextChanged += (s, e) => {
                _note.Content = _txtContent.Text;
                DatabaseHelper.SaveNote(_note);
            };

            this.Controls.Add(_btnDelete);
            this.Controls.Add(_btnColor);
            this.Controls.Add(_txtContent);
        }

        private void ChangeColor()
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = this.BackColor;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    ApplyColor(cd.Color);
                    
                    // Converte a cor para Hexadecimal para salvar no SQLite
                    _note.Color = "#" + cd.Color.R.ToString("X2") + cd.Color.G.ToString("X2") + cd.Color.B.ToString("X2");
                    DatabaseHelper.SaveNote(_note);
                }
            }
        }

        private void ApplyColor(Color color)
        {
            this.BackColor = color;
            _txtContent.BackColor = color;
            _btnDelete.BackColor = color;
            _btnColor.BackColor = color;

            // Ajusta a cor do texto do botão X para ficar visível (contraste)
            _btnDelete.ForeColor = (color.R * 0.299 + color.G * 0.587 + color.B * 0.114) > 186 ? Color.Black : Color.White;
        }
    }
}