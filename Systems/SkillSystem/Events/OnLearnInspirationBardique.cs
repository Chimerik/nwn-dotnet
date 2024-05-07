using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnInspirationBardique(Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BardInspiration))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.BardInspiration);

      int chaMod = player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) > 0 ? player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) : 1;
      DelayFeatInit(player.oid.LoginCreature, (byte)chaMod);

      return true;
    }
    private static async void DelayFeatInit(NwCreature creature, byte chaMod)
    {
      await NwTask.NextFrame();
      creature.SetFeatRemainingUses((Feat)CustomSkill.BardInspiration, chaMod);
    }

  }
}
