using System;

namespace ProjectManagementSystem.Common
{
    /// <summary>
    /// User right class
    /// </summary>
    [Serializable]
    public class UserAuthority
    {
        /// <summary>Right</summary>
        public string AUTHORITY_TYPE { get; set; }

        /// <summary>Right name</summary>
        public string AUTHORITY_NAME { get; set; }

        /// <summary>Function code</summary>
        public string FUNCTION_CODE { get; set; }

        /// <summary>Function name</summary>
        public string FUNCTION_NAME { get; set; }
    }
}
