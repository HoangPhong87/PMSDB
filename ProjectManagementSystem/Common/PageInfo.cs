using System.Collections.Generic;

namespace ProjectManagementSystem.Common
{
    /// <summary>
    /// Page information class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageInfo<T>
    {
        /// <summary>current page number</summary>
        public long CurrentPageNo { get; set; }

        /// <summary>Total number of pages</summary>
        public long TotalPageNo { get; set; }

        /// <summary>Total number of items</summary>
        public long TotalItems { get; set; }

        /// <summary>Number of items per page</summary>
        public long ItemsPerPage { get; set; }

        /// <summary>The collection of items</summary>
        public List<T> Items { get; set; }
    }
}