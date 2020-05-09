using System.Windows;

namespace FinalProjectForCSharp
{
    /// <summary>
    /// Interaction logic for SortByDialog.xaml
    /// </summary>
    public partial class SortByDialog : Window
    {
        public SortByDialog()
        {
            InitializeComponent();
        }

        private void btApply_Click(object sender, RoutedEventArgs e)
        {
            if (true == rbId.IsChecked)
            {
                MainWindow.CurrSortOrder = MainWindow.SortOrderEnum.Id;
            }
            else if (true == rbName.IsChecked)
            {
                MainWindow.CurrSortOrder = MainWindow.SortOrderEnum.Name;
            }
            else if (true == rbCars.IsChecked)
            {
                MainWindow.CurrSortOrder = MainWindow.SortOrderEnum.Cars;
            }
            DialogResult = true;
        }
    }
}