using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Radiant.Custom.ProductsHistoryCommon.DataBase;

namespace ProductsHistoryClient.View.Products
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

        private void OnMouseDownOnRow(object aSender, MouseButtonEventArgs aE)
        {
            if (aSender is DataGridRow row)
                row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
