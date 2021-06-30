
using System.Collections.Generic;
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
        queryString += $"{param.Key} {operation} @{param.Key} ";

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
    public static bool UpdateQuery(string tableName, Dictionary<string, string> queryParameters, Dictionary<string, string> whereParameters)
    {
      string queryString = $"UPDATE {tableName} SET ";

      foreach (var param in queryParameters)
      {
        if (param.Key.Contains("+"))
        {
          string key = param.Key.Replace("+", "");
          queryString += $"{param.Key} = {param.Key} + @{param.Key}, ";
        }
        else if (param.Key.Contains("-"))
        {
          string key = param.Key.Replace("-", "");
          queryString += $"{param.Key} = {param.Key} - @{param.Key}, ";
        }
        else
          queryString += $"{param.Key} = @{param.Key}, ";
      }

      queryString.Remove(queryString.Length - 2);
      queryString += " where ";

      foreach (var param in whereParameters)
        queryString += $"{param.Key} = @{param.Key}, ";

      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, queryString);

      foreach (var param in queryParameters)
        query.BindParam($"@{param.Key}", param.Value);

      foreach (var param in whereParameters)
        query.BindParam($"@{param.Key}", param.Value);

      query.Execute();

      if (query.Error != "")
      {
        Utils.LogMessageToDMs($"{queryString} - {query.Error}");
        return false;
      }

      return true;
    }
    public static IEnumerable<SQLResult> SelectQuery(string tableName, List<string> queryParameters, Dictionary<string, string> whereParameters)
    {
      string queryString = $"SELECT ";

      foreach (string param in queryParameters)
        queryString += $"{param}, ";

      queryString.Remove(queryString.Length - 2);
      queryString += $" FROM {tableName} WHERE ";

      foreach (var param in whereParameters)
        queryString += $"{param.Key} = @{param.Key}, ";

      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, queryString);

      foreach (var param in whereParameters)
        query.BindParam($"@{param.Key}", param.Value);

      query.Execute();

      if (query.Error != "")
      {
        Utils.LogMessageToDMs($"{queryString} - {query.Error}");
        return null;
      }

      return query.Results;
    }
  }
}
