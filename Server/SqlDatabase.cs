using System;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace PanzerBlitz
{
	public class SqlDatabase : Database
	{
		readonly MySqlConnection _Connection;

		public SqlDatabase(string Server, string Database, string Username, string Password)
		{
			var connectionString = new MySqlConnectionStringBuilder();
			connectionString.UserID = Username;
			connectionString.Password = Password;
			connectionString.Server = Server;
			connectionString.ConnectionTimeout = 30;

			Console.WriteLine(connectionString.GetConnectionString(false));
			_Connection = new MySqlConnection(connectionString.GetConnectionString(true));

			if (!IsInstalled(Database)) Install(Database);

			var command = _Connection.CreateCommand();
			command.CommandText = $"USE {Database}";
			ExecuteNonQuery(command);
		}

		T Execute<T>(MySqlCommand Command, Func<T> Executer)
		{
			Console.WriteLine(Command.CommandText);
			lock (_Connection)
			{
				_Connection.Open();
				var result = Executer();
				_Connection.Close();
				return result;
			}
		}

		MySqlDataReader ExecuteReader(MySqlCommand Command)
		{
			return Execute(Command, Command.ExecuteReader);
		}

		int ExecuteNonQuery(MySqlCommand Command)
		{
			return Execute(Command, Command.ExecuteNonQuery);
		}

		bool IsInstalled(string Database)
		{
			var command = _Connection.CreateCommand();
			command.CommandText = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @database";
			command.Parameters.AddWithValue("@database", Database);
			return ExecuteReader(command).HasRows;
		}

		void Install(string Database)
		{
			var command = _Connection.CreateCommand();
			command.CommandText = $"CREATE DATABASE {Database}";
			ExecuteNonQuery(command);
		}

		public PlayerOrm AddPlayer(string Username, string Password)
		{
			return null;
		}

		public PlayerOrm GetPlayer(string Username)
		{
			return null;
		}
	}
}
