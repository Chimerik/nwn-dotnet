using Anvil.API;

namespace NWN.Systems
{
  public static partial class PaladinUtils
  {
    public static async void RestorePaladinCharges(NwCreature creature)
    {
      byte? level = creature.GetClassInfo(ClassType.Paladin)?.Level;

      if (!level.HasValue)
        return;

      byte layOnHandsUses = (byte)(level.Value < 4 ? 3 : level.Value < 10 ? 4 : level.Value < 16 ? 5 : 6);
      byte sensDivinUses = (byte)(creature.GetAbilityModifier(Ability.Charisma) > 0 ? creature.GetAbilityModifier(Ability.Charisma) : 1);

      await NwTask.NextFrame();
      creature.SetFeatRemainingUses((Feat)CustomSkill.ImpositionDesMainsMineure, layOnHandsUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ImpositionDesMainsMajeure, layOnHandsUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ImpositionDesMainsGuerison, layOnHandsUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.SensDivin, sensDivinUses);
    }
  }
}
