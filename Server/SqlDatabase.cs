using System;
using System.Collections.Generic;
using System.Linq;

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
			SelectDatabase(Database);
		}

		T Execute<T>(MySqlCommand Command, Func<MySqlCommand, T> Executer)
		{
			Console.WriteLine(Command.CommandText);
			lock (_Connection)
			{
				_Connection.Open();
				var result = Executer(Command);
				_Connection.Close();
				return result;
			}
		}

		object ExecuteScalar(MySqlCommand Command)
		{
			return Execute(Command, i => i.ExecuteScalar());
		}

		List<Dictionary<string, object>> ExecuteReader(MySqlCommand Command)
		{
			return Execute(Command, ExecuteDictionary);
		}

		int ExecuteNonQuery(MySqlCommand Command)
		{
			return Execute(Command, i => Command.ExecuteNonQuery());
		}

		List<Dictionary<string, object>> ExecuteDictionary(MySqlCommand Command)
		{
			var reader = Command.ExecuteReader();
			var rows = new List<Dictionary<string, object>>();
			while (reader.Read())
			{
				rows.Add(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
			}
			return rows;
		}

		bool IsInstalled(string Database)
		{
			var command = _Connection.CreateCommand();
			command.CommandText = "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @database";
			command.Parameters.AddWithValue("@database", Database);
			return ExecuteReader(command).Count() > 0;
		}

		void Install(string Database)
		{
			var command = _Connection.CreateCommand();
			command.CommandText = $"CREATE DATABASE {Database}";
			ExecuteNonQuery(command);

			SelectDatabase(Database);

			command.CommandText =
				"CREATE TABLE players ("
				+ "id INT NOT NULL AUTO_INCREMENT,"
				+ "username varchar(32) NOT NULL,"
				+ "password_hash char(88) NOT NULL,"
				+ "password_salt char(8) NOT NULL,"
				+ "PRIMARY KEY (id))";
			ExecuteNonQuery(command);
		}

		void SelectDatabase(string Database)
		{
			var command = _Connection.CreateCommand();
			command.CommandText = $"USE {Database}";
			ExecuteNonQuery(command);
		}

		PlayerOrm RowToPlayer(Dictionary<string, object> Row)
		{
			return new PlayerOrm(
				(int)Row["id"], (string)Row["username"], (string)Row["password_hash"], (string)Row["password_salt"]);
		}

		public PlayerOrm AddPlayer(PlayerOrm Player)
		{
			if (GetPlayer(Player.Username) != null) return null;

			var command = _Connection.CreateCommand();
			command.CommandText = "INSERT INTO players (username, password_hash, password_salt) "
				+ "VALUES (@username, @password_hash, @password_salt)";
			command.Parameters.AddWithValue("@username", Player.Username);
			command.Parameters.AddWithValue("@password_hash", Player.PasswordHash);
			command.Parameters.AddWithValue("@password_salt", Player.PasswordSalt);
			ExecuteNonQuery(command);

			return GetPlayer(Player.Username);
		}

		public PlayerOrm GetPlayer(string Username)
		{
			var command = _Connection.CreateCommand();
			command.CommandText = "SELECT * FROM players WHERE username = @username";
			command.Parameters.AddWithValue("@username", Username);

			var rows = ExecuteReader(command);
			if (rows.Count == 0) return null;
			return RowToPlayer(rows.First());
		}
	}
}
