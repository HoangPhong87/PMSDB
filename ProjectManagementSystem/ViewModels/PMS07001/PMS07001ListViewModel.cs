#region License
/// <copyright file="PMS07001ListViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/23</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS07001
{
    using ProjectManagementSystem.Models.PMS07001;

    public class PMS07001ListViewModel
    {
        public Condition Condition { get; set; }

        public PMS07001ListViewModel()
        {
            Condition = new Condition();
        }
    }
}
