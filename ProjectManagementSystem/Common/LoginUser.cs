using System;
using System.Collections.Generic;

namespace ProjectManagementSystem.Common
{
    /// <summary>
    /// Login user entity class
    /// </summary>
    [Serializable]
    public class LoginUser
    {
        public int UserId { get; set; }
        public string CompanyCode { get; set; }
        public string UserAccount { get; set; }
        public string Password { get; set; }
        public string UserNameSei { get; set; }
        public string UserNameMei { get; set; }
        public string FuriganaSei { get; set; }
        public string FuriganaMei { get; set; }
        public string DisplayName { get; set; }
        public int? PositionId { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public string CompanyLogoImgPath { get; set; }
        public int? RoleId { get; set; }
        public string DecimalCalculationType { get; set; }
        public string ActualWorkInputMode { get; set; }
        public List<int> FunctionList { get; set; }
        public List<int> PlanFunctionList { get; set; }
        public int Working_time_unit_minute { get; set; }
        public bool Is_expired_password { get; set; }
        public string CompanyName { get; set; }
        public int DataEditTableTime { get; set; }
        public string ImageFilePath { get; set; }
    }
}