using System.Threading.Tasks;
using System.Windows;
using ProductsHistoryClient.Products;
using ProductsHistoryClient.View.Products;

namespace ProductsHistoryClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public MainWindow()
        {
            InitializeComponent();

            // Load products to UI
            Task.Run(ProductsManager.LoadProductsFromRemote).ContinueWith(aProducts =>
            {
                ProductsListView _ProductsListView = null;
                this.Dispatcher.Invoke(() =>
                {
                    _ProductsListView = new ProductsListView();
                });
                
                _ProductsListView.RefreshProducts(aProducts.Result);

                this.Dispatcher.Invoke(() =>
                {
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(_ProductsListView);
                });
            });
        }
    }
}
