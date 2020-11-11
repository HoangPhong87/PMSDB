using ProjectManagementSystem.Models.Entities;
using System;

namespace ProjectManagementSystem.Models.PMS01002
{
    using PetaPoco;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// project_info plus model class
    /// </summary>
    public class UnitPriceHistoryPlus : UnitPriceHistory
    {
        public bool? isNew { get; set; }
        public bool? isDelete { get; set; }
        public bool? isUpdate { get; set; }
    }
}