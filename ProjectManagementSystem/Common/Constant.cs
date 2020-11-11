using System.Collections.Generic;
using System.Collections.Specialized;

namespace ProjectManagementSystem.Common
{
    /// <summary>
    /// Common constants definition
    /// </summary>
    public class Constant
    {
        /// <summary>Test Environment IP</summary>
        public const string TEST_ENVIRONMENT_IP = "10.0.0.39";

        /// <summary>The database connection string name</summary>
        public const string CONNECTION_STRING_NAME = "PMSConnection";

        /// <summary>Key name for login user in session object</summary>
        public const string SESSION_LOGIN_USER = "SESSION_LOGIN_USER";

        // Set password out of date
        public const string PASSWORD_OUT_OF_DATE = "PASSWORD_OUT_OF_DATE";

        // Set display password
        public const string DISPLAY_PASSWORD = "••••••••••••••••••••••••••••••••";

        // Set string validate password
        public const string REG_PASSWORD = @"^[a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]*$";

        /// <summary>Key name for invalid login user in session object</summary>
        public const string SESSION_INVALID_LOGIN_USER = "SESSION_INVALID_LOGIN_USER";

        /// <summary>Key name for  in session object</summary>
        public const string SESSION_TRANSITION_DESTINATION = "SESSION_TRANSITION_DESTINATION";

        /// <summary>Key name for checking Back button is pressed</summary>
        public const string SESSION_IS_BACK = "SESSION_IS_BACK";

        /// <summary>Key name for save all data of search result in a List page</summary>
        public const string SESSION_SEARCH_RESULT = "SESSION_SEARCH_RESULT";

        /// <summary>Max size image</summary>
        public const int MaxContentLength = 1024 * 500; //500Kb;

        /// <summary>Input image</summary>
        public const string Input_Image = "/Content/img/input_image.jpg";

        /// <summary>No Image</summary>
        public const string No_Image = "/Content/img/no_image.jpg";

        /// <summary>Default value </summary>
        public const string DEFAULT_VALUE = "0";

        /// <summary>Project No max length</summary>
        public const int PROJECT_NO_MAX_LENGTH = 10;

        //Define TOTAL ACTUAL WORK TIME
        public const int TOTAL_ACTUAL_WORK_TIME = 744;

        public static string[] AllowedFileExtensions = new string[] { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };

        //Define MAX_ITEM_SUB_MENU
        public const int MAX_ITEM_SUB_MENU = 7;

        public const int MAX_LENGTH_NAME = 100;

        public const int MAX_LENGTH_ADDRESS = 255;

        public const int MAX_LENGTH_TEL_FAX_NO = 13;

        public const int MAX_LENGTH_REMARK = 4000;

        // Define value if button back of browser clicked
        public const int IS_BACK_FROM_BROWSER = 1;

        // Define COLLATION for SQL Query
        public const string SQL_COLLATION_LATIN = "SQL_Latin1_General_CP1_CS_AS";

        /// <summary>
        /// Containing constants represent the work time units
        /// </summary>
        public class TimeUnit
        {
            public const string HOUR = "1";
            public const string DAY = "2";
            public const string MONTH = "3";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { HOUR, "時間" },
                { DAY, "人日" },
                { MONTH, "人月" }
            }.AsReadOnly();
        }


        public class HttpResponseCode
        {
            public const int TIME_OUT = 419;
            public const int NOT_FOUND = 404;
            public const int CREATED = 201;
            public const int SUCCESSFUL = 200;
            public const int INTERNAL_SERVER_ERROR = 500;
            public const int EXPECTATION_FAILED = 417;
        }

        public class FunctionID
        {
            public const int Login = 1;
            public const int Top = 2;
            public const int PasswordReissue = 3;
            public const int PersonalSetting = 4;
            public const int ProjectList_Admin = 5;
            public const int ProjectList = 6;
            public const int ProjectRegist = 7;
            public const int ProjectDetail = 8;
            public const int ActualWorkList = 9;
            public const int ActualWorkRegist = 10;
            public const int ActualWorkDetail = 11;
            public const int AssignmentByProject = 12;
            public const int AssignmentByUser = 13;
            public const int UserList_Admin = 14;
            public const int UserList = 15;
            public const int UserRegist = 16;
            public const int CustomerList_Admin = 17;
            public const int CustomerList = 18;
            public const int CustomerRegist = 19;

