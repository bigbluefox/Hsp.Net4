using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;

namespace Hsp.Net4.Common.Data
{
    public static class DBHelper
    {

        private static SqlConnection connection;
        
        //获取数据库连接
        public static SqlConnection Connection
        {
            get
            {

                string connectionString = ConfigurationManager.ConnectionStrings["BOCC_FrameConnectionString"].ConnectionString;
                connection = new SqlConnection(connectionString);
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                else if (connection.State == System.Data.ConnectionState.Broken)
                {
                    connection.Close();
                    connection.Open();
                }
                return connection;
            }
        }
        public static void Close_Connection()
        {

            if (connection.State == System.Data.ConnectionState.Broken)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// 数据库连接串
        /// </summary>
        /// <returns></returns>
        public static string Get_ConnectionString()
        {
            return ConfigurationManager.AppSettings["ConnectionString"].ToString();
            //ConfigurationManager.ConnectionStrings["BOCC_FrameConnectionString"].ConnectionString;
        }

        //通用方法，执行增删改操作
        public static int ExecuteCommand(string safeSql)
        {
            int result = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(Get_ConnectionString()))
                {
                    // Open the connection
                    conn.Open();
                    // Do something useful
                    //SqlCommand cmd = new SqlCommand(safeSql, Connection);
                    SqlCommand cmd = new SqlCommand(safeSql, conn);
                    result = cmd.ExecuteNonQuery();
                    // Close it myself
                    conn.Close();
                }
            }
            catch (Exception e)

            {
                // Do something with the exception here...
                throw new Exception(e.ToString());

            }
            return result;
        }

