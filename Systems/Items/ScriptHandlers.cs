using System;
using System.Collections.Generic;
using Discord;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Items
{
  public static class ScriptHandlers
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
            { "event_equip_items_before", HandleBeforeEquipItem },
            { "event_unequip_items_before", HandleBeforeUnequipItem },
            { "event_validate_equip_items_before", HandleBeforeValidatingEquipItem},
            { "event_use_item_before", HandleBeforeUseItem},
    }; 
    private static int HandleBeforeEquipItem(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM"));
        int iSlot = int.Parse(EventsPlugin.GetEventData("SLOT"));
        var oUnequip = NWScript.GetItemInSlot(iSlot, player.oid);

        if (NWScript.GetIsObjectValid(oUnequip) == 1 && ObjectPlugin.CheckFit(player.oid, NWScript.GetBaseItemType(oUnequip)) == 0)
        {
          NWScript.SendMessageToPC(player.oid, "Attention, votre inventaire est plein. Déséquipper cet objet risquerait de vous le faire perdre !");
          EventsPlugin.SkipEvent();
          return 0;
        }

        if (NWScript.GetTag(oItem) == "amulettorillink")
          NWN.Utils.RemoveTaggedEffect(oidSelf, "erylies_spell_failure");
        else if (NWScript.GetTag(oUnequip) == "amulettorillink")
          NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, NWScript.TagEffect(NWScript.SupernaturalEffect(NWScript.EffectSpellFailure(50)), "erylies_spell_failure"), player.oid);

      }
      return 0;
    }
    private static int HandleBeforeUnequipItem(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM"));
        if (ObjectPlugin.CheckFit(player.oid, NWScript.GetBaseItemType(oItem)) == 0)
        {
          NWScript.SendMessageToPC(player.oid, "Attention, votre inventaire est plein. Déséquipper cet objet risquerait de vous le faire perdre !");
          EventsPlugin.SkipEvent();
          return 0;
        }

        if (NWScript.GetTag(oItem) == "amulettorillink")
        {
          NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, NWScript.TagEffect(NWScript.SupernaturalEffect(NWScript.EffectSpellFailure(50)), "erylies_spell_failure"), player.oid);
          (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{Bot._client.GetGuild(680072044364562528).EveryoneRole.Mention} {NWScript.GetName(player.oid)} vient d'ôter son amulette de traçage. L'Amiral surveille désormais directement ses activités en rp.");
        }
      }
      return 0;
    }
    private static int HandleBeforeValidatingEquipItem(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM_OBJECT_ID"));
        string itemTag = NWScript.GetTag(oItem);

        if (itemTag.Contains("_IT_SP"))
        {
          EventsPlugin.SetEventResult("1");
          EventsPlugin.SkipEvent();
          return 0;
        }
        
        switch (itemTag)
        {
          case "extracteur":
            int value;
            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.StripMiner)), out value)) 
            {
              int itemLevel = NWScript.GetLocalInt(oItem, "_ITEM_LEVEL");
              
              if (itemLevel > value)
              {
                EventsPlugin.SetEventResult("0");
                EventsPlugin.SkipEvent();

                if(EventsPlugin.GetCurrentEvent() == "NWNX_ON_VALIDATE_ITEM_EQUIP_BEFORE")
                  NWScript.SendMessageToPC(player.oid, $"Le niveau {itemLevel} de maîtrise des extracteur de roche est requis pour pouvoir utiliser cet outil.");
              }          
            }
            else
            {
              EventsPlugin.SetEventResult("0");
              EventsPlugin.SkipEvent();

              if (EventsPlugin.GetCurrentEvent() == "NWNX_ON_VALIDATE_ITEM_EQUIP_BEFORE")
                NWScript.SendMessageToPC(player.oid, $"Le don maîtrise des extracteur de roche est requis pour pouvoir utiliser cet outil.");
            }
              break;
        }
      }
      return 0;
    }
    private static int HandleBeforeUseItem(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM_OBJECT_ID"));
        uint oTarget;

        switch (NWScript.GetTag(oItem))
        {
          case "skillbook":
            FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player.oid);
            EventsPlugin.SkipEvent();
            BeforeUseHandlers.SkillBook.HandleActivate(oItem, player);
            NWScript.DelayCommand(0.2f, () => FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player.oid));
            break;
          case "blueprint":
            FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player.oid);
            EventsPlugin.SkipEvent();
            oTarget = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID"));
            BeforeUseHandlers.BluePrint.HandleActivate(oItem, player, oTarget);
            NWScript.DelayCommand(0.2f, () => FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player.oid));
            break;
          case "oreextractor":
            FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player.oid);
            EventsPlugin.SkipEvent();
            oTarget = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID"));
            BeforeUseHandlers.OreExtractor.HandleActivate(oItem, player, oTarget);
            NWScript.DelayCommand(0.2f, () => FeedbackPlugin.SetFeedbackMessageHidden(23, 1, player.oid));
            break;
          case "woodextractor":
            oTarget = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID"));
            BeforeUseHandlers.WoodExtractor.HandleActivate(oItem, player, oTarget);
            break;
        }
      }
      return 0;
    }
  }
}
