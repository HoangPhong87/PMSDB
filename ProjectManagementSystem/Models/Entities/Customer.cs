using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_customer")]
    [PetaPoco.PrimaryKey("company_code, customer_id", autoIncrement = false)]
    [Serializable]
    public class Customer
    {
        public string company_code { get; set; }

        public int customer_id { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("取引先名")]
        public string customer_name { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("取引先名（カナ）")]
        public string customer_name_kana { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("取引先名（英語）")]
        public string customer_name_en { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [DisplayName("表示名")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string display_name { get; set; }

        [StringLength(8, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("郵便番号")]
        [RegularExpression(@"^[0-9\-]+$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public string zip_code { get; set; }

        public string prefecture_code { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("市区町村")]
        public string city { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("住所1")]
        public string address_1 { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("住所2")]
        public string address_2 { get; set; }

        [DisplayName("電話番号")]
        [StringLength(13, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [RegularExpression(@"^[0-9\-]+$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public string tel_no { get; set; }

        [DisplayName("FAX番号")]
        [StringLength(13, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [RegularExpression(@"^[0-9\-]+$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public string fax_no { get; set; }

        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
        ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [StringLength(255, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("メールアドレス")] 
        public string mail_address { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("URL")]
        [RegularExpression(@"^[a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [DataType(DataType.Url, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public string url { get; set; }

        [StringLength(225, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("ロゴ画像")]
        public string logo_image_file_path { get; set; }

        [DisplayName("備考")]
        [StringLength(4000, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string remarks { get; set; }

        [DataType(DataType.Date)]
        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }

        [DataType(DataType.Date)]
        public DateTime upd_date { get; set; }

        public int upd_id { get; set; }

        [DisplayName("削除")]
        public string del_flg { get; set; }

        [PetaPoco.ResultColumn]
        public byte[] row_version { get; set; }
    }
}