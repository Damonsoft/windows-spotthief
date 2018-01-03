using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpotThief
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> wallpapers = new List<string>();

        string workDir = "";
        string helloDir = "";

        int imageCount = 0;

        string rootNodeName = "Images";
        string currNodeName = "";

        public MainWindow()
        {
            InitializeComponent();

            RefreshHelloDir();
            RefreshWorkDir();
            RefreshFiles();
        }

        public void PrepareUI()
        {
            // Get the root node...
            TreeViewItem Item = (TreeViewItem)FileView.Items[0];
            // Clear all the children...
            Item.Items.Clear();

        }

        public void RefreshWorkDir()
        {
            // Get a path to a random string encoded folder
            workDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()));
            // Create said folder :)
            Directory.CreateDirectory(workDir);
        }

        public void RefreshHelloDir()
        {
            helloDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets");
        }

        public void RefreshFiles()
        {
            // Prepare the UI...
            PrepareUI();
            // Get a list of files in the windows hello folder...
            string[] files = Directory.GetFiles(helloDir);
            // For each file...
            foreach (string file in files)
            {
                try
                {
                    // Make a copy in our temp folder...Also changing the destination file to a jpg...
                    File.Copy(file, System.IO.Path.Combine(workDir, "wallpaper(" + imageCount++ + ").jpg"));
                }
                catch (System.IO.IOException)
                {
                    // If we can't get the file ignore it...
                    continue;
                }
            }
            // Now get the list of files in our folder that made it...
            files = Directory.GetFiles(workDir);
            //For each file...
            foreach (string file in files)
            {
                // Open the image...
                try
                {
                    BitmapImage item = new BitmapImage(new Uri(file));
                    // Check to make sure it's a wallpaper...
                    if (item.Width != item.Height &&
                        !(item.Width <= 300) &&
                        !(item.Height <= 300))
                    {
                        // Add the file to the list...
                        wallpapers.Add(file);
                        // Create a new node presenting the file...
                        TreeViewItem node = new TreeViewItem();
                        // Set it's header as the file name...
                        node.Header = System.IO.Path.GetFileName(file);
                        // Set it's freaking color :(
                        node.Foreground = new SolidColorBrush(Colors.White);
                        // Add it to the tree view...
                        ((TreeViewItem)FileView.Items[0]).Items.Add(node);
                    }
                }
                catch (Exception)
                {
                    // Not all files are image files...
                    continue;
                }
            }
        }

        private void FileView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Get the selected node...
            TreeViewItem item = (TreeViewItem)e.NewValue;
            // Get it's text...
            currNodeName = item.Header.ToString();
            // If it's the root node then ignore it...
            if (currNodeName != rootNodeName)
            {
                // Otherwise open the file for viewing...
                currNodeName = System.IO.Path.Combine(workDir, currNodeName);
                // Set the picture boxes image...
                workingImage.Source = new BitmapImage(new Uri(currNodeName));
            }
        }

        private void Saveas_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = System.IO.Path.GetFileName(currNodeName); // Default file name
            dlg.DefaultExt = ".jpg"; // Default file extension
            dlg.Filter = "Picture Documents (.jpg)|*.jpg"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Copy the file to that path...
                File.Copy(currNodeName, dlg.FileName);
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // Get a list of all files...
                string[] files = Directory.GetFiles(workDir);

                foreach (string file in files)
                {
                    // Delete em all :)
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                // Finally kill our temp dir *-|===>
                Directory.Delete(workDir, false);
            }
            catch(Exception)
            {
                // :(
            }
        }
    }
}
