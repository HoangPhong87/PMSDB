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
    [Serializable]
    public class UserPlus : User
    {
        public int peta_rn { get; set; }

        public string display_name_group { get; set; }

        public string display_name_position { get; set; }

        public string is_active { get; set; }

        public string user_update { get; set; }

        public string user_regist { get; set; }

        public IList<UnitPriceHistoryPlus> unit_price_history { get; set; }

        public decimal? base_unit_cost { get; set; }

        public string temporary_retirement_flg { get; set; }
    }
}