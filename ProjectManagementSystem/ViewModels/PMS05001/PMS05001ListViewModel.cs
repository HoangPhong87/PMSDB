#region License
/// <copyright file="PMS05001ListViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/22</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS05001
{
    using ProjectManagementSystem.Models.PMS05001;

    public class PMS05001ListViewModel
    {
        public Condition Condition { get; set; }

        public PMS05001ListViewModel()
        {
            Condition = new Condition();
        }
    }
}
