using System;
using System.Threading.Tasks;
using System.Windows;
using Radiant.Custom.ProductsWatcher.ProductsHistoryClient.Products;
using Radiant.Custom.ProductsWatcher.ProductsHistoryClient.View.Products;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryClient
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

            //Load products to UI
            Action _LoadingRemoteDataAction = LoadingRemoteData;
            Action _LoadingRemoteDataCompletedAction = LoadingRemoteDataCompleted;
            Task.Run(async () => await ProductsManager.LoadProductsFromRemote(_LoadingRemoteDataAction, _LoadingRemoteDataCompletedAction)).ContinueWith(aProducts =>
             {
                 ProductsListView _ProductsListView = null;
                 this.Dispatcher.Invoke(() => { _ProductsListView = new ProductsListView(); });

                 _ProductsListView.RefreshProducts(aProducts.Result);

                 this.Dispatcher.Invoke(() =>
                 {
                     mainGrid.Children.Clear();
                     mainGrid.Children.Add(_ProductsListView);
                 });
             });
        }

        private void LoadingRemoteData()
        {
            this.Dispatcher.Invoke(() =>
            {
                StartingControl.lblInformation.Text = "Downloading remote data...";
            });
        }

        private void LoadingRemoteDataCompleted()
        {
            this.Dispatcher.Invoke(() =>
            {
                StartingControl.lblInformation.Text = "Loading Database...";
            });
        }
    }
}
