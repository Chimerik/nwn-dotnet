using Anvil.API;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static void DecrementFormeSauvage(NwCreature creature, byte nbSource = 1)
    {
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DruideCompagnonSauvage, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvage, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageRothe, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvage2, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageDilophosaure, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DruideAssistanceTerrestre, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DruideSanctuaireNaturel, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageOurs, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageCorbeau, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageTigre, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DruideFureurDesFlots, nbSource);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.FormeSauvage) < 2)
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageAir, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageTerre, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageFeu, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageEau, 0);
      }
      else
      {
        creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageAir, nbSource);
        creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageTerre, nbSource);
        creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageFeu, nbSource);
        creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageEau, nbSource);
      }
    }
  }
}
