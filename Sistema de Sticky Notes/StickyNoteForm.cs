using System;
using System.Drawing;
using System.Windows.Forms;

public class StickyNoteForm : Form
{
    private Note _note;
    private RichTextBox _txtContent;
    private Panel _topBar;

    public StickyNoteForm(Note note)
    {
        _note = note;
        InitializeComponent();
        this.BackColor = ColorTranslator.FromHtml(_note.Color);
        _txtContent.Text = _note.Content;
    }

    private void InitializeComponent()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.Size = new Size(250, 250);
        this.Padding = new Padding(5);

        _topBar = new Panel { Dock = DockStyle.Top, Height = 30, Cursor = Cursors.SizeAll };
        _topBar.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, 0xA1, 0x2, 0); } };

        var btnClose = new Button { Text = "X", Dock = DockStyle.Right, Width = 30, FlatStyle = FlatStyle.Flat };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.Click += (s, e) => { DatabaseHelper.DeleteNote(_note.Id); this.Close(); };

        _txtContent = new RichTextBox { 
            Dock = DockStyle.Fill, 
            BorderStyle = BorderStyle.None, 
            BackColor = ColorTranslator.FromHtml(_note.Color),
            Font = new Font("Segoe Print", 11)
        };
        _txtContent.TextChanged += (s, e) => { _note.Content = _txtContent.Text; DatabaseHelper.SaveNote(_note); };

        _topBar.Controls.Add(btnClose);
        this.Controls.Add(_txtContent);
        this.Controls.Add(_topBar);
    }

    // Código para permitir arrastar a janela sem borda
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool ReleaseCapture();
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
}