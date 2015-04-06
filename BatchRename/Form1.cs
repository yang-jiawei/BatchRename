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
    public partial class frm_Main : Form
    {
        private string _prefix = "[";
        private string _postfix = "]";
        private string _dash = "-";
        private bool _isReplace;
        private string _stringToReplace;
        private string _configFile;
        private string _directory;
        private List<Files.NamePair> _namePairs = new List<Files.NamePair>();
        private List<Files.NamePair> _pairsToReplace = new List<Files.NamePair>();
        private Files[] _files;
        private char[] _charsToTrim = {' ', '[', ']', '\"', '\'', '/', ','};
        private char[] _charsToCut = {':', '：'};
        private char[] _charsToSplit = {',', '，', '/'};

        public frm_Main()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            tb_Prefix.Text = ConfigAppSettings.GetSettingString("Prefix", "[");
            tb_Postfix.Text = ConfigAppSettings.GetSettingString("Postfix", "]");
            tb_Dash.Text = ConfigAppSettings.GetSettingString("Dash", "-");
            cb_IsReplace.Checked = ConfigAppSettings.GetSettingBool("IsReplace", false);
            tb_StringToReplace.Text = ConfigAppSettings.GetSettingString("StringToReplace", "");
            tb_ConfigFile.Text = ConfigAppSettings.GetSettingString("ConfigFile", "");
        }

        private void SaveSettings()
        {
            ConfigAppSettings.SetSettingString("Prefix", tb_Prefix.Text);
            ConfigAppSettings.SetSettingString("Postfix", tb_Postfix.Text);
            ConfigAppSettings.SetSettingString("Dash", tb_Dash.Text);
            ConfigAppSettings.SetSettingString("IsReplace", cb_IsReplace.Checked.ToString());
            ConfigAppSettings.SetSettingString("StringToReplace", tb_StringToReplace.Text);
            ConfigAppSettings.SetSettingString("ConfigFile", tb_ConfigFile.Text);
        }

        private void LoadParameters()
        {
            string tempString = tb_Prefix.Text;
            if (tempString.Length > 0)
            {
                _prefix = tempString;
            }
            tempString = tb_Postfix.Text;
            if (tempString.Length > 0)
            {
                _postfix = tempString;
            }
            tempString = tb_Dash.Text;
            if (tempString.Length > 0)
            {
                _dash = tempString;
            }
            _isReplace = cb_IsReplace.Checked;
            tempString = tb_StringToReplace.Text;
            if (tempString.Length > 0)
            {
                _stringToReplace = tempString;
            }
            _configFile = tb_ConfigFile.Text.Trim();
            _directory = tb_Directory.Text.Trim();
        }

        private void btn_Directory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                tb_Directory.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btn_ConfigFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = @"请选择文件",
                Filter = @"所有文件 (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            try
            {
                DialogResult dr = openFileDialog.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    tb_ConfigFile.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                statusStrip.Text = ex.Message;
            }
        }

        private void tsbtn_preview_Click(object sender, EventArgs e)
        {
            SaveSettings();
            LoadParameters();

            //分析config file
            FileInfo fileInfo = new FileInfo(_configFile); //根据选择文件打开获取长度信息
            if (fileInfo.Length > 0)
            {
                try
                {
                    StreamReader fs = new StreamReader(_configFile, System.Text.Encoding.GetEncoding("gb2312"));
                    string variable;
                    while ((variable = fs.ReadLine()) != null) //读取一行
                    {
                        variable = variable.Trim(_charsToTrim);
                        if (variable.StartsWith("//") || variable.Length <= 0)
                        {
                            continue;
                        }
                        string[] tempStrings = variable.Split(_charsToCut);
                        string tag = tempStrings[0].Trim(_charsToTrim);
                        string[] variables = tempStrings[1].Split(_charsToSplit);
                        foreach (string s in variables)
                        {
                            _namePairs.Add(new Files.NamePair {Tag = tag, StringToFind = s});
                        }
                    }
                    fs.Close();
                }
                catch (Exception ex)
                {
                    statusStrip.Text = ex.Message;
                }
            }

            //分析String to replace
            if (_stringToReplace.Length > 0)
            {
                try
                {

                    string[] stringToReplace = _stringToReplace.Split(_charsToSplit);
                    foreach (string s in stringToReplace)
                    {
                        string[] tempStrings = s.Split(_charsToCut);
                        if (tempStrings.Length > 1)
                        {
                            _pairsToReplace.Add(new Files.NamePair
                            {
                                StringToFind = tempStrings[0].Trim(_charsToTrim),
                                Tag = tempStrings[1].Trim(_charsToTrim)
                            });
                        }
                        else if (tempStrings.Length > 0)
                        {
                            _pairsToReplace.Add(new Files.NamePair
                            {
                                StringToFind = tempStrings[0].Trim(_charsToTrim),
                                Tag = ""
                            });
                        }
                    }
                }
                catch (Exception ex)
                {

                    statusStrip.Text = ex.Message;
                }
            }

            DataTable tmpDataTable = new DataTable();
            tmpDataTable.Columns.AddRange(new[]
            {
                new DataColumn("原文件名", typeof (string)),
                new DataColumn("新文件名", typeof (string))
            });

            DirectoryInfo directoryInfo = new DirectoryInfo(_directory);
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            _files = new Files[fileInfos.Length];
            for (int index = 0; index < _files.Length; index++)
            {
                _files[index] = new Files
                {
                    Directory = _directory,
                    HistoryName = fileInfos[index].Name,
                    Prefix = _prefix,
                    Postfix = _postfix,
                    Dash = _dash,
                    IsReplace = _isReplace,
                    Status = false,
                    StringToReplace = _pairsToReplace,
                    NameTag = new Files.NamePair {StringToFind = "", Tag = ""},
                };

                foreach (Files.NamePair namePair in _namePairs)
                {
                    if (_files[index].HistoryName.Contains(namePair.StringToFind))
                    {
                        _files[index].NameTag = new Files.NamePair
                        {
                            StringToFind = namePair.StringToFind,
                            Tag = namePair.Tag,
                        };
                        break;
                    }
                }

                _files[index].GetNewName();
                DataRow dr = tmpDataTable.NewRow();
                dr["原文件名"] = _files[index].HistoryName;
                dr["新文件名"] = _files[index].NewName;
                tmpDataTable.Rows.Add(dr);
            }

            dataGridView.DataSource = tmpDataTable;
        }
    }
}
