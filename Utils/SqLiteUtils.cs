
using System.Collections.Generic;
using System.Linq;
using NWN.API;
using NWN.Systems;

namespace NWN
{
  public static class SqLiteUtils
  {
    public static bool DeletionQuery(string tableName, Dictionary<string, string> queryParameters, string operation = "=")
    {
      string queryString = $"DELETE from {tableName} where ";

      foreach (var param in queryParameters)
        queryString += $"{param.Key} {operation} @{param.Key} and ";

      queryString.Remove(queryString.Length - 5);

      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, queryString);

      foreach (var param in queryParameters)
        query.BindParam($"@{param.Key}", param.Value);

      query.Execute();
      
      if (query.Error != "")
      {
        Utils.LogMessageToDMs($"{queryString} - {query.Error}");
        return false;
      }

      return true;
    }
    public static bool UpdateQuery(string tableName, List<string[]> queryParameters, List<string[]> whereParameters)
    {
      string queryString = $"UPDATE {tableName} SET ";

      foreach (var param in queryParameters)
      {
        if(param.Length > 2)
          queryString += $"{param[0]} = {param[0]} {param[2]} @{param[0]}, ";
        else
          queryString += $"{param[0]} = @{param[0]}, ";
      }

      queryString.Remove(queryString.Length - 2);
      queryString += " where ";

      foreach (var param in whereParameters)
        queryString += $"{param[0]} = @{param[0]} and ";

      queryString.Remove(queryString.Length - 5);

      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, queryString);

      foreach (var param in queryParameters)
        query.BindParam($"@{param[0]}", param[1]);

      foreach (var param in whereParameters)
        query.BindParam($"@{param[0]}", param[1]);

      query.Execute();

      if (query.Error != "")
      {
        Utils.LogMessageToDMs($"{queryString} - {query.Error}");
        return false;
      }

      return true;
    }
    public static IEnumerable<SQLResult> SelectQuery(string tableName, List<string> queryParameters, List<string[]> whereParameters, string orderBy = "")
    {
      string queryString = $"SELECT ";

      foreach (string param in queryParameters)
        queryString += $"{param}, ";

      queryString.Remove(queryString.Length - 2);
      queryString += $" FROM {tableName}";

      if (whereParameters.Count() > 0)
        queryString += $" WHERE ";

      foreach (var param in whereParameters)
      {
        if (param.Length > 2)
          queryString += $"{param[0]} {param[2]} @{param[0]} and ";
        else
          queryString += $"{param[0]} = @{param[0]} and ";
      }

      queryString.Remove(queryString.Length - 5);
      queryString += orderBy;

      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, queryString);

      foreach (var param in whereParameters)
        query.BindParam($"@{param[0]}", param[1]);

      query.Execute();

      if (query.Error != "")
      {
        Utils.LogMessageToDMs($"{queryString} - {query.Error}");
        return null;
      }

      return query.Results;
    }
    public static bool InsertQuery(string tableName, List<string[]> queryParameters, List<string> conflictParameters = null, List<string[]> updateParameters = null, List<string> whereParameters = null)
    {
      string queryString = $"INSERT INTO {tableName} (";

      foreach (var param in queryParameters)
      {
          queryString += $"{param[0]}, ";
      }

      queryString.Remove(queryString.Length - 2);
      queryString += ") VALUES (";

      foreach (var param in queryParameters)
      {
        queryString += $"@{param[0]}, ";
      }

      queryString.Remove(queryString.Length - 2);
      queryString += ") ";

      if(conflictParameters != null)
      {
        queryString += " ON CONFLICT (";
        foreach(string param in conflictParameters)
          queryString += $"{param}, ";

        queryString.Remove(queryString.Length - 2);
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

        queryString.Remove(queryString.Length - 2);
      }

      if(whereParameters != null)
      {
        queryString += " WHERE ";
        foreach (string param in whereParameters)
          queryString += $"{param} = @{param} and ";

        queryString.Remove(queryString.Length - 5);
      }

      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, queryString);

      foreach (var param in queryParameters)
        query.BindParam($"@{param[0]}", param[1]);

      query.Execute();

      if (query.Error != "")
      {
        Utils.LogMessageToDMs($"{queryString} - {query.Error}");
        return false;
      }

      return true;
    }
  }
}
