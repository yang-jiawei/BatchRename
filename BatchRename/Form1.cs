using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatchRename
{
    public partial class Form1 : Form
    {
        private char prefix;
        private char Postfix;
        private char dash;
        private string stringToReplace;

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_Directory_Click(object sender, EventArgs e)
        {
            string folderPath = "";
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog {};
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                folderPath = folderBrowserDialog.SelectedPath;
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            File[] files = new File[fileInfos.Length];
            for (int index = 0; index < files.Length; index++)
            {
                files[index] = new File()
                {
                    Directory = folderPath,
                    Prefix = prefix,
                    Postfix = Postfix,
                    Dash = dash,
                    HistoryName = fileInfos[index].Name,
                    Category = "",
                    IsReplace = false,
                    NewName = "",
                    Status = false,
                    StringToFind = "",
                    StringToReplace = stringToReplace
                };

            }
        }

        private void btn_ConfigFile_Click(object sender, EventArgs e)
        {

        }
    }
}
