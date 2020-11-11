#region License
/// <copyright file="SalesPaymentHistoryPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/11/28</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// sales_payment_history plus model class
    /// </summary>
    public class SalesPaymentHistoryPlus : SalesPaymentHistory
    {
        public string customer_name { get; set; }

        public string charge_person_name { get; set; }
    }
}