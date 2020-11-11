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
    /// r_contract_type_phase plus model class
    /// </summary>
    public class ContractTypePhasePlus : ContractTypePhase
    {
        public string phase_name { get; set; }

        public int project_count { get; set; }
    }
}