using System.Threading.Tasks;
using System.Windows;
using ProductsHistoryClient.Products;

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
            Task.Run(ProductsManager.LoadProductsFromRemote).ContinueWith(aProducts => { ProductsListView.RefreshProducts(aProducts.Result); });
        }
    }
}
