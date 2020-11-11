#region License
/// <copyright file="ContractTypePhasePlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/26</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS03001
{
    /// <summary>
    /// r_contract_type_category plus model class
    /// </summary>
    public class ContractTypeCategoryPlus : ContractTypeCategory
    {
        public string category { get; set; }

        public int project_count { get; set; }
    }
}