using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS09003;
using ProjectManagementSystem.ViewModels;

namespace ProjectManagementSystem.WorkerServices
{
    public interface IPMS09003Service
    {
        IEnumerable<Group> GetGroupList(string cCode);
        PageInfo<SalesCustomerPlus> Search(DataTablesModel model, Condition condition, string companyCode);

        IList<SalesCustomerPlus> SearchAll(string companyCode, Condition condition);

        List<SalesCustomerPlus> GetListSalesCustomer(Condition condition, string companycode, string orderBy, string orderType);

        PageInfo<SalesTagByCustomerPlus> SearchTagByCustomer(DataTablesModel model, string companyCode, ConditionSaleTag condition);

        PageInfo<SalesProjectByCustomerPlus> SearchProjectByCustomer(DataTablesModel model, string companyCode, ConditionSaleProject condition);
    }
}
