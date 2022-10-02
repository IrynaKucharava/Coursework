using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
        }

        private string key = string.Empty;
        private static string DELIMITER = " : ";
        
        private static string getFile(StringBuilder strResult, string key, ListView list) 
        {
            string resStr = string.Empty;
            list.Items.Clear();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM " + key);

            foreach (ManagementObject obj in searcher.Get())
            {
                ListViewGroup listViewGroup;

                try
                {
                    listViewGroup = list.Groups.Add(obj["Name"].ToString(), obj["Name"].ToString());
                }
                catch (Exception ex)
                {
                    listViewGroup = list.Groups.Add(obj.ToString(), obj.ToString());
                }

                foreach (PropertyData data in obj.Properties)
                {
                    ListViewItem item = new ListViewItem(listViewGroup);
                    
                    item.Text = data.Name;
                    
                    if (data.Value != null && !string.IsNullOrEmpty(data.Value.ToString()))
                    {
                        strResult.Append(data.Name).Append(DELIMITER);
                        switch (data.Value.GetType().ToString())
                        {
                            case "System.String[]":

                                string[] stringData = data.Value as string[];
                                string resStr1 = string.Empty;
                                foreach (string s in stringData)
                                {
                                    resStr += $"{s}";
                                }
                                strResult.AppendLine(resStr1);

                                break;
                            case "System.UInt16[]":

                                ushort[] ushortData = data.Value as ushort[];
                                string resStr2 = string.Empty;

                                foreach (ushort u in ushortData)
                                {
                                    resStr2 += $"{Convert.ToString(u)}";
                                }
                                strResult.AppendLine(resStr2);

                                break;

                            default:
                                strResult.AppendLine(data.Value.ToString());
                                break;
                        }
                    }
                }
            }
            return strResult.ToString();
        }
        
        private void GetHardWareInfo(string key, ListView list)
        {
            list.Items.Clear();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM " + key);

            try
            {
                foreach (ManagementObject obj in searcher.Get()) 
                {
                    ListViewGroup listViewGroup;

                    try
                    {
                        listViewGroup = list.Groups.Add(obj["Name"].ToString(), obj["Name"].ToString());
                    }
                    catch (Exception ex)
                    {
                        listViewGroup = list.Groups.Add(obj.ToString(), obj.ToString());
                    }

                    if (obj.Properties.Count == 0) 
                    {
                        MessageBox.Show("Не удалось получить информацию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    foreach (PropertyData data in obj.Properties)
                    {
                        ListViewItem item = new ListViewItem(listViewGroup);
                        if (list.Items.Count % 2 != 0)
                        {
                            item.BackColor = Color.White;
                        }
                        else 
                        {
                            item.BackColor = Color.WhiteSmoke;
                        }

                        item.Text = data.Name;
                        if (data.Value != null && !string.IsNullOrEmpty(data.Value.ToString()))
                        {
                            switch (data.Value.GetType().ToString())
                            {
                                case "System.String[]":

                                    string[] stringData = data.Value as string[];
                                    string resStr1 = string.Empty;
                                    foreach (string s in stringData) 
                                    {
                                        resStr1 += $"{s}";
                                    }
                                    item.SubItems.Add(resStr1);

                                    break;
                                case "System.UInt16[]":

                                    ushort[] ushortData = data.Value as ushort[];
                                    string resStr2 = string.Empty;

                                    foreach (ushort u in ushortData)
                                    {
                                        resStr2 += $"{Convert.ToString(u)}";
                                    }
                                    item.SubItems.Add(resStr2);

                                    break;
                                
                                default:
                                   item.SubItems.Add(data.Value.ToString());
                                    break;
                            }
                            list.Items.Add(item);
                            
                        }
                    }
                }
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (toolStripComboBox1.SelectedItem.ToString())
            {
                case "CPU":
                    key = "Win32_Processor";
                    break;
                case "Graphics card":
                    key = "Win32_VideoController";
                    break;
                case "Chipset":
                    key = "Win32_IDEController";
                    break;
                case "Battery":
                    key = "Win32_Battery";
                    break;
                case "BIOS":
                    key = "Win32_BIOS";
                    break;
                case "RAM":
                    key = "Win32_PhysicalMemory";
                    break;
                case "Cache memory":
                    key = "Win32_CacheMemory";
                    break;
                case "USB":
                    key = "Win32_USBController";
                    break;
                case "Disk":
                    key = "Win32_DiskDrive";
                    break;
                case "Logical disks":
                    key = "Win32_LogicalDisk";
                    break;
                case "Keyboard":
                    key = "Win32_Keyboard";
                    break;
                case "Network adapters":
                    key = "Win32_NetworkAdapter";
                    break;
                case "Users":
                    key = "Win32_Account";
                    break;
                default:
                    key = "Win32_Processor";
                    break;
            }
            GetHardWareInfo(key, listView1);
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripComboBox1.SelectedIndex = 0;
        }

        private void aboutInfCollectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 newForm = new Form2();
            newForm.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog1.FileName;
            StringBuilder result = new StringBuilder();
            System.IO.File.WriteAllText(filename, getFile(result, key, listView1));
            GetHardWareInfo(key, listView1);
        }
    }
}
