
using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Anvil.API;
using NWN.Systems;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace NWN
{
  public static class SqLiteUtils
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();

    public static async void DeletionQuery(string tableName, Dictionary<string, string> queryParameters, string operation = "=")
    {
      await Task.Run(async () =>
      {
        string queryString = $"DELETE from {tableName} where ";

        foreach (var param in queryParameters)
          queryString += $"{param.Key} {operation} ${param.Key} and ";

        queryString = queryString.Remove(queryString.Length - 5);

        //Log.Info(queryString);
        string logString = "Binding : ";

        try
        {
          using (var connection = new SqliteConnection(Config.dbPath))
          {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = queryString;

            foreach (var param in queryParameters)
            {
              command.Parameters.AddWithValue($"${param.Key}", param.Value);
              logString += $"${param.Key} = {param.Value} ";
            }

            //Log.Info(logString);

            await command.ExecuteNonQueryAsync();
          }
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"Delete Query - {e.Message}");
        }
      });
    }
    public static async void UpdateQuery(string tableName, List<string[]> queryParameters, List<string[]> whereParameters)
    {
      await Task.Run(async () =>
      {
        string queryString = $"UPDATE {tableName} SET ";

        foreach (var param in queryParameters)
        {
          if (param.Length > 2)
            queryString += $"{param[0]} = {param[0]} {param[2]} ${param[0]}, ";
          else
            queryString += $"{param[0]} = ${param[0]}, ";
        }

        queryString = queryString.Remove(queryString.Length - 2);
        queryString += " where ";

        foreach (var param in whereParameters)
          queryString += $"{param[0]} = ${param[0]} and ";

        queryString = queryString.Remove(queryString.Length - 5);

        string logString = "Binding SET : ";

        try
        {
          using (var connection = new SqliteConnection(Config.dbPath))
          {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = queryString;

            foreach (var param in queryParameters)
            {
              command.Parameters.AddWithValue($"${param[0]}", (object)param[1] ?? DBNull.Value);
              logString += $"${param[0]} = {param[1]} ";
            }

            if(whereParameters.Count > 0)
              logString += "Binding WHERE : ";

            foreach (var param in whereParameters)
            {
              command.Parameters.AddWithValue($"${param[0]}", param[1]);
              logString += $"${param[0]} = {param[1]} ";
            }

            //Log.Info(queryString);
            //Log.Info(logString);

            await command.ExecuteNonQueryAsync();
          }
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"Update Query - {e.Message}");
        }
      });
    }
    public static SQLQuery SelectQuery(string tableName, List<string> queryParameters, List<string[]> whereParameters, string orderBy = "")
    {
      string queryString = $"SELECT ";

      foreach (string param in queryParameters)
        queryString += $"{param}, ";

      queryString = queryString.Remove(queryString.Length - 2);
      queryString += $" FROM {tableName}";

      if (whereParameters.Count() > 0)
      {
        queryString += " WHERE ";

        foreach (var param in whereParameters)
        {
          if (param.Length > 2)
            queryString += $"{param[0]} {param[2]} @{param[0]} and ";
          else
            queryString += $"{param[0]} = @{param[0]} and ";
        }

        queryString = queryString.Remove(queryString.Length - 5);
      }
      queryString += orderBy;

      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, queryString);
  
      //Log.Info(queryString);
      string logString = "Binding WHERE : ";

      foreach (var param in whereParameters)
      {
        query.BindParam($"@{param[0]}", param[1]);
        logString += $"@{param[0]} = {param[1]} ";
      }

      //Log.Info(logString);

      query.Execute();

      if (query.Error != "")
        Utils.LogMessageToDMs($"{queryString} - {query.Error}");

      return query;
    }
    public static bool InsertQuery(string tableName, List<string[]> queryParameters, List<string> conflictParameters = null, List<string[]> updateParameters = null, List<string> whereParameters = null)
    {
      string queryString = $"INSERT INTO {tableName} (";

      foreach (var param in queryParameters)
          queryString += $"{param[0]}, ";

      queryString = queryString.Remove(queryString.Length - 2);
      queryString += ") VALUES (";

      foreach (var param in queryParameters)
        queryString += $"@{param[0]}, ";

      queryString = queryString.Remove(queryString.Length - 2);
      queryString += ") ";

      if(conflictParameters != null)
      {
        queryString += " ON CONFLICT (";
        foreach(string param in conflictParameters)
          queryString += $"{param}, ";

        queryString = queryString.Remove(queryString.Length - 2);
        queryString += ") ";
      }

      if (updateParameters != null)
      {
        queryString += " DO UPDATE SET ";
        foreach (var param in updateParameters)
        {
          if(param.Length > 1)
            queryString += $"{param[0]} = {param[0]} {param[1]} @{param[0]}, ";
          else
            queryString += $"{param[0]} = @{param[0]}, ";
        }

        queryString = queryString.Remove(queryString.Length - 2);
      }

      if(whereParameters != null)
      {
        queryString += " WHERE ";
        foreach (string param in whereParameters)
          queryString += $"{param} = @{param} and ";

        queryString = queryString.Remove(queryString.Length - 5);
      }

      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, queryString);

      //Log.Info(queryString);
      string logString = "Binding : ";

      foreach (var param in queryParameters)
      {
        query.BindParam($"@{param[0]}", param[1]);
        logString += $"Binding @{param[0]} = {param[1]} ";
      }
      
      //Log.Info(logString);
      query.Execute();

      if (query.Error != "")
      {
        Utils.LogMessageToDMs($"{queryString} - {query.Error}");
        return false;
      }

      return true;
    }
    public static bool CreateQuery(string queryString)
    {
      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, queryString);
      query.Execute();

      //Log.Info(queryString);

      if (query.Error != "")
      {
        Utils.LogMessageToDMs($"{queryString} - {query.Error}");
        return false;
      }

      return true;
    }
    public static NwStore StoreSerializationFormatProtection(SQLResult result, int index, Location location)
    {
      NwStore deserializedStore;

      try
      {
        deserializedStore = NwStore.Deserialize(result.GetString(index).ToByteArray());
        deserializedStore.Location = location;
      }
      catch (FormatException)
      {
        deserializedStore = result.GetObject<NwStore>(index, location);
      }

      return deserializedStore;
    }
    public static NwPlaceable PlaceableSerializationFormatProtection(SQLResult result, int index, Location location)
    {
      NwPlaceable deserializedPlaceable;

      try
      {
        deserializedPlaceable = NwPlaceable.Deserialize(result.GetString(index).ToByteArray());
        deserializedPlaceable.Location = location;
      }
      catch (FormatException)
      {
        deserializedPlaceable = result.GetObject<NwPlaceable>(index, location);
      }

      return deserializedPlaceable;
    }
  }
}
