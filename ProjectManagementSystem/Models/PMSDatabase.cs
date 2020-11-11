using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using log4net;
using PetaPoco;
using PetaPoco.Internal;

namespace ProjectManagementSystem.Models
{
    public class PMSDatabase : Database
    {
        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        
        private const string PARAMETER_DECLARE = "declare {0} as {1}";
        
        private const string PARAMETER_DECLARE_SIZE = "({0})";
        
        private const string PARAMETER_DECLARE_SCALE_SIZE = "({0}, {1})";
        
        private const string PARAMETER_OUTPUT = "set {0} = {1}";
        
        private const string PARAMETER_OUTPUT_NULL = "set {0} = null";
        
        private const string PARAMETER_OUTPUT_STRING = "set {0} = '{1}'";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionStringName"></param>
        public PMSDatabase(string connectionStringName) : base(connectionStringName)
        {
        }

        /// <summary>
        /// Get table name
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public string GetTableName(Type t)
        {
            return _dbType.EscapeSqlIdentifier(PocoData.ForType(t).TableInfo.TableName);
        }

        /// <summary>
        /// Get Primary key name
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public string GetPrimaryKeyName(Type t)
        {
            return _dbType.EscapeSqlIdentifier(PocoData.ForType(t).TableInfo.PrimaryKey);
        }

        /// <summary>
        /// Get column name
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public string[] GetColumnsName(Type t)
        {
            var pd = PocoData.ForType(t);

            string [] cols = (from c in pd.Columns
                              select GetTableName(t) + "." + _dbType.EscapeSqlIdentifier(c.Key)).ToArray();
                             //select new { GetTableName(t) + "." + _dbType.EscapeSqlIdentifier(c.Value.ColumnName)}).ToArray();

            return cols;
            //return PocoData.ForType(t).QueryColumns;
        }

        /// <summary>
        /// Event on Executing Command
        /// </summary>
        /// <param name="cmd"></param>
        public override void OnExecutingCommand(System.Data.IDbCommand cmd)
        {
            base.OnExecutingCommand(cmd);

            string log = GetSqlLog(cmd);

            logger.Debug(log);
        }

        /// <summary>
        /// Event on Exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override bool OnException(Exception ex)
        {
            logger.Fatal("A fatal error occurred", ex);
            logger.Fatal(LastCommand);

            return true;
        }
        /// <summary>
        /// Get SQL Log
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected string GetSqlLog(System.Data.IDbCommand cmd)
        {
            
            const string HEX_FORMAT = "{0:X}";
            const string HEX_HEADER = "0x";

            
            const string DB_TYPE_CHAR = "char";
            const string DB_TYPE_TEXT = "text";
            const string DB_TYPE_DATE = "date";

            var declare = new StringBuilder();
            var values = new StringBuilder();
            var binaryValues = new StringBuilder();
            string scaleSize = string.Empty;

            foreach (SqlParameter parameter in cmd.Parameters)
            {
                
                declare.AppendLine().AppendFormat(PARAMETER_DECLARE, parameter.ParameterName, parameter.SqlDbType);
                if (0 < parameter.Precision)
                {
                    declare.AppendFormat(PARAMETER_DECLARE_SCALE_SIZE, parameter.Precision, parameter.Scale);
                }
                else if (0 < parameter.Size)
                {
                    declare.AppendFormat(PARAMETER_DECLARE_SIZE, parameter.Size);
                }

                
                if (parameter.Value is DBNull)
                {
                    values.AppendLine().AppendFormat(PARAMETER_OUTPUT_NULL, parameter.ParameterName);
                }
                else if (0 <= parameter.SqlDbType.ToString().ToLower().IndexOf(DB_TYPE_CHAR)
                    || 0 <= parameter.SqlDbType.ToString().ToLower().IndexOf(DB_TYPE_TEXT)
                    || 0 <= parameter.SqlDbType.ToString().ToLower().IndexOf(DB_TYPE_DATE))
                {
                    values.AppendLine().AppendFormat(PARAMETER_OUTPUT_STRING, parameter.ParameterName, parameter.Value);
                }
                else if (typeof(byte[]).Equals(parameter.Value.GetType()))
                {
                    binaryValues.Length = 0;
                    foreach (byte target in (byte[])parameter.Value)
                    {
                        binaryValues.AppendFormat(HEX_FORMAT, target);
                    }
                    binaryValues.Insert(0, HEX_HEADER);

                    values.AppendLine().AppendFormat(PARAMETER_OUTPUT, parameter.ParameterName, binaryValues.ToString());
                }
                else
                {
                    values.AppendLine().AppendFormat(PARAMETER_OUTPUT, parameter.ParameterName, parameter.Value);
                }
            }

            var sb = new StringBuilder();
            if (0 < declare.Length)
            {
                sb.AppendLine(declare.ToString());
            }
            if (0 < values.Length)
            {
                sb.AppendLine(values.ToString()).AppendLine();
            }
            sb.AppendLine(cmd.CommandText);

            return sb.ToString();
        }
    }
}