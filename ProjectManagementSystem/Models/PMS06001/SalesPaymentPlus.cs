#region License
/// <copyright file="SalesPaymentPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/19</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// sales_payment plus model class
    /// </summary>
    public class SalesPaymentPlus : SalesPayment
    {
        public string customer_name { get; set; }

        public string charge_person_name { get; set; }

        public string end_user_name { get; set; }
    }
}