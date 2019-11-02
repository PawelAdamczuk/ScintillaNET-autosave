using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace InfiniNotes
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Scintilla.SavePointReached += Scintilla_SavePointReached;
            Scintilla.SavePointLeft += Scintilla_SavePointLeft;
            Scintilla.TextChanged += Scintilla_TextChanged;

            if(!File.Exists(@"c:\tmp\tmp.txt"))
#pragma warning disable CS0642 // Possible mistaken empty statement
                using (File.Create(@"c:\tmp\tmp.txt")) ;
#pragma warning restore CS0642 // Possible mistaken empty statement

            using (StreamReader sr = new StreamReader(@"c:\tmp\tmp.txt", Encoding.UTF8))
            {
                var fileContent = sr.ReadToEnd();
                Scintilla.Text = fileContent;
                Scintilla.SetSavePoint();
            }

            SaveTimer.Start();
        }

        private void Scintilla_SavePointLeft(object sender, EventArgs e)
        {
            tsslSaving.Text = "Dirty";
        }

        private void Scintilla_SavePointReached(object sender, EventArgs e)
        {
            tsslSaving.Text = "Saved";
        }

        private void Scintilla_TextChanged(object sender, EventArgs e)
        {
            SaveTimer.Stop();
            SaveTimer.Start();
        }

        private async void SaveTimer_Tick(object sender, EventArgs e)
        {
            if (!Scintilla.Modified)
                return;

            SaveTimer.Enabled = false;

            var filePath = @"c:\tmp\tmp.txt";
            byte[] encodedText = Encoding.UTF8.GetBytes(Scintilla.Text);

            using (FileStream sourceStream = new FileStream(filePath,
            FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };

            Scintilla.SetSavePoint();
            SaveTimer.Enabled = true;
        }
    }
}
