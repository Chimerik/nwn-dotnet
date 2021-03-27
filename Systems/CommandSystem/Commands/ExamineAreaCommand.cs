using System;
using System.Threading.Tasks;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteExamineAreaCommand(ChatSystem.Context ctx, Options.Result options)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT description from areaDescriptions where areaTag = @areaTag");
      NWScript.SqlBindString(query, "@areaTag", ctx.oSender.Area.Tag);

      if (NWScript.SqlStep(query) != 0)
      {
        string originalDesc = ctx.oSender.Description;
        string tempDescription = ctx.oSender.Area.Name.ColorString(Color.ORANGE) + "\n\n" + NWScript.SqlGetString(query, 0);
        ctx.oSender.Description = tempDescription;
        ctx.oSender.ClearActionQueue();
        ctx.oSender.ActionExamine(ctx.oSender);

        Task spawnResources = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          ctx.oSender.Description = originalDesc;
        });
      }
      else
      {
        ctx.oSender.SendServerMessage($"Aucune description n'a pour le moment été rédigée pour {ctx.oSender.Area.Name}. N'hésitez pas à le signaler au staff si vous aimeriez en voir une !");
      }
    }
  }
}
