using Anvil.API;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static void DecrementFormeSauvage(NwCreature creature, byte nbSource = 1)
    {
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DruideCompagnonSauvage, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageBlaireau, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageChat, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageAraignee, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageLoup, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageRothe, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvagePanthere, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageOursHibou, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageDilophosaure, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DruideAssistanceTerrestre, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DruideSanctuaireNaturel, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageOurs, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageCorbeau, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageTigre, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageAir, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageTerre, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageFeu, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FormeSauvageEau, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DruideFureurDesFlots, nbSource);
    }
  }
}
