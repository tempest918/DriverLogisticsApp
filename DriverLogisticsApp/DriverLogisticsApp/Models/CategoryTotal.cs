namespace DriverLogisticsApp.Models
{
    // holds the total amount for a specific expense category
    public class CategoryTotal
    {
        public string CategoryName { get; set; }
        public decimal TotalAmount { get; set; }
    }
}