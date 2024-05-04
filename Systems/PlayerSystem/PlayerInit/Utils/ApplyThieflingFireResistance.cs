using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyThieflingFireResistance()
      {
        if (Utils.In(oid.LoginCreature.Race.Id, CustomRace.AsmodeusThiefling, CustomRace.MephistoThiefling, CustomRace.ZarielThiefling))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ThieflingFireResistance));
      }
    }
  }
}
