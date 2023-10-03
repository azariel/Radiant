using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryClient.View.Products
{
    /// <summary>
    /// Interaction logic for ProductsListView.xaml
    /// </summary>
    public partial class ProductsListView : UserControl
    {
        public ProductsListView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Note: This function must be thread safe.
        /// </summary>
        public void RefreshProducts(List<RadiantClientProductModel> aProducts)
        {
            this.Dispatcher.Invoke(() =>
            {
                dataGridProducts.IsReadOnly = true;

                dataGridProducts.Items.Clear();

                foreach (RadiantClientProductModel _Product in aProducts)
                {
                    var _ProductViewModel = new ProductViewModel(_Product);

                    dataGridProducts.Items.Add(_ProductViewModel);
                }

                dataGridProducts.IsReadOnly = false;
            });
        }

        private void OnMouseUpOnRow(object aSender, MouseButtonEventArgs aE)
        {
            if (aSender is not DataGridRow _Row)
                return;

            // Hide all rows details
            for (int i = 0; i < dataGridProducts.Items.Count; i++)
            {
                DataGridRow _RowToHide = (DataGridRow)dataGridProducts.ItemContainerGenerator.ContainerFromIndex(i);

                if (_Row != _RowToHide)
                    _RowToHide.DetailsVisibility = Visibility.Collapsed;
            }

            // Show or hide selected row
            _Row.DetailsVisibility = _Row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
