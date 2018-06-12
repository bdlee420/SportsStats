using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SportsStats.DataProviders
{
    public class DataAccess
    {
        static string connString = ConfigurationManager.AppSettings["ConnectionString_MSSQL"];
       
        public string SQLGetFieldValueParam(string sql, List<SqlParameter> lstSP)
        {
            string val = String.Empty;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter odr = null;
            try
            {

                conn = new SqlConnection(connString);
                conn.Open();
                cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                foreach (SqlParameter sp in lstSP)
                {
                    cmd.Parameters.Add(sp);
                }
                object result = cmd.ExecuteScalar();
                if (result == null)
                {
                    val = String.Empty;
                }
                else
                {
                    val = result.ToString();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (cmd != null) { cmd.Dispose(); }
                if (conn != null) { conn.Close(); }
                if (odr != null) { odr.Dispose(); }
            }
            return val;
        }
        public bool SQLDoesRecordExistParam(string sql, List<SqlParameter> lstSP)
        {
            bool exists = false;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter odr = null;
            try
            {
                conn = new SqlConnection(connString);
                conn.Open();
                cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                foreach (SqlParameter sp in lstSP)
                {
                    cmd.Parameters.Add(sp);
                }
                if (Convert.ToInt32(cmd.ExecuteScalar().ToString()) > 0)
                {
                    exists = true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (cmd != null) { cmd.Dispose(); }
                if (conn != null) { conn.Close(); }
                if (odr != null) { odr.Dispose(); }
            }
            return exists;
        }
        public void SQLExecuteCommandParam(string procName, List<SqlParameter> lstSP)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                conn = new SqlConnection(connString);
                conn.Open();
                cmd = new SqlCommand(procName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter sp in lstSP)
                {
                    cmd.Parameters.Add(sp);
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw (e);
            }
            finally
            {
                if (cmd != null) { cmd.Dispose(); }
                if (conn != null) { conn.Close(); }
            }
        }
        public DataTable SQLGetDataTable(string procName, List<SqlParameter> lstSP = null)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter odr = null;
            DataTable dt = null;
            try
            {
                dt = new DataTable();
                conn = new SqlConnection(connString);
                cmd = new SqlCommand(procName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (lstSP != null && lstSP.Any())
                {
                    foreach (SqlParameter sp in lstSP)
                    {
                        cmd.Parameters.Add(sp);
                    }
                }
                odr = new SqlDataAdapter(cmd);
                odr.Fill(dt);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (cmd != null) { cmd.Dispose(); }
                if (conn != null) { conn.Close(); }
                if (odr != null) { odr.Dispose(); }
            }
            return dt;
        }
        public DataSet SQLGetDataSet(string procName, List<SqlParameter> lstSP = null)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter odr = null;
            DataSet ds = null;
            try
            {
                ds = new DataSet();
                conn = new SqlConnection(connString);
                cmd = new SqlCommand(procName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (lstSP != null && lstSP.Any())
                {
                    foreach (SqlParameter sp in lstSP)
                    {
                        cmd.Parameters.Add(sp);
                    }
                }
                odr = new SqlDataAdapter(cmd);
                odr.Fill(ds);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (cmd != null) { cmd.Dispose(); }
                if (conn != null) { conn.Close(); }
                if (odr != null) { odr.Dispose(); }
            }
            return ds;
        }
        public string SQLExecuteQueryReturnIDParam(string procName, List<SqlParameter> lstSP)
        {
            string val = String.Empty;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlParameter opID = null;
            try
            {

                conn = new SqlConnection(connString);
                opID = new SqlParameter("out_ID", SqlDbType.Int);
                opID.Direction = ParameterDirection.ReturnValue;
                conn.Open();
                cmd = new SqlCommand(procName, conn);
                foreach (SqlParameter sp in lstSP)
                {
                    cmd.Parameters.Add(sp);
                }
                cmd.Parameters.Add(opID);
                cmd.CommandType = CommandType.StoredProcedure;
                val = cmd.ExecuteScalar().ToString();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (cmd != null) { cmd.Dispose(); }
                if (conn != null) { conn.Close(); }
            }
            return val;
        }
        public SqlParameter CreateSqlParameter(string parameterName, SqlDbType dbt, Object val)
        {
            SqlParameter sp = new SqlParameter(parameterName, dbt);
            sp.Direction = ParameterDirection.Input;
            sp.Value = val;
            return sp;
        }
        public SqlParameter CreateSqlParameter(string parameterName, SqlDbType dbt)
        {
            SqlParameter sp = new SqlParameter(parameterName, dbt);
            sp.Direction = ParameterDirection.Input;
            return sp;
        }
    }
}