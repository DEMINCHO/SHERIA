using Microsoft.VisualBasic;
using SHERIA.Helpers;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace SHERIA.Models
{
    public class DBHandler : IDisposable
    {
        public readonly IConfiguration config;
        //private static Logger logger = LogManager.GetCurrentClassLogger();
        private SqlConnection connection;
        private string connectionstring;

        public DBHandler(string connstring)
        {
            connection = new SqlConnection(connstring);
            this.connection.Open();
            connectionstring = connstring;
        }

        public void Dispose()
        {
            connection.Close();
        }

        #region Databases
        public enum DataBaseObject
        {
            HostDB,
            BrokerDB
        }

        public string GetDataBaseConnection(DataBaseObject databaseobject)
        {
            string connection_string = connectionstring; //config["ConnectionStrings:DefaultConnection"];
            switch (databaseobject)
            {
                case DataBaseObject.HostDB:
                    connection_string = connectionstring; //config["ConnectionStrings:DefaultConnection"];
                    break;
                default:
                    connection_string = connectionstring; //config["ConnectionStrings:DefaultConnection"];
                    break;
            }
            return connection_string;
        }
        #endregion

        #region OnbordClient

        public List<ClientRecordModel>? GetClientRecord()
        {
            List<ClientRecordModel> recordlist = new List<ClientRecordModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecords("client_record");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new ClientRecordModel
                    {
                        id = Convert.ToInt32(dr["id"]),
                        id_number = Convert.ToString(dr["id_number"]),
                        kra_pin = Convert.ToString(dr["kra_pin"]),
                        phone_number = Convert.ToString(dr["primary_phone_number"])

                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetSubjectTypes | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public Int64 AddClient(ClientRecordModel model)
        {
            try
            {
                Int64 i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("add_client_record", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@client_type", model.client_type);
                    cmd.Parameters.AddWithValue("@first_name", model.first_name);
                    cmd.Parameters.AddWithValue("@middle_name", model.middle_name);
                    cmd.Parameters.AddWithValue("@last_name", model.last_name);
                    cmd.Parameters.AddWithValue("@id_number", model.id_number);
                    cmd.Parameters.AddWithValue("@company_name", model.company_name);
                    cmd.Parameters.AddWithValue("@kra_pin", model.kra_pin);
                    cmd.Parameters.AddWithValue("@cert_of_incoporation", model.cert_of_incoporation);
                    cmd.Parameters.AddWithValue("@nationality", model.nationality);
                    cmd.Parameters.AddWithValue("@country", model.country);
                    cmd.Parameters.AddWithValue("@phone_number", model.phone_number);
                    cmd.Parameters.AddWithValue("@sec_phone_number", model.sec_phone_number);
                    cmd.Parameters.AddWithValue("@email", model.email);
                    cmd.Parameters.AddWithValue("@physical_address", model.physical_address);
                    cmd.Parameters.AddWithValue("@postal_address", model.postal_address);
                    cmd.Parameters.AddWithValue("@remarks", model.remarks);
                    cmd.Parameters.AddWithValue("@created_by", model.created_by);
                    cmd.ExecuteNonQuery();

                    i = Convert.ToInt64(cmd.Parameters["@id"].Value.ToString());
                }
                return i;

            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddPost | Exception ->" + ex.Message);
                return 0;
            }
        }
        public Int64 AddProductPurchase(ProductpurchaseModel model)
        {
            try
            {
                Int64 i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("add_product_purchase", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@client_name", model.client_name);
                    cmd.Parameters.AddWithValue("@mobile", model.mobile);
                    cmd.Parameters.AddWithValue("@email", model.email);
                    cmd.Parameters.AddWithValue("@id_no", model.id_no);
                    cmd.Parameters.AddWithValue("@product", model.product);
                    cmd.Parameters.AddWithValue("@product_total", model.product_total);
                    cmd.Parameters.AddWithValue("@price", model.price);
                    cmd.Parameters.AddWithValue("@start_date", model.start_date);
                    cmd.Parameters.AddWithValue("@return_date", model.return_date);
                    cmd.Parameters.AddWithValue("@created_by", model.created_by);
                    cmd.ExecuteNonQuery();

                    i = Convert.ToInt64(cmd.Parameters["@id"].Value.ToString());
                }
                return i;

            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddProductPurchase | Exception ->" + ex.Message);
                return 0;
            }
        }

        public bool AddClientFiles(ClientFilesModel model)
        {
            try
            {
                Int64 i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("add_client_file", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@client_id", model.client_id);
                    cmd.Parameters.AddWithValue("@file_name", model.file_name);

                    cmd.ExecuteNonQuery();
                    i = Convert.ToInt64(cmd.Parameters["@id"].Value.ToString());
                }
                if (i > 0)

                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddPost | Exception ->" + ex.Message);
                return false;
            }
        }

        #endregion

        #region Matters
        //open Matters
        public List<ProductModel> GetProductRecord()
        {
            List<ProductModel> recordlist = new List<ProductModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecords("product_records");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new ProductModel
                    {
                        id = Convert.ToInt64(dr["id"]),
                        name = Convert.ToString(dr["name"]),
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetProductRecord | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool UpdateProductRecord(ProductModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("update_product_record", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", model.id);
                    cmd.Parameters.AddWithValue("@name", model.name);
                    cmd.Parameters.AddWithValue("@type", model.type);
                    cmd.Parameters.AddWithValue("@total", model.total);
                    cmd.Parameters.AddWithValue("@price", model.price);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateProductRecord | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool AddProductRecord(ProductModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("add_product_record", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@name", model.name);
                    cmd.Parameters.AddWithValue("@type", model.type);
                    cmd.Parameters.AddWithValue("@total", model.total);
                    cmd.Parameters.AddWithValue("@price", model.price);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddProductRecord| Exception ->" + ex.Message);
                return false;
            }
        }

        public bool Update_Open_Matter_Status(Int64 id, string module, string param1 = "", DataBaseObject database = DataBaseObject.HostDB)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(database)))
                {
                    using (SqlCommand cmd = new SqlCommand("update_matters", connect))
                    {
                        connect.Open();
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@recordid", id);
                        if (param1 != "")
                            cmd.Parameters.AddWithValue("@param1", param1);
                        cmd.Parameters.AddWithValue("@module", module);
                        i = cmd.ExecuteNonQuery();
                    }
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
               FileLogHelper.log_message_fields("ERROR", "DeleteRecord | Exception ->" + ex.Message);
                return false;
            }
        }

        //end Open Matters

        #endregion

        #region Tasks Management
        public List<TasksModel> GetTasksRecord()
        {
            List<TasksModel> recordlist = new List<TasksModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecords("tasks");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new TasksModel
                    {
                        id = Convert.ToInt64(dr["id"]),
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "Gettasks | Exception ->" + ex.Message);
            }

            return recordlist;
        }
        public bool UpdateTasksRecord(TasksModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("update_tasks_record", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", model.id);
                    cmd.Parameters.AddWithValue("@task_name", model.task_name);
                    cmd.Parameters.AddWithValue("@matter_id", model.matter_id);
                    cmd.Parameters.AddWithValue("@start_date", model.start_date);
                    cmd.Parameters.AddWithValue("@due_date", model.due_date);
                    cmd.Parameters.AddWithValue("@task_status", model.task_status);
                    cmd.Parameters.AddWithValue("@assigned_to", model.assigned_to);
                    cmd.Parameters.AddWithValue("@priority", model.priority);
                    cmd.Parameters.AddWithValue("@description", model.description);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateTask | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool AddTasksRecord(TasksModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("add_task_record", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@task_name", model.task_name);
                    cmd.Parameters.AddWithValue("@matter_id", model.matter_id);
                    cmd.Parameters.AddWithValue("@start_date", model.start_date);
                    cmd.Parameters.AddWithValue("@due_date", model.due_date);
                    cmd.Parameters.AddWithValue("@task_status", model.task_status);
                    cmd.Parameters.AddWithValue("@assigned_to", model.assigned_to);
                    cmd.Parameters.AddWithValue("@priority", model.priority);
                    cmd.Parameters.AddWithValue("@description", model.description);
                    cmd.Parameters.AddWithValue("@created_by", model.created_by);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddMatters| Exception ->" + ex.Message);
                return false;
            }
        }

        #endregion

        #region Adhoc
        public DataTable ValidateUserLogin(string user_type, string email_address)
        {
            DataTable dt = new DataTable();
            try
            {
                using SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));
                using SqlCommand cmd = new SqlCommand("validate_user_login", connect);
                using SqlDataAdapter sd = new SqlDataAdapter(cmd);
                connect.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@username", email_address);
                cmd.Parameters.AddWithValue("@profiletype", user_type);
                sd.Fill(dt);
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "ValidateUserLogin | Exception ->" + ex.Message);
            }

            return dt;
        }

        public bool UpdateProfile(Int16 record_id, Int16 profile_id, string attribute, string new_value)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("update_profile", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@record_id", record_id);
                    cmd.Parameters.AddWithValue("@profile_id", profile_id);
                    cmd.Parameters.AddWithValue("@attribute", attribute);
                    cmd.Parameters.AddWithValue("@new_value", new_value);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateProfile | Exception ->" + ex.Message);
                return false;
            }
        }

        public DataTable GetAdhocData(string sql)
        {
            DataTable dt = new DataTable();

            try
            {
                using SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));
                using SqlCommand cmd = new SqlCommand(sql, connect);
                using SqlDataAdapter sd = new SqlDataAdapter(cmd);
                sd.Fill(dt);
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetAdhocData | Exception ->" + ex.Message);
            }
            return dt;
        }

        public DataTable GetRecords(string module, string param1 = "", string param2 = "")
        {
            DataTable dt = new DataTable();

            try
            {
                using SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));
                using SqlCommand cmd = new SqlCommand("get_records", connect);
                using SqlDataAdapter sd = new SqlDataAdapter(cmd);
                connect.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@module", module);
                cmd.Parameters.AddWithValue("@param1", param1);
                cmd.Parameters.AddWithValue("@param2", param2);
                sd.Fill(dt);
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetRecords | Exception ->" + ex.Message);
            }

            return dt;
        }

        public DataTable GetUnapprovedRecords(string module, string param1 = "")
        {
            DataTable dt = new DataTable();

            try
            {
                using SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));
                using SqlCommand cmd = new SqlCommand("get_records_unapproved", connect);
                using SqlDataAdapter sd = new SqlDataAdapter(cmd);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@module", module);
                if (param1 != "")
                    cmd.Parameters.AddWithValue("@param1", param1);
                sd.Fill(dt);
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetUnapprovedRecords | Exception ->" + ex.Message);
            }

            return dt;
        }

        public DataTable GetRecordsById(string module, Int64 id, string param1 = "", string param2 = "")
        {
            DataTable dt = new DataTable();

            try
            {
                using SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));
                using SqlCommand cmd = new SqlCommand("get_records_by_id", connect);
                using SqlDataAdapter sd = new SqlDataAdapter(cmd);
                connect.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@module", module);
                cmd.Parameters.AddWithValue("@id", id);
                sd.Fill(dt);
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetRecordsById | Exception ->" + ex.Message);
            }

            return dt;
        }

        public string GetScalarItem(string sql)
        {
            string scalaritem = "";

            try
            {
                using SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));
                using SqlCommand command = new SqlCommand(sql, connect);
                connect.Open();
                scalaritem = (string)(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetScalarItem | Exception ->" + ex.Message);
                scalaritem = "";
            }
            return scalaritem;
        }

        public bool ClearAppointment(string module,string param1 = "")
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("delete_appointment", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@module", module);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "DeleteAppointment | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool DeleteRecord(Int64 id, Int64 deleted_by, string module, string param1 = "")
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("delete_records", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@recordid", id);
                    cmd.Parameters.AddWithValue("@deleted_by", deleted_by);
                    cmd.Parameters.AddWithValue("@module", module);
                    if (param1 != "")
                        cmd.Parameters.AddWithValue("@param1", param1);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "DeleteRecord | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool ApproveRecord(Int64 id, Int64 approved_by, string module, string action_flag = "")
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("approve_records", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@record_id", id);
                    cmd.Parameters.AddWithValue("@approved_by", approved_by);
                    cmd.Parameters.AddWithValue("@module", module);
                    if (action_flag != "")
                        cmd.Parameters.AddWithValue("@action_flag", action_flag);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "ApproveRecord | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool AddAuditTrail(AuditTrailModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("add_audit_trail", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@user_name", model.user_name);
                    cmd.Parameters.AddWithValue("@action_type", model.action_type);
                    cmd.Parameters.AddWithValue("@action_description", model.action_description);
                    cmd.Parameters.AddWithValue("@page_accessed", model.page_accessed);
                    cmd.Parameters.AddWithValue("@client_ip_address", model.client_ip_address);
                    cmd.Parameters.AddWithValue("@session_id", model.session_id);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddAuditTrail | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool AllocateDeallocateRolePermission(string action, int role_id, int permission_id)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("allocate_deallocate_role_permission", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@role_id", role_id);
                    cmd.Parameters.AddWithValue("@permission_id", permission_id);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AllocateDeallocateRolePermission | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool AllocateDeallocateUserRole(string action, int user_id, int role_id)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("allocate_deallocate_user_role", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@role_id", role_id);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AllocateDeallocateUserRole | Exception ->" + ex.Message);
                return false;
            }
        }
        #endregion

        #region Access Control
        //Roles
        public List<RolesModel> GetRoles()
        {
            List<RolesModel> recordlist = new List<RolesModel>();

            try
            {
                DataTable dt = new DataTable();
                dt = GetRecords("roles");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new RolesModel
                    {
                        id = Convert.ToInt32(dr["id"]),
                        role_name = Convert.ToString(dr["role_name"])!,
                        role_type = Convert.ToString(dr["role_type"])!,
                        remarks = Convert.ToString(dr["remarks"])!,
                        is_sys_admin = Convert.ToBoolean(dr["is_sys_admin"])
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetPortalRoles | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool AddRole(RolesModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("add_role", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@role_name", model.role_name);
                    cmd.Parameters.AddWithValue("@role_type", model.role_type);
                    cmd.Parameters.AddWithValue("@remarks", model.remarks);
                    cmd.Parameters.AddWithValue("@is_sys_admin", model.is_sys_admin);
                    

                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddRole | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool UpdateRole(RolesModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("update_role", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", model.id);
                    cmd.Parameters.AddWithValue("@role_name", model.role_name);
                    cmd.Parameters.AddWithValue("@role_type", model.role_type);
                    cmd.Parameters.AddWithValue("@remarks", model.remarks);
                    cmd.Parameters.AddWithValue("@is_sys_admin", model.is_sys_admin);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateRole | Exception ->" + ex.Message);
                return false;
            }
        }
        //End Roles

        //Role Menu Access
        public bool AddMenuAccess(string page_url, string main_menu_name, string sub_menu_name, int role_id, int can_access, int menu_order, int sub_menu_order)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("add_menu_access", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@role_id", role_id);
                    cmd.Parameters.AddWithValue("@main_menu_name", main_menu_name);
                    cmd.Parameters.AddWithValue("@sub_menu_name", sub_menu_name);
                    cmd.Parameters.AddWithValue("@page_url", page_url);
                    cmd.Parameters.AddWithValue("@can_access", can_access);
                    cmd.Parameters.AddWithValue("@menu_order", menu_order);
                    cmd.Parameters.AddWithValue("@sub_menu_order", sub_menu_order);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetPermissions | Exception ->" + ex.Message);
                return false;
            }
        }
        //End Role Menu Access

        //Permissions
        public List<PermissionsModel> GetPermissions()
        {
            List<PermissionsModel> recordlist = new List<PermissionsModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecords("permissions");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new PermissionsModel
                    {
                        id = Convert.ToInt32(dr["id"]),
                        permission_name = Convert.ToString(dr["permission_name"])
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetPermissions | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool AddPermission(PermissionsModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("add_permission", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@permission_name", model.permission_name);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddPermission | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool UpdatePermission(PermissionsModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("update_permission", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", model.id);
                    cmd.Parameters.AddWithValue("@permission_name", model.permission_name);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdatePermission | Exception ->" + ex.Message);
                return false;
            }
        }
        //End Permissions

        //Role Permissions
        public List<RolePermissionModel> GetRolePermissions(int role_id)
        {
            List<RolePermissionModel> recordlist = new List<RolePermissionModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecordsById("role_allocated_permissions", role_id);

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new RolePermissionModel
                    {
                        id = Convert.ToInt32(dr["id"]),
                        role_id = Convert.ToInt32(dr["role_id"]),
                        permission_id = Convert.ToInt32(dr["permission_id"])
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetRolePermissions | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool AddRolePermission(RolePermissionModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("add_role_permission_mapping", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@role_id", model.role_id);
                    cmd.Parameters.AddWithValue("@permission_id", model.permission_id);

                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddRolePermission | Exception ->" + ex.Message);
                return false;
            }
        }
        //End Role Permissions

        //Users
        public List<PortalUsersModel> GetPortalUsers()
        {
            List<PortalUsersModel> recordlist = new List<PortalUsersModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecords("portal_users");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new PortalUsersModel
                    {
                        id = Convert.ToInt32(dr["id"]),
                        role_id = Convert.ToInt32(dr["role_id"]),
                        mobile = Convert.ToString(dr["mobile"])!,
                        email = Convert.ToString(dr["email"])!,
                        name = Convert.ToString(dr["name"])!,
                        password = Convert.ToString(dr["password"])!,
                        avatar = Convert.ToString(dr["avatar"])!,
                        locked = Convert.ToBoolean(dr["locked"]),
                        google_authenticate = Convert.ToBoolean(dr["google_authenticate"]),
                        sec_key = Convert.ToString(dr["sec_key"])!
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetUserRoles | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool AddPortalUser(PortalUsersModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("add_portal_user", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@role_id", model.role_id);
                    cmd.Parameters.AddWithValue("@mobile", model.mobile);
                    cmd.Parameters.AddWithValue("@email", model.email);
                    cmd.Parameters.AddWithValue("@name", model.name);
                    cmd.Parameters.AddWithValue("@password", model.password);
                    cmd.Parameters.AddWithValue("@avatar", model.avatar);
                    cmd.Parameters.AddWithValue("@locked", model.locked);
                    cmd.Parameters.AddWithValue("@google_authenticate", model.google_authenticate);
                    cmd.Parameters.AddWithValue("@created_by", model.created_by);
                    cmd.Parameters.AddWithValue("@sec_key", model.sec_key);

                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddPortalUser | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool UpdatePortalUser(PortalUsersModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("update_portal_user", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", model.id);
                    cmd.Parameters.AddWithValue("@role_id", model.role_id);
                    cmd.Parameters.AddWithValue("@mobile", model.mobile);
                    cmd.Parameters.AddWithValue("@email", model.email);
                    cmd.Parameters.AddWithValue("@name", model.name);
                    cmd.Parameters.AddWithValue("@password", model.password);
                    cmd.Parameters.AddWithValue("@avatar", model.avatar);
                    cmd.Parameters.AddWithValue("@locked", model.locked);
                    cmd.Parameters.AddWithValue("@google_authenticate", model.google_authenticate);
                    cmd.Parameters.AddWithValue("@sec_key", model.sec_key);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdatePortalUser | Exception ->" + ex.Message);
                return false;
            }
        }
        //End Users

        //User Roles
        public List<UserRoleModel> GetUserRoles(int user_id)
        {
            List<UserRoleModel> recordlist = new List<UserRoleModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecordsById("user_allocated_roles", user_id);

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new UserRoleModel
                    {
                        id = Convert.ToInt32(dr["id"]),
                        user_id = Convert.ToInt32(dr["user_id"]),
                        role_id = Convert.ToInt32(dr["role_id"])
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetUserRoles | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool AddUserRole(UserRoleModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("add_user_role_mapping", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@user_id", model.user_id);
                    cmd.Parameters.AddWithValue("@role_id", model.role_id);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddUserRole | Exception ->" + ex.Message);
                return false;
            }
        }
        //End User Roles
        #endregion

        #region Settings
        // Parameters
        public List<ParametersModel> GetParameters()
        {
            List<ParametersModel> recordlist = new List<ParametersModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecords("parameters");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new ParametersModel
                    {
                        id = Convert.ToInt16(dr["Id"]),
                        item_key = Convert.ToString(dr["item_key"]),
                        item_value = Convert.ToString(dr["item_value"])
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetParameters | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool AddParameter(ParametersModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("add_parameter", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@item_key", model.item_key);
                    cmd.Parameters.AddWithValue("@item_value", model.item_value);
                    cmd.Parameters.AddWithValue("@comments", model.comments);
                    //cmd.Parameters.AddWithValue("@created_by", model.created_by);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddParameter | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool UpdateParameter(ParametersModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using SqlCommand cmd = new SqlCommand("update_parameter", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", model.id);
                    cmd.Parameters.AddWithValue("@item_key", model.item_key);
                    cmd.Parameters.AddWithValue("@item_value", model.item_value);
                    cmd.Parameters.AddWithValue("@comments", model.comments);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateParameter | Exception ->" + ex.Message);
                return false;
            }
        }
        //End Parameters
        #endregion

        #region Reports
        public List<ReportsModel> GetReports()
        {
            List<ReportsModel> recordlist = new List<ReportsModel>();

            try
            {
                DataTable dt = new DataTable();
                dt = GetRecords("reports");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new ReportsModel
                    {
                        id = Convert.ToInt16(dr["id"]),
                        name = Convert.ToString(dr["name"])!,
                        view_name = Convert.ToString(dr["view_name"])!,
                        enabled = Convert.ToInt16(dr["enabled"])
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetReports | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool AddReport(ReportsModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using (SqlCommand cmd = new SqlCommand("add_report", connect))
                    {
                        connect.Open();
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@name", model.name);
                        cmd.Parameters.AddWithValue("@view_name", model.view_name);
                        cmd.Parameters.AddWithValue("@enabled", Convert.ToBoolean(model.enabled));
                        cmd.Parameters.AddWithValue("@created_by", model.created_by);

                        i = (int)cmd.ExecuteNonQuery();
                    }
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddReport | Exception ->" + ex.Message);
                return false;
            }
        }

       

        public bool UpdateReport(ReportsModel model)
        {
            try
            {
                int i = 0;
                using (SqlConnection connect = new SqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using (SqlCommand cmd = new SqlCommand("update_report", connect))
                    {
                        connect.Open();
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id", model.id);
                        cmd.Parameters.AddWithValue("@name", model.name);
                        cmd.Parameters.AddWithValue("@view_name", model.view_name);
                        cmd.Parameters.AddWithValue("@enabled", Convert.ToBoolean(model.enabled));

                        i = (int)cmd.ExecuteNonQuery();
                    }
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateReport | Exception ->" + ex.Message);
                return false;
            }
        }
        #endregion

        






    }
}