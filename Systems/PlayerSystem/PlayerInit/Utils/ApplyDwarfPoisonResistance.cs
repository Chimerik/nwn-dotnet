using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDwarfPoisonResistance()
      {
        if (Utils.In(oid.LoginCreature.Race.Id, CustomRace.GoldDwarf, CustomRace.ShieldDwarf, CustomRace.StrongheartHalfling))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.DwarfPoisonResistance));
      }
    }
  }
}
