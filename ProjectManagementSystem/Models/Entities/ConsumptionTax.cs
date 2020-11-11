using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_consumption_tax")]
    [PetaPoco.PrimaryKey("company_code, apply_start_date", autoIncrement = false)]
    [Serializable]
    public class ConsumptionTax
    {
        public string company_code { get; set; }

        [DisplayName("適用開始日")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public DateTime? apply_start_date { get; set; }

        [DisplayName("消費税率")]
        [Range(0, 999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public decimal tax_rate { get; set; }

        [DisplayName("備考")]
        public string remarks { get; set; }

        [DisplayName("登録日時")]
        [DataType(DataType.Date)]
        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }
    }
}