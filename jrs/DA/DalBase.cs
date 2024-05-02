// ***********************************************************************
// Assembly         : jrs
// Author           : Alberto
// Created          : 04-03-2019
//
// Last Modified By : Alberto
// Last Modified On : 04-03-2019
// ***********************************************************************
// <copyright file="DalBase.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Data;
using System.Data.Common;


namespace jrs.DA
{
    /// <summary>
    /// Class DalBase
    /// </summary>
    public class DalBase
    {
        #region Constructors / Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DalBase"/> class.
        /// </summary>
        public DalBase()
        {
            
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DalBase"/> class.
        /// </summary>
        ~DalBase()
        {
            Command = null;

            if (DataReader != null)
            {
                DataReader.Close();
                DataReader.Dispose();
            }

            if (Connection != null)
            {
                if(Connection.State == ConnectionState.Open)
                    CloseConnection();
            }
        }

        #endregion Constructors / Destructor

        #region Properties

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public DbConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        public DbCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the data reader.
        /// </summary>
        /// <value>The data reader.</value>
        public DbDataReader DataReader { get; set; }

        #endregion Properties

        #region Methods

        #region Public

        /// <summary>
        /// Opens the connection.
        /// </summary>
        public void OpenConnection()
        {
            Connection.Open();
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void CloseConnection()
        {
            Connection.Close();
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.NullReferenceException">Object Command cannot be null.</exception>
        public int ExecuteCommand()
        {
            if (Command == null)
                throw new NullReferenceException("Object Command cannot be null.");

            var iRes = Command.ExecuteNonQuery();

            return iRes;
        }

        /// <summary>
        /// Executes the data reader.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Object Command cannot be null.</exception>
        public void ExecuteDataReader()
        {
            if (Command == null)
                throw new NullReferenceException("Object Command cannot be null.");

            DataReader = Command.ExecuteReader();
        }

        /// <summary>
        /// Closes the data reader.
        /// </summary>
        public void CloseDataReader()
        {
            if (DataReader == null) 
                return;

            DataReader.Close();
            DataReader.Dispose();
        }

        /// <summary>
        /// Determines whether this instance has rows.
        /// </summary>
        /// <returns><c>true</c> if this instance has rows; otherwise, <c>false</c>.</returns>
        public bool HasRows()
        {
            if (DataReader == null)
                return false;

            return DataReader.HasRows;
        }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <exception cref="System.Exception">
        /// Impossibile creare l'oggetto Command se non si dispone di una connessione valida
        /// or
        /// Impossibile creare l'oggetto Command se non si dispone di una connessione valida
        /// </exception>
        public void CreateCommand()
        {
            if(Connection == null)
                throw new Exception("Impossibile creare l'oggetto Command se non si dispone di una connessione valida");

            Command = Connection.CreateCommand();
        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <exception cref="System.NullReferenceException">Object Command cannot be null.</exception>
        public void AddParameter(ref DbParameter parameter, ParameterDirection parameterDirection, DbType parameterType, string parameterName, object parameterValue)
        {
            if (Command == null)
                throw new NullReferenceException("Object Command cannot be null.");

            parameter = Command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = parameterType;
            parameter.Value = parameterValue;
            parameter.Direction = parameterDirection;

            Command.Parameters.Add(parameter);
        }

        public void AddParameter(ref DbParameter parameter, ParameterDirection parameterDirection, DbType parameterType, int size, string parameterName, object parameterValue)
        {
            if (Command == null)
                throw new NullReferenceException("Object Command cannot be null.");

            parameter = Command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = parameterType;
            parameter.Size = size;
            parameter.Value = parameterValue;
            parameter.Direction = parameterDirection;

            Command.Parameters.Add(parameter);
        }

       

        #endregion Public

        #region Private

        #endregion Private

        #endregion Methods
    }
}
