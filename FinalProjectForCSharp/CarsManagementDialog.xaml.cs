using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FinalProjectForCSharp
{
    /// <summary>
    /// Interaction logic for CarsManagementDialog.xaml
    /// </summary>
    public partial class CarsManagementDialog : Window
    {
        private readonly OwnersCarsContext ownersCarsContext;

        private delegate IEnumerable<Car> ListHandler(IEnumerable<Car> ownersList);

        private readonly Owner owner;

        private Car selectedCar;

        public CarsManagementDialog()
        {
            InitializeComponent();
        }

        public CarsManagementDialog(OwnersCarsContext ownersCarsContext, Owner owner)
        {
            this.ownersCarsContext = ownersCarsContext;
            this.owner = owner;
            InitializeComponent();
            tblkOwnerName.Text = owner.Name;
            RefreshAll(GetList);
        }

        private void RefreshAll(ListHandler listHandler, IEnumerable<Car> carsList = null)
        {
            lvCars.ItemsSource = listHandler(carsList);
            lvCars.Items.Refresh();
            tbMakeModel.Text = "";
        }

        private IEnumerable<Car> GetList(IEnumerable<Car> carsList = null)
        {
            return (from c in ownersCarsContext.Cars where c.OwnerId == owner.Id select c).ToList();
        }

        private void btDone_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ownersCarsContext.Cars.Add(new Car() { MakeModel = tbMakeModel.Text, OwnerId = owner.Id });
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
            if (null == selectedCar)
            {
                return;
            }
            MessageBoxResult messageBoxResult = MessageBox.Show(this, "Are you sure to delete: \n" + selectedCar, "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (MessageBoxResult.Yes == messageBoxResult)
            {
                try
                {
                    ownersCarsContext.Cars.Remove(selectedCar);
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
            if (null == selectedCar)
            {
                return;
            }
            try
            {
                selectedCar.MakeModel = tbMakeModel.Text;
                ownersCarsContext.SaveChanges();
                RefreshAll(GetList);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(this, "Adding cannot finished: \n" + ex.Message, "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lvCars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (-1 == lvCars.SelectedIndex)
            {
                selectedCar = null;
                tblkId.Text = "";
                tbMakeModel.Text = "";
            }
            else
            {
                selectedCar = lvCars.SelectedItem as Car;
                tblkId.Text = selectedCar.Id.ToString();
                tbMakeModel.Text = selectedCar.MakeModel;
            }
        }
    }
}