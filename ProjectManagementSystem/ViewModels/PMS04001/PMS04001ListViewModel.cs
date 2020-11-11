#region License
/// <copyright file="PMS04001ListViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/20</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS04001
{
    using ProjectManagementSystem.Models.PMS04001;

    public class PMS04001ListViewModel
    {
        public Condition Condition { get; set; }

        public PMS04001ListViewModel()
        {
            Condition = new Condition();
        }
    }
}
