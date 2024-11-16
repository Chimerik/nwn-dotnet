using Anvil.API;

namespace NWN.Systems
{
  public static partial class ClercUtils
  {
    public static async void RestoreClercDomaine(NwCreature creature,  bool shortRest = false)
    {
      await NwTask.NextFrame();
      int chaMod = creature.GetAbilityModifier(Ability.Wisdom);

      if (creature.KnowsFeat((Feat)CustomSkill.ClercMartial))
        creature.SetFeatRemainingUses((Feat)CustomSkill.ClercMartial, (byte)(chaMod > 0 ? chaMod : 1));
      else if (creature.KnowsFeat((Feat)CustomSkill.ClercIllumination))
      {
        if(!shortRest || creature.GetClassInfo(ClassType.Cleric).Level > 5)
          creature.SetFeatRemainingUses((Feat)CustomSkill.ClercIllumination, (byte)(chaMod > 0 ? chaMod : 1));
      }
      else if (creature.KnowsFeat((Feat)CustomSkill.ClercFureurOuraganFoudre) && !shortRest)
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.ClercFureurOuraganFoudre, (byte)(chaMod > 0 ? chaMod : 1));
        creature.SetFeatRemainingUses((Feat)CustomSkill.ClercFureurOuraganTonnerre, (byte)(chaMod > 0 ? chaMod : 1));
      }
    }
  }
}
