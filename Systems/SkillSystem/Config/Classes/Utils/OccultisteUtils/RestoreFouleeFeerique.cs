using Anvil.API;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static async void RestoreFouleeFeerique(NwCreature creature)
    {
      await NwTask.NextFrame();

      byte maxUses = (byte)(creature.GetAbilityModifier(Ability.Charisma) > 1 ? creature.GetAbilityModifier(Ability.Charisma) : 1);

      creature.SetFeatRemainingUses((Feat)CustomSkill.FouleeRafraichissante, maxUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FouleeProvocatrice, maxUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FouleeEvanescente, maxUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FouleeRedoutable, maxUses);
    }
  }
}
