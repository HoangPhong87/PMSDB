#region Licence
/// <copyright file="IRepository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/019</createdDate>
#endregion
namespace ProjectManagementSystem.Models.Repositories
{
    using System;
    using System.Collections.Generic;
    using PetaPoco;
    using ProjectManagementSystem.Common;

    public interface IRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IList<T> FindAll<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IList<T> FindAllMst<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderby"></param>
        /// <returns></returns>
        IList<T> FindAllMst<T>(params string[] orderby);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKeyValue"></param>
        /// <returns></returns>
        T FindById<T>(object primaryKeyValue);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        IList<T> Select<T>(IDictionary<string, object> condition);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        IList<T> Select<T>(IDictionary<string, object> condition, params string[] orderby);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="colmuns"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        int Update<T>(IDictionary<string, object> colmuns, IDictionary<string, object> condition);

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
        PageInfo<T> Page<T>(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Sql sql);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco"></param>
        /// <returns></returns>
        object Insert<T>(object poco);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="poco"></param>
        /// <returns></returns>
        object Insert<T>(string tableName, object poco);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        int Delete<T>(IDictionary<string, object> condition);

        /// <summary>
        /// Construct a string representing name of columns to be selected on pivot query
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of colum by month</returns>
        string BuildColumnByMonth(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Remove wildcard characters
        /// </summary>
        /// <param name="value">text value</param>
        /// <returns>new text value</returns>
        string replaceWildcardCharacters(string value);
    }
}
