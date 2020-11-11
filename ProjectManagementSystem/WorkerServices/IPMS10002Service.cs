using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10002;
using ProjectManagementSystem.ViewModels;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices
{
    public interface IPMS10002Service
    {
        /// <summary>
        /// Get infomartion list by page
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        PageInfo<InformationPlus> Search(DataTablesModel model, Condition condition, string companyCode);

        /// <summary>
        /// Get list tag
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>CustomerTagPlus</returns>
        List<InformationPlus> GetListInfomation(Condition condition, string company_code, string orderBy, string orderType);

        /// <summary>
        /// Get information detail
        /// </summary>
        /// <param name="company_code"></param>
        /// <param name="infoId"></param>
        /// <returns></returns>
        InformationPlus GetInformation(string company_code, int infoId);

        /// <summary>
        /// Edit information data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int EditInformationData(Information data);
    }
}
