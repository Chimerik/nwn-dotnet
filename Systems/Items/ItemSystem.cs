using NWN.API;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.API.Constants;
using System.Linq;
using NWN.Core;
using NWN.API.Events;
using NLog;
using System.Threading.Tasks;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ItemSystem))]
  public class ItemSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static FeedbackService feedbackService;
    public ItemSystem(FeedbackService feedback)
    {
      feedbackService = feedback;
      //NwModule.Instance.OnAcquireItem += OnAcquireItem;
      //NwModule.Instance.OnUnacquireItem += OnUnacquireItem;
    }
    public static void OnItemEquipBefore(OnItemEquip onItemEquip)
    {
      NwCreature oPC = onItemEquip.EquippedBy;
      NwItem oItem = onItemEquip.Item;

      if (oPC == null || oItem == null)
        return;

      NwItem oUnequip = oPC.GetItemInSlot(onItemEquip.Slot);
      
      if (oUnequip != null && !oPC.Inventory.CheckFit(oUnequip))
      {
        oPC.ControllingPlayer.SendServerMessage($"Attention, votre inventaire est plein. Vous risqueriez de perdre votre {oUnequip.Name} en déséquipant !", Color.RED);
        onItemEquip.PreventEquip = true;
        return;
      }
    }

    [ScriptHandler("b_unequip")]
    private void HandleUnequipItemBefore(CallInfo callInfo)
    {
      NwCreature oPC = (NwCreature)callInfo.ObjectSelf;

      if (oPC.ControllingPlayer == null)
        return;

      NwItem oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM")).ToNwObject<NwItem>();

      if (oItem == null)
        return;

      if (oPC.Inventory.CheckFit(oItem))
        return;

      if (oPC.GetLocalVariable<int>("CUSTOM_EFFECT_NOARMOR").HasValue
        || oPC.GetLocalVariable<int>("CUSTOM_EFFECT_NOWEAPON").HasValue
        || oPC.GetLocalVariable<int>("CUSTOM_EFFECT_NOACCESSORY").HasValue
        || oItem.GetLocalVariable<int>("_DURABILITY") <= 0)
      {
        oPC.ControllingPlayer.SendServerMessage($"Attention, votre inventaire est plein. Votre {oItem.Name} a été déposé au sol !", Color.RED);
        oItem.Clone(oPC.Location);
        oItem.Destroy();
        oPC.GetLocalVariable<int>("CUSTOM_EFFECT_NOARMOR").Delete();
        oPC.GetLocalVariable<int>("CUSTOM_EFFECT_NOWEAPON").Delete();
        oPC.GetLocalVariable<int>("CUSTOM_EFFECT_NOACCESSORY").Delete();
      }
      else
        oPC.ControllingPlayer.SendServerMessage($"Attention, votre inventaire est plein. Vous risqueriez de perdre votre {oItem.Name} en déséquipant !", Color.RED);

      EventsPlugin.SkipEvent();
    }
    public static void OnItemUseBefore(OnItemUse onItemUse)
    {
      NwCreature oPC = onItemUse.UsedBy;

      NwItem oItem = onItemUse.Item;
      NwGameObject oTarget = onItemUse.TargetObject;

      if(oPC.ControllingPlayer == null || oItem == null)
        return;
      
      switch (oItem.Tag)
      {
        case "skillbook":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;
          Items.ItemUseHandlers.SkillBook.HandleActivate(oItem, oPC.ControllingPlayer.LoginCreature);
          break;

        case "blueprint":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;
          Items.ItemUseHandlers.Blueprint.HandleActivate(oItem, oPC.ControllingPlayer.LoginCreature, oTarget);

          break;

        case "oreextractor":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;
          Items.ItemUseHandlers.ResourceExtractor.HandleActivate(oItem, oPC.ControllingPlayer.LoginCreature, oTarget);

          break;
        case "private_contract":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;
          new PrivateContract(oPC.ControllingPlayer.LoginCreature, oItem);

          break;
        case "shop_clearance":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;
          new PlayerShop(oPC.ControllingPlayer.LoginCreature, oItem);

          break;
        case "auction_clearanc":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;
          new PlayerAuction(oPC.ControllingPlayer.LoginCreature, oItem);

          break;
        case "forgehammer":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;

          if (oTarget is NwItem)
            new CraftTool(oPC.ControllingPlayer.LoginCreature, (NwItem)oTarget);
          else
            oPC.ControllingPlayer.SendServerMessage($"Vous ne pouvez pas modifier l'apparence de {oTarget.Name.ColorString(Color.WHITE)}.".ColorString(Color.RED));

          break;
        case "Peaudejoueur":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;
          CreaturePlugin.RunEquip(oPC, onItemUse.Item, (int)InventorySlot.CreatureSkin);          
          break;
        case "potion_cure_mini":
            new PotionCureMini(oPC.ControllingPlayer);
          break;
        case "potion_cure_frog":
          new PotionCureFrog(oPC.ControllingPlayer);
          break;
      }

      Task wait = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
      });
    }
    public static void OnAcquireItem(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwCreature oPC = (NwCreature)onAcquireItem.AcquiredBy;
      //Console.WriteLine(NWScript.GetName(oPC));

      NwItem oItem = onAcquireItem.Item;
      NwGameObject oAcquiredFrom = onAcquireItem.AcquiredFrom;

      if (oPC.ControllingPlayer == null || oItem == null)
        return;

      //Console.WriteLine(NWScript.GetTag(oItem));

      if (oItem.GetLocalVariable<int>("_MAX_DURABILITY").HasNothing)
      {
        int durability = ItemUtils.GetBaseItemCost(oItem) * 25;
        oItem.GetLocalVariable<int>("_MAX_DURABILITY").Value = durability;
        oItem.GetLocalVariable<int>("_DURABILITY").Value = durability;
      }

      if (oItem.Tag == "undroppable_item")
      {
        oItem.Clone(oAcquiredFrom);
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
    public static void OnUnacquireItem(ModuleEvents.OnUnacquireItem onUnacquireItem)
    {
      NwItem oItem = onUnacquireItem.Item;

      if (onUnacquireItem.LostBy.ControllingPlayer == null || oItem == null)
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
      oPC.LoginCreature.ApplyEffect(EffectDuration.Permanent, eff);

      //await (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{oPC.Name} ne dispose pas d'amulette de traçage, ce qui le rend suspect aux yeux de l'Amiral.");

      await NwTask.WaitUntil(() => oPC.LoginCreature == null || oPC.LoginCreature.GetItemInSlot(InventorySlot.Neck)?.Tag == "amulettorillink");

      if (oPC.LoginCreature == null)
        return;

      OnTorilNecklaceEquipped(oPC);
    }
    public static async void OnTorilNecklaceEquipped(NwPlayer oPC)
    {
      oPC.SendServerMessage("Votre lien avec la Toile se renforce de manière significative.", API.Color.PINK);

      if (oPC.LoginCreature.ActiveEffects.Any(e => e.Tag == "erylies_spell_failure"))
        oPC.LoginCreature.RemoveEffect(oPC.LoginCreature.ActiveEffects.Where(e => e.Tag == "erylies_spell_failure").FirstOrDefault());

      await NwTask.WaitUntil(() => oPC.LoginCreature == null || oPC.LoginCreature.GetItemInSlot(InventorySlot.Neck)?.Tag != "amulettorillink");
      
      if (oPC.LoginCreature == null)
        return;

      OnTorilNecklaceRemoved(oPC);
    }
    public static void NoEquipRuinedItem(OnItemValidateEquip onItemValidateEquip)
    {
      if (onItemValidateEquip.Item.GetLocalVariable<int>("_DURABILITY") <= 0)
      {
        onItemValidateEquip.Result = EquipValidationResult.Denied;
        onItemValidateEquip.UsedBy.ControllingPlayer.SendServerMessage($"{onItemValidateEquip.Item.Name} nécessite des réparations.", Color.RED);
      }
    }
    public static void NoUseRuinedItem(OnItemValidateUse onItemValidateUse)
    {
      if (onItemValidateUse.Item.GetLocalVariable<int>("_DURABILITY") <= 0)
        onItemValidateUse.CanUse = false;
    }
  }
}
