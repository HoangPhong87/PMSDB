using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10002;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories
{
    public interface IPMS10002Repository
    {
        /// <summary>
        /// Get list information data
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="condition"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        PageInfo<InformationPlus> Search(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition, string companyCode);

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

        /// <summary>
        /// Build SQL infomation list
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        Sql BuildSqlInformationList(string companyCode, Condition condition);
    }
}