            public const int ContractTypeList_Admin = 20;
            public const int ContractTypeList = 21;
            public const int ContractTypeRegist = 22;
            public const int GroupList_Admin = 23;
            public const int GroupList = 24;
            public const int GroupRegist = 25;
            public const int PhaseList_Admin = 26;
            public const int PhaseList = 27;
            public const int PhaseRegist = 28;
            public const int ConsumptionTaxList_Admin = 29;
            public const int ConsumptionTaxList = 30;
            public const int ConsumptionTaxRegist = 31;

            public const int SalesCustomer_Admin = 32;
            public const int SalesCustomer = 33;
            public const int SalesGroup_Admin = 34;
            public const int SalesGroup = 35;
            public const int SalesPersonal_Admin = 36;
            public const int SalesPersonal = 37;
            public const int SalesNormalPersonal = 56;
            public const int SalesPayment_Admin = 51;
            public const int SalesPayment = 52;

            public const int TagList_Admin = 38;
            public const int TagList = 39;
            public const int TagRegist = 40;

            public const int OutputExcel = 41;

            public const int InfoList_Admin = 42;
            public const int InfoList = 43;
            public const int InfoRegist = 44;

            public const int ProjectPlanRegist = 45;

            public const int CompanySetting = 46;
            public const int CategoryList_Admin = 47;
            public const int CategoryList = 48;
            public const int CategoryRegist = 49;

            public const int OverHeadCostList_Admin = 53;
            public const int OverHeadCostList = 54;
            public const int OverHeadCostRegist = 55;

            public const int ProjectPlanReadOnly = 50;

            public const int ActualWorkList_Admin = 57;

            public const int BranchList_Admin = 58;
            public const int BranchList = 59;
            public const int BranchRegist = 60;

            public const int Budget_Setting = 61;
            public const int BudgetActual_List = 62;

            public const int UnitPriceInfo_List = 63;
            //入力状況確認
            public const int CheckInputStatus = 64;

            public const int CheckProjectStatus = 65;
            public const int CheckOperationInputStatus = 66;

            public const int ActualWorkListByIndividualPhase = 69;
            public const int CheckBranchListIndividual = 70;
            public const int INCLUEDE_DELETED_RETIREMENT = 71;
        }

        public class PopupWindow
        {
            public static readonly IList<string> Items = new List<string>
            {
                "SMS0101003",
                "SMS0301002"
            }.AsReadOnly();
        }
        
        public class DialogType
        {
            public const string INFORMATION = "<i class='fa fa-check-circle'></i>";
            public const string WARNING = "<i class='fa fa-warning'></i>";
        }

