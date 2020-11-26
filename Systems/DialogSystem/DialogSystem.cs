using System;
using System.Collections.Generic;
using System.Text;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Blueprint;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class DialogSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { "diag_root", HandleDialogStart },
            { "intro_start", HandleIntroStart },
            { "intro_validation", HandleIntroValidation },
        };

    private static int HandleDialogStart(uint oidSelf)
    {
      uint oPC = NWScript.GetLastSpeaker();

      if(!Convert.ToBoolean(NWScript.GetIsObjectValid(oPC)))
        oPC = NWScript.GetLastUsedBy();

      Player player;
      if (Players.TryGetValue(oPC, out player))
      {
        uint shop;

        switch (NWScript.GetTag(oidSelf))
        {
          case "petitepatate":
            new Bank(player);
            break;
          case "micropatate":
            new Storage(player);
            break;
          case "intro_mirror":
            new IntroMirror(player);
            break;
            case "blueprintbank":
            shop = NWScript.GetNearestObjectByTag("skillbank_shop", oidSelf);

            if (!Convert.ToBoolean(NWScript.GetIsObjectValid(shop)))
            {
              shop = NWScript.CreateObject(NWScript.OBJECT_TYPE_STORE, "generic_shop_res", NWScript.GetLocation(oidSelf), 0, "skillbank_shop");
              NWScript.SetLocalObject(shop, "_STORE_NPC", oidSelf);

              foreach (int baseItemType in CollectSystem.forgeBasicBlueprints)
              {
                Blueprint blueprint = new Blueprint(baseItemType);

                if (!CollectSystem.blueprintDictionnary.ContainsKey(baseItemType))
                  CollectSystem.blueprintDictionnary.Add(baseItemType, blueprint);

                uint oBlueprint = NWScript.CreateItemOnObject("blueprintgeneric", shop, 10, "blueprint");
                NWScript.SetName(oBlueprint, $"Patron pour {blueprint.name}");
                NWScript.SetLocalInt(oBlueprint, "_BASE_ITEM_TYPE", baseItemType);
                ItemPlugin.SetBaseGoldPieceValue(oBlueprint, blueprint.goldCost);
              }
            }

            NWScript.OpenStore(shop, player.oid);
            break;
            case "skillbank":
              shop = NWScript.GetNearestObjectByTag("skillbank_shop", oidSelf);

              if (!Convert.ToBoolean(NWScript.GetIsObjectValid(shop)))
              {
                shop = NWScript.CreateObject(NWScript.OBJECT_TYPE_STORE, "generic_shop_res", NWScript.GetLocation(oidSelf), 0, "skillbank_shop");

                foreach (Feat feat in SkillSystem.languageSkillBooks)
                {
                  uint skillBook = NWScript.CreateItemOnObject("skillbookgeneriq", shop, 10, "skillbook");
                  ItemPlugin.SetItemAppearance(skillBook, NWScript.ITEM_APPR_TYPE_SIMPLE_MODEL, 2, Utils.random.Next(0, 50));
                  NWScript.SetLocalInt(skillBook, "_SKILL_ID", (int)feat);

                  int value;
                  if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", (int)feat), out value))
                    NWScript.SetName(skillBook, NWScript.GetStringByStrRef(value));

                  if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", (int)feat), out value))
                    NWScript.SetDescription(skillBook, NWScript.GetStringByStrRef(value));

                  if (int.TryParse(NWScript.Get2DAString("feat", "CRValue", (int)feat), out value))
                    ItemPlugin.SetBaseGoldPieceValue(skillBook, value * 1000);
                }
              }

              NWScript.OpenStore(shop, player.oid);
              break;
        }
      }
        
      return 0;
    }
    private static int HandleIntroStart(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(NWScript.GetLastSpeaker(), out player))
      {
        Area area; 
        if (Module.areaDictionnary.TryGetValue(NWScript.GetObjectUUID(NWScript.GetArea(player.oid)), out area))
          area.StartEntryScene(player);
      }

      return 0;
    }
    private static int HandleIntroValidation(uint oidSelf)
    {
      uint oPC = NWScript.GetLastSpeaker();

      if (ObjectPlugin.GetInt(oPC, "_STARTING_SKILL_POINTS") > 0)
      {
        NWScript.SendMessageToPC(oPC, $"Il vous reste encore {ObjectPlugin.GetInt(oPC, "_STARTING_SKILL_POINTS")} points de compétences à dépenser auprès du reflet avant de pouvoir débarquer !");
        return 0;
      }
      else
      {
        NWScript.SetLocalInt(oidSelf, "_GO", 1);
        return 1;
      }
    }
  }
}