        public static int ExecuteCommand(string sql, params SqlParameter[] values)
        {
            //SqlCommand cmd = new SqlCommand(sql, Connection);
            //cmd.Parameters.AddRange(values);
            //return cmd.ExecuteNonQuery();
            int result = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(Get_ConnectionString()))
                {
                    // Open the connection
                    conn.Open();
                    // Do something useful
                    //SqlCommand cmd = new SqlCommand(safeSql, Connection);
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddRange(values);
                    result = cmd.ExecuteNonQuery();
                    // Close it myself
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                // Do something with the exception here...
                throw new Exception(e.ToString());

            }
            return result;
        }

        #region private utility methods & constructors

        /// <summary>
        /// This method is used to attach array of SqlParameters to a SqlCommand.
        /// 
        /// This method will assign a value of DbNull to any parameter with a direction of
        /// InputOutput and a value of null.  
        /// 
        /// This behavior will prevent default values from being used, but
        /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
        /// where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">An array of SqlParameters to be added to command</param>
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                             p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        /// <summary>
        /// This method assigns dataRow column values to an array of SqlParameters
        /// </summary>
        /// <param name="commandParameters">Array of SqlParameters to be assigned values</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values</param>
        private static void AssignParameterValues(SqlParameter[] commandParameters, DataRow dataRow)
        {
            if ((commandParameters == null) || (dataRow == null))
            {
                // Do nothing if we get no data
                return;
            }

            int i = 0;
            // Set the parameters values
            foreach (SqlParameter commandParameter in commandParameters)
            {
                // Check the parameter name
                if (commandParameter.ParameterName == null ||
                    commandParameter.ParameterName.Length <= 1)
                    throw new Exception(
                        string.Format(
                            "Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: '{1}'.",
                            i, commandParameter.ParameterName));
                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
                i++;
            }
        }

        /// <summary>
        /// This method assigns an array of values to an array of SqlParameters
        /// </summary>
        /// <param name="commandParameters">Array of SqlParameters to be assigned values</param>
        /// <param name="parameterValues">Array of objects holding the values to be assigned</param>
        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                // Do nothing if we get no data
                return;
            }

            // We must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            // Iterate through the SqlParameters, assigning the values from the corresponding position in the 
            // value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                // If the current array value derives from IDbDataParameter, then assign its Value property
                if (parameterValues[i] is IDbDataParameter)
                {
                    var paramInstance = (IDbDataParameter)parameterValues[i];
                    if (paramInstance.Value == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = paramInstance.Value;
                    }
                }
                else if (parameterValues[i] == null)
                {
                    commandParameters[i].Value = DBNull.Value;
                }
                else
                {
                    commandParameters[i].Value = parameterValues[i];
                }
            }
        }

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command
        /// </summary>
        /// <param name="command">The SqlCommand to be prepared</param>
        /// <param name="connection">A valid SqlConnection, on which to execute this command</param>
        /// <param name="transaction">A valid SqlTransaction, or 'null'</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="mustCloseConnection"><c>true</c> if the connection was opened by the method, otherwose is false.</param>
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction,
                                           CommandType commandType, string commandText, SqlParameter[] commandParameters,
                                           out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null)
                    throw new ArgumentException(
                        "The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }

        #endregion private utility methods & constructors

        #region ExecuteNonQuery

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset and takes no parameters) against the database specified in 
        /// the connection string
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(connectionString, commandType, commandText, null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText,
                                          params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            // Create & open a SqlConnection, and dispose of it after we are done
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns no resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="spName">The name of the stored prcedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connection">A valid SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(connection, commandType, commandText, null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText,
                                          params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            var cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Finally, execute the command
            int retval = cmd.ExecuteNonQuery();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            if (mustCloseConnection)
                connection.Close();
            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified SqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">A valid SqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="transaction">A valid SqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the specified SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid SqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText,
                                          params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException(
                    "The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Create a command and prepare it for execution
            var cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters,
                           out mustCloseConnection);

            // Finally, execute the command
            int retval = cmd.ExecuteNonQuery();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified 
        /// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid SqlTransaction</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException(
                    "The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection,
                                                                                             spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteNonQuery



        #region GetScalar

        //通用方法，执行查询操作
        public static int GetScalar(string safeSql)
        {
            //SqlCommand cmd = new SqlCommand(safeSql, Connection);
            //int result = Convert.ToInt32(cmd.ExecuteScalar());
            //return result;
            int result = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(Get_ConnectionString()))
                {
                    // Open the connection
                    conn.Open();
                    // Do something useful
                    //SqlCommand cmd = new SqlCommand(safeSql, Connection);
                    SqlCommand cmd = new SqlCommand(safeSql, conn);
                    result = cmd.ExecuteNonQuery();
                    // Close it myself
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                // Do something with the exception here...
                throw new Exception(e.ToString());

            }
            return result;
        }

        public static int GetScalar(string sql, params SqlParameter[] values)
        {
            //SqlCommand cmd = new SqlCommand(sql, Connection);
            //cmd.Parameters.AddRange(values);
            //int result = Convert.ToInt32(cmd.ExecuteScalar());
            int result = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(Get_ConnectionString()))
                {
                    // Open the connection
                    conn.Open();
                    // Do something useful
                    //SqlCommand cmd = new SqlCommand(safeSql, Connection);
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddRange(values);
                    result = cmd.ExecuteNonQuery();
                    // Close it myself
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                // Do something with the exception here...
                throw new Exception(e.ToString());

            }
            return result;
        }

        #endregion

        #region Scalar用指定的数据库连接字符串执行一个命令并返回一个数据集的第一列

        /// <summary>
        /// 用指定的数据库连接字符串执行一个命令并返回一个数据集的第一列
        /// </summary>
        /// <remarks>
        ///例如:
        /// Object obj = ExecuteScalar("SELECT * FROM PublishOrders");
        /// </remarks>
        /// <param name="cmdText">sql命令语句</param>
        /// <returns>用 Convert.To{Type}把类型转换为想要的 </returns>
        public static object ExecuteScalar(string cmdText)
        {
            return ExecuteScalar(CommandType.Text, cmdText, new SqlParameter());
        }

        /// <summary>
        /// 用指定的数据库连接字符串执行一个命令并返回一个数据集的第一列
        /// </summary>
        /// <param name="cmdText">sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns></returns>
        public static object ExecuteScalar(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteScalar(CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// 用指定的数据库连接字符串执行一个命令并返回一个数据集的第一列
        /// </summary>
        /// <remarks>
        ///例如:
        /// Object obj = ExecuteScalar(CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <returns>用 Convert.To{Type}把类型转换为想要的 </returns>
        public static object ExecuteScalar(CommandType cmdType, string cmdText)
        {
            return ExecuteScalar(cmdType, cmdText, new SqlParameter());
        }

        /// <summary>
        /// 用指定的数据库连接执行一个命令并返回一个数据集的第一列
        /// </summary>
        /// <remarks>
        /// 例如:
        /// Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="sql">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>用 Convert.To{Type}把类型转换为想要的 </returns>
        public static object ExecuteScalar(CommandType cmdType, string sql, params SqlParameter[] commandParameters)
        {
            using (SqlConnection conn = new SqlConnection(Get_ConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = cmdType;
                cmd.Parameters.AddRange(commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        #endregion

        public static SqlDataReader GetReader(string safeSql)
        {
            SqlCommand cmd = new SqlCommand(safeSql, Connection);
            SqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        public static SqlDataReader GetReader(string sql, params SqlParameter[] values)
        {
            SqlCommand cmd = new SqlCommand(sql, Connection);
            cmd.Parameters.AddRange(values);
            SqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        public static DataTable GetDataSet(string safeSql)
        {
            // DataSet ds = new DataSet();
            // SqlCommand cmd = new SqlCommand(safeSql, Connection);
            // SqlDataAdapter da = new SqlDataAdapter(cmd);
            // da.Fill(ds);            
            //return ds.Tables[0];
            DataTable dta = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(Get_ConnectionString()))
                {
                    conn.Open();
                    DataSet ds = new DataSet();
                    SqlCommand cmd = new SqlCommand(safeSql, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    dta = ds.Tables[0];
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return dta;
        }

        /// <summary>
        /// 根据脚本和参数获取DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static DataTable GetDataSet(string sql, params SqlParameter[] values)
        {
            //DataSet ds = new DataSet();
            //SqlCommand cmd = new SqlCommand(sql, Connection);
            //cmd.Parameters.AddRange(values);
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //da.Fill(ds);
            //return ds.Tables[0];
            DataTable dta = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(Get_ConnectionString()))
                {

                    conn.Open();
                    DataSet ds = new DataSet();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddRange(values);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    dta = ds.Tables[0];
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());

            }
            return dta;
        }

        //通用方法，关闭数据库DataReader
        public static void CloseAll(SqlDataReader sdr)
        {
            try
            {
                sdr.Close();
                // sc.Close();
            }
            catch (Exception) { }

        }
        #region 张洪芬添加

        #region 获得Command对象

        /// <summary>
        /// 根据sql语句返回Command对象
        /// 内部对象
        /// 仅供该类自己调用
        /// </summary>  
        public static SqlCommand GetCommand(string strSQL, SqlConnection Conn)
        {
            SqlCommand Cmd = new SqlCommand(strSQL, Conn);
            return Cmd;
        }

        #endregion

        #region 存储过程参数传入

        /// <summary>
        /// 传入存储过程Input参数
        /// 内部方法
        /// 仅供类自己调用
        /// </summary>
        private static SqlParameter SqlDbMakeProcInputPrams(string strPramName, SqlDbType DbType, Int32 Size, string strValue)
        {
            SqlParameter param;

            if (Size > 0)
            {
                param = new SqlParameter(strPramName, DbType, Size);
            }
            else
            {
                param = new SqlParameter(strPramName, DbType);
            }

            param.Direction = ParameterDirection.Input;

            if (!(strValue == null))
            {
                param.Value = strValue;
            }
            return param;
        }

        /// <summary>
        /// 传入存储过程Input参数：无Size值
        /// 内部方法
        /// 仅供类自己调用
        /// </summary>
        private static SqlParameter SqlDbMakeProcInputPrams(string strPramName, SqlDbType DbType, string strValue)
        {
            SqlParameter param;

            param = new SqlParameter(strPramName, DbType);

            param.Direction = ParameterDirection.Input;

            if (!(strValue == null))
            {
                param.Value = strValue;
            }
            return param;
        }

        /// <summary>
        /// 传入存储过程Output参数
        /// 内部方法
        /// 仅供类自己调用
        /// </summary>
        private static SqlParameter SqlDbMakeProcOutputPrams(string strPramName, SqlDbType DbType, Int32 Size)
        {
            SqlParameter param;
            param = new SqlParameter(strPramName, DbType, Size);

            param.Direction = ParameterDirection.Output;

            return param;
        }

        /// <summary>
        /// 传入存储过程Output参数：无Size值
        /// 内部方法
        /// 仅供类自己调用
        /// </summary>
        private static SqlParameter SqlDbMakeProcOutputPrams(string strPramName, SqlDbType DbType)
        {
            SqlParameter param;
            param = new SqlParameter(strPramName, DbType);

            param.Direction = ParameterDirection.Output;

            return param;
        }

        #endregion

        #region 存储过程：返回字符串，不需要传入Input的Size值

        /// <summary>
        /// 执行存储过程(注意：不设置Size值：系统本身设置为200)
        /// 公共方法
        /// 提供给其他函数调用
        /// </summary>
        public static string ExecSQLDBStordProc(string strProName, string strDbPrams, string strLrPramsValue, string strDbRetPrams)
        {
            string sRet = "";
            int i = 0;
            SqlConnection Conn = Connection;
            if (Conn == null)
            {
                //throw new ArgumentNullException("Conn", Resources.ArgumentNullExceptionMessage);
            }
            try
            {
                // 获取的连接为null时，则认为此操作没有在事务中，按一般操作来执行
                SqlCommand cmd = GetCommand(strProName, Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                string[] lsArrayLRPrams = strLrPramsValue.Split(',');
                string[] lsArrayDBPrams = strDbPrams.Split(',');
                string[] lsArrayDBRetPrams = strDbRetPrams.Split(',');
                if (!(lsArrayLRPrams.Length == lsArrayDBPrams.Length))
                {
                    sRet = "0000：输入参数有误！";
                }
                else
                {
                    #region// 过程
                    if (Conn.State != ConnectionState.Open)
                    {
                        Conn.Open();
                    }
                    if (strDbPrams.IndexOf(",") <= 0)//仅为一个参数时
                    {
                        cmd.Parameters.Add(strDbPrams, SqlDbType.VarChar, 200);
                        cmd.Parameters[strDbPrams].Direction = ParameterDirection.Input;
                        cmd.Parameters[strDbPrams].Value = strLrPramsValue;
                    }
                    else
                    {
                        for (i = 0; i <= lsArrayLRPrams.Length - 1; i++)
                        {
                            cmd.Parameters.Add(SqlDbMakeProcInputPrams(lsArrayDBPrams[i], SqlDbType.VarChar, 200, lsArrayLRPrams[i]));
                        }
                    }

                    if (strDbRetPrams.Trim().IndexOf(",") <= 0)
                    {
                        cmd.Parameters.Add(strDbRetPrams.Trim(), SqlDbType.VarChar, 200);
                        cmd.Parameters[strDbRetPrams.Trim()].Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();
                        sRet = cmd.Parameters[strDbRetPrams.Trim()].Value.ToString().Trim();
                    }
                    else
                    {
                        for (i = 0; i <= lsArrayDBRetPrams.Length - 1; i++)
                        {
                            cmd.Parameters.Add(SqlDbMakeProcOutputPrams(lsArrayDBRetPrams[i], SqlDbType.VarChar, 200));
                        }
                        cmd.ExecuteNonQuery();

                        for (i = 0; i <= lsArrayDBRetPrams.Length - 1; i++)
                        {
                            sRet += cmd.Parameters[lsArrayDBRetPrams[i].Trim()].Value.ToString().Trim() + ",";
                        }
                    }
                    #endregion
                }
            }
            catch //(Exception ex)
            {
                //throw new Exception(ex.ToString());
                //Conn.Close();
                //Conn.Dispose();
            }
            finally
            {
                //if (Transaction.Current == null)
                //Dispose(Conn);
                Conn.Close();
                Conn.Dispose();
            }

            return sRet;
        }

        #endregion

        #endregion
    }
    /// <summary>
    /// SqlHelperParameterCache provides functions to leverage a static cache of procedure parameters, and the
    /// ability to discover parameters for stored procedures at run-time.
    /// </summary>
    public sealed class SqlHelperParameterCache
    {
        #region private methods, variables, and constructors

        //Since this class provides only static methods, make the default constructor private to prevent 
        //instances from being created with "new SqlHelperParameterCache()"

        private static readonly Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        private SqlHelperParameterCache()
        {
        }

        /// <summary>
        /// Resolve at run time the appropriate set of SqlParameters for a stored procedure
        /// </summary>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
        /// <returns>The parameter array discovered.</returns>
        private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string spName,
                                                             bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName");

            var cmd = new SqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            SqlCommandBuilder.DeriveParameters(cmd);
            connection.Close();

            if (!includeReturnValueParameter)
            {
                cmd.Parameters.RemoveAt(0);
            }

            var discoveredParameters = new SqlParameter[cmd.Parameters.Count];

            cmd.Parameters.CopyTo(discoveredParameters, 0);

            // Init the parameters with a DBNull value
            foreach (SqlParameter discoveredParameter in discoveredParameters)
            {
                discoveredParameter.Value = DBNull.Value;
            }
            return discoveredParameters;
        }

        /// <summary>
        /// Deep copy of cached SqlParameter array
        /// </summary>
        /// <param name="originalParameters"></param>
        /// <returns></returns>
        private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {
            var clonedParameters = new SqlParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        #endregion private methods, variables, and constructors

        #region caching functions

        /// <summary>
        /// Add parameter array to the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters to be cached</param>
        public static void CacheParameterSet(string connectionString, string commandText,
                                             params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        /// <summary>
        /// Retrieve a parameter array from the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An array of SqlParamters</returns>
        public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            var cachedParameters = paramCache[hashKey] as SqlParameter[];
            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }

        #endregion caching functions

        #region Parameter Discovery Functions

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of SqlParameters</returns>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of SqlParameters</returns>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName,
                                                       bool includeReturnValueParameter)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName");

            using (var connection = new SqlConnection(connectionString))
            {
                return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of SqlParameters</returns>
        internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
        {
            return GetSpParameterSet(connection, spName, false);
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of SqlParameters</returns>
        internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName,
                                                         bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            using (var clonedConnection = (SqlConnection)((ICloneable)connection).Clone())
            {
                return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of SqlParameters</returns>
        private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string spName,
                                                                bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName");

            string hashKey = connection.ConnectionString + ":" + spName +
                             (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            var cachedParameters = paramCache[hashKey] as SqlParameter[];
            if (cachedParameters == null)
            {
                SqlParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                paramCache[hashKey] = spParameters;
                cachedParameters = spParameters;
            }

            return CloneParameters(cachedParameters);
        }

        #endregion Parameter Discovery Functions

    }
}