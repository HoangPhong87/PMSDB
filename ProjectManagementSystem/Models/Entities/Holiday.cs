using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_holiday")]
    [PetaPoco.PrimaryKey("company_code, holiday_date", autoIncrement = false)]
    public class Holiday
    {
        public string company_code { get; set; }

        [DisplayName("休日")]
        [DataType(DataType.Date)]
        public DateTime? holiday_date { get; set; }

        [DisplayName("休日名")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string holiday_name { get; set; }

        [DisplayName("備考")]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string remarks { get; set; }

        [DisplayName("休日")]
        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }
    }
}