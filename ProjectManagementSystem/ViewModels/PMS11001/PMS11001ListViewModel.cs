#region License
/// <copyright file="PMS11001ListViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HungLQ</author>
/// <createdDate>2017/06/28</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS11001
{
    using ProjectManagementSystem.Models.PMS11001;

    public class PMS11001ListViewModel
    {
        public Condition Condition { get; set; }

        public PMS11001ListViewModel()
        {
            Condition = new Condition();
        }
    }
}
