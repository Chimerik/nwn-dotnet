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
      NWScript.SqlBindString(query, "@areaTag", oPC.ControlledCreature.Area.Tag);

      if (NWScript.SqlStep(query) != 0)
      {
        string originalDesc = oPC.ControlledCreature.Description;
        string tempDescription = oPC.ControlledCreature.Area.Name.ColorString(ColorConstants.Orange) + "\n\n" + NWScript.SqlGetString(query, 0);
        oPC.ControlledCreature.Description = tempDescription;
        oPC.ControlledCreature.ClearActionQueue();
        oPC.ActionExamine(oPC.ControlledCreature);

        Task waitForDescriptionRewrite = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          oPC.ControlledCreature.Description = originalDesc;
        });
      }
      else
      {
        oPC.SendServerMessage($"Aucune description n'a pour le moment été rédigée pour {oPC.ControlledCreature.Area.Name}. N'hésitez pas à le signaler au staff si vous aimeriez en voir une !");
      }
    }
  }
}
