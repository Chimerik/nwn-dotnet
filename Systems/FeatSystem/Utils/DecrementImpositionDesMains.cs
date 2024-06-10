using Anvil.API;

namespace NWN.Systems
{
  public static partial class FeatUtils
  {
    public static void DecrementLayOnHands(NwCreature creature, byte nbCharge = 1)
    {
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.ImpositionDesMainsMineure, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.ImpositionDesMainsMajeure, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.ImpositionDesMainsGuerison, nbCharge);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.ImpositionDesMainsMajeure) < 2)
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.ImpositionDesMainsMajeure, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.ImpositionDesMainsGuerison, 0);
      }
    }
  }
}
