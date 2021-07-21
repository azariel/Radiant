using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;
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
        public void RefreshProducts(List<RadiantProductModel> aProducts)
        {
            this.Dispatcher.Invoke(() =>
            {
                dataGridProducts.IsReadOnly = true;

                dataGridProducts.Items.Clear();

                foreach (var _ViewModel in aProducts.Select(s => new ProductViewModel(s)))
                    dataGridProducts.Items.Add(_ViewModel);

                dataGridProducts.IsReadOnly = false;
            });
        }
    }
}
