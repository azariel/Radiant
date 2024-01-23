namespace Radiant.Custom.Finance.Budget.RadiantBudgetBridge.Models
{
    public class CsvEntryModel
    {
        // HEADER = "Date";"Created at";"Title";"Comment";"Payment type";"Main category";"Subcategory";"Account";"Amount";"Cleared"
        public string Date { get; set; }
        public string CreatedAt { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public string PaymentType { get; set; }
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }
        public string Account { get; set; }
        public string Amount { get; set; }
        public string Cleared { get; set; }
    }
}
