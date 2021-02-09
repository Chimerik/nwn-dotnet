using System;
using System.Numerics;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.API.Constants;
using System.Linq;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    private void HandlePlayerDeath(ModuleEvents.OnPlayerDeath onPlayerDeath)
    {
      if (Players.TryGetValue(NWScript.GetLastPlayerDied(), out Player player))
      {
        NWScript.SendMessageToPC(player.oid, "Tout se brouille autour de vous. Avant de perdre connaissance, vous sentez comme un étrange maëlstrom vous aspirer.");

        CreatePlayerCorpse(player);
        StripPlayerGoldAfterDeath(player);
        StripPlayerOfCraftResources(player);

        player.EmitDeath(new Player.DeathEventArgs(player, onPlayerDeath.Killer));

        Task task3 = NwTask.Run(async () =>
        {
          // Executed in the server thread, you can use NWN APIs here.
          await NwTask.Delay(TimeSpan.FromSeconds(1.3));
          SavePlayerCorpseToDatabase(player.characterId, player.deathCorpse, NWScript.GetTag(NWScript.GetArea(player.deathCorpse)), NWScript.GetPosition(player.deathCorpse));
          await NwTask.Delay(TimeSpan.FromSeconds(2.7));
          SendPlayerToLimbo(player);
          return true;
        });
      }
    }
    private void CreatePlayerCorpse(Player player)
    {
      NwCreature oPCCorpse = NwCreature.Create("pccorpse", player.oid.Location);
      oPCCorpse.Name = $"Corps inconscient de {player.oid.Name}";
      oPCCorpse.Description = $"Corps inconscient de {player.oid.Name}. \n\n\n Allez savoir combien de temps il va tenir dans cet état.";
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, oPCCorpse, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
      oPCCorpse.ApplyEffect(EffectDuration.Instant, API.Effect.CutsceneGhost());
      oPCCorpse.Position = player.oid.Position;
      NWScript.SetLocalInt(oPCCorpse, "_PC_ID", player.characterId);

      oPCCorpse.CreatureAppearanceType = player.oid.CreatureAppearanceType;

      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, NWScript.GetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, player.oid), oPCCorpse);
      NWScript.SetColor(oPCCorpse, NWScript.COLOR_CHANNEL_TATTOO_1, NWScript.GetColor(player.oid, NWScript.COLOR_CHANNEL_TATTOO_1));
      NWScript.SetColor(oPCCorpse, NWScript.COLOR_CHANNEL_TATTOO_2, NWScript.GetColor(player.oid, NWScript.COLOR_CHANNEL_TATTOO_2));
      NWScript.SetColor(oPCCorpse, NWScript.COLOR_CHANNEL_HAIR, NWScript.GetColor(player.oid, NWScript.COLOR_CHANNEL_HAIR));
      NWScript.SetObjectVisualTransform(oPCCorpse, NWScript.OBJECT_VISUAL_TRANSFORM_SCALE, NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_SCALE));

      Task equipCorpse = NwTask.Run(async () =>
      {
        await oPCCorpse.ActionEquipItem(player.oid.GetItemInSlot(InventorySlot.Chest).Copy(oPCCorpse, true), InventorySlot.Chest);
        await oPCCorpse.ActionEquipItem(player.oid.GetItemInSlot(InventorySlot.Head).Copy(oPCCorpse, true), InventorySlot.Head);
        await oPCCorpse.ActionEquipItem(player.oid.GetItemInSlot(InventorySlot.Cloak).Copy(oPCCorpse, true), InventorySlot.Cloak);
        await oPCCorpse.ActionEquipItem(player.oid.GetItemInSlot(InventorySlot.RightHand).Copy(oPCCorpse, true), InventorySlot.RightHand);
        await oPCCorpse.ActionEquipItem(player.oid.GetItemInSlot(InventorySlot.LeftHand).Copy(oPCCorpse, true), InventorySlot.LeftHand);
        oPCCorpse.GetItemInSlot(InventorySlot.Chest).Droppable = false;
        oPCCorpse.GetItemInSlot(InventorySlot.Head).Droppable = false;
        oPCCorpse.GetItemInSlot(InventorySlot.Cloak).Droppable = false;
        oPCCorpse.GetItemInSlot(InventorySlot.RightHand).Droppable = false;
        oPCCorpse.GetItemInSlot(InventorySlot.LeftHand).Droppable = false;

        await NwTask.Delay(TimeSpan.FromSeconds(2));
        SetupPCCorpse(oPCCorpse);
        return true;
      });

      NwItem oCorpseItem = NwItem.Create("item_pccorpse", oPCCorpse);
      NWScript.SetLocalInt(oCorpseItem, "_PC_ID", player.characterId);
      oCorpseItem.Name = $"Corps inconscient de {player.oid.Name}";
      oCorpseItem.Description = $"Corps inconscient de {player.oid.Name}\n\n\n Pas très ragoûtant. Allez savoir combien de temps il va tenir avant de se lâcher.";

      Task serializeCorpse = NwTask.Run(async () =>
      {
        await NwTask.WhenAll(equipCorpse);
        NWScript.SetLocalString(oCorpseItem, "_SERIALIZED_CORPSE", ObjectPlugin.Serialize(oPCCorpse));
        player.deathCorpse = oPCCorpse;
        return true;
      });
    }
    private static void StripPlayerGoldAfterDeath(Player player)
    {
      while (player.oid.Gold > 0)
      {
        if (player.oid.Gold >= 50000)
        {
          NWScript.DelayCommand(1.4f, () => NWScript.CreateItemOnObject("nw_it_gold001", player.deathCorpse, 50000));
          player.oid.TakeGold(50000);
        }
        else
        {
          player.oid.TakeGold(player.oid.Gold);
          NWScript.DelayCommand(1.4f, () => NWScript.CreateItemOnObject("nw_it_gold001", player.deathCorpse, player.oid.Gold));
          break;
        }
      }
    }
    private static void StripPlayerOfCraftResources(Player player)
    {
      foreach (NwItem oItem in player.oid.Items.Where(i => Craft.Collect.System.IsItemCraftMaterial(i.Tag) || i.Tag == "blueprint"))
      {
        oItem.Copy(player.deathCorpse.ToNwObject<NwGameObject>(), true);
        oItem.Destroy();
      }
    }
    public static void SavePlayerCorpseToDatabase(int characterId, uint deathCorpse, string areaTag, Vector3 position)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"INSERT INTO playerDeathCorpses (characterId, deathCorpse, areaTag, position) VALUES (@characterId, @deathCorpse, @areaTag, @position)");
      NWScript.SqlBindInt(query, "@characterId", characterId);
      NWScript.SqlBindObject(query, "@deathCorpse", deathCorpse);
      NWScript.SqlBindString(query, "@areaTag", areaTag);
      NWScript.SqlBindVector(query, "@position", position);
      NWScript.SqlStep(query);
    }
    private static void SendPlayerToLimbo(Player player)
    {
      player.oid.Location = NwModule.FindObjectsWithTag<NwWaypoint>("WP__RESPAWN_AREA").FirstOrDefault().Location;

      Task teleportPlayer = NwTask.Run(async () =>
      {
        await NwTask.WaitUntilValueChanged(() => player.oid.Area != null);
        player.oid.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(VfxType.ImpRestorationGreater));
        player.oid.ApplyEffect(EffectDuration.Instant, API.Effect.Resurrection());
        player.oid.ApplyEffect(EffectDuration.Instant, API.Effect.Heal(player.oid.MaxHP));
        return true;
      });
    }
    public static void Respawn(Player player, string entity)
    {
      // TODO : Diminuer la durabilité de tous les objets équipés et dans l'inventaire du PJ

      DestroyPlayerCorpse(player);
      player.oid.ClearActionQueue();
      player.oid.Location = NwModule.FindObjectsWithTag<NwWaypoint>("WP_RESPAWN_DISPENSAIRE").FirstOrDefault().Location;

      if (NWScript.GetTag(NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_NECK, player.oid)) != "amulettorillink")
      {
        API.Effect eff = API.Effect.SpellFailure(50);
        eff.Tag = "erylies_spell_failure";
        eff.SubType = EffectSubType.Supernatural;
        player.oid.ApplyEffect(EffectDuration.Permanent, eff);
      }
      switch (entity)
      {
        case "radiant":
          ApplyRadiantRespawnEffects(player);
          break;
        case "dire":
          ApplyDireRespawnEffects(player);
          break;
      }

      player.bankGold -= 50;
      NWScript.SendMessageToPC(player.oid, "Afin de vous remettre sur pied, les 'soigneurs' ont demandé à la banque de prélever 50 pièces d'or sur votre compte.");

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
    public static void DestroyPlayerCorpse(Player player)
    {
      DeletePlayerCorpseFromDatabase(player.characterId);

      NwObject oCorpse = NwModule.FindObjectsWithTag("pccorpse").Where(c => c.GetLocalVariable<int>("_PC_ID").Value == player.characterId).FirstOrDefault();
      NWScript.DestroyObject(NWScript.GetNearestObjectByTag("pccorpse_bodybag", oCorpse));
      ((NwGameObject)oCorpse).Destroy();

      ((NwGameObject)NwModule.FindObjectsWithTag("item_pccorpse").Where(c => c.GetLocalVariable<int>("_PC_ID").Value == player.characterId).FirstOrDefault()).Destroy();
    }
    public static void DeletePlayerCorpseFromDatabase(int characterId)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"DELETE FROM playerDeathCorpses WHERE characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", characterId);
      NWScript.SqlStep(query);
    }
    private static void ApplyRadiantRespawnEffects(Player player)
    {
      // TODO : augmentation du niveau d'influence de l'entité
      Core.Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_IMPROVE_ABILITY_SCORE);
      Core.Effect eDur = NWScript.EffectVisualEffect(NWScript.VFX_DUR_CESSATE_POSITIVE);
      Core.Effect eCon = NWScript.EffectAbilityIncrease(NWScript.ABILITY_CONSTITUTION, 2);
      Core.Effect eRegen = NWScript.EffectRegenerate(1, 1.0f);
      Core.Effect eJS = NWScript.EffectSavingThrowIncrease(0, 1);
      Core.Effect eTempHP = NWScript.EffectTemporaryHitpoints(10);
      Core.Effect eDamImm1 = NWScript.EffectDamageImmunityIncrease(1, 10);
      Core.Effect eDamImm2 = NWScript.EffectDamageImmunityIncrease(2, 10);
      Core.Effect eDamImm3 = NWScript.EffectDamageImmunityIncrease(4, 10);
      Core.Effect eDamImm4 = NWScript.EffectDamageImmunityIncrease(8, 10);
      Core.Effect eDamImm5 = NWScript.EffectDamageImmunityIncrease(16, 10);
      Core.Effect eDamImm6 = NWScript.EffectDamageImmunityIncrease(32, 10);
      Core.Effect eDamImm7 = NWScript.EffectDamageImmunityIncrease(64, 10);
      Core.Effect eDamImm8 = NWScript.EffectDamageImmunityIncrease(128, 10);
      Core.Effect eDamImm9 = NWScript.EffectDamageImmunityIncrease(256, 10);
      Core.Effect eDamImm10 = NWScript.EffectDamageImmunityIncrease(512, 10);
      Core.Effect eDamImm11 = NWScript.EffectDamageImmunityIncrease(1024, 10);
      Core.Effect eDamImm12 = NWScript.EffectDamageImmunityIncrease(2048, 10);

      Core.Effect eLink = NWScript.EffectLinkEffects(eVis, eDur);
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
    private static void ApplyDireRespawnEffects(PlayerSystem.Player player)
    {
      // TODO : augmentation du niveau d'influence de l'entité
      Core.Effect Vis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_IMPROVE_ABILITY_SCORE);
      Core.Effect Dur = NWScript.EffectVisualEffect(NWScript.VFX_DUR_CESSATE_POSITIVE);
      Core.Effect eStr = NWScript.EffectAbilityIncrease(NWScript.ABILITY_STRENGTH, 2);
      Core.Effect eDam = NWScript.EffectDamageIncrease(1, NWScript.DAMAGE_TYPE_DIVINE);
      Core.Effect eAtt = NWScript.EffectAttackIncrease(1);
      Core.Effect eMS = NWScript.EffectMovementSpeedIncrease(10);

      Core.Effect Link = NWScript.EffectLinkEffects(Vis, Dur);
      Link = NWScript.EffectLinkEffects(Link, eStr);
      Link = NWScript.EffectLinkEffects(Link, eDam);
      Link = NWScript.EffectLinkEffects(Link, eAtt);
      Link = NWScript.EffectLinkEffects(Link, eMS);

      // +1 jet d'attaque; +1 dégat divin; +movement speed 10 %

      NWScript.DelayCommand(5.0f, () => NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, Link, player.oid));
    }
    public static void SetupPCCorpse(uint oPCCorpse)
    {
      NWScript.AssignCommand(NwModule.Instance, () => NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectDeath(), oPCCorpse));
      uint wp = NWScript.CreateObject(NWScript.OBJECT_TYPE_WAYPOINT, "NW_WAYPOINT001", NWScript.GetLocation(oPCCorpse), 0, $"wp_pccorpse_{NWScript.GetLocalInt(oPCCorpse, "_PC_ID")}");
      NWScript.DelayCommand(1.0f, () => VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, oPCCorpse, VisibilityPlugin.NWNX_VISIBILITY_DEFAULT));
      NWScript.DelayCommand(1.2f, () => NWScript.SetTag(oPCCorpse, "pccorpse"));
      NWScript.DelayCommand(1.2f, () => NWScript.SetTag(NWScript.GetNearestObjectByTag("BodyBag", wp), "pccorpse_bodybag"));
      NWScript.DelayCommand(1.3f, () => NWScript.DestroyObject(wp));
    }
  }
}
