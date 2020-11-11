using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("enrollment_history")]
    [PetaPoco.PrimaryKey("company_code, user_sys_id", autoIncrement = false)]
    public class GroupHistory
    {
        public string company_code { get; set; }

        public int user_sys_id { get; set; }

        public int actual_work_year { get; set; }

        public int actual_work_month { get; set; }

        public int? group_id { get; set; }
        public int? location_id { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("備考")]
        public string remarks { get; set; }


        [DisplayName("登録日時")]
        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        [DisplayName("更新日時")]
        public DateTime? upd_date { get; set; }

        public int upd_id { get; set; }
    }
}