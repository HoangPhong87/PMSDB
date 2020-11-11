#region License
/// <copyright file="PMS09004Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/07/15</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS09004;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    /// <summary>
    /// Service class
    /// </summary>
    public class PMS09004Service : IPMS09004Service
    {
        /// <summary>
        /// Interface
        /// </summary>
        private IPMS09004Repository _repository;

        /// <summary>
        /// Contractor
        /// </summary>
        public PMS09004Service()
            : this(new PMS09004Repository())
        {
        }

        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="_repository"></param>
        public PMS09004Service(IPMS09004Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Sales payment list
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PageInfo<PaymentSalesDetail> GetSalesPaymentDetailList(DataTablesModel model, Condition condition)
        {
            return this._repository.GetSalesPaymentDetailList(model.iDisplayStart, model.iDisplayLength, model.sColumns, model.iSortCol_0, model.sSortDir_0, condition);
        }

        /// <summary>
        /// Sales payment list export
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IList<PaymentSalesDetail> GetSalesPaymentDetailListExport(Condition condition)
        {
            return this._repository.GetSalesPaymentDetailListExport(condition);
        }

    }
}
