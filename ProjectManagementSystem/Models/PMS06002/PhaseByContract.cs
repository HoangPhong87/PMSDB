using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS06002
{
    [Serializable]

    public class PhaseByContract
    {
        public int? ContractTypeId { get; set; }
        public int? PhaseId { get; set; }
    }
}