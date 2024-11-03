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
        if (oid.LoginCreature.Race.Id == CustomRace.InfernalThiefling)
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ThieflingFireResistance));
        else if (oid.LoginCreature.Race.Id == CustomRace.AbyssalThiefling)
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ThieflingPoisonResistance));
        else if (oid.LoginCreature.Race.Id == CustomRace.ChtonicThiefling)
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ThieflingNecroticResistance));
      }
    }
  }
}
