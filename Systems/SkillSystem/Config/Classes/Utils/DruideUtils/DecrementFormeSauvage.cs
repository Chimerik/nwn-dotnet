using Anvil.API;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static void DecrementFormeSauvage(NwCreature creature, byte nbSource = 1)
    {
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DruideCompagnonSauvage, nbSource);
    }
  }
}
