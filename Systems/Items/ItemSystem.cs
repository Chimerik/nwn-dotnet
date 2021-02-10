using NWN.API;
using NWN.Core.NWNX;
using NWN.Services;
using NWNX.API.Events;
using NWNX.Services;
using NWN.API.Constants;
using System.Linq;
using NWN.Core;
using Discord;
using NWN.API.Events;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ItemSystem))]
  public class ItemSystem
  {
    public ItemSystem(NativeEventService eventService, NWNXEventService nwnxEventService)
    {
      nwnxEventService.Subscribe<ItemEvents.OnItemEquipBefore>(OnItemEquipBefore);
      nwnxEventService.Subscribe<ItemEvents.OnItemUseBefore>(OnItemUseBefore);
      eventService.Subscribe<NwModule, ModuleEvents.OnAcquireItem>(NwModule.Instance, OnAcquireItem);
      eventService.Subscribe<NwModule, ModuleEvents.OnUnacquireItem>(NwModule.Instance, OnUnacquireItem);
    }
    private void OnItemEquipBefore(ItemEvents.OnItemEquipBefore onItemEquip)
    {
      if (!(onItemEquip.Creature is NwPlayer))
        return;

      NwPlayer oPC = (NwPlayer)onItemEquip.Creature;
      NwItem oItem = onItemEquip.Item;

      if (oPC == null || oItem == null)
        return;

      int iSlot = int.Parse(EventsPlugin.GetEventData("SLOT"));
      NwItem oUnequip = oPC.GetItemInSlot((InventorySlot)iSlot);

      if (oUnequip != null && ObjectPlugin.CheckFit(oPC, (int)oUnequip.BaseItemType) == 0)
      {
        oPC.SendServerMessage("Attention, votre inventaire est plein. Déséquipper cet objet risquerait de vous le faire perdre !");
        onItemEquip.Skip = true;
        return;
      }

      if (oItem.Tag == "amulettorillink")
        oPC.RemoveEffect(oPC.ActiveEffects.Where(e => e.Tag == "erylies_spell_failure").FirstOrDefault());
      else if (oUnequip?.Tag == "amulettorillink")
      {
        API.Effect eff = API.Effect.SpellFailure(50);
        eff.Tag = "erylies_spell_failure";
        eff.SubType = EffectSubType.Supernatural;
        oPC.ApplyEffect(EffectDuration.Permanent, eff);
      }
    }
    [ScriptHandler("b_unequip")]
    private void HandleUnequipItemBefore(CallInfo callInfo)
    {
      if (!(callInfo.ObjectSelf is NwPlayer))
        return;

      NwPlayer oPC = (NwPlayer)callInfo.ObjectSelf;
      NwItem oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM")).ToNwObject<NwItem>();

      if (oPC == null || oItem == null)
        return;

      if (ObjectPlugin.CheckFit(oPC, (int)oItem.BaseItemType) == 0)
      {
        oPC.SendServerMessage("Attention, votre inventaire est plein. Déséquipper cet objet risquerait de vous le faire perdre !");
        EventsPlugin.SkipEvent();
        return;
      }

      if (oItem.Tag == "amulettorillink")
      {
        API.Effect eff = API.Effect.SpellFailure(50);
        eff.Tag = "erylies_spell_failure";
        eff.SubType = API.Constants.EffectSubType.Supernatural;
        oPC.ApplyEffect(EffectDuration.Permanent, eff);
        (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{oPC.Name} vient d'ôter son amulette de traçage. L'Amiral surveille désormais directement ses activités en rp.");
      }
    }
    private void OnItemUseBefore(ItemEvents.OnItemUseBefore onItemUse)
    {
      if (!(onItemUse.Creature is NwPlayer))
        return;

      NwPlayer player = (NwPlayer)onItemUse.Creature;
      NwItem oItem = onItemUse.Item;
      NwGameObject oTarget = onItemUse.TargetObject;

      if(player == null || oItem == null)
        return;

      switch (oItem.Tag)
      {
        case "skillbook":
          FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player);
          onItemUse.Skip = true;
          Items.ItemUseHandlers.SkillBook.HandleActivate(oItem, player);
          NWScript.DelayCommand(0.2f, () => FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player));
          break;

        case "blueprint":
          FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player);
          onItemUse.Skip = true;
          Items.ItemUseHandlers.Blueprint.HandleActivate(oItem, player, oTarget);
          NWScript.DelayCommand(0.2f, () => FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player));
          break;

        case "oreextractor":
          FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player);
          onItemUse.Skip = true;
          Items.ItemUseHandlers.ResourceExtractor.HandleActivate(oItem, player, oTarget);
          NWScript.DelayCommand(0.2f, () => FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player));
          break;
      }
    }
    private void OnAcquireItem(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      if (!(onAcquireItem.AcquiredBy is NwPlayer))
        return;

      NwPlayer oPC = (NwPlayer)onAcquireItem.AcquiredBy;
      //Console.WriteLine(NWScript.GetName(oPC));

      NwItem oItem = onAcquireItem.Item;
      NwGameObject oAcquiredFrom = onAcquireItem.AcquiredFrom;

      if (oPC == null || oItem == null)
        return;

      //Console.WriteLine(NWScript.GetTag(oItem));

      if (oItem.Tag == "undroppable_item")
      {
        oItem.Copy(oAcquiredFrom, true);
        oItem.Destroy();
        return;
      }

      if (oItem.Tag == "item_pccorpse" && oAcquiredFrom?.Tag == "pccorpse_bodybag")
      {
        PlayerSystem.DeletePlayerCorpseFromDatabase(oItem.GetLocalVariable<int>("_PC_ID").Value);

        NwCreature oCorpse = NwObject.FindObjectsWithTag<NwCreature>("pccorpse").Where(c => c.GetLocalVariable<int>("_PC_ID").Value == oItem.GetLocalVariable<int>("_PC_ID")).FirstOrDefault();
        if(oCorpse != null)
          oCorpse.Destroy();
        oAcquiredFrom.Destroy();
      }
      //En pause jusqu'à ce que le système de transport soit en place
      //if (NWScript.GetMovementRate(oPC) != CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
      //if (NWScript.GetWeight(oPC) > int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oPC, NWScript.ABILITY_STRENGTH))))
      //CreaturePlugin.SetMovementRate(oPC, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE);
    }
    private void OnUnacquireItem(ModuleEvents.OnUnacquireItem onUnacquireItem)
    {
      if (!(onUnacquireItem.LostBy is NwPlayer))
        return;

      NwPlayer oPC = (NwPlayer)onUnacquireItem.LostBy;
      NwItem oItem = onUnacquireItem.Item;

      if (oPC == null || oItem == null)
        return;

      NwGameObject oGivenTo = oItem.Possessor;

      if (oItem.Tag == "item_pccorpse" && oGivenTo == null) // signifie que l'item a été drop au sol et pas donné à un autre PJ ou mis dans un placeable
      {
        NwCreature oCorpse = ObjectPlugin.Deserialize(oItem.GetLocalVariable<string>("_SERIALIZED_CORPSE")).ToNwObject<NwCreature>();
        ObjectPlugin.AddToArea(oCorpse, oItem.Area, oItem.Position);
        NWN.Utils.DestroyInventory(oCorpse);
        ObjectPlugin.AcquireItem(oCorpse, oItem);
        VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, oCorpse, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
        PlayerSystem.SetupPCCorpse(oCorpse);
        //NWScript.DelayCommand(1.3f, () => ObjectPlugin.AcquireItem(NWScript.GetNearestObjectByTag("pccorpse_bodybag", oCorpse), oItem));

        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT characterName from playerCharacters where rowid = @characterId");
        NWScript.SqlBindInt(query, "@characterId", NWScript.GetLocalInt(oItem, "_PC_ID"));
        NWScript.SqlStep(query);

        PlayerSystem.SavePlayerCorpseToDatabase(oItem.GetLocalVariable<int>("_PC_ID").Value, oCorpse, oCorpse.Area.Tag, oCorpse.Position);
      }

      /* En pause jusqu'à ce que le système de transport soit en place
      if (NWScript.GetMovementRate(oPC) == CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
        if (NWScript.GetWeight(oPC) <= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oPC, NWScript.ABILITY_STRENGTH))))
          CreaturePlugin.SetMovementRate(oPC, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_PC);*/
    }
  }
}