        public class RegistType
        {
            public const string TEMP_REGIST = "仮登録";
            public const string ACTUAL_REGIST = "本登録";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { TEMP_REGIST, TEMP_REGIST },
                { ACTUAL_REGIST, ACTUAL_REGIST }
            }.AsReadOnly();
        }

        public class GetImage
        {
            public const string USER_IMAGE = "USER_IMAGE";
            public const string CUSTOMER_IMAGE = "CUSTOMER_IMAGE";
            public const string COMPANY_IMAGE = "COMPANY_IMAGE";
        }

        public class DeleteFlag
        {
            public const string NON_DELETE = "0";

            public const string DELETE = "1";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { NON_DELETE, "" },
                { DELETE, "削除" }
            }.AsReadOnly();
        }

        public class TimeConditionTypeList
        {
            public const string START_END = "0";

            public const string START = "1";

            public const string END = "2";

            public const string ACCEPTANCE = "3";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { START_END, "開始日・納品日" },
                { START, "開始日" },
                { END, "納品日" },
                { ACCEPTANCE, "検収日" }
            }.AsReadOnly();
        }

        public class SalesTypeStatusList
        {
            public const string DETERMINE_SALES = "0";

            public const string ESTIMATE_SALES = "1";

            public const string ALL = "2";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { ALL, "すべて" },
                { DETERMINE_SALES, "売上確定分" },
                { ESTIMATE_SALES, "見込み分" }
            }.AsReadOnly();
        }

        public class CompleteStatusSearchList
        {
            public const string OPEN = "0";

            public const string CLOSE = "1";

            public const string ALL = "2";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { ALL, "すべて" },
                { OPEN, "稼働中" },
                { CLOSE, "終了済" }
            }.AsReadOnly();
        }

        public class SuportFlag
        {

            public const string NON_REQUIRED = "0";

            public const string REQUIRED = "1";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { NON_REQUIRED, "不要" },
                { REQUIRED, "必要" }
            }.AsReadOnly();
        }

        public class ActualWorkModeFlag
        {

            public const string MODE_OLD = "0";

            public const string MODE_NEW = "1";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { MODE_OLD, "一括入力" },
                { MODE_NEW, "個別入力" }
            }.AsReadOnly();
        }

        public class UserLockFlag
        {

            public const string NON_LOCKED = "0";

            public const string LOCKED = "1";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { NON_LOCKED, "" },
                { LOCKED, "" }
            }.AsReadOnly();
        }

        public class BudgetSettingFlag
        {
            public const string OBJECT = "1";
            public const string BEYOND_OBJECT = "0";
        }

        public class StatusSalesType
        {
            public const string RECORDED_ON_SALES = "0";
            public const string PROSPECT = "1";
            public const string MISSED_OR_PENDING = "2";
            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { RECORDED_ON_SALES, "売上計上" },
                { PROSPECT, "見込み" },
                { MISSED_OR_PENDING, "失注・保留" },
            }.AsReadOnly();
        }
        # region 削除フラグ
        /// <summary>
        /// Password Lock Flag
        /// </summary>
        public class PasswordLockFlag
        {
            public const string NON_LOCK = "0";

            public const string LOCK = "1";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { NON_LOCK, "" },
                { LOCK, "パスワードロックフラグ" }
            }.AsReadOnly();
        }
        #endregion

        /// <summary>
        /// Temporary retirement flag
        /// </summary>
        public class Temporary_Retirement_Flg
        {
            public const string NON_RETIREMENT = "0";

            public const string RETIREMENT = "1";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { NON_RETIREMENT, "" },
                { RETIREMENT, "休職フラグ" }
            }.AsReadOnly();
        }

        public class WindowName
        {
            
            public const string COOKIE_NAME = "WindowName";

            
            public const string MAIN = "Main";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { "PMS01001", MAIN }
            }.AsReadOnly();
        }

        //Setting Role
        public class AuthorityRole
        {
            public const int SYSTEM_ADMINISTRATOR = 1;

            public const int PROJECT_MANAGER = 2;

            public const int GENERAL_USER = 3;
        }

        public class OrderingFlag
        {
            public const string SALES = "1";

            public const string PAYMENT = "2";
        }

        public class SupportTest
        {
            public const string SUPPORT = "1";

            public const string NOT_SUPPORT = "0";
        }

        public class LicenseDataType
        {
            public const string USER = "01";
            public const string CUSTOMER = "02";
            public const string GROUP = "03";
            public const string CONTRACT_TYPE = "04";
            public const string PHASE = "05";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { USER, "登録可能ユーザー数" },
                { CUSTOMER, "登録可能取引先数" },
                { GROUP, "登録可能所属数" },
                { CONTRACT_TYPE, "登録可能契約種別数" },
                { PHASE, "登録可能フェーズ数" }
            }.AsReadOnly();
        }

        public class WorkingDateType
        {
            public const string WEEKDAY = "01";
            public const string HOLIDAY = "02";
            public const string PUBLIC_HOLIDAY = "03";
            public const string NATIONAL_HOLIDAY = "04";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                {WEEKDAY,"平日"},
                {HOLIDAY,"休日"},
                {PUBLIC_HOLIDAY,"祝日"},
                {NATIONAL_HOLIDAY,"祭日"}
            }.AsReadOnly();
        }

        public class CustomerCsvImportKey
        {
            public const int customer_name = 0;
            public const int customer_name_kana = 1;
            public const int customer_name_en = 2;
            public const int display_name = 3;
            public const int zip_code = 4;
            public const int prefecture_name = 5;
            public const int city = 6;
            public const int address_1 = 7;
            public const int address_2 = 8;
            public const int tel_no = 9;
            public const int fax_no = 10;
            public const int mail_address = 11;
            public const int url = 12;
            public const int remarks = 13;
        }

        public class Month
        {
            public const int January = 1;
            public const int February = 2;
            public const int March = 3;
            public const int April = 4;
            public const int May = 5;
            public const int June = 6;
            public const int July = 7;
            public const int August = 8;
            public const int September = 9;
            public const int October = 10;
            public const int November = 11;
            public const int December = 12;
        }

        public class Quarter
        {
            public class QuarterText
            {
                public const string First_Quarter = "第１四半期";
                public const string Second_Quarter = "第２四半期";
                public const string Third_Quarter = "第３四半期";
                public const string Fourth_Quarter = "第４四半期";
            }

            public class QuarterValue
            {
                public const string First_Quarter = "1,2,3";
                public const string Second_Quarter = "4,5,6";
                public const string Third_Quarter = "7,8,9";
                public const string Fourth_Quarter = "10,11,12";
            }

        }

        public class CopyType
        {
            public const int NORMAL = 0;

            public const int ALL_INFORMATION = 1;
        }
    }

    public class SortTypeSummarySale
    {
        public const string Earnings = "0";
        public const string PROFIT = "1";
        public const string PROFIT_RATE = "2";
    }

    public class SearchObject
    {
        public const string Customer = "Customer";
        public const string EndUser = "EndUser";
    }

    #region ExcelCreatorテンプレート用

    /// <summary>
    /// ExcelCreatorのテンプレートで使用する変数名を定義するクラス
    /// </summary>
    public static class ExcelCellName
    {       
        /// <summary>
        /// 予 算 
        /// </summary>
        public const string BUDGET = "**Budget";

        /// <summary>
        /// 実 績 
        /// </summary>
        public const string ACTUAL = "**Actual";

        /// <summary>
        /// 利 益 率
        /// </summary>
        public const string ACHIEVE_RATE = "**AchieveRate";

        /// <summary>
        /// 達 成 率
        /// </summary>
        public const string RATE = "**Rate";

        /// <summary>
        /// 予 算 
        /// </summary>
        public const string BUDGET_ALL = "**BudgetAll";

        /// <summary>
        /// 実 績 
        /// </summary>
        public const string ACTUAL_ALL = "**ActualAll";

        /// <summary>
        /// 利 益 率
        /// </summary>
        public const string ACHIEVE_RATE_ALL = "**AchieveRateAll";

        /// <summary>
        /// 達 成 率
        /// </summary>
        public const string RATE_ALL = "**RateAll";

        /// <summary>
        /// 予 算 
        /// </summary>
        public const string BUDGET_MONTH = "**BudgetMonth";

        /// <summary>
        /// 実 績 
        /// </summary>
        public const string ACTUAL_MONTH = "**ActualMonth";

        /// <summary>
        /// 利 益 率
        /// </summary>
        public const string ACHIEVE_RATE_MONTH = "**AchieveRateMonth";

        /// <summary>
        /// 達 成 率
        /// </summary>
        public const string RATE_MONTH = "**RateMonth";

        public const string BUDGET_TOTAL_MONTH = "**BudgetTotalMonth";

        public const string ACTUAL_TOTAL_MONTH = "**ActualTotalMonth";

        public const string ACHIEVE_RATE_TOTAL_MONTH = "**AchieveRateTotalMonth";

        public const string RATE_TOTAL_MONTH = "**RateTotalMonth";

        /// <summary>
        /// 社員番号
        /// </summary>
        public const string EMPLOYEE_NO = "**employeeNo";

        /// <summary>
        /// 所属名
        /// </summary>
        public const string GROUP_NAME = "**groupName";

        /// <summary>
        /// 入社年月日
        /// </summary>
        public const string HIRE_DATE = "**hiredate";

        /// <summary>
        /// 拠点
        /// </summary>
        public const string BRANCH = "**branch";

        /// <summary>
        /// 社員名
        /// </summary>
        public const string EMPLOYEE_NAME = "**employeeName";

        /// <summary>
        /// 対象月
        /// </summary>
        public const string TARGET_YEAR = "**targetYear";

        /// <summary>
        /// 対象月
        /// </summary>
        public const string TAREGET_MONTH = "**targetMonth";

        /// <summary>
        /// 勤務日
        /// </summary>
        public const string WORKING_DATE = "**workingDate";

        /// <summary>
        /// 勤務区分
        /// </summary>
        public const string ATTENDANCE_TYPE = "**attendanceType";

        /// <summary>
        /// 有休時間（時）
        /// </summary>
        public const string ALLOWED_COST_HOUR = "**allowedCostHour";

        /// <summary>
        /// 有休時間（分）
        /// </summary>
        public const string ALLOWED_COST_MINUTE = "**allowedCostMinute";

        /// <summary>
        /// 備考
        /// </summary>
        public const string REMARKS = "**remarks";

        /// <summary>
        /// 打刻システム出社時間（時）
        /// </summary>
        public const string CLOCK_IN_START_HOUR = "**irecoStartHour";

        /// <summary>
        /// 打刻システム出社時間（分）
        /// </summary>
        public const string CLOCK_IN_START_MINUTE = "**irecoStartMinute";

        /// <summary>
        /// 打刻システム退社時間（時）
        /// </summary>
        public const string CLOCK_IN_END_HOUR = "**irecoEndHour";

        /// <summary>
        /// 打刻システム退社時間（分）
        /// </summary>
        public const string CLOCK_IN_END_MINUTE = "**irecoEndMinute";

        /// <summary>
        /// 出社時間（時）
        /// </summary>
        public const string START_HOUR = "**iproStartHour";

        /// <summary>
        /// 出社時間（分）
        /// </summary>
        public const string START_MINUTE = "**iproStartMinute";

        /// <summary>
        /// 退社時間（時）
        /// </summary>
        public const string END_HOUR = "**iproEndHour";

        /// <summary>
        /// 退社時間（分）
        /// </summary>
        public const string END_MINUTE = "**iproEndMinute";

        /// <summary>
        /// 休憩時間（時）
        /// </summary>
        public const string REST_HOUR = "**restHour";

        /// <summary>
        /// 休憩時間（分）
        /// </summary>
        public const string REST_MINUTE = "**restMinute";

        /// <summary>
        /// 祝日フラグ
        /// </summary>
        public const string IS_PUBLIC_HOLIDAY = "**isPublicHoliday";

        /// <summary>
        /// 期間
        /// </summary>
        public const string PERIOD = "**Period";

        /// <summary>
        /// 所属一覧
        /// </summary>
        public const string GROUP_LIST = "**GroupList";

        /// <summary>
        /// 契約種別一覧
        /// </summary>
        public const string CONTRACT_TYPE_LIST = "**ContractTypeList";

        /// <summary>
        /// 所属
        /// </summary>
        public const string GROUP = "**Group";

        /// <summary>
        /// 契約種別
        /// </summary>
        public const string CONTRACT_TYPE = "**ContractType";

        /// <summary>
        /// 予 算 
        /// </summary>
        public const string BUDGET_SALES = "**BudgetSales";
        public const string BUDGET_PROFIT = "**BudgetProfit";
        /// <summary>
        /// 実 績 
        /// </summary>
        public const string ACTUAL_SALES = "**ActualSales";
        public const string ACTUAL_PROFIT = "**ActualProfit";
        /// <summary>
        /// 利 益 率
        /// </summary>
        public const string ACHIEVE_RATE_PROFIT = "**AchieveRateProfit";

        /// <summary>
        /// 達 成 率
        /// </summary>
        public const string RATE_SALES = "**RateSales";
        public const string RATE_PROFIT = "**RateProfit";

        /// <summary>
        /// 予 算 
        /// </summary>
        public const string BUDGET_ALL_SALES = "**BudgetAllSales";
        public const string BUDGET_ALL_PROFIT = "**BudgetAllProfit";

        /// <summary>
        /// 実 績 
        /// </summary>
        public const string ACTUAL_ALL_SALES = "**ActualAllSales";
        public const string ACTUAL_ALL_PROFIT = "**ActualAllProfit";
        /// <summary>
        /// 利 益 率
        /// </summary>
        public const string ACHIEVE_RATE_ALL_PROFIT = "**AchieveRateAllProfit";

        /// <summary>
        /// 達 成 率
        /// </summary>
        public const string RATE_ALL_SALES = "**RateAllSales";
        public const string RATE_ALL_PROFIT = "**RateAllProfit";

        /// <summary>
        /// 予 算 
        /// </summary>
        public const string BUDGET_MONTH_SALES = "**BudgetMonthSales";
        public const string BUDGET_MONTH_PROFIT = "**BudgetMonthProfit";
        /// <summary>
        /// 実 績 
        /// </summary>
        public const string ACTUAL_MONTH_SALES = "**ActualMonthSales";
        public const string ACTUAL_MONTH_PROFIT = "**ActualMonthProfit";

        /// <summary>
        /// 利 益 率
        /// </summary>
        public const string ACHIEVE_RATE_MONTH_PROFIT = "**AchieveRateMonthProfit";

        /// <summary>
        /// 達 成 率
        /// </summary>
        public const string RATE_MONTH_SALES = "**RateMonthSales";
        public const string RATE_MONTH_PROFIT = "**RateMonthProfit";

        /// <summary>
        /// 月
        /// </summary>
        public const string MONTH = "**Month";

        public const string TOTAL_MONTH = "**TotalMonth";

        public const string BUDGET_TOTAL_MONTH_SALES = "**BudgetTotalMonthSales";
        public const string BUDGET_TOTAL_MONTH_PROFIT = "**BudgetTotalMonthProfit";

        public const string ACTUAL_TOTAL_MONTH_SALES = "**ActualTotalMonthSales";
        public const string ACTUAL_TOTAL_MONTH_PROFIT = "**ActualTotalMonthProfit";

        public const string ACHIEVE_RATE_TOTAL_MONTH_PROFIT = "**AchieveRateTotalMonthProfit";

        public const string RATE_TOTAL_MONTH_SALES = "**RateTotalMonthSales";
        public const string RATE_TOTAL_MONTH_PROFIT = "**RateTotalMonthProfit";

        public const string INCLUDING_EXPECTED = "**IncludingExpected";

    }

    #endregion
}