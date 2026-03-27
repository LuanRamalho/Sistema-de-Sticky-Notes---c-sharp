using System;
using System.Drawing;
using System.Windows.Forms;

namespace StickyNotesApp
{
    public class StickyNoteControl : UserControl
    {
        private Note _note;
        private RichTextBox _txtContent = null!;
        private Button _btnDelete = null!;
        private Button _btnColor = null!;
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
            this.Margin = new Padding(12);

            _btnDelete = new Button {
                Text = "✕",
                Size = new Size(25, 25),
                Location = new Point(192, 3),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            _btnDelete.FlatAppearance.BorderSize = 0;
            _btnDelete.Click += (s, e) => {
                if (MessageBox.Show("Excluir nota?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    DatabaseHelper.DeleteNote(_note);
                    OnDeleteRequested?.Invoke(this);
                }
            };

            _btnColor = new Button {
                Text = "🎨",
                Size = new Size(25, 25),
                Location = new Point(165, 3),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnColor.FlatAppearance.BorderSize = 0;
            _btnColor.Click += (s, e) => ChangeColor();

            _txtContent = new RichTextBox {
                Location = new Point(10, 35),
                Size = new Size(200, 175),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe Print", 11),
                ScrollBars = RichTextBoxScrollBars.None
            };
            
            _txtContent.TextChanged += (s, e) => {
                _note.Content = _txtContent.Text;
                DatabaseHelper.SaveNote(_note);
            };

            this.Controls.AddRange(new Control[] { _btnDelete, _btnColor, _txtContent });
        }

        private void ChangeColor()
        {
            using ColorDialog cd = new();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                ApplyColor(cd.Color);
                _note.Color = ColorTranslator.ToHtml(cd.Color);
                DatabaseHelper.SaveNote(_note);
            }
        }

        private void ApplyColor(Color color)
        {
            // Define o fundo do controle e dos componentes
            this.BackColor = color;
            _txtContent.BackColor = color;
            _btnDelete.BackColor = color;
            _btnColor.BackColor = color;
            
            // --- NOVA LÓGICA DE CONTRASTE ---

            // Calcula a luminância percebida (fórmula W3C)
            // O valor resultante estará entre 0.0 (preto puro) e 1.0 (branco puro)
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255.0;

            // Se a luminância for maior que 0.5 (fundo claro), usa preto.
            // Caso contrário (fundo escuro), usa branco.
            Color contrastColor = luminance > 0.5 ? Color.Black : Color.White;

            // Aplica a cor contrastante aos ícones e textos dos botões
            _btnDelete.ForeColor = contrastColor; 
            _btnColor.ForeColor = contrastColor;
            
            // Opcional: Se você quiser que o texto da nota também mude, descomente a linha abaixo:
            // _txtContent.ForeColor = contrastColor; 
        }
    }
}