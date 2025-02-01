using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyElvenSleepImmunity()
      {
        switch(oid.LoginCreature.Race.Id)
        {
          case CustomRace.Elf:
          case CustomRace.HighElf:
          case CustomRace.WoodElf:

            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.SleepImmunityEffectTag))
              oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.sleepImmunity);

            break;

          default: return;
        }
      }
    }
  }
}
