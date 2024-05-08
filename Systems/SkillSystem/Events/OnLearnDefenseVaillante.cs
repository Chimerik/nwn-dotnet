using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDefenseVaillante(Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DefenseVaillante))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.DefenseVaillante);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DegatsVaillants))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.DegatsVaillants);

      DelayInspitInit(player.oid.LoginCreature, player.oid.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.BardInspiration));

      return true;
    }
    private static async void DelayInspitInit(NwCreature creature, byte chaMod)
    {
      await NwTask.NextFrame();
      creature.SetFeatRemainingUses((Feat)CustomSkill.DefenseVaillante, chaMod);
      creature.SetFeatRemainingUses((Feat)CustomSkill.DegatsVaillants, chaMod);
    }

  }
}
