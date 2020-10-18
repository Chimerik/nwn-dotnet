using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Systems;

namespace NWN.ScriptHandlers
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

      return 1;
    }

    private static int HandleBlockTesterActivate(uint oItem, uint oActivator, uint oTarget)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(oActivator, out player))
      {
        player.BoulderBlock();
      }

      return 1;
    }
    private static int HandleBlueprintActivate(uint oItem, uint oActivator, uint oTarget)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(oActivator, out player))
      {
        var item = oItem;
        CollectSystem.Blueprint blueprint;
        CollectSystem.BlueprintType blueprintType = CollectSystem.GetBlueprintTypeFromName(NWScript.GetName(item));

        if(blueprintType == CollectSystem.BlueprintType.Invalid)
        {
          // TODO : envoyer l'erreur sur Discord
          return 1;
        }

        if (CollectSystem.blueprintDictionnary.ContainsKey(blueprintType))
          blueprint = CollectSystem.blueprintDictionnary[blueprintType];
        else
          blueprint = new CollectSystem.Blueprint(blueprintType);
        
        if(oTarget == NWScript.OBJECT_INVALID)
        {
          // TODO : afficher les valeurs du blueprint level, ressources consommées et temps nécessaire en fonction des compétences de l'utilisateur
        }
        else
        {
          // TODO : vérifier s'il y a déjà un job en cours, si oui, avertir le PJ du risque d'annulation de son job
          // TODO : interdire le changement de job s'il reste moins de 600 s avant la fin

          var target = oTarget;
          if(NWScript.GetTag(target) == blueprint.workshopTag)
          {
            int iMineralCost = blueprint.mineralsCost;
            float iJobDuration = iMineralCost / 50;

            // TODO : réduction du coût et de la durée en fonction de : 1) skill forge 2) skill spécialité item 3) niveau d'amélioration du blueprint
            
            // TODO : vérifier que le joueurs dispose des ressources nécessaires, puis les supprimer
            // Y aura probablement un truc à réfléchir ici pour déterminer le type de ressources en fonction du blueprint

            // TODO : s'il s'agit d'une copie de blueprint, alors le nombre d'utilisation diminue de 1

            ObjectPlugin.SetString(player.oid, "_CURRENT_CRAFT_JOB", NWScript.GetName(item), 1);
            ObjectPlugin.SetFloat(player.oid, "_CURRENT_CRAFT_JOB_REMAINING_TIME", iJobDuration, 1);
            ObjectPlugin.SetString(player.oid, "_CURRENT_CRAFT_JOB_MATERIAL", "Tritanium", 1);
          }
          else if(NWScript.GetObjectType(oTarget) == NWScript.OBJECT_TYPE_ITEM && NWScript.GetNearestObjectByTag(blueprint.workshopTag, oActivator) != NWScript.OBJECT_INVALID)
          {
            // TODO : 
          }
        }
      }

      return 1;
    }
    private static int HandleSkillBookActivate(uint oItem, uint oActivator, uint oTarget)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(oActivator, out player))
      {
        var FeatBook = oItem;
        int FeatId = NWScript.GetLocalInt(FeatBook, "_SKILL_ID");
        if (CreaturePlugin.GetHighestLevelOfFeat(player.oid, FeatId) == 65535) // TODO : faire un enum // Valeur retournée par la fonction si la cible ne possède pas le don
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

      return 1;
    }
  }
}
