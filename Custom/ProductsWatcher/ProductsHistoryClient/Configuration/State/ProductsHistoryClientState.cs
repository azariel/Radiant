namespace ProductsHistoryClient.Configuration.State
{
    public class ProductsHistoryClientState
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public ProductsHistoryClientStateDataBase RemoteDataBaseState { get; set; } = new();
    }
}
