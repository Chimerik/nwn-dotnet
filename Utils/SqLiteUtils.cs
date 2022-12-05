
using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Anvil.API;
using NWN.Systems;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Numerics;
using System.Text.Json;

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

    public static async Task<List<string[]>> SelectQueryAsync(string tableName, List<string> queryParameters, List<string[]> whereParameters, string orderBy = "")
    {
      return await Task.Run(async () =>
      {
        string queryString = "SELECT ";

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

        string logString = "Binding WHERE : ";

        try
        {
          using (var connection = new SqliteConnection(Config.dbPath))
          {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = queryString;

            foreach (var param in whereParameters)
            {
              command.Parameters.AddWithValue($"@{param[0]}", (object)param[1] ?? DBNull.Value);
              logString += $"@{param[0]} = {param[1]} ";
            }

            //Log.Info(queryString);
            //Log.Info(logString);

            using (var reader = await command.ExecuteReaderAsync())
            {
              List<string[]> results = new List<string[]>();

              while (reader.Read())
              {
                string[] row = new string[queryParameters.Count];

                for (int i = 0; i < queryParameters.Count; i++)
                {
                  if (!reader.IsDBNull(i))
                    row[i] = reader.GetString(i);
                  else
                    row[i] = "";
                }

                results.Add(row);
              }

              return results;
            }
          }
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"Select Query - {e.Message}");
          Utils.LogMessageToDMs(queryString);
          Utils.LogMessageToDMs(logString);
          return null;
        }
      });
    }
    public static List<string[]> SelectQuery(string tableName, List<string> queryParameters, List<string[]> whereParameters, string orderBy = "")
    {
        string queryString = "SELECT ";

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

        string logString = "Binding WHERE : ";

        try
        {
          using (var connection = new SqliteConnection(Config.dbPath))
          {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = queryString;

            foreach (var param in whereParameters)
            {
              command.Parameters.AddWithValue($"@{param[0]}", (object)param[1] ?? DBNull.Value);
              logString += $"@{param[0]} = {param[1]} ";
            }

            //Log.Info(queryString);
            //Log.Info(logString);

            using (var reader = command.ExecuteReader())
            {
              List<string[]> results = new List<string[]>();

              while (reader.Read())
              {
                string[] row = new string[queryParameters.Count];

                for (int i = 0; i < queryParameters.Count; i++)
                {
                  if (!reader.IsDBNull(i))
                    row[i] = reader.GetString(i);
                  else
                    row[i] = "";
                }

                results.Add(row);
              }

              return results;
            }
          }
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"Select Query - {e.Message}");
          Utils.LogMessageToDMs(queryString);
          Utils.LogMessageToDMs(logString);
          return null;
        }
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
    public static async Task<bool> InsertQueryAsync(string tableName, List<string[]> queryParameters, List<string> conflictParameters = null, List<string[]> updateParameters = null, List<string> whereParameters = null)
    {
      return await Task.Run(async () =>
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

        if (conflictParameters != null)
        {
          queryString += " ON CONFLICT (";
          foreach (string param in conflictParameters)
            queryString += $"{param}, ";

          queryString = queryString.Remove(queryString.Length - 2);
          queryString += ") ";
        }

        if (updateParameters != null)
        {
          queryString += " DO UPDATE SET ";
          foreach (var param in updateParameters)
          {
            if (param.Length > 1)
              queryString += $"{param[0]} = {param[0]} {param[1]} @{param[0]}, ";
            else
              queryString += $"{param[0]} = @{param[0]}, ";
          }

          queryString = queryString.Remove(queryString.Length - 2);
        }

        if (whereParameters != null)
        {
          queryString += " WHERE ";
          foreach (string param in whereParameters)
            queryString += $"{param} = @{param} and ";

          queryString = queryString.Remove(queryString.Length - 5);
        }

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
              command.Parameters.AddWithValue($"@{param[0]}", (object)param[1] ?? DBNull.Value);
              logString += $"@{param[0]} = {param[1]} ";
            }

            //Log.Info(queryString);
            //Log.Info(logString);

            await command.ExecuteNonQueryAsync();
          }
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"Insert Query - {e.Message}");
          Utils.LogMessageToDMs(queryString);
          Utils.LogMessageToDMs(logString);
          return false;
        }

        return true;
      });
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
    public static NwStore StoreSerializationFormatProtection(string serializedStore, Location location)
    {
      NwStore deserializedStore = NwStore.Deserialize(serializedStore.ToByteArray());
      deserializedStore.Location = location;

      return deserializedStore;
    }
    public static NwPlaceable PlaceableSerializationFormatProtection(string serializedPlaceable, Location location)
    {
      NwPlaceable deserializedPlaceable = NwPlaceable.Deserialize(serializedPlaceable.ToByteArray());
      deserializedPlaceable.Location = location;
 
      return deserializedPlaceable;
    }
    public class DataBaseLocation
    {
      public string areaTag { get; set; }
      public float X { get; set; }
      public float Y { get; set; }
      public float Z { get; set; }
      public float facing { get; set; }

      public DataBaseLocation(Location location)
      {
        areaTag = location.Area.Tag;
        X = location.Position.X;
        Y = location.Position.Y;
        Z = location.Position.Z;
        facing = location.Rotation;
      }
      public DataBaseLocation()
      {

      }
    }
    public static string SerializeLocation(Location location)
    {
      return JsonSerializer.Serialize(new DataBaseLocation(location));
    }
    public static Location DeserializeLocation(string serializedLocation)
    {
      try
      {
        DataBaseLocation dbLocation = JsonSerializer.Deserialize<DataBaseLocation>(serializedLocation);
        return Location.Create(NwModule.Instance.Areas.FirstOrDefault(a => a.Tag == dbLocation.areaTag), new Vector3(dbLocation.X, dbLocation.Y, dbLocation.Z), dbLocation.facing);
      }
      catch (Exception)
      {
        Utils.LogMessageToDMs($"unable to deserialize location : {serializedLocation}");
        return NwModule.Instance.StartingLocation;
      }      
    }
    public static async Task<int> GetOfflinePlayerSkillPoints(string characterId, int skillId)
    {
      var result = await SelectQueryAsync("playerCharacters",
        new List<string>() { { "serializedLearnableSkills" } },
        new List<string[]>() { new string[] { "ROWID", characterId } });

      if (result == null && result.Count < 1)
        return 0;

      string serializedLearnableSkills = result.FirstOrDefault()[0];

      Task<int> loadSkillsTask = Task.Run(() =>
      {
        if (string.IsNullOrEmpty(serializedLearnableSkills) || serializedLearnableSkills == "null")
          return 0;

        Dictionary<int, LearnableSkill.SerializableLearnableSkill> serializableSkills = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, LearnableSkill.SerializableLearnableSkill>>(serializedLearnableSkills);

        if (serializableSkills.TryGetValue(skillId, out LearnableSkill.SerializableLearnableSkill skill))
          return skill.currentLevel + skill.bonusPoints;
        else
          return 0;
      });

      await loadSkillsTask;
      return loadSkillsTask.Result;
    }
  }
}
