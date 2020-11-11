using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("information")]
    [PetaPoco.PrimaryKey("company_code, info_id", autoIncrement = false)]
    [Serializable]
    public class Information
    {
        public string company_code { get; set; }

        public int info_id { get; set; }

        [DisplayName("掲載開始日")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [DataType(DataType.Date)]
        public DateTime publish_start_date { get; set; }

        [DisplayName("掲載終了日")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [DataType(DataType.Date)]
        public DateTime publish_end_date { get; set; }

        [DisplayName("掲載内容")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string content { get; set; }

        public int? display_order { get; set; }

        [DataType(DataType.Date)]
        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }

        [DataType(DataType.Date)]
        public DateTime upd_date { get; set; }

        public int upd_id { get; set; }

        public string del_flg { get; set; }
    }
}