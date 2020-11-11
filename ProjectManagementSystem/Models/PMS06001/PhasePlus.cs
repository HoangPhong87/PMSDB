#region License
/// <copyright file="PhasePlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/19</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;
using System;
using System.ComponentModel;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// m_phase plus model class
    /// </summary>
    public class PhasePlus : Phase
    {
        public bool check { get; set; }

        public decimal estimate_man_days { get; set; }
    }
}