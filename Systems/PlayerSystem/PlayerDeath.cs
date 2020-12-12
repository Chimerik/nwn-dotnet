using System;
using System.Numerics;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    private static int HandlePlayerDeath(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(NWScript.GetLastPlayerDied(), out player))
      {
        NWScript.SendMessageToPC(player.oid, "Tout se brouille autour de vous. Avant de perdre connaissance, vous sentez comme un étrange maëlstrom vous aspirer.");
        
        CreatePlayerCorpse(player);
        StripPlayerGoldAfterDeath(player);
        StripPlayerOfCraftResources(player);
        SavePlayerCorpseToDatabase(player.characterId, player.deathCorpse, NWScript.GetTag(NWScript.GetArea(player.deathCorpse)), NWScript.GetPosition(player.deathCorpse));
        NWScript.DelayCommand(3.0f, () => SendPlayerToLimbo(player));
      }

      return 0;
    }
    private static void CreatePlayerCorpse(Player player)
    {
      var oPCCorpse = NWScript.CreateObject(NWScript.OBJECT_TYPE_PLACEABLE, "pccorpse", NWScript.GetLocation(player.oid));
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_pccorpse_remove_item_after", oPCCorpse);

      NWScript.SetName(oPCCorpse, $"Cadavre de {NWScript.GetName(player.oid)}");
      NWScript.SetDescription(oPCCorpse, $"Cadavre de {NWScript.GetName(player.oid)}");
      NWScript.SetLocalInt(oPCCorpse, "_PC_ID", player.characterId);
      uint oCorpseItem = NWScript.CreateItemOnObject("item_pccorpse", oPCCorpse);
      NWScript.SetLocalInt(oCorpseItem, "_PC_ID", player.characterId);
      NWScript.SetName(oCorpseItem, $"Cadavre de {NWScript.GetName(player.oid)}");

      player.deathCorpse = oPCCorpse;
    }
    private static void StripPlayerGoldAfterDeath(Player player)
    {
      int remainingGold = NWScript.GetGold(player.oid);

      while (remainingGold > 0)
      {
        if (remainingGold >= 50000)
        {
          NWScript.CreateItemOnObject("nw_it_gold001", player.oid, 50000);
          remainingGold -= 50000;
        }
        else
        {
          NWScript.CreateItemOnObject("nw_it_gold001", player.oid, remainingGold);
          break;
        }
      }

      CreaturePlugin.SetGold(player.oid, 0);
    }
    private static void StripPlayerOfCraftResources(Player player)
    {
      var oItem = NWScript.GetFirstItemInInventory(player.oid);
      while(Convert.ToBoolean(NWScript.GetIsObjectValid(oItem)))
      {
        if(CollectSystem.IsItemCraftMaterial(NWScript.GetTag(oItem)) || NWScript.GetTag(oItem) == "blueprint")
        {
          NWScript.CopyObject(oItem, NWScript.GetLocation(player.deathCorpse), player.deathCorpse);
          NWScript.DestroyObject(oItem);
        }
          
        oItem = NWScript.GetNextItemInInventory(player.oid);
      }
    }
    public static void SavePlayerCorpseToDatabase(int characterId, uint deathCorpse, string areaTag, Vector3 position)
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO playerDeathCorpses (characterId, deathCorpse, areaTag, position) VALUES (@characterId, @deathCorpse, @areaTag, @position)");
      NWScript.SqlBindInt(query, "@characterId", characterId);
      NWScript.SqlBindObject(query, "@deathCorpse", deathCorpse);
      NWScript.SqlBindString(query, "@areaTag", areaTag);
      NWScript.SqlBindVector(query, "@position", position);
      NWScript.SqlStep(query);
    }
    private static void SendPlayerToLimbo(Player player)
    {
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_IMP_RESTORATION_GREATER), player.oid);
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectResurrection(), player.oid);
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectHeal(NWScript.GetMaxHitPoints(player.oid)), player.oid);

      NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetWaypointByTag("WP__RESPAWN_AREA"))));
    }
    public static void Respawn(Player player, string entity)
    {
      // TODO : Diminuer la durabilité de tous les objets équipés et dans l'inventaire du PJ

      DestroyPlayerCorpse(player);
      NWScript.AssignCommand(player.oid, () => NWScript.ClearAllActions());
      NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetWaypointByTag("WP_RESPAWN_DISPENSAIRE"))));
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, NWScript.TagEffect(NWScript.SupernaturalEffect(NWScript.EffectSpellFailure(50)), "erylies_spell_failure"), player.oid);

      switch (entity)
      {
        case "radiant":
          ApplyRadiantRespawnEffects(player);
          break;
        case "dire":
          ApplyDireRespawnEffects(player);
          break;
      }

      //NWScript.SendMessageToPC(player.oid, "Votre récente déconvenue vous a affligé d'une blessure durable. Il va falloir passer du temps en rééducation pour vous en débarrasser");

      // TODO : refaire le système de malus en utilisant le plugin feat de nwnx
      /*int iRandomMalus = Utils.random.Next(1130, 1130); // TODO : il faudra mettre en paramètre de conf le range des feat ID pour les malus

      if (CreaturePlugin.GetHighestLevelOfFeat(player.oid, iRandomMalus) != (int)Feat.Invalid)
      {
        int successorId;
        if (int.TryParse(NWScript.Get2DAString("feat", "SUCCESSOR", iRandomMalus), out successorId))
        {
          CreaturePlugin.AddFeat(player.oid, successorId);
          iRandomMalus = successorId;
        }
      }
      else
        CreaturePlugin.AddFeat(player.oid, iRandomMalus);

      Func<Player, int, int> handler;
      if (SkillSystem.RegisterAddCustomFeatEffect.TryGetValue(iRandomMalus, out handler))
      {
        try
        {
          handler.Invoke(player, iRandomMalus);
        }
        catch (Exception e)
        {
          Utils.LogException(e);
        }
      }*/
    }
    private static void DestroyPlayerCorpse(Player player)
    {
      DeletePlayerCorpseFromDatabase(player.characterId);

      var oCorpse = NWScript.GetObjectByTag("pccorpse");
      int i = 1;
      while (Convert.ToBoolean(NWScript.GetIsObjectValid(oCorpse)))
      {
        if (player.characterId == NWScript.GetLocalInt(oCorpse, "_PC_ID"))
        {
          NWScript.DestroyObject(oCorpse);
          break;
        }
        oCorpse = NWScript.GetObjectByTag("pccorpse", i++);
      }

      var oCorpseItem = NWScript.GetObjectByTag("item_pccorpse");
      i = 1;
      while (NWScript.GetIsObjectValid(oCorpseItem) == 1)
      {
        if (player.characterId == NWScript.GetLocalInt(oCorpseItem, "_PC_ID"))
        {
          NWScript.DestroyObject(oCorpseItem);
          break;
        }
        oCorpseItem = NWScript.GetObjectByTag("item_pccorpse", i++);
      }
    }
    public static void DeletePlayerCorpseFromDatabase(int characterId)
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"DELETE FROM playerDeathCorpses WHERE characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", characterId);
      NWScript.SqlStep(query);
    }
    private static int HandleAfterItemRemovedFromPCCorpse(uint oidSelf)
    {
      var oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM"));
      
      if (NWScript.GetTag(oItem) == "item_pccorpse")
      {
        DeletePlayerCorpseFromDatabase(NWScript.GetLocalInt(oItem, "_PC_ID"));
        NWScript.DestroyObject(oidSelf);
      }

      return 0;
    }
    private static void ApplyRadiantRespawnEffects(Player player)
    {
      // TODO : augmentation du niveau d'influence de l'entité
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
    }
    private static void ApplyDireRespawnEffects(Player player)
    {
      // TODO : augmentation du niveau d'influence de l'entité
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
    }
  }
}
