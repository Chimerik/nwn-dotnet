using Anvil.API;

namespace NWN.Systems
{
  public static partial class PaladinUtils
  {
    public static async void ConsumeOathCharge(NwCreature creature)
    { 
      await NwTask.NextFrame();
      creature.SetFeatRemainingUses((Feat)CustomSkill.DevotionArmeSacree, 0);
      creature.SetFeatRemainingUses((Feat)CustomSkill.DevotionSaintesRepresailles, 0);
      creature.SetFeatRemainingUses((Feat)CustomSkill.DevotionRenvoiDesImpies, 0);
      creature.SetFeatRemainingUses((Feat)CustomSkill.AnciensRenvoiDesInfideles, 0);
      creature.SetFeatRemainingUses((Feat)CustomSkill.AnciensGuerisonRayonnante, 0);
      creature.SetFeatRemainingUses((Feat)CustomSkill.AnciensCourrouxDeLaNature, 0);
      creature.SetFeatRemainingUses((Feat)CustomSkill.PaladinVoeuHostile, 0);
      creature.SetFeatRemainingUses((Feat)CustomSkill.PaladinPuissanceInquisitrice, 0);
      creature.SetFeatRemainingUses((Feat)CustomSkill.PaladinConspuerEnnemi, 0);
    }
  }
}
