using System;
using System.ComponentModel.DataAnnotations;
using ProjectManagementSystem.Resources;
using System.ComponentModel;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("project_attached_file")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, file_no", autoIncrement = false)]
    public class ProjectAttachFile
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        public long? file_no { get; set; }

        [DisplayName("ファイル名")]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string file_name { get; set; }

        [DisplayName("表示タイトル名")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string display_title { get; set; }

        public string file_path { get; set; }

        public string public_flg { get; set; }

        [DisplayName("備考")]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string remarks { get; set; }

        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        public DateTime? upd_date { get; set; }

        public int upd_id { get; set; }

    }
}