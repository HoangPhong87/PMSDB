using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10005;
using ProjectManagementSystem.ViewModels;

namespace ProjectManagementSystem.WorkerServices
{
    public interface IPMS10005Service
    {
        PageInfo<OverHeadCostPlus> Search(DataTablesModel model, Condition condition);

        List<OverHeadCostPlus> GetOverheadCostList(Condition condition, string orderBy, string orderType);

        OverHeadCostPlus GetOverheadCostInfo(string company_code, int overhead_cost_id);

        int EditOverheadCostData(OverHeadCostPlus data);

        int DeleteOverHeadCode(string companyCode, int overhead_cost_id);

        int CheckExistOfOverHeadCode(string companyCode, int overhead_cost_id);
    }
}
