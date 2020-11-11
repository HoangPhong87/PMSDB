#region License
/// <copyright file="TargetCategoryPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/28</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// target_category plus model class
    /// </summary>
    public class TargetCategoryPlus : TargetCategory
    {
        public string category_name { get; set; }

        public string sub_category_name { get; set; }

        public bool is_default { get; set; }

        public TargetCategoryPlus()
        {
            is_default = false;
        }
    }
}