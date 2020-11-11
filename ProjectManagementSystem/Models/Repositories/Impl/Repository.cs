#region Licence
/// <copyright file="Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/019</createdDate>
#endregion

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using PetaPoco;
    using ProjectManagementSystem.Common;

    /// <summary>
    /// Base class for all repository classes
    /// </summary>
    public class Repository : IRepository
    {
        private readonly PMSDatabase _database;

        /// <summary>
        /// 
        /// </summary>
        public Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        public Repository(PMSDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> FindAll<T>()
        {

            return Select<T>(null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> FindAllMst<T>()
        {

            return FindAllMst<T>(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public IList<T> FindAllMst<T>(params string[] orderby)
        {
            IDictionary<string, object> condition = new Dictionary<string, object>()
            {
                { "DEL_FLG", "0" },
            };

            return Select<T>(condition, orderby);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public IList<T> FindAll<T>(params string[] orderby)
        {

            return Select<T>(null, orderby);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKeyValue"></param>
        /// <returns></returns>
        public T FindById<T>(object primaryKeyValue)
        {
            string tableName = _database.GetTableName(typeof(T));
            string primaryKey = _database.GetPrimaryKeyName(typeof(T));

            T ret = default(T);

            if (!string.IsNullOrEmpty(tableName))
            {
                var primaryKeyValues = GetPrimaryKeyValues(primaryKey, primaryKeyValue);
                var list = Select<T>(primaryKeyValues, null);

                ret = list.SingleOrDefault();
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IList<T> Select<T>(IDictionary<string, object> condition)
        {
            return Select<T>(condition, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public IList<T> Select<T>(IDictionary<string, object> condition, params string[] orderby)
        {
            IList<T> list = new List<T>();

            string tableName = _database.GetTableName(typeof(T));

            if (!string.IsNullOrEmpty(tableName))
            {
                string[] cols = _database.GetColumnsName(typeof(T));

                Sql sql = Sql.Builder.Select(cols).From(tableName);

                if (condition != null)
                {
                    var index = 0;
                    var s = BuildConditionSql(condition, ref index);
                    var aaa = condition.Select(x => x.Value).ToArray();
                    sql = sql.Where(s, condition.Select(x => x.Value).ToArray());
                }

                if (orderby != null)
                {
                    sql = sql.OrderBy(orderby);
                }

                list = _database.Fetch<T>(sql);
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public PageInfo<T> Page<T>(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Sql sql)
        {
            int startPage = (startItem == 0) ? 1 : startItem / itemsPerPage + 1;

            if (sortCol.HasValue && !string.IsNullOrEmpty(columns))
            {
                string[] sCol = columns.Split(',');

                if (sortCol < sCol.Length && !string.IsNullOrEmpty(sCol[sortCol.Value]))
                {
                    sql.Append(string.Format(" order by {0} {1}", sCol[sortCol.Value], sortDir));
                }
            }

            Page<T> page = _database.Page<T>(startPage, itemsPerPage, sql);

            PageInfo<T> pageInfo = new PageInfo<T>()
            {
                CurrentPageNo = page.CurrentPage,
                TotalPageNo = page.TotalPages,
                TotalItems = page.TotalItems,
                ItemsPerPage = page.ItemsPerPage,
                Items = page.Items
            };

            return pageInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="colmuns"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int Update<T>(IDictionary<string, object> colmuns, IDictionary<string, object> condition)
        {
            int count = 0;

            string tableName = _database.GetTableName(typeof(T));

            if (!string.IsNullOrEmpty(tableName))
            {
                string sql = string.Empty;
                var index = 0;
                object[] args = new object[0];

                if (colmuns != null)
                {
                    var s = BuildUpdateColmunsSql(colmuns, ref index);
                    sql += string.Format("SET {0}", s);
                    args = args.Concat(colmuns.Select(x => x.Value)).ToArray();
                }

                if (condition != null)
                {
                    var s = BuildConditionSql(condition, ref index);
                    sql += string.Format(" WHERE {0}", s);
                    args = args.Concat(condition.Select(x => x.Value)).ToArray();
                }

                count = _database.Update<T>(sql, args);
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string BuildConditionSql(IDictionary<string, object> condition, ref int index)
        {
            var tempIndex = index;
            index += condition.Count;
            return string.Join(" AND ", condition.Select((x, i) => string.Format("[{0}] = @{1}", x.Key, tempIndex + i)).ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colmuns"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string BuildUpdateColmunsSql(IDictionary<string, object> colmuns, ref int index)
        {
            var tempIndex = index;
            index += colmuns.Count;
            return string.Join(", ", colmuns.Select((x, i) => string.Format("[{0}] = @{1}", x.Key, tempIndex + i)).ToArray());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="primaryKeyName"></param>
        /// <param name="primaryKeyValue"></param>
        /// <returns></returns>
        private IDictionary<string, object> GetPrimaryKeyValues(string primaryKeyName, object primaryKeyValue)
        {
            Dictionary<string, object> primaryKeyValues;

            primaryKeyName = primaryKeyName.Replace("[", "").Replace("]", "");

            var multiplePrimaryKeysNames = primaryKeyName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            if (primaryKeyValue != null)
            {
                if (multiplePrimaryKeysNames.Length == 1 && primaryKeyValue is string)
                {
                    primaryKeyValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { primaryKeyName, primaryKeyValue } };
                }
                else
                {
                    primaryKeyValues = multiplePrimaryKeysNames.ToDictionary(x => x, x => primaryKeyValue.GetType().GetProperties().Single(y => string.Equals(x, y.Name, StringComparison.OrdinalIgnoreCase)).GetValue(primaryKeyValue, null), StringComparer.OrdinalIgnoreCase);
                }
            }
            else
            {
                primaryKeyValues = multiplePrimaryKeysNames.ToDictionary(x => x, x => (object)null, StringComparer.OrdinalIgnoreCase);
            }
            return primaryKeyValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco"></param>
        /// <returns></returns>
        public object Insert<T>(object poco)
        {
            return _database.Insert(poco);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="poco"></param>
        /// <returns></returns>
        public object Insert<T>(string tableName, object poco)
        {
            return _database.Insert(tableName, "", false, poco);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int Delete<T>(IDictionary<string, object> condition)
        {
            int deletecount = 0;
            string tableName = _database.GetTableName(typeof(T));

            if (!string.IsNullOrEmpty(tableName))
            {
                string sql = "DELETE FROM " + tableName + " ";
                var index = 0;
                object[] args = new object[0];

                if (condition != null)
                {
                    var s = BuildConditionSql(condition, ref index);
                    sql += string.Format(" WHERE {0}", s);
                    args = args.Concat(condition.Select(x => x.Value)).ToArray();
                }

                deletecount = _database.Execute(sql, args);
            }

            return deletecount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void Initialize<T>(object data)
        {
            System.Type t = typeof(T);

            MemberInfo[] members = t.GetMembers(BindingFlags.Public | BindingFlags.Instance);

            foreach (MemberInfo m in members)
            {
                if (m.MemberType == MemberTypes.Property)
                {
                    PropertyInfo pi = t.GetProperty(m.Name);

                    if (pi != null)
                    {
                        if (pi.PropertyType.Name.Equals("String"))
                        {
                            var ret = (string)pi.GetValue(data, null);
                            if (ret == null)
                            {
                                pi.SetValue(data, "", null);
                            }
                        }
                        else if (pi.PropertyType.Name.Equals("DateTime"))
                        {
                            var ret = (DateTime)pi.GetValue(data, null);
                            if (ret == DateTime.MinValue)
                            {
                                pi.SetValue(data, new DateTime(1753, 1, 1, 0, 0, 0), null);
                            }
                        }
                        else if (pi.PropertyType.Name.Equals("Byte"))
                        {
                            var ret = (Byte[])pi.GetValue(data, null);
                            if (ret == null)
                            {
                                //pi.SetValue(data, "", null);
                            }
                        }

                    }
                }
            }

        }

        /// <summary>
        /// Construct a string representing name of columns to be selected on pivot query
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of colum by month</returns>
        public string BuildColumnByMonth(DateTime startDate, DateTime endDate)
        {
            var sb = new StringBuilder();
            var y = startDate.Year;
            var m = startDate.Month;

            while (y < endDate.Year || (y == endDate.Year && m <= endDate.Month))
            {
                sb.AppendFormat("[{0}/{1:00}],", y, m);
                m++;
                if (m == 13)
                {
                    m = 1;
                    y++;
                }
            }
            return sb.ToString(0, sb.Length - 1);
        }

        /// <summary>
        /// Remove wildcard characters
        /// </summary>
        /// <param name="value">text value</param>
        /// <returns>new text value</returns>
        public string replaceWildcardCharacters(string value)
        {
            return value.Replace("\\", "\\\\").Replace("[", "\\[").Replace("_", "\\_").Replace("%", "\\%").Replace("'", "''").Replace("@", "@\\");
        }
    }
}