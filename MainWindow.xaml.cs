using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace File_explorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            foreach (string drive in System.Environment.GetLogicalDrives())
            {
                var particije = NapraviIteme(drive);
                particije.Tag = drive;
                DriveInfo info = new DriveInfo(drive);
                if (info.IsReady)
                {
                    UcitavanjeFajlova(particije);
                }
                TreeV.Items.Add(particije);
            }
        }
        public TreeViewItem NapraviIteme(string Item)
        {
            TreeViewItem itemi = new TreeViewItem { Header = Item };
            return itemi;
        }

        public void UcitavanjeFajlova(TreeViewItem ucitaj)
        {
            DirectoryInfo info = new DirectoryInfo((string) ucitaj.Tag);
            try
            {
                foreach (DirectoryInfo dirs in info.GetDirectories())
                {
                    var subDir = NapraviIteme(dirs.Name);
                    subDir.Tag = dirs.FullName;
                    subDir.Expanded += Expand;
                    ucitaj.Items.Add(subDir);
                    try
                    {
                        if(dirs.GetDirectories().Length > 0 || dirs.GetFiles().Length > 0)
                        {
                            subDir.Items.Add("*");
                        }
                    }
                    catch { }
                }
                foreach(FileInfo file in info.GetFiles())
                {
                    var napraviFile = NapraviIteme(file.Name);
                    napraviFile.Tag = file.FullName;
                    ucitaj.Items.Add(napraviFile);
                }
            }
            catch { }
        }

        private void Expand(object posiljaoc, RoutedEventArgs non)
        {
            var treevi = (TreeViewItem)posiljaoc;
            if (treevi.Items.Contains("*"))
            {
                treevi.Items.Remove("*");
                UcitavanjeFajlova(treevi);
            }
        }

        public string velicina(long file)
        {
            decimal numToTest = file / 1048576;
            if (numToTest == 0)
            {
                return $"{((float)file / 1024).ToString("F2", CultureInfo.InvariantCulture)} KB";
            }
            else if (numToTest <= 1024) 
            {
                return $"{numToTest} MB";
            }
            else
            {
                return $"{((float)file / 1073741824).ToString("F2", CultureInfo.InvariantCulture)} GB";
            }
        }

        private void TreeV_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var poslato = (TreeView)sender;
            if (poslato.SelectedItem != null)
            {
                var selitem = (TreeViewItem)poslato.SelectedItem;
                if (Directory.Exists((string)selitem.Tag))
                {
                    DirectoryInfo dirinfo = new DirectoryInfo((string)selitem.Tag);
                    DIF.Content = "Folder sadrzi:";
                    tip.Content = "Direktorijum";
                    rta.Content = dirinfo.FullName;
                    try
                    {
                        vel.Content = $"{dirinfo.GetDirectories().Length + dirinfo.GetFiles().Length} fajlova";
                    }
                    catch { }
                 }

                else if (File.Exists((string)selitem.Tag))
                {
                    DIF.Content = "Velicina fajla";
                    FileInfo fileinfo = new FileInfo((string)selitem.Tag);
                    tip.Content = fileinfo.Extension;
                    rta.Content = fileinfo.FullName;
                    vel.Content = velicina(fileinfo.Length);
                }
            }
        }
    }
}
