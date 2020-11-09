using System;
using System.Net;
using System.Web;
using NWN.Core;
using NWN.Core.NWNX;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTestCommand(ChatSystem.Context ctx, Options.Result options)
    {
      SQLitePCL.Batteries.Init();
      var sql = $"SELECT characterName from playerCharacters where rowid = @id";

      using (var connection = new SqliteConnection(ModuleSystem.db_path))
      {
        //connection.Open();
        var potager = connection.Query(sql, new { id = 1 }).FirstOrDefault();
        NWScript.SendMessageToPC(NWScript.GetFirstPC(), $"result : {potager}");

        //connection.ExecuteAsync();
        //connection.ExecuteAsync($"delete from playerCharacters where rowid = 1");
        //connection.QueryAsync<long>($"delete from playerCharacters where rowid = 1");
        //connection.Execute(sql, new { uuid = NWScript.GetObjectUUID(selectedObject), loc = Utils.LocationToString(NWScript.GetLocation(selectedObject)) }); // TODO : à refaire, il ne faut pas utiliser UUID entre différents reboot de serveur, mais plutôt un id incrémenté en BDD
      }
    }
    public static String Translate(String word)
    {
      var toLanguage = "en";
      var fromLanguage = "fr";
      var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={HttpUtility.UrlEncode(word)}";
      var webClient = new WebClient
      {
        Encoding = System.Text.Encoding.UTF8
      };
      var result = webClient.DownloadString(url);
      try
      {
        result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
        return result;
      }
      catch
      {
        return "Error";
      }
    }
  }
}
