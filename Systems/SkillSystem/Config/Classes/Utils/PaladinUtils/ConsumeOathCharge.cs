using Anvil.API;

namespace NWN.Systems
{
  public static partial class PaladinUtils
  {
    public static void ConsumeOathCharge(NwCreature creature)
    {
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.SensDivin);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DevotionArmeSacree);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DevotionSaintesRepresailles);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DevotionRenvoiDesImpies);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.AnciensRenvoiDesInfideles);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.AnciensGuerisonRayonnante);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.AnciensCourrouxDeLaNature);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.PaladinVoeuHostile);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.PaladinPuissanceInquisitrice);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.PaladinConspuerEnnemi);
    }
  }
}
