using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("progress_history")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, regist_date", autoIncrement = false)]
    public class ProgressHistory
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        [DisplayName("登録日")]
        [DataType(DataType.Date)]
        public DateTime? regist_date { get; set; }

        [DisplayName("進捗")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public int? progress { get; set; }

        [DisplayName("備考")]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string remarks { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }
    }
}