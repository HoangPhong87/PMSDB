using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_overhead_cost")]
    [PetaPoco.PrimaryKey("m_overhead_cost, overhead_cost_id", autoIncrement = false)]
    [Serializable]
    public class M_OverheadCost
    {
        public string company_code { get; set; }

        public int overhead_cost_id { get; set; }

        [DisplayName("諸経費種別")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string overhead_cost_type { get; set; }

        [DisplayName("備考")]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string remarks { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }
    }
}