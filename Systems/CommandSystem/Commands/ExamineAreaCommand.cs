using System;
using System.Threading.Tasks;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  class ExamineArea
  {
    public ExamineArea(NwPlayer oPC)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT description from areaDescriptions where areaTag = @areaTag");
      NWScript.SqlBindString(query, "@areaTag", oPC.Area.Tag);

      if (NWScript.SqlStep(query) != 0)
      {
        string originalDesc = oPC.Description;
        string tempDescription = oPC.Area.Name.ColorString(Color.ORANGE) + "\n\n" + NWScript.SqlGetString(query, 0);
        oPC.Description = tempDescription;
        oPC.ClearActionQueue();
        oPC.ActionExamine(oPC);

        Task waitForDescriptionRewrite = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          oPC.Description = originalDesc;
        });
      }
      else
      {
        oPC.SendServerMessage($"Aucune description n'a pour le moment été rédigée pour {oPC.Area.Name}. N'hésitez pas à le signaler au staff si vous aimeriez en voir une !");
      }
    }
  }
}
