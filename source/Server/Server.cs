using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Logging;

namespace Server
{
	public class Server :IServer
	{
		private ILog log = LogManager.GetLogger(typeof(Server));
		private readonly IDataService dataService;

		public Server(string connectionString)
		{
			dataService = new DataService(connectionString);
			log.Info("Db: {0}", dataService.GetVersion());
		}

		public string Foo(string value)
		{
			// hackupSql until sprocs exist
			int id = dataService.Insert(value);
			return "id:" + id;
		}

		public string[] Query(int count)
		{
			// hackupSql until sprocs exist
			return dataService.Query(count);
		}
	}
}
