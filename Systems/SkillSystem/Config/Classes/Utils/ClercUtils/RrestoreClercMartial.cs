using Anvil.API;

namespace NWN.Systems
{
  public static partial class ClercUtils
  {
    public static async void RestoreClercMartial(NwCreature creature)
    {
      await NwTask.NextFrame();
      int chaMod = creature.GetAbilityModifier(Ability.Wisdom);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercMartial, (byte)(chaMod > 0 ? chaMod : 1));
    }
  }
}
