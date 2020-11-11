using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("target_function")]
    [PetaPoco.PrimaryKey("company_code, role_id, function_id", autoIncrement = false)]
    public class TargetFunction
    {
        public string company_code { get; set; }

        public int role_id { get; set; }

        public int function_id { get; set; }

        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }
    }
}