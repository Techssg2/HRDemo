using Aeon.CreatePayloadCompleted.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CreatePayloadCompleted.src.SQLExcute
{
    public class SQLQuery<TEntity> where TEntity : BaseEntity, new()
    {
        #region Constructors

        public SQLQuery()
        {
            this.GetConfiguration();
        }

        #endregion

        #region Properties

        public string ConnectionString { get; set; }

        #endregion

        #region Methods

        protected virtual void GetConfiguration()
        {
            try
            {
                this.ConnectionString = ConfigurationManager.AppSettings[WebAnalysisConnection];
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError(ex);
            }
        }

        public virtual List<TEntity> GetItems()
        {
            List<TEntity> m_Entities = new List<TEntity>();

            try
            {
                using (SqlConnection m_SqlConnection = new SqlConnection(this.ConnectionString))
                {
                    try
                    {
                        string m_SelectCommandText = "Select * From " + this.GetTableName();
                        if (!string.IsNullOrEmpty(m_SqlConnection.ConnectionString.ToString()) || m_SqlConnection.ConnectionString.ToString() != "")
                        {
                            m_SqlConnection.Open();
                            using (SqlCommand m_SqlCommand = new SqlCommand(m_SelectCommandText, m_SqlConnection))
                            {
                                SqlDataReader m_SqlDataReader = m_SqlCommand.ExecuteReader();
                                while (m_SqlDataReader.Read())
                                {
                                    TEntity m_Entity = new TEntity();
                                    m_Entity.Fill(m_SqlDataReader);
                                    m_Entities.Add(m_Entity);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.WriteLogError(ex);
                    }
                    finally
                    {
                        try
                        {
                            m_SqlConnection.Close();
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError(ex);
            }

            return m_Entities;
        }
        public virtual List<TEntity> GetItemsByQuery(string selectCommandText)
        {
            List<TEntity> m_Entities = new List<TEntity>();

            using (SqlConnection m_SqlConnection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    string m_SelectCommandText = selectCommandText;
                    if (!string.IsNullOrEmpty(m_SqlConnection.ConnectionString.ToString()) || m_SqlConnection.ConnectionString.ToString() != "")
                    {
                        m_SqlConnection.Open();
                        using (SqlCommand m_SqlCommand = new SqlCommand(m_SelectCommandText, m_SqlConnection))
                        {
                            SqlDataReader m_SqlDataReader = m_SqlCommand.ExecuteReader();
                            while (m_SqlDataReader.Read())
                            {
                                TEntity m_Entity = new TEntity();
                                m_Entity.Fill(m_SqlDataReader);
                                m_Entities.Add(m_Entity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utilities.WriteLogError("GetItemsByQuery >>" + ex);

                }
                finally
                {
                    try
                    {
                        m_SqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        Utilities.WriteLogError("GetItemsByQuery finally >>" + ex);
                    }
                }
            }

            return m_Entities;
        }
        public bool ExecuteRunQuery(string strQuery)
        {
            bool success = false;
            using (SqlConnection m_SqlConnection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    string m_SelectCommandText = strQuery;

                    if (!string.IsNullOrEmpty(m_SqlConnection.ConnectionString.ToString()) || m_SqlConnection.ConnectionString.ToString() != "")
                    {
                        m_SqlConnection.Open();
                        using (SqlCommand cmd = new SqlCommand(m_SelectCommandText, m_SqlConnection))
                        {
                            cmd.CommandText = strQuery;
                            cmd.CommandTimeout = 120;
                            cmd.ExecuteNonQuery();
                        }
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    Utilities.WriteLogError("ExecuteRunQuery():" + ex);

                }
                finally
                {
                    try
                    {
                        m_SqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        Utilities.WriteLogError("ExecuteRunQuery():" + ex);
                    }
                }
            }
            return success;
        }
        public virtual int GetTotalItems(string selectCommandText)
        {
            int total = 0;

            using (SqlConnection m_SqlConnection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    string m_SelectCommandText = selectCommandText;
                    if (!string.IsNullOrEmpty(m_SqlConnection.ConnectionString.ToString()) || m_SqlConnection.ConnectionString.ToString() != "")
                    {
                        m_SqlConnection.Open();
                        using (SqlCommand m_SqlCommand = new SqlCommand(m_SelectCommandText, m_SqlConnection))
                        {
                            SqlDataReader m_SqlDataReader = m_SqlCommand.ExecuteReader();
                            while (m_SqlDataReader.Read())
                            {
                                TEntity m_Entity = new TEntity();
                                total = (int)m_SqlDataReader["Total"];
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utilities.WriteLogError("GetTotalItems >>" + ex);

                }
                finally
                {
                    try
                    {
                        m_SqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        Utilities.WriteLogError("GetTotalItems finally >>" + ex);
                    }
                }
            }

            return total;
        }
        public virtual bool DeleteItemByQuery(string selectCommandText)
        {
            //Validate
            bool m_Result = false;

            try
            {
                using (SqlConnection m_SqlConnection = new SqlConnection(this.ConnectionString))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(m_SqlConnection.ConnectionString.ToString()) || m_SqlConnection.ConnectionString.ToString() != "")
                        {
                            m_SqlConnection.Open();

                            //Build command
                            string m_DeleteCommandText = selectCommandText;
                            //Execute
                            using (SqlCommand m_SqlCommand = new SqlCommand(m_DeleteCommandText, m_SqlConnection))
                            {
                                m_Result = m_SqlCommand.ExecuteNonQuery() > 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.WriteLogError(ex);
                    }
                    finally
                    {
                        try
                        {
                            m_SqlConnection.Close();
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError(ex);
            }

            return m_Result;
        }
        public virtual bool Delete(params TEntity[] entities)
        {
            //Validate
            bool m_Result = false;

            try
            {
                using (SqlConnection m_SqlConnection = new SqlConnection(this.ConnectionString))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(m_SqlConnection.ConnectionString.ToString()) || m_SqlConnection.ConnectionString.ToString() != "")
                        {
                            m_SqlConnection.Open();

                            //Build command
                            string[] m_IDs = entities.Where(e => e != null && e.ID != Guid.Empty).Select(e => e.ID.ToString()).ToArray();
                            if (m_IDs.Length > 0)
                            {
                                string m_DeleteCommandText = "Delete from " + this.GetTableName() + " where id in (" + string.Join(", ", m_IDs) + ")";

                                //Execute
                                using (SqlCommand m_SqlCommand = new SqlCommand(m_DeleteCommandText, m_SqlConnection))
                                {
                                    m_Result = m_SqlCommand.ExecuteNonQuery() > 0;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.WriteLogError(ex);
                    }
                    finally
                    {
                        try
                        {
                            m_SqlConnection.Close();
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError(ex);
            }

            return m_Result;
        }
        public virtual DataSet ExecuteStore(string sql, Dictionary<string, object> parameters)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(this.ConnectionString))
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null && parameters.Count() > 0)
                        {
                            foreach (var item in parameters)
                            {
                                cmd.Parameters.Add(new SqlParameter(item.Key, item.Value));
                            }
                        }
                        using (SqlDataAdapter ad = new SqlDataAdapter())
                        {
                            ad.SelectCommand = cmd;
                            ad.Fill(ds);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError(ex);
            }
            return ds;
        }
        protected virtual string BuildInsertCommand()
        {
            string m_InsertCommand = string.Empty;

            //Get Field
            FieldAttribute[] m_Fields = this.GetFields();

            //Build Command Text
            string[] m_FieldNames = m_Fields.Where(f => !f.ExcludeUpdate).Select(f => f.FieldName).ToArray();
            string[] m_FieldParameters = m_FieldNames.Select(f => "@" + f).ToArray();
            m_FieldNames = m_FieldNames.Select(f => "[" + f + "]").ToArray();   //Add []

            //Table
            string m_TableName = this.GetTableName();
            m_InsertCommand = @"INSERT INTO " + m_TableName + @" (" + string.Join(", ", m_FieldNames) + ") VALUES (" + string.Join(", ", m_FieldParameters) + "); SELECT CAST(scope_identity() AS int);";

            return m_InsertCommand;
        }
        protected virtual string BuildUpdateCommand()
        {
            string m_UpdateCommand = string.Empty;

            //Get Field
            FieldAttribute[] m_Fields = this.GetFields();

            //Build Command Text
            string[] m_FieldNamevalues = m_Fields.Where(f => !f.ExcludeUpdate).Select(f => "[" + f.FieldName + "] = @" + f.FieldName).ToArray();
            //TableName
            string m_TableName = this.GetTableName();
            m_UpdateCommand = @"UPDATE " + m_TableName + " SET " + string.Join(", ", m_FieldNamevalues) + " WHERE [ID] = @ID;";

            return m_UpdateCommand;
        }
        protected virtual Dictionary<FieldAttribute, PropertyInfo> GetDictionaryFieldPropertyInfos()
        {
            Dictionary<FieldAttribute, PropertyInfo> m_DictionaryFieldPropertyInfos = new Dictionary<FieldAttribute, PropertyInfo>();
            PropertyInfo[] m_PropertyInfos = typeof(TEntity).GetProperties();
            foreach (var m_PropertyInfo in m_PropertyInfos.Where(f => f.GetCustomAttribute<FieldAttribute>() != null))
            {
                FieldAttribute m_FieldAttribute = m_PropertyInfo.GetCustomAttribute<FieldAttribute>();
                if (!m_FieldAttribute.ExcludeUpdate)
                    m_DictionaryFieldPropertyInfos.Add(m_FieldAttribute, m_PropertyInfo);
            }
            return m_DictionaryFieldPropertyInfos;
        }
        protected virtual FieldAttribute[] GetFields()
        {
            return this.GetDictionaryFieldPropertyInfos().Keys.ToArray();
        }
        protected virtual void FillParameters(SqlCommand sqlCommand, TEntity entity)
        {
            var m_DictionaryFieldPropertyInfos = this.GetDictionaryFieldPropertyInfos();
            foreach (var m_Pair in m_DictionaryFieldPropertyInfos)
            {
                object m_Value = m_Pair.Value.GetValue(entity, new object[] { });
                if (m_Value == null) m_Value = DBNull.Value;
                sqlCommand.Parameters.Add(new SqlParameter(m_Pair.Key.FieldName, m_Pair.Key.Type) { Value = m_Value });
            }
        }

        protected virtual string GetTableName()
        {
            return typeof(TEntity).GetCustomAttribute<TableAttribute>().TableName;
        }

        #endregion

        #region Constants

        public const string WebAnalysisConnection = "Edoc2Database";

        #endregion
    }
    public class BaseEntity
    {
        #region Constructors

        #endregion

        #region Properties

        [FieldAttribute(FieldName = "ID", ExcludeUpdate = true, Type = SqlDbType.UniqueIdentifier)]
        public Guid ID { get; set; }

        #endregion

        #region Methods

        public virtual void Fill(SqlDataReader sqlDataReader)
        {
            this.ID = (Guid)sqlDataReader[FieldName_ID];
        }
        public virtual void FillOut(SqlCommand sqlCommand)
        {
            if (this.ID != Guid.Empty)
                sqlCommand.Parameters.Add(new SqlParameter(FieldName_ID, System.Data.SqlDbType.UniqueIdentifier) { Value = this.ID });
        }

        protected virtual T GetData<T>(SqlDataReader sqlDataReader, string fieldName)
        {
            T m_T = default(T);

            object m_Value = sqlDataReader[fieldName];
            if (m_Value != DBNull.Value) m_T = (T)m_Value;

            return m_T;
        }



        #endregion

        #region Constants

        public const string FieldName_ID = "ID";

        #endregion
    }

    public static class CheckColumn
    {
        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

    }



    public class TableAttribute : Attribute
    {
        #region Properties

        public string TableName { get; set; }

        #endregion
    }
    public class FieldAttribute : Attribute
    {
        #region Properties

        public bool ExcludeUpdate { get; set; }
        public string FieldName { get; set; }
        public SqlDbType Type { get; set; }

        #endregion
    }
}
