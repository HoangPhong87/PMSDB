#region License
/// <copyright file="PMS03001ListViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/26</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS03001
{
    using ProjectManagementSystem.Models.PMS03001;

    public class PMS03001ListViewModel
    {
        public Condition Condition { get; set; }

        public PMS03001ListViewModel()
        {
            Condition = new Condition();
        }
    }
}
