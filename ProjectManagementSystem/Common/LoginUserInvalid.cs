using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Common
{
	/// <summary>
    /// Invalid login user class
    /// </summary>
    [Serializable]
    public class UserLoginInvalid
    {
        public int UserId { get; set; }

        public string CompanyCode { get; set; }

        public int InvalidCount { get; set; } 

        public UserLoginInvalid(string companyCode, int userId)
        {
            this.CompanyCode = companyCode;
            this.UserId = userId;
            InvalidCount = 0;
        }

    }
}