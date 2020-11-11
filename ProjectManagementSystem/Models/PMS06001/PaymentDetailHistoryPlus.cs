#region License
/// <copyright file="PaymentDetailHistoryPlus.cs" company="i-Enter Asia">
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
    /// payment_detail_history plus model class
    /// </summary>
    public class PaymentDetailHistoryPlus : PaymentDetailHistory
    {
        public string target_time { get; set; }
    }
}