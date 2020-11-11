using ProjectManagementSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS06002
{
    public class PhaseInContractType
    {
        public int contract_type_id { get; set; }
        public string contract_type { get; set; }
        public List<Phase> list_phase { get; set; }
    }
}