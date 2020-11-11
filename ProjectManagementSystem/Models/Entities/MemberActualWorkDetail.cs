using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("member_actual_work_detail")]
    [PetaPoco.PrimaryKey("company_code, user_sys_id, actual_work_year, actual_work_month, actual_work_date, detail_no", autoIncrement = false)]
    public class MemberActualWorkDetail
    {
        public string company_code { get; set; }

        public int user_sys_id { get; set; }

        public int actual_work_year { get; set; }

        public int actual_work_month { get; set; }

        public int actual_work_date { get; set; }

        public long detail_no { get; set; }

        public int project_sys_id { get; set; }

        public int phase_id { get; set; }

        public decimal actual_work_time { get; set; }

        [DisplayName("備考")]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string remarks { get; set; }

        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        public DateTime? upd_date { get; set; }

        public int upd_id { get; set; }
    }
}