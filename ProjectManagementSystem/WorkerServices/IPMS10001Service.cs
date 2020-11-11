using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10001;
using ProjectManagementSystem.ViewModels;

namespace ProjectManagementSystem.WorkerServices
{
    public interface IPMS10001Service
    {
        PageInfo<CustomerTagPlus> Search(DataTablesModel model, Condition condition, string companyCode);

        /// <summary>
        /// Get list tag
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>CustomerTagPlus</returns>
        List<CustomerTagPlus> GetListTag(Condition condition, string companycode, string orderBy, string orderType);

        CustomerTagPlus GetTagInfo(string cCode, int tagId);

        int EditTagData(CustomerTag data);
        int GetDataSalesPayment(string company_code, int customer_id, int tag_id);
    }
}
