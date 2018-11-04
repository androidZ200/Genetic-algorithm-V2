using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace генетический_алгоритм__версия_2_
{
    public partial class FormOptions : Form
    {
        FormMain form;
        public FormOptions(FormMain f)
        {
            InitializeComponent();
            AutoSaveCheckBox.Checked = f.AutoSave;
            form = f;
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            string save = null;
            save = form.Getsave();
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string fileName = saveFileDialog1.FileName;
            System.IO.File.WriteAllText(fileName, save);
            form.fileName = fileName;
            MessageBox.Show("Файл сохранен");
        }
        private void OpenButton_Click(object sender, EventArgs e)
        {
            string open;
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string fileName = openFileDialog1.FileName;
            form.fileName = fileName;
            open = System.IO.File.ReadAllText(fileName);
            form.SetSave(open);
            MessageBox.Show("Файл открыт");
        }
        private void FormOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (form.fileName != null)
                form.AutoSave = AutoSaveCheckBox.Checked;
            else if(AutoSaveCheckBox.Checked) MessageBox.Show("Путь не указан");
        }
    }
}
