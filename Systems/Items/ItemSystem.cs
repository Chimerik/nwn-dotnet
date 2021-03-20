using NWN.API;
using NWN.Core.NWNX;
using NWN.Services;
using NWNX.API.Events;
using NWNX.Services;
using NWN.API.Constants;
using System.Linq;
using NWN.Core;
using NWN.API.Events;
using NLog;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ItemSystem))]
  public class ItemSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public ItemSystem(NWNXEventService nwnxEventService)
    {
      nwnxEventService.Subscribe<ItemEvents.OnItemEquipBefore>(OnItemEquipBefore);
      nwnxEventService.Subscribe<ItemEvents.OnItemUseBefore>(OnItemUseBefore);
      NwModule.Instance.OnAcquireItem += OnAcquireItem;
      NwModule.Instance.OnUnacquireItem += OnUnacquireItem;
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
      
      if (oUnequip != null && !oPC.Inventory.CheckFit(oUnequip))
      {
        oPC.SendServerMessage("Attention, votre inventaire est plein. Déséquipper cet objet risquerait de vous le faire perdre !", API.Color.RED);
        onItemEquip.Skip = true;
        return;
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
    }
    private void OnItemUseBefore(ItemEvents.OnItemUseBefore onItemUse)
    {
      if (!(PlayerSystem.Players.TryGetValue(onItemUse.Creature, out PlayerSystem.Player player)))
        return;

      NwItem oItem = onItemUse.Item;
      NwGameObject oTarget = onItemUse.TargetObject;

      if(player == null || oItem == null)
        return;

      switch (oItem.Tag)
      {
        case "skillbook":
          FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player.oid);
          onItemUse.Skip = true;
          Items.ItemUseHandlers.SkillBook.HandleActivate(oItem, player.oid);
          NWScript.DelayCommand(0.2f, () => FeedbackPlugin.SetFeedbackMessageHidden(23, 0, player.oid));
          break;

        case "blueprint":
          FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player.oid);
          onItemUse.Skip = true;
          Items.ItemUseHandlers.Blueprint.HandleActivate(oItem, player.oid, oTarget);
          NWScript.DelayCommand(0.2f, () => FeedbackPlugin.SetFeedbackMessageHidden(23, 0, player.oid));
          break;

        case "oreextractor":
          FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player.oid);
          onItemUse.Skip = true;
          Items.ItemUseHandlers.ResourceExtractor.HandleActivate(oItem, player.oid, oTarget);
          NWScript.DelayCommand(0.2f, () => FeedbackPlugin.SetFeedbackMessageHidden(23, 0, player.oid));
          break;
        case "private_contract":
          FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player.oid);
          onItemUse.Skip = true;
          new PrivateContract(player.oid, oItem);
          NWScript.DelayCommand(0.2f, () => FeedbackPlugin.SetFeedbackMessageHidden(23, 0, player.oid));
          break;
        case "shop_clearance":
          FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player.oid);
          onItemUse.Skip = true;
          new PlayerShop(player.oid, oItem);
          NWScript.DelayCommand(0.2f, () => FeedbackPlugin.SetFeedbackMessageHidden(23, 0, player.oid));
          break;
        case "auction_clearanc":
          FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player.oid);
          onItemUse.Skip = true;
          new PlayerAuction(player.oid, oItem);
          NWScript.DelayCommand(0.2f, () => FeedbackPlugin.SetFeedbackMessageHidden(23, 0, player.oid));
          break;
        case "forgehammer":
          FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player.oid);
          onItemUse.Skip = true;

          if (oTarget is NwItem)
            new CraftTool(player, (NwItem)oTarget);
          else
            player.oid.SendServerMessage($"Vous ne pouvez pas modifier l'apparence de {oTarget.Name.ColorString(Color.WHITE)}.".ColorString(Color.RED));

          NWScript.DelayCommand(0.2f, () => FeedbackPlugin.SetFeedbackMessageHidden(23, 0, player.oid));
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

      if (oItem.GetLocalVariable<int>("_DURABILITY").HasNothing)
        oItem.GetLocalVariable<int>("_DURABILITY").Value = ItemUtils.GetItemMaxDurability(oItem);

      if (oItem.Tag == "undroppable_item")
      {
        oItem.Clone(oAcquiredFrom, null, true);
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
        oCorpse.Location = oItem.Location;
        Utils.DestroyInventory(oCorpse);
        oCorpse.AcquireItem(oItem);
        VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, oCorpse, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
        PlayerSystem.SetupPCCorpse(oCorpse);
        //NWScript.DelayCommand(1.3f, () => ObjectPlugin.AcquireItem(NWScript.GetNearestObjectByTag("pccorpse_bodybag", oCorpse), oItem));

        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT characterName from playerCharacters where rowid = @characterId");
        NWScript.SqlBindInt(query, "@characterId", NWScript.GetLocalInt(oItem, "_PC_ID"));
        NWScript.SqlStep(query);

        PlayerSystem.SavePlayerCorpseToDatabase(oItem.GetLocalVariable<int>("_PC_ID").Value, oCorpse);
      }

      /* En pause jusqu'à ce que le système de transport soit en place
      if (NWScript.GetMovementRate(oPC) == CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
        if (NWScript.GetWeight(oPC) <= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oPC, NWScript.ABILITY_STRENGTH))))
          CreaturePlugin.SetMovementRate(oPC, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_PC);*/
    }
    public static async void OnTorilNecklaceRemoved(NwPlayer oPC)
    {
      oPC.SendServerMessage("Votre lien avec la Toile semble particulièrement faible. Un échec des sorts de 50 % vous est appliqué.", API.Color.PINK);
      API.Effect eff = API.Effect.SpellFailure(50);
      eff.Tag = "erylies_spell_failure";
      eff.SubType = EffectSubType.Supernatural;
      oPC.ApplyEffect(EffectDuration.Permanent, eff);

      //await (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{oPC.Name} ne dispose pas d'amulette de traçage, ce qui le rend suspect aux yeux de l'Amiral.");

      await NwTask.WaitUntil(() => oPC.GetItemInSlot(InventorySlot.Neck)?.Tag == "amulettorillink");
      OnTorilNecklaceEquipped(oPC);
    }
    public static async void OnTorilNecklaceEquipped(NwPlayer oPC)
    {
      oPC.SendServerMessage("Votre lien avec la Toile se renforce de manière significative.", API.Color.PINK);

      if (oPC.ActiveEffects.Any(e => e.Tag == "erylies_spell_failure"))
        oPC.RemoveEffect(oPC.ActiveEffects.Where(e => e.Tag == "erylies_spell_failure").FirstOrDefault());

      await NwTask.WaitUntil(() => oPC.GetItemInSlot(InventorySlot.Neck)?.Tag != "amulettorillink");
      OnTorilNecklaceRemoved(oPC);
    }
    public static async void OnArmorRemoved(NwPlayer oPC)
    {
      oPC.SendServerMessage("Attention, en l'absence d'une armure ou d'un vêtement, une vulnérabilité de 100 % aux dégâts tranchants vous est appliquée !", API.Color.ROSE);
      API.Effect eff = API.Effect.DamageImmunityDecrease(DamageType.Slashing, 100);
      eff.SubType = EffectSubType.Supernatural;
      eff.Tag = "NO_ARMOR_MALUS";
      oPC.ApplyEffect(EffectDuration.Permanent, eff);

      await NwTask.WaitUntil(() => oPC.GetItemInSlot(InventorySlot.Chest) != null);
      OnArmorEquipped(oPC);
    }
    public static async void OnArmorEquipped(NwPlayer oPC)
    {
      if (oPC.ActiveEffects.Any(e => e.Tag == "NO_ARMOR_MALUS"))
      {
        oPC.RemoveEffect(oPC.ActiveEffects.Where(e => e.Tag == "NO_ARMOR_MALUS").FirstOrDefault());

        if (oPC.GetItemInSlot(InventorySlot.Head) == null)
          oPC.SendServerMessage("Attention, en l'absence d'un casque, une vulnérabilité de 100 % aux dégâts perforants vous est appliquée !", API.Color.CYAN);

        if (oPC.GetItemInSlot(InventorySlot.LeftHand) == null && (oPC.GetItemInSlot(InventorySlot.RightHand) == null || ItemUtils.GetItemCategory((BaseItemType)oPC.GetItemInSlot(InventorySlot.RightHand)?.BaseItemType) == ItemUtils.ItemCategory.OneHandedMeleeWeapon))
          oPC.SendServerMessage("Attention, en l'absence d'un bouclier, d'une arme à deux mains, ou d'une arme secondaire, une vulnérabilité de 100 % aux dégâts contondants vous est appliquée !", API.Color.WHITE);
      }

      await NwTask.WaitUntil(() => oPC.GetItemInSlot(InventorySlot.Chest) == null);
      OnArmorRemoved(oPC);
    }
    public static async void OnHelmetRemoved(NwPlayer oPC)
    {
      oPC.SendServerMessage("Attention, en l'absence d'un casque, une vulnérabilité de 100 % aux dégâts perforants vous est appliquée !", API.Color.CYAN);
      API.Effect eff = API.Effect.DamageImmunityDecrease(DamageType.Piercing, 100);
      eff.SubType = EffectSubType.Supernatural;
      eff.Tag = "NO_HELMET_MALUS";
      oPC.ApplyEffect(EffectDuration.Permanent, eff);

      await NwTask.WaitUntil(() => oPC.GetItemInSlot(InventorySlot.Head) != null);
      OnHelmetEquipped(oPC);
    }
    public static async void OnHelmetEquipped(NwPlayer oPC)
    {
      if (oPC.ActiveEffects.Any(e => e.Tag == "NO_HELMET_MALUS"))
      {
        oPC.RemoveEffect(oPC.ActiveEffects.Where(e => e.Tag == "NO_HELMET_MALUS").FirstOrDefault());

        if (oPC.GetItemInSlot(InventorySlot.Chest) == null)
          oPC.SendServerMessage("Attention, en l'absence d'une armure ou d'un vêtement, une vulnérabilité de 100 % aux dégâts tranchants vous est appliquée !", API.Color.ROSE);

        if (oPC.GetItemInSlot(InventorySlot.LeftHand) == null && (oPC.GetItemInSlot(InventorySlot.RightHand) == null || ItemUtils.GetItemCategory((BaseItemType)oPC.GetItemInSlot(InventorySlot.RightHand)?.BaseItemType) == ItemUtils.ItemCategory.OneHandedMeleeWeapon))
          oPC.SendServerMessage("Attention, en l'absence d'un bouclier, d'une arme à deux mains, ou d'une arme secondaire, une vulnérabilité de 100 % aux dégâts contondants vous est appliquée !", API.Color.WHITE);
      }

      await NwTask.WaitUntil(() => oPC.GetItemInSlot(InventorySlot.Head) == null);
      OnHelmetRemoved(oPC);
    }
    public static async void OnShieldRemoved(NwPlayer oPC)
    {
      oPC.SendServerMessage("Attention, en l'absence d'un bouclier, d'une arme à deux mains, ou d'une arme secondaire, une vulnérabilité de 100 % aux dégâts contondants vous est appliquée !", API.Color.WHITE);
      API.Effect eff = API.Effect.DamageImmunityDecrease(DamageType.Bludgeoning, 100);
      eff.SubType = EffectSubType.Supernatural;
      eff.Tag = "NO_SHIELD_MALUS";
      oPC.ApplyEffect(EffectDuration.Permanent, eff);
      
      await NwTask.WaitUntil(() => oPC.GetItemInSlot(InventorySlot.LeftHand) != null || (oPC.GetItemInSlot(InventorySlot.RightHand) != null && ItemUtils.GetItemCategory((BaseItemType)oPC.GetItemInSlot(InventorySlot.RightHand).BaseItemType) != ItemUtils.ItemCategory.OneHandedMeleeWeapon));
      OnShieldEquipped(oPC);
    }
    public static async void OnShieldEquipped(NwPlayer oPC)
    {
      if (oPC.ActiveEffects.Any(e => e.Tag == "NO_SHIELD_MALUS"))
      {
        oPC.RemoveEffect(oPC.ActiveEffects.Where(e => e.Tag == "NO_SHIELD_MALUS").FirstOrDefault());

        if (oPC.GetItemInSlot(InventorySlot.Head) == null)
          oPC.SendServerMessage("Attention, en l'absence d'un casque, une vulnérabilité de 100 % aux dégâts perforants vous est appliquée !", API.Color.CYAN);

        if (oPC.GetItemInSlot(InventorySlot.Chest) == null)
          oPC.SendServerMessage("Attention, en l'absence d'une armure ou d'un vêtement, une vulnérabilité de 100 % aux dégâts tranchants vous est appliquée !", API.Color.ROSE);
      }

      await NwTask.WaitUntil(() => oPC.GetItemInSlot(InventorySlot.LeftHand) == null && (oPC.GetItemInSlot(InventorySlot.RightHand) == null || ItemUtils.GetItemCategory((BaseItemType)oPC.GetItemInSlot(InventorySlot.RightHand)?.BaseItemType) == ItemUtils.ItemCategory.OneHandedMeleeWeapon));
      OnShieldRemoved(oPC);
    }
    public static void ApplyNakedMalus(NwPlayer oPC)
    {
      if (oPC.GetItemInSlot(InventorySlot.Head) == null)
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == "NO_HELMET_MALUS"))
        {
          API.Effect eff = API.Effect.DamageImmunityDecrease(DamageType.Piercing, 100);
          eff.SubType = EffectSubType.Supernatural;
          eff.Tag = "NO_HELMET_MALUS";
          oPC.ApplyEffect(EffectDuration.Permanent, eff);
        }
        
        oPC.SendServerMessage("Attention, en l'absence d'un casque, une vulnérabilité de 100 ¨% aux dégâts perforants vous est appliquée !", API.Color.BROWN);
      }
      else if (oPC.ActiveEffects.Any(e => e.Tag == "NO_HELMET_MALUS"))
        oPC.RemoveEffect(oPC.ActiveEffects.Where(e => e.Tag == "NO_HELMET_MALUS").FirstOrDefault());

      if (oPC.GetItemInSlot(InventorySlot.Chest) == null)
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == "NO_ARMOR_MALUS"))
        {
          API.Effect eff = API.Effect.DamageImmunityDecrease(DamageType.Slashing, 100);
          eff.SubType = EffectSubType.Supernatural;
          eff.Tag = "NO_ARMOR_MALUS";
          oPC.ApplyEffect(EffectDuration.Permanent, eff);
        }

        oPC.SendServerMessage("Attention, en l'absence d'une armure ou d'un vêtement, une vulnérabilité de 100 ¨% aux dégâts tranchants vous est appliquée !", API.Color.SILVER);
      }
      else if (oPC.ActiveEffects.Any(e => e.Tag == "NO_ARMOR_MALUS"))
        oPC.RemoveEffect(oPC.ActiveEffects.Where(e => e.Tag == "NO_ARMOR_MALUS").FirstOrDefault());

      if (oPC.GetItemInSlot(InventorySlot.LeftHand) == null && (oPC.GetItemInSlot(InventorySlot.RightHand) == null 
        ||  (oPC.GetItemInSlot(InventorySlot.RightHand) != null && ItemUtils.GetItemCategory(oPC.GetItemInSlot(InventorySlot.RightHand).BaseItemType) == ItemUtils.ItemCategory.OneHandedMeleeWeapon)))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == "NO_SHIELD_MALUS"))
        {
          API.Effect eff = API.Effect.DamageImmunityDecrease(DamageType.Bludgeoning, 100);
          eff.SubType = EffectSubType.Supernatural;
          eff.Tag = "NO_SHIELD_MALUS";
          oPC.ApplyEffect(EffectDuration.Permanent, eff);
        }

        oPC.SendServerMessage("Attention, en l'absence d'un bouclier, d'une arme à deux mains, ou d'une arme secondaire, une vulnérabilité de 100 ¨% aux dégâts contondants vous est appliquée !", API.Color.MAROON);
      }
      else if (oPC.ActiveEffects.Any(e => e.Tag == "NO_SHIELD_MALUS"))
        oPC.RemoveEffect(oPC.ActiveEffects.Where(e => e.Tag == "NO_SHIELD_MALUS").FirstOrDefault());
    }
  }
}
