using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Utils.Logging;

namespace Server
{
	public class Server :IServer
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Server));
		private readonly DatabaseFactory databaseFactory;

		public Server(string connectionString, string providerName)
		{
			databaseFactory = new DatabaseFactory(connectionString, providerName);

			using (var connection = databaseFactory.CreateOpenConnection())
			{
				log.Info("Db: {0}", databaseFactory.ExecuteScalar<string>(connection, "select @@version", CommandType.Text));
			}
		}

		public string Foo(string value)
		{
			// cant use proc return value with MySql so using out param
			var outParameter = databaseFactory.CreateParameter("@id", DbType.Int32, null);
			outParameter.Direction = ParameterDirection.Output;
			var parms = new[]
			{
				databaseFactory.CreateParameter("@dataValue", DbType.String, value ?? "null"),
				outParameter
			};

			using (var connection = databaseFactory.CreateOpenConnection())
			{
				databaseFactory.ExecuteNonQuery(connection, "demo.AddData", CommandType.StoredProcedure, parms);
				return "id:" + outParameter.Value;
			}
		}

		public string[] Query(int count)
		{
			List<string> result = new List<string>();

			using (var connection = databaseFactory.CreateOpenConnection())
			{
				DbParameter[] parameters = 
				{
					databaseFactory.CreateParameter("count", DbType.Int32, count)
				};

				using (IDataReader reader = databaseFactory.ExecuteReader(connection, "demo.Query", CommandType.StoredProcedure, parameters))
				{
					while (reader.Read())
					{
						result.Add(reader.GetInt32(0) + ":" + reader.GetString(1));
					}
					return result.ToArray();
				}
			}
		}
	}
}
