using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class ClercUtils
  {
    public static async void RestoreClercDomaine(NwCreature creature,  bool shortRest = false)
    {
      await NwTask.NextFrame();
      byte wisMod = CreatureUtils.GetAbilityModifierMin1(creature, Ability.Wisdom);

      //creature.SetFeatRemainingUses((Feat)CustomSkill.ClercMartial, wisMod);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercFureurOuragan, wisMod);

      if (creature.KnowsFeat((Feat)CustomSkill.ClercIllumination))
      {
        if (!shortRest || creature.GetClassInfo(ClassType.Cleric).Level > 5)
          creature.SetFeatRemainingUses((Feat)CustomSkill.ClercIllumination, wisMod);
      }

      if (creature.KnowsFeat((Feat)CustomSkill.ClercFrappeGuidee))
      {
        if (!shortRest || creature.GetClassInfo(ClassType.Cleric).Level > 5)
          creature.SetFeatRemainingUses((Feat)CustomSkill.ClercFrappeGuidee, wisMod);
      }

      if (!shortRest && creature.KnowsFeat((Feat)CustomSkill.ClercRepliqueInvoquee))
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.MoveRepliqueDuplicite, 0);

        foreach (var associate in creature.Associates.Where(e => e.Tag == EffectSystem.repliqueTag))
        {
          creature.UnpossessFamiliar();
          associate.Unsummon();
        }
      }
    }
  }
}
