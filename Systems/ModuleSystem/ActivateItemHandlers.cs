using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Systems;
using static NWN.Systems.Blueprint;

namespace NWN.Systems
{
  static public class ActivateItemHandlers
  {
    public static Dictionary<string, Func<uint, uint, uint, int>> Register = new Dictionary<string, Func<uint, uint, uint, int>>
    {
            { "MenuTester", HandleMenuTesterActivate },
            { "test_block", HandleBlockTesterActivate },
            { "skillbook", HandleSkillBookActivate },
            { "blueprint", HandleBlueprintActivate },
    };

    private static int HandleMenuTesterActivate(uint oItem, uint oActivator, uint oTarget)
    {
      Console.WriteLine($"You activated the item {NWScript.GetName(oItem)}! {NWScript.GetName(oActivator)}");

      return 0;
    }

    private static int HandleBlockTesterActivate(uint oItem, uint oActivator, uint oTarget)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(oActivator, out player))
      {
        player.BoulderBlock();
      }

      return 0;
    }
    private static int HandleBlueprintActivate(uint oItem, uint oActivator, uint oTarget)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(oActivator, out player))
      {
        Blueprint blueprint = InitializeBlueprint(oItem);

        if(blueprint.type != BlueprintType.Invalid)
        {
          if (oTarget == NWScript.OBJECT_INVALID)
            blueprint.DisplayBlueprintInfo(player, oItem);
          else
          {
            if (player.craftJob.CanStartJob(oActivator, oItem))
            {
              if (NWScript.GetTag(oTarget) == blueprint.craftedItemTag)
              {
                if (NWScript.GetNearestObjectByTag(blueprint.workshopTag, oActivator) != NWScript.OBJECT_INVALID)
                {
                  string sMaterial = blueprint.GetMaterialFromTargetItem(oTarget);
                  CollectSystem.MineralType mineralType = CollectSystem.GetMineralTypeFromName(sMaterial);

                  if (mineralType == CollectSystem.MineralType.Invalid)
                    player.craftJob.Start(CraftJob.JobType.Item, blueprint, player, oItem, oTarget, sMaterial, mineralType);
                  else
                    NWScript.SendMessageToPC(oActivator, "Cet objet ne peut pas être amélioré.");
                }
                else
                  NWScript.SendMessageToPC(oActivator, $"Vous devez être à proximité d'un atelier de type {blueprint.workshopTag} pour commencer ce travail");
              }
              else
                NWScript.SendMessageToPC(oActivator, "Ce patron ne permet pas d'améliorer ce type d'objet");
            }
          }
        }
        else
        {
          NWScript.SendMessageToPC(oActivator, "[ERREUR HRP] - Ce patron n'est pas correctement initialisé. Le staff a été informé.");
          Utils.LogMessageToDMs($"Invalid blueprint : {NWScript.GetName(oItem)} used by {NWScript.GetName(oActivator)}");
        }
      }

      return 0;
    }
    
    private static int HandleSkillBookActivate(uint oItem, uint oActivator, uint oTarget)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(oActivator, out player))
      {
        var FeatBook = oItem;
        int FeatId = NWScript.GetLocalInt(FeatBook, "_SKILL_ID");
        if (CreaturePlugin.GetHighestLevelOfFeat(player.oid, FeatId) == (int)Feat.Invalid) 
        {
          SkillBook.pipeline.Execute(new SkillBook.Context(
          oItem: FeatBook,
          oActivator: player,
          SkillId: FeatId
        ));
        }
        else
          NWScript.SendMessageToPC(player.oid, "Vous connaissez déjà les bases d'entrainement de cette capacité");
      }

      return 0;
    }
  }
}
