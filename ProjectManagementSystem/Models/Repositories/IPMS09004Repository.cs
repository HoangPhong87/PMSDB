#region License
/// <copyright file="IPMS09004Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/07/15</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS09004;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories
{
    /// <summary>
    /// Project screen repository interface class
    /// </summary>
    public interface IPMS09004Repository
    {
        /// <summary>
        /// Get sales payment list
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        PageInfo<PaymentSalesDetail> GetSalesPaymentDetailList(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition);

        /// <summary>
        /// Get sales payment list export
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IList<PaymentSalesDetail> GetSalesPaymentDetailListExport(Condition condition);
    }
}
