using CsvHelper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FinalProjectForCSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OwnersCarsContext ownersCarsContext = new OwnersCarsContext();

        private delegate IEnumerable<Owner> ListHandler(IEnumerable<Owner> ownersList);

        private Owner selectedOwner;

        public enum SortOrderEnum { Id, Name, Cars }

        public static SortOrderEnum CurrSortOrder = SortOrderEnum.Id;

        public MainWindow()
        {
            InitializeComponent();
            RefreshAll(GetList);
        }

        private void RefreshAll(ListHandler listHandler, IEnumerable<Owner> ownersList = null)
        {
            lvOwners.ItemsSource = listHandler(ownersList);
            lvOwners.Items.Refresh();
            tbName.Text = "";
            imgButton.Source = null;
        }

        private IEnumerable<Owner> GetList(IEnumerable<Owner> ownersList = null)
        {
            return (from o in ownersCarsContext.Owners.Include("CarsInGarage") select o).ToList();
        }

        private IEnumerable<Owner> FilterList(IEnumerable<Owner> ownersList)
        {
            string keyword = tbSearch.Text;
            if ("" == keyword)
            {
                return ownersList;
            }
            return from o in ownersList where o.Name.Contains(keyword) select o;
        }

        private IEnumerable<Owner> SortList(IEnumerable<Owner> ownerList)
        {
            IEnumerable<Owner> result = null;
            switch (CurrSortOrder)
            {
                case SortOrderEnum.Id:
                    result = from o in ownerList orderby o.Id select o;
                    break;

                case SortOrderEnum.Name:
                    result = from o in ownerList orderby o.Name select o;
                    break;

                case SortOrderEnum.Cars:
                    result = from o in ownerList orderby o.CarsNumber descending select o;
                    break;
            }
            return result;
        }

        private ImageSource ByteToImage(byte[] imageData)
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageData);
                bitmapImage.EndInit();
                return bitmapImage as ImageSource;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        private byte[] ImageToBype(ImageSource imageData)
        {
            if (imageData != null)
            {
                byte[] bytes = null;
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imageData as BitmapSource));
                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }
                return bytes;
            }
            return null;
        }

        private void miExportToCsv_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.csv)|*.csv|All files (*.*)|*.*";
            if (true == saveFileDialog.ShowDialog())
            {
                using (CsvWriter csvWriter = new CsvWriter(new StreamWriter(saveFileDialog.FileName), CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(GetList());
                }
            }
        }

        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = "Image Files|*.jpg;*.png;*.bmp;*.gif";
            if (true == openFileDlg.ShowDialog())
            {
                imgButton.Source = new BitmapImage(new Uri(openFileDlg.FileName));
            }
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ownersCarsContext.Owners.Add(new Owner() { Id = 0, Name = tbName.Text, Photo = ImageToBype(imgButton.Source) });
                ownersCarsContext.SaveChanges();
                RefreshAll(GetList);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(this, "Adding cannot finished: \n" + ex.Message, "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            if (null == selectedOwner)
            {
                return;
            }
            MessageBoxResult messageBoxResult = MessageBox.Show(this, "Are you sure to delete: \n" + selectedOwner, "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (MessageBoxResult.Yes == messageBoxResult)
            {
                try
                {
                    ownersCarsContext.Owners.Remove(selectedOwner);
                    ownersCarsContext.SaveChanges();
                    RefreshAll(GetList);
                }
                catch (SystemException ex)
                {
                    MessageBox.Show(this, "Deletion cannot finished: \n" + ex.Message, "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (null == selectedOwner)
            {
                return;
            }
            try
            {
                selectedOwner.Name = tbName.Text;
                selectedOwner.Photo = ImageToBype(imgButton.Source);
                ownersCarsContext.SaveChanges();
                RefreshAll(GetList);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(this, "Adding cannot finished: \n" + ex.Message, "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btManageCars_Click(object sender, RoutedEventArgs e)
        {
            if (null == selectedOwner)
            {
                MessageBox.Show(this, "Please select one owner \n", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CarsManagementDialog carsManagementDialog = new CarsManagementDialog(ownersCarsContext, selectedOwner);
            if (true == carsManagementDialog.ShowDialog())
            {
                RefreshAll(GetList);
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshAll(FilterList, GetList());
        }

        private void btSortBy_Click(object sender, RoutedEventArgs e)
        {
            SortByDialog sortByDialog = new SortByDialog();
            if (true == sortByDialog.ShowDialog())
            {
                RefreshAll(SortList, GetList());
            }
        }

        private void lvOwners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (-1 == lvOwners.SelectedIndex)
            {
                selectedOwner = null;
                tblkId.Text = "";
                tbName.Text = "";
                imgButton.Source = null;
            }
            else
            {
                selectedOwner = lvOwners.SelectedItem as Owner;
                tblkId.Text = selectedOwner.Id.ToString();
                tbName.Text = selectedOwner.Name;
                imgButton.Source = ByteToImage(selectedOwner.Photo);
            }
        }
    }
}