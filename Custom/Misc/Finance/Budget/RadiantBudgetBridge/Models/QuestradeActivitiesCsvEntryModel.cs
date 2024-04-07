namespace Radiant.Custom.Finance.Budget.RadiantBudgetBridge.Models
{
    public class QuestradeActivitiesCsvEntryModel
    {
        // HEADER = Transaction Date,Settlement Date,Action,Symbol,Description,Quantity,Price,Gross Amount,Commission,Net Amount,Currency,Account #,Activity Type,Account Type
        public string TransactionDate { get; set; }
        public string SettlementDate { get; set; }
        public string Action { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string GrossAmount { get; set; }
        public string Commission { get; set; }
        public string NetAmount { get; set; }
        public string Currency { get; set; }
        public string AccountNumber { get; set; }
        public string ActivityType { get; set; }
        public string AccountType { get; set; }
    }
}
