namespace SHERIA.Models
{
    public class MattersModel
    {
        public Int64 id { set; get; }
        public string? matter_name { set; get; }
        public string? matter_number { set; get; }
        public Int64 assigned_to { set; get; }
        public Int64 client_id { set; get; }
        public DateTime start_date { set; get; }
        public DateTime close_date { set; get; }
        public string? practice_area { set; get; }
        public string? matter_status { set; get; }
        public string? matter_billing { set; get; }
        public string? description { set; get; }
        public Int64 created_by { set; get; }
        public Int64 Deleted_by { set; get; }
    }
    public class ProductModel
    {
        public Int64 id { set; get; }
        public string? name { set; get; }
        public Int64 type { set; get; }
        public string? total { set; get; }
        public string? price { set; get; }
    }
}
