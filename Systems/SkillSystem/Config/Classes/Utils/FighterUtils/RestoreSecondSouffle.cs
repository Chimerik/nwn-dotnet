using Anvil.API;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static async void RestoreSecondSouffle(NwCreature creature,  bool shortRest = false)
    {
      if (!creature.KnowsFeat((Feat)CustomSkill.FighterSecondWind))
        return;

      await NwTask.NextFrame();
      byte fighterLevel = creature.GetClassInfo(ClassType.Fighter).Level;
      byte maxUses = (byte)(fighterLevel > 9 ? 4 : fighterLevel > 3 ? 3 : 2);
      byte currentUses = creature.GetFeatRemainingUses((Feat)CustomSkill.FighterSecondWind);

      if (shortRest)
      {
        if (currentUses < maxUses)
          creature.IncrementRemainingFeatUses((Feat)CustomSkill.FighterSecondWind);
      }
      else
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.FighterSecondWind, maxUses);
      }
    }
  }
}
