using ProjectManagementSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS06002
{
    /// <summary>
    /// m_phase plus model class
    /// </summary>s
    public class PhasePlus:Phase
    {
        public int contract_type_id { get; set; }
        public string contract_type { get; set; }
    }
}