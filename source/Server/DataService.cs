using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utils.Logging;

namespace Server
{
	internal class DataService : IDataService
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DataService));
		private readonly string connectionString;

		public DataService(string connectionString)
		{
			this.connectionString = connectionString;
			log.Info("ConnectionString {0}", connectionString);
		}

		public string GetVersion()
		{
			return ExecuteScalar<string>("select @@version");
		}

		public int Insert(string value)
		{
			const string sql = @"INSERT INTO [Demo].[Data] ([DataValue]) OUTPUT INSERTED.ID VALUES (@Value)";
			var parms = new[] { new SqlParameter("@Value", value ?? "null") };
			return ExecuteScalar<int>(sql, parms);
		}

		public string[] Query(int count)
		{
			const string sql = @"SELECT TOP 10 [Id],[DataValue] FROM [Demo].[Data] ORDER BY Id DESC";

			List<string> result = new List<string>();
			using (SqlDataReader reader = ExecuteReader(sql, new SqlParameter("count", count)))
			{
				while (reader.Read())
				{
					result.Add(reader.GetInt32(0) + ":" +reader.GetString(1));
				}
			}
			return result.ToArray();
		}

		private SqlCommand CreateCommand(string text, CommandType commandType, SqlParameter[] parameters)
		{
			var connection = new SqlConnection(connectionString);
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText = text;
			command.CommandType = commandType;
			command.Parameters.AddRange(parameters);

			log.Info("Command {0} {1} {2}", commandType, text, string.Join(", ", parameters.Select(ParameterToString)));
			return command;
		}

		private object ParameterToString(SqlParameter arg)
		{
			return $"{arg.ParameterName} = '{arg.Value}'";
		}

		private T ExecuteScalar<T>(string text, params SqlParameter[] parameters)
		{
			using (SqlCommand command = CreateCommand(text, CommandType.Text, parameters))
			{
				return (T)command.ExecuteScalar();
			}
		}

		private SqlDataReader ExecuteReader(string text, params SqlParameter[] parameters)
		{
			using (SqlCommand command = CreateCommand(text, CommandType.Text, parameters))
			{
				return command.ExecuteReader();
			}
		}
	}
}