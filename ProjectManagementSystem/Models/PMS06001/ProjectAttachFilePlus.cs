#region License
/// <copyright file="ProjectAttachFilePlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/10/24</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// project_attached_file plus model class
    /// </summary>
    public class ProjectAttachFilePlus : ProjectAttachFile
    {
        public bool file_public_flag
        {
            get { return (public_flg == Constant.DeleteFlag.DELETE); }
            set { public_flg = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public bool change_file { get; set; }

        public bool change_info { get; set; }

        public bool delete_file { get; set; }

        public string file_path_old { get; set; }
    }
}