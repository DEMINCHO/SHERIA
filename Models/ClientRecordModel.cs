namespace SHERIA.Models
{
    public class ClientRecordModel
    { 
        public Int64 id { get; set; }
        public string? client_type { get; set; }
        public string? first_name { get; set; }
        public string? middle_name { get; set; }
        public string? last_name { get; set; }
        public string? company_name { get; set; }
        public string? id_number { get; set; }
        public string? kra_pin { get; set; }
        public string? cert_of_incoporation { get; set; }
        public string? nationality { get; set; }
        public string? country { get; set; }
        public string? phone_number { get; set; }
        public string? sec_phone_number { get; set; }
        public string? email { get; set; }
        public string? physical_address { get; set; }
        public string? postal_address { get; set; }
        public string? remarks { get; set; }
        public Int64 created_by { get; set; }
        public DateTime created_on { get; set; }
        public string? approved { get; set; }
        public DateTime approved_on { get; set; }
        public bool is_deleted { get; set; }
        public DateTime deleted_on { get; set; }
        public Int64 deleted_by { get; set; }

    }

    public class ProductpurchaseModel
    {
        public Int64 id { get; set; }
        public string? client_name { get; set; }
        public string? mobile { get; set; }
        public string? email { get; set; }
        public string? id_no { get; set; }
        public Int64 product { get; set; }
        public Int64 product_total { get; set; }
        public string? price { get; set; }
        public string? start_date { get; set; }
        public string? return_date { get; set; }
        public Int64 created_by { get; set; }
    }
}
