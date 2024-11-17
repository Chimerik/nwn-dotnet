using Anvil.API;

namespace NWN.Systems
{
  public static partial class PaladinUtils
  {
    public static async void RestorePaladinCharges(NwCreature creature, bool shortRest = false)
    {
      byte? level = creature.GetClassInfo(ClassType.Paladin)?.Level;

      if (!level.HasValue)
        return;

      byte maxChannelUse = (byte)(level.Value > 10 ? 3 : 2);

      await NwTask.NextFrame();

      if (shortRest)
      {
        byte currentChannelUse = creature.GetFeatRemainingUses((Feat)CustomSkill.SensDivin);

        if (currentChannelUse < maxChannelUse)
        {
          creature.IncrementRemainingFeatUses((Feat)CustomSkill.SensDivin);
          creature.IncrementRemainingFeatUses((Feat)CustomSkill.DevotionArmeSacree);
          creature.IncrementRemainingFeatUses((Feat)CustomSkill.DevotionSaintesRepresailles);
          creature.IncrementRemainingFeatUses((Feat)CustomSkill.DevotionRenvoiDesImpies);
          creature.IncrementRemainingFeatUses((Feat)CustomSkill.AnciensRenvoiDesInfideles);
          creature.IncrementRemainingFeatUses((Feat)CustomSkill.AnciensGuerisonRayonnante);
          creature.IncrementRemainingFeatUses((Feat)CustomSkill.AnciensCourrouxDeLaNature);
          creature.IncrementRemainingFeatUses((Feat)CustomSkill.PaladinVoeuHostile);
          creature.IncrementRemainingFeatUses((Feat)CustomSkill.PaladinPuissanceInquisitrice);
          creature.IncrementRemainingFeatUses((Feat)CustomSkill.PaladinConspuerEnnemi);
        }
      }
      else
      {
        byte layOnHandsUses = (byte)(level.Value < 4 ? 3 : level.Value < 10 ? 4 : level.Value < 16 ? 5 : 6);
        creature.SetFeatRemainingUses((Feat)CustomSkill.ImpositionDesMains, layOnHandsUses);
        creature.SetFeatRemainingUses((Feat)CustomSkill.SensDivin, maxChannelUse);
        creature.SetFeatRemainingUses((Feat)CustomSkill.DevotionArmeSacree, maxChannelUse);
        creature.SetFeatRemainingUses((Feat)CustomSkill.DevotionSaintesRepresailles, maxChannelUse);
        creature.SetFeatRemainingUses((Feat)CustomSkill.DevotionRenvoiDesImpies, maxChannelUse);
        creature.SetFeatRemainingUses((Feat)CustomSkill.AnciensRenvoiDesInfideles, maxChannelUse);
        creature.SetFeatRemainingUses((Feat)CustomSkill.AnciensGuerisonRayonnante, maxChannelUse);
        creature.SetFeatRemainingUses((Feat)CustomSkill.AnciensCourrouxDeLaNature, maxChannelUse);
        creature.SetFeatRemainingUses((Feat)CustomSkill.PaladinVoeuHostile, maxChannelUse);
        creature.SetFeatRemainingUses((Feat)CustomSkill.PaladinPuissanceInquisitrice, maxChannelUse);
        creature.SetFeatRemainingUses((Feat)CustomSkill.PaladinConspuerEnnemi, maxChannelUse);
      }
    }
  }
}
