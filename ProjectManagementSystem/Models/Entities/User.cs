using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_user")]
    [PetaPoco.PrimaryKey("company_code, user_sys_id", autoIncrement = false)]
    [Serializable]
    public class User
    {
        public string company_code { get; set; }

        public int user_sys_id { get; set; }

        [DisplayName("ユーザーアカウント")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(32, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [RegularExpression(@"^[a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public string user_account { get; set; }

        [DisplayName("パスワード")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(32, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string password { get; set; }

        [DisplayName("ユーザー名(姓)")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string user_name_sei { get; set; }

        [DisplayName("ユーザー名(名)")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string user_name_mei { get; set; }

        [DisplayName("フリガナ(姓)")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string furigana_sei { get; set; }

        [DisplayName("フリガナ(名)")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string furigana_mei { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [DisplayName("表示名")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string display_name { get; set; }

        [DisplayName("社員No.")]
        [StringLength(20, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public string employee_no { get; set; }

        public int? position_id { get; set; }

        public int? group_id { get; set; }
        
        public int? location_id { get; set; }

        [DisplayName("権限")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        public int? role_id { get; set; }

        [DisplayName("電話番号")] 
        [StringLength(13, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [RegularExpression(@"^[0-9\-]+$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public string tel_no { get; set; }

        [DisplayName("メールアドレス1")]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
        ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string mail_address_1 { get; set; }

        [DisplayName("メールアドレス2")]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
        ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string mail_address_2 { get; set; }

        [DisplayName("期間（入社年月日）")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public DateTime? entry_date { get; set; }

        [DisplayName("期間（退職年月日）")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public DateTime? retirement_date { get; set; }

        [DisplayName("期間（生年月日）")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public DateTime? birth_date { get; set; }

        [DisplayName("プロフィール画像")]
        [StringLength(255, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string image_file_path { get; set; }

        public int? language_id { get; set; }

        public string actual_work_input_mode { get; set; }

        public string password_lock_flg { get; set; }

        [DataType(DataType.Date)]
        public DateTime? password_last_update { get; set; }

        [DisplayName("備考")]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string remarks { get; set; }

        public int display_order { get; set; }

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