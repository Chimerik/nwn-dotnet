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
      NwPlayer oPC = (NwPlayer)onItemEquip.EquippedBy;
      NwItem oItem = onItemEquip.Item;

      if (oPC == null || oItem == null)
        return;

      NwItem oUnequip = oPC.GetItemInSlot(onItemEquip.Slot);
      
      if (oUnequip != null && !oPC.Inventory.CheckFit(oUnequip))
      {
        oPC.SendServerMessage($"Attention, votre inventaire est plein. Vous risqueriez de perdre votre {oUnequip.Name} en déséquipant !", Color.RED);
        onItemEquip.PreventEquip = true;
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


      if (oPC.Inventory.CheckFit(oItem))
        return;

      if (oPC.GetLocalVariable<int>("CUSTOM_EFFECT_NOARMOR").HasValue
        || oPC.GetLocalVariable<int>("CUSTOM_EFFECT_NOWEAPON").HasValue
        || oPC.GetLocalVariable<int>("CUSTOM_EFFECT_NOACCESSORY").HasValue)
      {
        oPC.SendServerMessage($"Attention, votre inventaire est plein. Votre {oItem.Name} a été déposé au sol !", Color.RED);
        oItem.Clone(oPC.Location);
        oItem.Destroy();
        oPC.GetLocalVariable<int>("CUSTOM_EFFECT_NOARMOR").Delete();
        oPC.GetLocalVariable<int>("CUSTOM_EFFECT_NOWEAPON").Delete();
        oPC.GetLocalVariable<int>("CUSTOM_EFFECT_NOACCESSORY").Delete();
      }
      else
        oPC.SendServerMessage($"Attention, votre inventaire est plein. Vous risqueriez de perdre votre {oItem.Name} en déséquipant !", Color.RED);

      EventsPlugin.SkipEvent();
    }
    public static void OnItemUseBefore(OnItemUse onItemUse)
    {
      NwPlayer oPC = (NwPlayer)onItemUse.UsedBy;

      NwItem oItem = onItemUse.Item;
      NwGameObject oTarget = onItemUse.TargetObject;

      if(oPC == null || oItem == null)
        return;
      
      switch (oItem.Tag)
      {
        case "skillbook":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC);
          onItemUse.PreventUseItem = true;
          Items.ItemUseHandlers.SkillBook.HandleActivate(oItem, oPC);
          break;

        case "blueprint":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC);
          onItemUse.PreventUseItem = true;
          Items.ItemUseHandlers.Blueprint.HandleActivate(oItem, oPC, oTarget);

          break;

        case "oreextractor":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC);
          onItemUse.PreventUseItem = true;
          Items.ItemUseHandlers.ResourceExtractor.HandleActivate(oItem, oPC, oTarget);

          break;
        case "private_contract":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC);
          onItemUse.PreventUseItem = true;
          new PrivateContract(oPC, oItem);

          break;
        case "shop_clearance":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC);
          onItemUse.PreventUseItem = true;
          new PlayerShop(oPC, oItem);

          break;
        case "auction_clearanc":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC);
          onItemUse.PreventUseItem = true;
          new PlayerAuction(oPC, oItem);

          break;
        case "forgehammer":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC);
          onItemUse.PreventUseItem = true;

          if (oTarget is NwItem)
            new CraftTool(oPC, (NwItem)oTarget);
          else
            oPC.SendServerMessage($"Vous ne pouvez pas modifier l'apparence de {oTarget.Name.ColorString(Color.WHITE)}.".ColorString(Color.RED));

          break;
        case "Peaudejoueur":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC);
          onItemUse.PreventUseItem = true;
          CreaturePlugin.RunEquip(oPC, onItemUse.Item, (int)InventorySlot.CreatureSkin);          
          break;
        case "potion_cure_mini":
            new PotionCureMini(oPC);
          break;
        case "potion_cure_frog":
          new PotionCureFrog(oPC);
          break;
      }

      Task wait = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC);
      });
    }
    public static void OnAcquireItem(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwPlayer oPC = (NwPlayer)onAcquireItem.AcquiredBy;
      //Console.WriteLine(NWScript.GetName(oPC));

      NwItem oItem = onAcquireItem.Item;
      NwGameObject oAcquiredFrom = onAcquireItem.AcquiredFrom;

      if (oPC == null || oItem == null)
        return;

      //Console.WriteLine(NWScript.GetTag(oItem));

      if (oItem.GetLocalVariable<int>("_DURABILITY").HasNothing)
        oItem.GetLocalVariable<int>("_DURABILITY").Value = ItemUtils.GetItemMaxDurability(oItem) * 25;

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
  }
}
