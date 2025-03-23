using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void BersekerRestorePresenceIntimidante(NwCreature caster)
    {
      caster.DecrementRemainingFeatUses(Feat.BarbarianRage);
      caster.IncrementRemainingFeatUses((Feat)CustomSkill.BersekerPresenceIntimidante);
      caster.SetFeatRemainingUses((Feat)CustomSkill.BersekerRestorePresenceIntimidante, 0);
    }
  }
}
