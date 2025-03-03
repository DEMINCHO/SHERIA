namespace SHERIA.Models
{
    public class TasksModel
    {
        public Int64 id { set; get; }
        public string? task_name { set; get; }
        public Int64 matter_id { set; get; }
        public DateTime start_date { set; get; }
        public DateTime due_date { set; get; }
        public string? task_status { set; get; }
        public Int64 assigned_to { set; get; }
        public string? priority { set; get; }
        public string? description { set; get; }
        public Int64 created_by { set; get; }
        public Int64 Deleted_by { set; get; }
    }
}
