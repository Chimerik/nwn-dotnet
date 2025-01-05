using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleGuerreLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Guerre");
          player.oid.SetTextureOverride("clerc", "guerre");

          player.LearnClassSkill(CustomSkill.ClercBenedictionDuDieuDeLaGuerre);
          player.LearnClassSkill(CustomSkill.ClercFrappeGuidee);

          player.LearnAlwaysPreparedSpell((int)Spell.ShieldOfFaith, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.DivineFavor, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.MagicWeapon, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.ShelgarnsPersistentBlade, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.EclairTracant, CustomClass.Clerc);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.EspritsGardiens, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.CapeDuCroise, CustomClass.Clerc);

          break;

        case 6: 

          player.LearnClassSkill(CustomSkill.AttaqueSupplementaire);
          CreatureUtils.InitializeNumAttackPerRound(player.oid.LoginCreature); 

          break;

        case 7:

          player.LearnAlwaysPreparedSpell((int)Spell.ElementalShield, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.FreedomOfMovement, CustomClass.Clerc);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell((int)Spell.FlameStrike, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.HoldMonster, CustomClass.Clerc);

          break;

        case 17: player.LearnClassSkill(CustomSkill.ClercGuerreAvatarDeBataille); break;
      }
    }
  }
}
