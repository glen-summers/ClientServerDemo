using System.Data;
using System.Data.Common;
using System.Linq;
using Utils.Logging;

namespace Server
{
	internal class DatabaseFactory
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DatabaseFactory));

		private readonly string connectionString;
		private readonly DbProviderFactory factory;

		public DatabaseFactory(string connectionString, string providerName)
		{
			this.connectionString = connectionString;
			factory = DbProviderFactories.GetFactory(providerName);
		}

		public DbConnection CreateOpenConnection()
		{
			DbConnection connection = factory.CreateConnection();
			connection.ConnectionString = connectionString; //nullcheck
			connection.Open();
			return connection;
		}

		public DbParameter CreateParameter(string name, DbType type, object value)
		{
			var dbParameter = factory.CreateParameter();
			dbParameter.ParameterName = name;
			dbParameter.DbType = type;
			dbParameter.Value = value;
			return dbParameter;
		}

		public int ExecuteNonQuery(IDbConnection connection, string text, CommandType type, params DbParameter[] parameters)
		{
			using (IDbCommand command = CreateCommand(connection, text, type, parameters))
			{
				return command.ExecuteNonQuery();
			}
		}

		public T ExecuteScalar<T>(IDbConnection connection, string text, CommandType type, params DbParameter[] parameters)
		{
			using (IDbCommand command = CreateCommand(connection, text, type, parameters))
			{
				var executeScalar = command.ExecuteScalar();
				return (T)executeScalar;
			}
		}

		public IDataReader ExecuteReader(IDbConnection connection, string text, CommandType type, params DbParameter[] parameters)
		{
			using (IDbCommand command = CreateCommand(connection, text, type, parameters))
			{
				return command.ExecuteReader();
			}
		}

		private IDbCommand CreateCommand(IDbConnection connection, string text, CommandType commandType, DbParameter[] parameters)
		{
			// keep connection string over multiple commands?
			// get connection and reuse, GetCommand uses connection?
			IDbCommand command = connection.CreateCommand();
			command.CommandText = text;
			command.CommandType = commandType;
			foreach (var parameter in parameters)
			{
				command.Parameters.Add(parameter);
			}

			// dont log out parms here
			log.Info("Command {0} {1} {2}", commandType, text, string.Join(", ", parameters.Select(ParameterToString)));
			return command;
		}

		private object ParameterToString(DbParameter arg)
		{
			return $"{arg.ParameterName} = '{arg.Value}' [{arg.Direction}]";
		}
	}
}