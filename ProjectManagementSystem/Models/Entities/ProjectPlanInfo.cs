using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("project_plan_info")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id", autoIncrement = false)]
    public class ProjectPlanInfo
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        [DisplayName("現状の課題")]
        [StringLength(500, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string issues { get; set; }

        [DisplayName("目的")]
        [StringLength(500, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string purpose { get; set; }

        [DisplayName("目標１")]
        [StringLength(40, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string target_01 { get; set; }

        [DisplayName("目標２")]
        [StringLength(40, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string target_02 { get; set; }

        [DisplayName("目標３")]
        [StringLength(40, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string target_03 { get; set; }

        [DisplayName("目標４")]
        [StringLength(40, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string target_04 { get; set; }

        [DisplayName("目標５")]
        [StringLength(40, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string target_05 { get; set; }

        [DisplayName("目標６")]
        [StringLength(40, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string target_06 { get; set; }

        [DisplayName("目標７")]
        [StringLength(40, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string target_07 { get; set; }

        [DisplayName("目標８")]
        [StringLength(40, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string target_08 { get; set; }

        [DisplayName("目標９")]
        [StringLength(40, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string target_09 { get; set; }

        [DisplayName("目標10")]
        [StringLength(40, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string target_10 { get; set; }

        [DisplayName("制約事項１")]
        [StringLength(150, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string restriction_01 { get; set; }

        [DisplayName("制約事項２")]
        [StringLength(150, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string restriction_02 { get; set; }

        [DisplayName("制約事項３")]
        [StringLength(150, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string restriction_03 { get; set; }

        [DisplayName("制約事項４")]
        [StringLength(150, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string restriction_04 { get; set; }

        [DisplayName("制約事項５")]
        [StringLength(150, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string restriction_05 { get; set; }

        [DisplayName("制約事項６")]
        [StringLength(150, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string restriction_06 { get; set; }

        [DisplayName("懸念事項１")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string concerns_01 { get; set; }

        [DisplayName("対策１")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string measures_01 { get; set; }

        [DisplayName("懸念事項２")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string concerns_02 { get; set; }

        [DisplayName("対策２")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string measures_02{ get; set; }

        [DisplayName("懸念事項３")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string concerns_03 { get; set; }

        [DisplayName("対策３")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string measures_03 { get; set; }

        [DisplayName("懸念事項４")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string concerns_04 { get; set; }

        [DisplayName("対策４")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string measures_04 { get; set; }

        [DisplayName("懸念事項５")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string concerns_05 { get; set; }

        [DisplayName("対策５")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string measures_05 { get; set; }

        [DisplayName("テスト計画の作成支援")]
        public string support_test_plan_flg { get; set; }

        [DisplayName("ユーザーテストの支援")]
        public string support_user_test_flg { get; set; }

        [DisplayName("負荷テストの支援")]
        public string support_stress_test_flg { get; set; }

        [DisplayName("セキュリティテストの支援")]
        public string support_security_test_flg { get; set; }

        [DisplayName("その他")]
        [StringLength(700, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string remarks { get; set; }

        [DataType(DataType.Date)]
        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }

        [DataType(DataType.Date)]
        public DateTime upd_date { get; set; }

        public int upd_id { get; set; }

        public string del_flg { get; set; }

        public byte[] row_version { get; set; }
    }
}