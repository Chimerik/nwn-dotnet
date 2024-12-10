using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Occultiste
  {
    public static void HandleGrandAncienLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          NwClass.FromClassId(CustomClass.Occultiste).Name.SetPlayerOverride(player.oid, "Mécène Grand Ancien");
          player.oid.SetTextureOverride("occultiste", "warlock_ancien");

          player.LearnAlwaysPreparedSpell((int)Spell.TashasHideousLaughter, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.ForceFantasmagorique, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.MurmuresDissonnants, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.DetectionDesPensees, CustomClass.Occultiste);

          player.LearnClassSkill(CustomSkill.EspritEveille);
          player.LearnClassSkill(CustomSkill.SortsPsychiques);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.VoraciteDhadar, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell((int)Spell.ClairaudienceAndClairvoyance, CustomClass.Occultiste);

          break;

        case 6: player.LearnClassSkill(CustomSkill.CombattantClairvoyant); break;

        case 7:

          player.LearnAlwaysPreparedSpell((int)Spell.Confusion, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.InvocationDaberration, CustomClass.Occultiste);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell(CustomSpell.AlterationMemorielle, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.Telekinesie, CustomClass.Occultiste);

          break;

        case 10:

          player.LearnAlwaysPreparedSpell((int)Spell.BestowCurse, CustomClass.Occultiste);
          player.LearnClassSkill(CustomSkill.BouclierPsychique);

          break;
      }
    }
  }
}
