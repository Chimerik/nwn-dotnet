using Anvil.API;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static async void RestoreEnnemiJure(NwCreature creature)
    {
      byte? level = creature.GetClassInfo(ClassType.Ranger)?.Level;

      if (!level.HasValue)
        return;

      byte maxUse = (byte)(level.Value > 16 ? 6 : level.Value > 12 ? 5 : level.Value > 8 ? 4 : level.Value > 4 ? 3 : 2);

      await NwTask.NextFrame();

      creature.SetFeatRemainingUses((Feat)CustomSkill.ImpositionDesMains, maxUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.RangerInfatiguable, (byte)(creature.GetAbilityModifier(Ability.Wisdom) > 1 ? creature.GetAbilityModifier(Ability.Wisdom) : 1));
    }
  }
}
