using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PathsSearcher
{
    public partial class Form1 : Form
    {
        static string[] drives = System.Environment.GetLogicalDrives();
        static string pathfile = Application.StartupPath.ToString() + @"DirPaths.txt";

        private static string keyWord;
        private static string pathFind;
        private string drive;
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();


        static void WalkDirectoryTree(System.IO.DirectoryInfo root)
        {

            System.IO.DirectoryInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            try
            {
                files = root.GetDirectories(keyWord);
            }

            catch (UnauthorizedAccessException e)
            {

                log.Add(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.DirectoryInfo fi in files)
                {
                    using (FileStream fs = new FileStream(pathfile, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        fs.Seek(0, SeekOrigin.End);
                        using (StreamWriter writer = new StreamWriter(fs))
                        {
                            writer.WriteLine(fi.FullName);
                        }
                    }

                }

                
                try
                {
                    subDirs = root.GetDirectories();
                }

                catch (UnauthorizedAccessException e)
                {

                    log.Add(e.Message);
                }

                catch (System.IO.DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                }

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    WalkDirectoryTree(dirInfo);
                }
            }
        }

        static void WalkDirectoryTreeFile(System.IO.DirectoryInfo root)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;


            try
            {
                if ((keyWord == ".jpg") || (keyWord == ".pdf") || (keyWord == ".exe")
                || (keyWord == ".zip") || (keyWord == ".txt") || (keyWord == ".lnk")
                || (keyWord == ".json") || (keyWord == ".xls") || (keyWord == ".rar")
                || (keyWord == ".img") || (keyWord == ".mp4") || (keyWord == ".docx")
                || (keyWord == ".dat") || (keyWord == ".rtf") || (keyWord == ".html")
                || (keyWord == ".exl") || (keyWord == ".dll")
                    ) { keyWord = "*" + keyWord; }
                files = root.GetFiles(keyWord);
            }

            catch (UnauthorizedAccessException e)
            {

                log.Add(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {

                    using (FileStream fs = new FileStream(pathfile, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        fs.Seek(0, SeekOrigin.End);
                        using (StreamWriter writer = new StreamWriter(fs))
                        {
                            writer.WriteLine(fi.FullName);
                        }
                    }
                }


                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {

                    WalkDirectoryTreeFile(dirInfo);
                }
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBoxKey_TextChanged(object sender, EventArgs e)
        {
             keyWord = textBoxKey.Text;
        }

        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {
             pathFind = textBoxPath.Text;
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            if (keyWord == null)
            {
                keyWord = "";
            }
            if (pathFind == null)
            {
                pathFind = "";
            }
            if (drive == null)
            {
                drive = "";
            }
            if (drive == "" && pathFind == "")
            {
                MessageBox.Show("Укажите путь или диск");
            }
            else
            {
                BackgroundWorker bw;
                progressBar1.Visible = true;
                bw = new BackgroundWorker();
                bw.DoWork += (obj, ea) => TasksAsync(1);
                bw.RunWorkerAsync();
            }
        }
        private async void TasksAsync(int times)
        {

            if (drive != "" && pathFind != "")
            {
                string path = pathFind;
                DirectoryInfo di = new DirectoryInfo(path);
                if (keyWord.Contains('.')) { WalkDirectoryTreeFile(di); }

                WalkDirectoryTree(di);


            }
            else if (drive != "")
            {
                DriveInfo di = new DriveInfo(drive);

                if (!di.IsReady)
                {
                    MessageBox.Show("The drive {0} could not be read", di.Name);
                    Application.Exit();
                }
                DirectoryInfo rootDir = di.RootDirectory;
                if (keyWord.Contains('.')) { WalkDirectoryTreeFile(rootDir); }

                WalkDirectoryTree(rootDir);

            }

            else
            {
                string path = pathFind;
                DirectoryInfo di = new DirectoryInfo(path);
                if (keyWord.Contains('.')) { WalkDirectoryTreeFile(di); }

                WalkDirectoryTree(di);

            }
            MessageBox.Show(
                "Выполнено",
                "Сообщение",
                MessageBoxButtons.OK,
                MessageBoxIcon.None,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);

            Application.Exit();
        }
        private void ButtonReset_Click(object sender, EventArgs e)
        {
            keyWord = "";
            drive = "";
            pathFind = "";
            textBoxKey.Clear();
            comboBox1.ResetText();
            textBoxPath.Clear();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            drive = comboBox1.SelectedItem.ToString();
        }

        private async void  Form1_Load(object sender, EventArgs e)
        {
             progressBar1.Visible = false;

            if (File.Exists(pathfile))
            {
                File.Delete(pathfile);
            }
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;



            for (int i = 0; i < drives.Length; i++)
            {
                comboBox1.Items.Insert(i, drives[i]);
            }
            comboBox1.Items.Insert(drives.Length, "");
        }
    }
}
