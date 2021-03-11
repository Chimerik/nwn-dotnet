using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class CraftTool
  {
    Player player;
    NwItem item;
    int LocationTypeColorChoice;
    ItemAppearanceArmorColor? colorChannelChoice;
    ItemAppearanceArmorModel? armorPartChoice;

    public CraftTool(Player player, NwItem item)
    {
      if (item.Possessor != player.oid) // TODO : vérifier qu'il est bien le crafteur de l'objet
      {
        player.oid.SendServerMessage($"Vous devez être en position de l'objet {item.Name.ColorString(Color.LIME)} pour pouvoir le modifier", Color.ORANGE);
        return;
      }

      this.player = player;
      this.item = item;

      switch (item.BaseItemType)
      {
        case BaseItemType.Armor:
          DrawArmorModificationMenu();
          break;
        case BaseItemType.Bastardsword:
        case BaseItemType.Battleaxe:
        case BaseItemType.Club:
        case BaseItemType.Dagger:
        case BaseItemType.DireMace:
        case BaseItemType.Doubleaxe:
        case BaseItemType.DwarvenWaraxe:
        case BaseItemType.Greataxe:
        case BaseItemType.Greatsword:
        case BaseItemType.Halberd:
        case BaseItemType.Handaxe:
        case BaseItemType.HeavyCrossbow:
        case BaseItemType.HeavyFlail:
        case BaseItemType.Kama:
        case BaseItemType.Katana:
        case BaseItemType.Kukri:
        case BaseItemType.LightCrossbow:
        case BaseItemType.LightFlail:
        case BaseItemType.LightHammer:
        case BaseItemType.LightMace:
        case BaseItemType.Longbow:
        case BaseItemType.Longsword:
        case BaseItemType.MagicStaff:
        case BaseItemType.Morningstar:
        case BaseItemType.Quarterstaff:
        case BaseItemType.Rapier:
        case BaseItemType.Scimitar:
        case BaseItemType.Scythe:
        case BaseItemType.Shortbow:
        case BaseItemType.ShortSpear:
        case BaseItemType.Shortsword:
        case BaseItemType.Sickle:
        case BaseItemType.Sling:
        case BaseItemType.ThrowingAxe:
        case BaseItemType.Trident:
        case BaseItemType.TwoBladedSword:
        case BaseItemType.Warhammer:
        case BaseItemType.Whip:
          break;
        case BaseItemType.Amulet:
        case BaseItemType.Arrow:
        case BaseItemType.Belt:
        case BaseItemType.Bolt:
        case BaseItemType.Book:
        case BaseItemType.Boots:
        case BaseItemType.Bracer:
        case BaseItemType.Bullet:
        case BaseItemType.Cloak:
        case BaseItemType.EnchantedPotion:
        case BaseItemType.EnchantedScroll:
        case BaseItemType.EnchantedWand:
        case BaseItemType.Gloves:
        case BaseItemType.Grenade:
        case BaseItemType.Helmet:
        case BaseItemType.LargeShield:
        case BaseItemType.MagicRod:
        case BaseItemType.MagicWand:
        case BaseItemType.Potions:
        case BaseItemType.Ring:
        case BaseItemType.Scroll:
        case BaseItemType.Shuriken:
        case BaseItemType.SmallShield:
        case BaseItemType.SpellScroll:
        case BaseItemType.TowerShield:
        case BaseItemType.TrapKit:
          break;
      }
    }
    private void DrawArmorModificationMenu()
    {
      LocationTypeColorChoice = 0;
      armorPartChoice = null;
      colorChannelChoice = null;

      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Quel type de modification souhaitez-vous effectuer sur votre armure {item.Name.ColorString(Color.GREEN)} ?"
      };

      player.menu.choices.Add(($"Modifier l'apparence.".ColorString(Color.ORANGE), () => HandleArmorPartChoice()));
      player.menu.choices.Add(($"Modifier les couleurs.".ColorString(Color.PINK), () => HandleColorLocationChoice()));
      
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleArmorPartChoice()
    {
      armorPartChoice = null;

      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Quelle partie de l'armure souhaitez-vous modifier ?"
      };

      player.menu.choices.Add(($"Robe", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.Robe)));
      player.menu.choices.Add(($"Cou", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.Neck)));
      player.menu.choices.Add(($"Torse", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.Torso)));
      player.menu.choices.Add(($"Pelvis", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.Pelvis)));
      player.menu.choices.Add(($"Ceinture", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.Belt)));
      player.menu.choices.Add(($"Epaule gauche", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.LeftShoulder)));
      player.menu.choices.Add(($"Epaule droite", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.RightShoulder)));
      player.menu.choices.Add(($"Biceps gauche.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.LeftBicep)));
      player.menu.choices.Add(($"Biceps droit.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.RightBicep)));
      player.menu.choices.Add(($"Avant-bras gauche.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.LeftForearm)));
      player.menu.choices.Add(($"Avant-bras droit.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.RightForearm)));
      player.menu.choices.Add(($"Main gauche.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.LeftHand)));
      player.menu.choices.Add(($"Main droite.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.RightHand)));
      player.menu.choices.Add(($"Cuisse gauche.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.LeftThigh)));
      player.menu.choices.Add(($"Cuisse droite.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.RightThigh)));
      player.menu.choices.Add(($"Cuisse gauche.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.LeftShin)));
      player.menu.choices.Add(($"Cuisse droite.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.RightShin)));
      player.menu.choices.Add(($"Cuisse gauche.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.LeftFoot)));
      player.menu.choices.Add(($"Cuisse droite.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.RightFoot)));

      player.menu.choices.Add(("Retour.", () => DrawArmorModificationMenu()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private void ValidateArmorPartChoice(ItemAppearanceArmorModel choice)
    {
      armorPartChoice = choice;
      ApplyArmorModifications(0);
    }
    private void HandleColorLocationChoice()
    {
      LocationTypeColorChoice = 0;
      colorChannelChoice = null;

      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"S'agit-il d'une modification de couleur globale ou localisée ?"
      };

      player.menu.choices.Add(($"Globale.".ColorString(Color.ORANGE), () => ValidateGlobalColorChoice()));
      player.menu.choices.Add(($"Localisée.".ColorString(Color.PINK), () => ValidateLocalColorChoice()));

      player.menu.choices.Add(("Retour.", () => DrawArmorModificationMenu()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));
      player.menu.Draw();
    }

    private void ValidateGlobalColorChoice()
    {
      LocationTypeColorChoice = 1;
      HandleColorChannelChoice();
    }
    private void ValidateLocalColorChoice()
    {
      LocationTypeColorChoice = 2;
      HandleColorChannelChoice();
    }

    private void HandleColorChannelChoice()
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Quel canal de couleur souhaitez-vous modifier ?"
      };
      
      player.menu.choices.Add(($"Tissu 1", () => ValidateColorChannelChoice(ItemAppearanceArmorColor.Cloth1)));
      player.menu.choices.Add(($"Tissu 2.", () => ValidateColorChannelChoice(ItemAppearanceArmorColor.Cloth2)));
      player.menu.choices.Add(($"Cuir 1", () => ValidateColorChannelChoice(ItemAppearanceArmorColor.Leather1)));
      player.menu.choices.Add(($"Cuir 2.", () => ValidateColorChannelChoice(ItemAppearanceArmorColor.Leather2)));
      player.menu.choices.Add(($"Metal 1", () => ValidateColorChannelChoice(ItemAppearanceArmorColor.Metal1)));
      player.menu.choices.Add(($"Metal 2.", () => ValidateColorChannelChoice(ItemAppearanceArmorColor.Metal2)));

      player.menu.choices.Add(("Retour.", () => DrawArmorModificationMenu()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }

    private void ValidateColorChannelChoice(ItemAppearanceArmorColor choice)
    {
      colorChannelChoice = choice;
      if(LocationTypeColorChoice == 2)
        HandleArmorPartChoice();
      else
        ApplyArmorModifications(0);
    }

    private void ApplyArmorModifications(int modification)
    {
      player.menu.Clear();

      if(item == null || item.Possessor != player.oid)
      {
        player.oid.SendServerMessage($"L'objet que vous essayez de modifier n'existe plus ou n'est plus en votre possession.", Color.RED);
        player.menu.Close();
        return;
      }

      if (LocationTypeColorChoice > 0)
      {
        player.menu.titleLines = new List<string> {
        "Faites défiler les couleurs à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur de couleur à l'oral (entre 0 et 64)"
        };

        if(LocationTypeColorChoice == 1)
        {
          byte currentValue = item.Appearance.GetArmorColor((ItemAppearanceArmorColor)colorChannelChoice);

          if (modification != 0)
          {
            FeedbackPlugin.SetFeedbackMessageHidden(50, 1, player.oid);
            FeedbackPlugin.SetFeedbackMessageHidden(51, 1, player.oid);

            if (modification == 1)
            {
              currentValue++;
              if (currentValue > 64)
                currentValue = 0;
            }
            else if (modification == -1)
            {
              currentValue--;
              if (currentValue > 64)
                currentValue = 64;
            }

            item.Appearance.SetArmorColor((ItemAppearanceArmorColor)colorChannelChoice, currentValue);
            NwItem newItem = item.Clone(player.oid, "", true);
            player.oid.ActionEquipItem(newItem, InventorySlot.Chest);
            item.Destroy();
            item = newItem;

            Task waitDestruction = NwTask.Run(async () =>
            {
              await NwTask.Delay(TimeSpan.FromSeconds(0.4));
              FeedbackPlugin.SetFeedbackMessageHidden(50, 0, player.oid);
              FeedbackPlugin.SetFeedbackMessageHidden(51, 0, player.oid);
            });
          }

          player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        }
        else
        {
          byte currentValue = item.Appearance.GetArmorPieceColor((ItemAppearanceArmorModel)armorPartChoice, (ItemAppearanceArmorColor)colorChannelChoice);

          FeedbackPlugin.SetFeedbackMessageHidden(50, 1, player.oid);
          FeedbackPlugin.SetFeedbackMessageHidden(51, 1, player.oid);

          if (modification != 0)
          {
            if (modification == 1)
            {
              currentValue++;
              if (currentValue > 64)
                currentValue = 0;
            }
            else if (modification == -1)
            {
              currentValue--;
              if (currentValue > 64)
                currentValue = 64;
            }

            item.Appearance.SetArmorPieceColor((ItemAppearanceArmorModel)armorPartChoice, (ItemAppearanceArmorColor)colorChannelChoice, currentValue);
            NwItem newItem = item.Clone(player.oid, "", true);
            player.oid.ActionEquipItem(newItem, InventorySlot.Chest);
            item.Destroy();
            item = newItem;

            Task waitDestruction = NwTask.Run(async () =>
            {
              await NwTask.Delay(TimeSpan.FromSeconds(0.4));
              FeedbackPlugin.SetFeedbackMessageHidden(50, 0, player.oid);
              FeedbackPlugin.SetFeedbackMessageHidden(51, 0, player.oid);
            });
          }

          player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        }
      }
      else
      {
        player.menu.titleLines = new List<string> {
        "Faites défiler les apparences à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur d'apparence à l'oral (entre 0 et 255)"
        };

        if((ItemAppearanceArmorModel)armorPartChoice == ItemAppearanceArmorModel.Torso)
        {
          byte currentValue = item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Torso);

          if (modification != 0)
          {
            FeedbackPlugin.SetFeedbackMessageHidden(50, 1, player.oid);
            FeedbackPlugin.SetFeedbackMessageHidden(51, 1, player.oid);

            if (modification == 1)
              currentValue++;
            else if (modification == -1)
              currentValue--;

            int currentAC = ItemPlugin.GetBaseArmorClass(item);

            //item.Appearance.SetArmorModel(ItemAppearanceArmorModel.Torso, currentValue);
            //NwItem newItem = item.Clone(player.oid, "", true);
            NwItem newItem = NWScript.CopyItemAndModify(item, NWScript.ITEM_APPR_TYPE_ARMOR_MODEL, NWScript.ITEM_APPR_ARMOR_MODEL_TORSO, currentValue, 1).ToNwObject<NwItem>();

            Log.Info($"current AC : {currentAC}");
            Log.Info($"new AC : {ItemPlugin.GetBaseArmorClass(newItem)}");

            while (currentAC != ItemPlugin.GetBaseArmorClass(newItem))
            {
              newItem.Destroy();

              if (modification == 1)
                currentValue++;
              else if (modification == -1)
                currentValue--;

              if (currentValue > 255)
                currentValue = 0;

              newItem = newItem = NWScript.CopyItemAndModify(item, NWScript.ITEM_APPR_TYPE_ARMOR_MODEL, NWScript.ITEM_APPR_ARMOR_MODEL_TORSO, currentValue, 1).ToNwObject<NwItem>();
            }

            item.Destroy(0.2f);
            item = newItem;

            if (player.oid.Inventory.CheckFit(newItem))
              player.oid.ActionEquipItem(newItem, InventorySlot.Chest);
            else
            {
              newItem.Location = player.oid.Location;

              Task delayedEquip = NwTask.Run(async () =>
              {
                await NwTask.Delay(TimeSpan.FromSeconds(0.2));
                player.oid.AcquireItem(newItem);
                await player.oid.ActionEquipItem(newItem, InventorySlot.Chest);
              });
            }

            Task waitDestruction = NwTask.Run(async () =>
            {
              await NwTask.Delay(TimeSpan.FromSeconds(0.4));
              FeedbackPlugin.SetFeedbackMessageHidden(50, 0, player.oid);
              FeedbackPlugin.SetFeedbackMessageHidden(51, 0, player.oid);
            });
          }

          player.menu.titleLines.Add($"Apparence actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        }
        else
        {
          byte currentValue = item.Appearance.GetArmorModel((ItemAppearanceArmorModel)armorPartChoice);

          FeedbackPlugin.SetFeedbackMessageHidden(50, 1, player.oid);
          FeedbackPlugin.SetFeedbackMessageHidden(51, 1, player.oid);

          if (modification != 0)
          {
            if (modification == 1)
              currentValue++;
            else if (modification == -1)
              currentValue--;

            item.Appearance.SetArmorModel((ItemAppearanceArmorModel)armorPartChoice, currentValue);
            NwItem newItem = item.Clone(player.oid, "", true);
            player.oid.ActionEquipItem(newItem, InventorySlot.Chest);
            item.Destroy();
            item = newItem;
          }

          Task waitDestruction = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.4));
            FeedbackPlugin.SetFeedbackMessageHidden(50, 0, player.oid);
            FeedbackPlugin.SetFeedbackMessageHidden(51, 0, player.oid);
          });

          player.menu.titleLines.Add($"Apparence actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        }
      }

      player.menu.choices.Add(($"Suivant", () => ApplyArmorModifications(1)));
      player.menu.choices.Add(($"Précédent.", () => ApplyArmorModifications(-1)));

      player.menu.choices.Add(("Retour.", () => DrawArmorModificationMenu()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
  }
}
