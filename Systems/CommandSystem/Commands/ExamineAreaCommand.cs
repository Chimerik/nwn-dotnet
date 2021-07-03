using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NWN.API;

namespace NWN.Systems
{
  class ExamineArea
  {
    public ExamineArea(NwPlayer oPC)
    {
      var result = SqLiteUtils.SelectQuery("areaDescriptions",
        new List<string>() { { "description" } },
        new List<string[]>() { new string[] { "areaTag", oPC.ControlledCreature.Area.Tag } });

      if(result != null && result.Count() > 0)
      {
        string originalDesc = oPC.ControlledCreature.Description;
        string tempDescription = oPC.ControlledCreature.Area.Name.ColorString(ColorConstants.Orange) + "\n\n" + result.FirstOrDefault().GetString(0);
        oPC.ControlledCreature.Description = tempDescription;

        Task waitForDescriptionRewrite = NwTask.Run(async () =>
        {
          await oPC.ControlledCreature.ClearActionQueue();
          await oPC.ActionExamine(oPC.ControlledCreature);
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
