using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_language")]
    [PetaPoco.PrimaryKey("language_id")]
    public class Language
    {
        public int language_id { get; set; }

        public string language_code { get; set; }

        public string language { get; set; }

        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }
    }
}