using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Aeon.HR.BusinessObjects.Helpers
{
    public class DataHelper : IDisposable
    {
        private SqlConnection Conn;
        protected readonly string _connString;
       protected readonly ILogger _logger;

        #region Construction
        public DataHelper(string connString, ILogger logger)
        {
            _logger = logger;
            _connString = connString;
            Conn = new SqlConnection(connString);
        }
        #endregion
        public void Dispose()
        {
            try
            {
                Conn?.Close();
                Conn?.Dispose();

            }
            catch (Exception ex)
            {
                ex.LogError(_logger, "Aeon.HR.BusinessObjects.DataHelper.Dispose");
            }
        }

        public SqlDataReader GetAsDataReader(string sqlCommandText)
        {
            SqlDataReader returnValue = null;
            try
            {
                Conn = new SqlConnection(_connString);
                Conn.Open();
                SqlCommand command = new SqlCommand(sqlCommandText, Conn);
                returnValue = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                returnValue = null;
                ex.LogError(_logger, "Aeon.HR.BusinessObjects.DataHelper.GetDataAsDataReader");
            }
            return returnValue;
        }

        public SqlDataReader GetAsDataReader(string storedProcedureName, SqlParameter[] paras)
        {
            SqlDataReader returnValue = null;
            try
            {
                Conn = new SqlConnection(_connString);
                Conn.Open();
                SqlCommand command = new SqlCommand(storedProcedureName, Conn);
                command.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter currentParam in paras)
                {
                    command.Parameters.Add(currentParam);
                }
                returnValue = command.ExecuteReader();

            }
            catch (Exception ex)
            {
                returnValue = null;
                ex.LogError(_logger, "Aeon.HR.BusinessObjects.DataHelper.GetAsDataReader");
            }
            return returnValue;
        }

        public Dictionary<string, DataTable> GetDicSyncsData(string storedProcedureName, SqlParameter[] paras)
        {
            Dictionary<string, DataTable> dicSyncsData = new Dictionary<string, DataTable>();
            try
            {
                Conn = new SqlConnection(_connString);
                Conn.Open();
                SqlCommand command = new SqlCommand(storedProcedureName, Conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                foreach (SqlParameter currentParam in paras)
                {
                    command.Parameters.Add(currentParam);
                }
                SqlDataAdapter sqlReader = new SqlDataAdapter(command);
                DataSet syncDataSet = new DataSet();
                sqlReader.Fill(syncDataSet);
                if (syncDataSet is null || syncDataSet.Tables.Count == 0)
                    return null;
                foreach (DataTable syncTable in syncDataSet.Tables)
                {
                    if (syncTable.Rows.Count > 0)
                    {
                        string syncTableName = syncTable.Rows[0][0]?.ToString();
                        syncTable.Columns.RemoveAt(0);
                        dicSyncsData.Add(syncTableName, syncTable);
                    }
                }
            }
            catch (Exception ex)
            {
                dicSyncsData = null;
                ex.LogError(_logger, "Aeon.HR.BusinessObjects.DataHelper.GetDicSyncsData");
            }
            return dicSyncsData;
        }

        public DataTable GetDataAsTable(string sqlCommandText)
        {
            DataTable returnValue = new DataTable();
            try
            {
                using (SqlConnection connection =
                   new SqlConnection(_connString))
                {
                    SqlCommand command =
                        new SqlCommand(sqlCommandText, connection);
                    command.CommandType = CommandType.Text;                   
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        returnValue.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue = new DataTable();
                ex.LogError(_logger, "Aeon.HR.BusinessObjects.DataHelper.GetDataAsTable");
            }
            return returnValue;
        }
        public DataTable GetDataAsTable(string storedProcedureName, SqlParameter[] paras, CommandType commandType = CommandType.StoredProcedure)
        {
            DataTable returnValue = new DataTable();
            try
            {
                using (SqlConnection connection =
                   new SqlConnection(_connString))
                {
                    SqlCommand command =
                        new SqlCommand(storedProcedureName, connection);
                    command.CommandType = commandType;
                    foreach (SqlParameter currentParam in paras)
                    {
                        command.Parameters.Add(currentParam);
                    }
                    connection.Open();
                                       
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        returnValue.Load(reader);                       
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue = new DataTable();
                ex.LogError(_logger, "Aeon.HR.BusinessObjects.DataHelper.GetDataAsTable");
            }
            return returnValue;
        }
        public int GetItemsCount(string sqlCommandText)
        {
            int returnValue = 0;
            try
            {
                using (Conn = new SqlConnection(_connString))
                {
                    Conn.Open();
                    SqlCommand command = new SqlCommand(sqlCommandText, Conn);
                    returnValue = Convert.ToInt32(command.ExecuteScalar());
                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                returnValue = 0;
                ex.LogError(_logger, "Aeon.HR.BusinessObjects.DataHelper.GetItemsCount");
            }
            return returnValue;
        }
        public int GetItemsCount(string storedProcedureName, SqlParameterCollection paras)
        {
            int returnValue = 0;
            try
            {
                using (Conn = new SqlConnection(_connString))
                {
                    Conn.Open();
                    SqlCommand command = new SqlCommand(storedProcedureName, Conn);
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter currentParam in paras)
                    {
                        command.Parameters.Add(currentParam);
                    }
                    returnValue = Convert.ToInt32(command.ExecuteScalar());
                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                returnValue = 0;
                ex.LogError(_logger, "Aeon.HR.BusinessObjects.DataHelper.GetItemsCount");
            }
            return returnValue;
        }
        public virtual bool ExecuteSQL(List<string> sqlArr)
        {
            bool returnValue = true;
            try
            {
                using (Conn = new SqlConnection(_connString))
                {
                    Conn.Open();
                    SqlCommand command = Conn.CreateCommand();
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("TransactionName");
                    command.Connection = Conn;
                    command.Transaction = transaction;
                    try
                    {
                        foreach (string sqlCommand in sqlArr)
                        {
                            command.CommandText = sqlCommand;
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        ex.LogError(_logger, "Aeon.HR.BusinessObjects.DataHelper.executeSQL-Transaction error");
                        transaction.Rollback();
                        returnValue = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogError(_logger, "Aeon.HR.BusinessObjects.DataHelper.executeSQL");
                returnValue = false;
            }
            return returnValue;
        }
        public virtual bool ExecuteSQLNonQuery(string sqlCommand)
        {
            bool returnValue = true;
            try
            {
                using (Conn = new SqlConnection(_connString))
                {
                    Conn.Open();
                    SqlCommand command = Conn.CreateCommand();
                    command.Connection = Conn;
                    command.CommandText = sqlCommand;
                    int status = command.ExecuteNonQuery();
                    returnValue = status == 1;
                }
            }
            catch (Exception ex)
            {
                ex.LogError(_logger, "Aeon.HR.BusinessObjects.DataHelper.ExecuteSQLNonQuery");
                returnValue = false;
            }
            return returnValue;
        }   
    }

    public static class DataHelperExtension
    {
        #region IDataReader extention
        public static string GetValueAsString(this IDataReader dataReader, string fieldName)
        {
            string returnValue = string.Empty;
            try
            {
                returnValue = dataReader[fieldName] as string;
            }
            catch (Exception ex)
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }
        public static double GetValueAsDouble(this IDataReader dataReader, string fieldName)
        {
            double returnValue = double.MinValue;
            try
            {
                returnValue = (double)dataReader[fieldName];
            }
            catch
            {
                returnValue = double.MinValue;
            }
            return returnValue;
        }
        public static char GetValueAsChar(this IDataReader dataReader, string fieldName)
        {
            char returnValue = Char.MinValue;
            try
            {
                returnValue = (char)dataReader[fieldName];
            }
            catch (Exception ex)
            {
                returnValue = Char.MinValue;
            }
            return returnValue;
        }
        public static int GetValueAsInt(this IDataReader dataReader, string fieldName)
        {
            int returnValue = 0;
            try
            {
                returnValue = (int)dataReader[fieldName];
            }
            catch
            {
                returnValue = 0;
            }
            return returnValue;
        }
        public static int GetValueToInt(this IDataReader dataReader, string fieldName)
        {
            int returnValue = 0;
            try
            {
                returnValue = Convert.ToInt32(dataReader[fieldName]);
            }
            catch
            {
                returnValue = 0;
            }
            return returnValue;
        }
        public static DateTime GetValueAsDate(this IDataReader dataReader, string fieldName)
        {
            DateTime returnValue = new DateTime();
            try
            {
                returnValue = (DateTime)dataReader[fieldName];
            }
            catch
            {
                returnValue = new DateTime();
            }
            return returnValue;
        }

        #endregion

        #region Datarow extention
        public static string GetValueAsString(this DataRow dataReader, string fieldName)
        {
            string returnValue = string.Empty;
            try
            {
                returnValue = (string)dataReader[fieldName];
            }
            catch (Exception ex)
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }
        public static double GetValueAsDouble(this DataRow dataReader, string fieldName)
        {
            double returnValue = double.MinValue;
            try
            {
                returnValue = (double)dataReader[fieldName];
            }
            catch (Exception ex)
            {
                returnValue = double.MinValue;
            }
            return returnValue;
        }
        public static char GetValueAsChar(this DataRow dataReader, string fieldName)
        {
            char returnValue = Char.MinValue;
            try
            {
                returnValue = (char)dataReader[fieldName];
            }
            catch (Exception ex)
            {
                returnValue = Char.MinValue;
            }
            return returnValue;
        }
        public static int GetValueAsInt(this DataRow dataReader, string fieldName)
        {
            int returnValue = 0;
            try
            {
                returnValue = (int)dataReader[fieldName];
            }
            catch (Exception ex)
            {
                returnValue = 0;
            }
            return returnValue;
        }
        public static DateTime GetValueAsDate(this DataRow dataReader, string fieldName)
        {
            DateTime returnValue = new DateTime();
            try
            {
                returnValue = (DateTime)dataReader[fieldName];
            }
            catch (Exception ex)
            {
                returnValue = new DateTime();
            }
            return returnValue;
        }
        #endregion
    }
    public static class DataMappingHelper
    {
        public static List<T> ToObject<T>(this DataTable dataRow)
        where T : new()
        {
            List<T> item = new List<T>();

            foreach (DataRow currentRow in dataRow.Rows)
            {
                item.Add(currentRow.ToObject<T>());
            }

            return item;
        }
        public static T ToObject<T>(this DataRow dataRow)
        where T : new()
        {
            T item = new T();

            foreach (DataColumn column in dataRow.Table.Columns)
            {
                PropertyInfo property = GetProperty(typeof(T), column.ColumnName);

                if (property != null && dataRow[column] != DBNull.Value && dataRow[column].ToString() != "NULL")
                {
                    property.SetValue(item, ChangeType(dataRow[column], property.PropertyType), null);
                }
            }

            return item;
        }

        private static PropertyInfo GetProperty(Type type, string attributeName)
        {
            PropertyInfo property = type.GetProperty(attributeName);

            if (property != null)
            {
                return property;
            }
            return type.GetProperties()
                 .Where(p => p.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase))
                 .FirstOrDefault();
        }

        public static object ChangeType(object value, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                return Convert.ChangeType(value, Nullable.GetUnderlyingType(type));
            }
            else if (type.Name == "DateTime")
            {
                if (value == null)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        return Convert.ChangeType(value, type);
                    }
                    catch (Exception ex)
                    {
                        value = DateTime.ParseExact(value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    }
                }
            }

            return Convert.ChangeType(value, type);
        }
    }
}
