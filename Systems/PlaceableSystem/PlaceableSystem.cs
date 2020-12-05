using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class PlaceableSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
      { "ench_bsn_onclose", EnchantmentBasinSystem.HandleClose },
      { "ondeath_clean_dm_plc", HandleCleanDMPLC },
      { "plc_used", HandlePlaceableUsed },
      { "os_statuemaker", HandleStatufyCreature },
      { "oc_statue", HandleCancelStatueConversation },
    };
    private static int HandleCleanDMPLC(uint oidSelf)
    {
      int plcID = NWScript.GetLocalInt(oidSelf, "_ID");
      if (plcID > 0)
      {
        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "DELETE FROM dm_persistant_placeable where rowid = @plcID");
        NWScript.SqlBindInt(query, "@rowid", plcID);
        NWScript.SqlStep(query);
      }
      else
        Utils.LogMessageToDMs($"Persistent placeable {NWScript.GetName(oidSelf)} in area {NWScript.GetName(NWScript.GetArea(oidSelf))} does not have a valid ID !");

      return 0;
    }
    private static int HandlePlaceableUsed(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(NWScript.GetLastUsedBy(), out player))
      {
        switch (NWScript.GetTag(oidSelf))
        {
          case "respawn_neutral":
            NWScript.AssignCommand(player.oid, () => NWScript.ClearAllActions());
            NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetWaypointByTag("WP_RESPAWN_DISPENSAIRE"))));
            break;  
          case "respawn_radiant":
            Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_IMPROVE_ABILITY_SCORE);
            Effect eDur = NWScript.EffectVisualEffect(NWScript.VFX_DUR_CESSATE_POSITIVE);
            Effect eCon = NWScript.EffectAbilityIncrease(NWScript.ABILITY_CONSTITUTION, 2);
            Effect eRegen = NWScript.EffectRegenerate(1, 1.0f);
            Effect eJS = NWScript.EffectSavingThrowIncrease(0, 1);
            Effect eTempHP = NWScript.EffectTemporaryHitpoints(10);
            Effect eDamImm1 = NWScript.EffectDamageImmunityIncrease(1, 10);
            Effect eDamImm2 = NWScript.EffectDamageImmunityIncrease(2, 10);
            Effect eDamImm3 = NWScript.EffectDamageImmunityIncrease(4, 10);
            Effect eDamImm4 = NWScript.EffectDamageImmunityIncrease(8, 10);
            Effect eDamImm5 = NWScript.EffectDamageImmunityIncrease(16, 10);
            Effect eDamImm6 = NWScript.EffectDamageImmunityIncrease(32, 10);
            Effect eDamImm7 = NWScript.EffectDamageImmunityIncrease(64, 10);
            Effect eDamImm8 = NWScript.EffectDamageImmunityIncrease(128, 10);
            Effect eDamImm9 = NWScript.EffectDamageImmunityIncrease(256, 10);
            Effect eDamImm10 = NWScript.EffectDamageImmunityIncrease(512, 10);
            Effect eDamImm11 = NWScript.EffectDamageImmunityIncrease(1024, 10);
            Effect eDamImm12 = NWScript.EffectDamageImmunityIncrease(2048, 10);

            Effect eLink = NWScript.EffectLinkEffects(eVis, eDur);
            eLink = NWScript.EffectLinkEffects(eLink, eCon);
            eLink = NWScript.EffectLinkEffects(eLink, eRegen);
            eLink = NWScript.EffectLinkEffects(eLink, eJS);
            eLink = NWScript.EffectLinkEffects(eLink, eTempHP);
            eLink = NWScript.EffectLinkEffects(eLink, eDamImm1);
            eLink = NWScript.EffectLinkEffects(eLink, eDamImm2);
            eLink = NWScript.EffectLinkEffects(eLink, eDamImm3);
            eLink = NWScript.EffectLinkEffects(eLink, eDamImm4);
            eLink = NWScript.EffectLinkEffects(eLink, eDamImm5);
            eLink = NWScript.EffectLinkEffects(eLink, eDamImm6);
            eLink = NWScript.EffectLinkEffects(eLink, eDamImm7);
            eLink = NWScript.EffectLinkEffects(eLink, eDamImm8);
            eLink = NWScript.EffectLinkEffects(eLink, eDamImm9);
            eLink = NWScript.EffectLinkEffects(eLink, eDamImm10);
            eLink = NWScript.EffectLinkEffects(eLink, eDamImm11);
            eLink = NWScript.EffectLinkEffects(eLink, eDamImm12);

            NWScript.DelayCommand(5.0f, () => NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, eLink, player.oid));
            NWScript.AssignCommand(player.oid, () => NWScript.ClearAllActions());
            NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetWaypointByTag("WP_RESPAWN_DISPENSAIRE"))));
            break;
          case "respawn_dire":
            Effect Vis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_IMPROVE_ABILITY_SCORE);
            Effect Dur = NWScript.EffectVisualEffect(NWScript.VFX_DUR_CESSATE_POSITIVE);
            Effect eStr = NWScript.EffectAbilityIncrease(NWScript.ABILITY_STRENGTH, 2);
            Effect eDam = NWScript.EffectDamageIncrease(1, NWScript.DAMAGE_TYPE_DIVINE);
            Effect eAtt = NWScript.EffectAttackIncrease(1);
            Effect eMS = NWScript.EffectMovementSpeedIncrease(10);

            Effect Link = NWScript.EffectLinkEffects(Vis, Dur);
            Link = NWScript.EffectLinkEffects(Link, eStr);
            Link = NWScript.EffectLinkEffects(Link, eDam);
            Link = NWScript.EffectLinkEffects(Link, eAtt);
            Link = NWScript.EffectLinkEffects(Link, eMS);

            // +1 jet d'attaque; +1 dégat divin; +movement speed 10 %

            NWScript.DelayCommand(5.0f, () => NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, Link, player.oid));
            NWScript.AssignCommand(player.oid, () => NWScript.ClearAllActions());
            NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetWaypointByTag("WP_RESPAWN_DISPENSAIRE"))));
            break;
          case "theater_rope":

            if (!Convert.ToBoolean(NWScript.GetLocalInt(NWScript.GetArea(oidSelf), "_THEATER_CURTAIN_OPEN")))
            {
              for (int i = 0; i < 4; i++)
                VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetObjectByTag("theater_curtain", i), VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);

              NWScript.SetLocalInt(NWScript.GetArea(oidSelf), "_THEATER_CURTAIN_OPEN", 1);
            }
            else
            {
              for (int i = 0; i < 4; i++)
                VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetObjectByTag("theater_curtain", i), VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);

              NWScript.DeleteLocalInt(NWScript.GetArea(oidSelf), "_THEATER_CURTAIN_OPEN");
            }
            break;
        }
      }
      return 0;
    }
    private static int HandleCancelStatueConversation(uint oidSelf)
    {
      return 0;
    }
    private static int HandleStatufyCreature(uint oidSelf)
    {
      if (Convert.ToBoolean(NWScript.GetIsPC(NWScript.GetLastPerceived())))
      {
        NWScript.PlayAnimation(Utils.random.Next(100, 116));
        NWScript.SetEventScript(oidSelf, NWScript.EVENT_SCRIPT_CREATURE_ON_NOTICE, "");
        NWScript.SetAILevel(oidSelf, NWScript.AI_LEVEL_VERY_LOW);
        NWScript.DelayCommand(1.0f, () => FreezeCreature(oidSelf));
      }
      
      return 0;
    }
    private static void FreezeCreature(uint creature)
    {
        
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, NWScript.EffectVisualEffect(NWScript.VFX_DUR_FREEZE_ANIMATION), creature);
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, NWScript.EffectVisualEffect(NWScript.VFX_DUR_ICESKIN), creature);
      NWScript.SetObjectHiliteColor(creature, 0x000000);
      NWScript.SetObjectMouseCursor(creature, NWScript.MOUSECURSOR_WALK);
      NWScript.SetPlotFlag(creature, 1);
    }
  }
}
