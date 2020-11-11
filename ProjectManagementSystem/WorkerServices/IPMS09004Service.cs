#region License
/// <copyright file="IPMS09004Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/07/15</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS09004;
using ProjectManagementSystem.ViewModels;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices
{
    /// <summary>
    /// Service interface
    /// </summary>
    public interface IPMS09004Service
    {
        /// <summary>
        /// Get payment sales detail list
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        PageInfo<PaymentSalesDetail> GetSalesPaymentDetailList(DataTablesModel model, Condition condition);

        /// <summary>
        /// Sales payment list export
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IList<PaymentSalesDetail> GetSalesPaymentDetailListExport(Condition condition);
    }
}
