#region License
/// <copyright file="SalesPaymentDetailPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/21</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// sales_payment_detail plus model class
    /// </summary>
    public class SalesPaymentDetailPlus : SalesPaymentDetail
    {
        public string customer_name { get; set; }

        public string target_time { get; set; }
    }
}