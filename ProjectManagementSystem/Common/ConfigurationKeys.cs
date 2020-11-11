
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ProjectManagementSystem.Common
{
    /// <summary>
    /// A description of the outline of ConfigurationKeys
    /// </summary>
    public sealed class ConfigurationKeys
    {
        public static readonly string SMTP_SERVER = "smtpServer";

        public static readonly string ENABLE_SSL = "EnableSsl";

        public static readonly string SMTP_PORT = "smtpPort";

        public static readonly string SMTP_USER = "smtpUser";

        public static readonly string SMTP_PASS = "smtpPass";

        public static readonly string SMTP_SUPPORT = "smtpSupport";

        public static readonly string SAVE_BASE_FILE_PATH = "saveBaseFilePath";

        public static readonly string SAVE_ATTACH_FILE_PATH = "saveAttachFilePath";

        public static readonly string ATTACH_FILE_FOLDER = "attachFileFolder";

        // USER
        public static readonly string TEMP_USER_PATH = "temp_user";

        public static readonly string USER_PATH = "m_user";

        //CUSTOMER
        public static readonly string TEMP_CUSTOMER_PATH = "temp_customer";

        public static readonly string CUSTOMER_PATH = "m_customer";

        //CUSTOMER
        public static readonly string TEMP_COMPANY_PATH = "temp_company";

        public static readonly string COMPANY_PATH = "m_company";

        //NAME IMAGE
        public static readonly string LOGO_IMAGE = "logo_image";

        public static readonly string PROFILE_IMAGE = "profile_image";

        public static readonly string LIST_ITEMS_PER_PAGE = "list_items_per_page";

        public static readonly string ENCRYTION_STRING = "encryption_string";

        public static readonly string VERSION_SYSTEM = "version_system";

        public static readonly string APP_DATA_FILE = "app_data_file";
    }
}