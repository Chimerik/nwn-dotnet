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

          player.LearnAlwaysPreparedSpell(CustomSpell.ProtectionContreLeMalEtLeBien, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell(CustomSpell.BouclierDeLaFoi, CustomClass.Paladin);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.Aide, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell(CustomSpell.ZoneDeVerite, CustomClass.Paladin);

          break;


        case 7: 

          player.LearnClassSkill(CustomSkill.PaladinAuraDeDevotion);

          var tlk = NwFeat.FromFeatId(CustomSkill.AuraDeProtection).Name;
          tlk.SetPlayerOverride(player.oid, "Aura de Dévotion");

          EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.AuraDeProtectionEffectTag);

          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeProtection(player.oid.LoginCreature, 7));
          UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>().SetRadius(3);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell((int)Spell.DispelMagic, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell(CustomSpell.LueurDespoir, CustomClass.Paladin);

          break;

        case 13:

          player.LearnAlwaysPreparedSpell((int)Spell.FreedomOfMovement, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell(CustomSpell.GardienDeLaFoi, CustomClass.Paladin);

          break;

        case 15: player.LearnClassSkill(CustomSkill.DevotionChatimentProtecteur); break;

        case 17:

          player.LearnAlwaysPreparedSpell((int)Spell.FlameStrike, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell(CustomSpell.Communion, CustomClass.Paladin);

          break;

        case 20:

          player.LearnClassSkill(CustomSkill.DevotionNimbeSacree);

          break;
      }
    }
  }
}
