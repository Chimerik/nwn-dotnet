using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Paladin
  {
    public static void HandleDevotionLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Serment de Dévotion");
          player.oid.SetTextureOverride("paladin", "devotion");

          player.LearnClassSkill(CustomSkill.DevotionArmeSacree);
          player.LearnClassSkill(CustomSkill.DevotionSaintesRepresailles);
          player.LearnClassSkill(CustomSkill.DevotionRenvoiDesImpies);

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.ProtectionContreLeMalEtLeBien, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.ShieldOfFaith, CustomClass.Paladin);

          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Aid, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.LesserRestoration, CustomClass.Paladin);

          break;


        case 7:

          player.LearnClassSkill(CustomSkill.PaladinAuraDeDevotion);
          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeProtection(player.oid.LoginCreature, 7));
          UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(3);

          break;

        case 9:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.DispelMagic, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.LueurDespoir, CustomClass.Paladin);

          break;

        case 13:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.FreedomOfMovement, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.GardienDeLaFoi, CustomClass.Paladin);

          break;

        case 15: player.LearnClassSkill(CustomSkill.DevotionChatimentProtecteur); break;

        case 17:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.FlameStrike, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.Communion, CustomClass.Paladin);

          break;

        case 20:

          player.LearnClassSkill(CustomSkill.DevotionNimbeSacree);

          break;
      }
    }
  }
}
