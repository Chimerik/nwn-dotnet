using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMageDeGuerre(PlayerSystem.Player player, int customSkillId)
    {
      if(!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.MageDeGuerre)))
        player.oid.LoginCreature.AddFeat(Feat.Ambidexterity);

      player.oid.LoginCreature.OnSpellAction -= SpellSystem.CancelSomaticSpellIfOffHandBusy;

      return true;
    }
  }
}
