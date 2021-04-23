using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWNX.API;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;

namespace NWN.Systems
{
  class CraftTool
  {
    PlayerSystem.Player player;
    NwItem item;
    int LocationTypeColorChoice;
    ItemAppearanceArmorColor? colorChannelChoice;
    ItemAppearanceArmorModel? armorPartChoice;
    ItemAppearanceWeaponModel? weaponPartChoice;
    ItemAppearanceWeaponColor? weaponColorChoice;
    string serializedInitialItem;
    string file;
    List<ItemAppearanceArmorColor> colorChannelList;

    public CraftTool(PlayerSystem.Player player, NwItem item)
    {
      if (item.Possessor != player.oid) // TODO : vérifier qu'il est bien le crafteur de l'objet
      {
        player.oid.SendServerMessage($"Vous devez être en possession de l'objet {item.Name.ColorString(Color.LIME)} pour pouvoir le modifier", Color.ORANGE);
        return;
      }

      // TODO : ajouter un métier permettant de modifier n'importe quelle tenue
      if (item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").HasValue && item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").Value != player.oid.Name 
        && !player.oid.IsDM)
      {
        player.oid.SendServerMessage($"Il est indiqué : Pour tout modification, s'adresser à {item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").Value.ColorString(Color.WHITE)}", Color.ORANGE);
        return;
      }

      this.player = player;
      this.item = item;
      this.serializedInitialItem = item.Serialize().ToBase64EncodedString();

      colorChannelList = new List<ItemAppearanceArmorColor>{ ItemAppearanceArmorColor.Cloth1, ItemAppearanceArmorColor.Cloth2, ItemAppearanceArmorColor.Leather1, ItemAppearanceArmorColor.Leather2, ItemAppearanceArmorColor.Metal1, ItemAppearanceArmorColor.Metal2 };

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
          DrawWeaponModificationMenu();
          break;
        case BaseItemType.Amulet:
        case BaseItemType.Arrow:
        case BaseItemType.Belt:
        case BaseItemType.Bolt:
        case BaseItemType.Book:
        case BaseItemType.Boots:
        case BaseItemType.Bracer:
        case BaseItemType.Bullet:
        case BaseItemType.EnchantedPotion:
        case BaseItemType.EnchantedScroll:
        case BaseItemType.EnchantedWand:
        case BaseItemType.Gloves:
        case BaseItemType.Grenade:
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
          DrawSimpleModificationMenu();
          break;
        case BaseItemType.Helmet:
        case BaseItemType.Cloak:
          DrawHelmetCloakModificationMenu();
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
      player.menu.choices.Add(($"Modifier le nom.".ColorString(Color.ORANGE), () => GetNewName()));
      player.menu.choices.Add(($"Modifier la description.".ColorString(Color.PINK), () => GetNewDescription()));
      player.menu.choices.Add(($"Annuler toutes les modifications en cours.".ColorString(Color.ORANGE), () => HandleReinitialisation()));

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

      player.menu.choices.Add(($"Robe", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.Robe, "parts_robe")));
      player.menu.choices.Add(($"Cou", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.Neck, "parts_neck")));
      player.menu.choices.Add(($"Torse", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.Torso, "parts_chest")));
      player.menu.choices.Add(($"Pelvis", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.Pelvis, "parts_pelvis")));
      player.menu.choices.Add(($"Ceinture", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.Belt, "parts_belt")));
      player.menu.choices.Add(($"Epaule gauche", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.LeftShoulder, "parts_shoulder")));
      player.menu.choices.Add(($"Epaule droite", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.RightShoulder, "parts_shoulder")));
      player.menu.choices.Add(($"Biceps gauche.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.LeftBicep, "parts_bicep")));
      player.menu.choices.Add(($"Biceps droit.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.RightBicep, "parts_bicep")));
      player.menu.choices.Add(($"Avant-bras gauche.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.LeftForearm, "parts_forearm")));
      player.menu.choices.Add(($"Avant-bras droit.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.RightForearm, "parts_forearm")));
      player.menu.choices.Add(($"Main gauche.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.LeftHand, "parts_hand")));
      player.menu.choices.Add(($"Main droite.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.RightHand, "parts_hand")));
      player.menu.choices.Add(($"Cuisse gauche.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.LeftThigh, "parts_legs")));
      player.menu.choices.Add(($"Cuisse droite.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.RightThigh, "parts_legs")));
      player.menu.choices.Add(($"Tibia gauche.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.LeftShin, "parts_shin")));
      player.menu.choices.Add(($"Tibia droit.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.RightShin, "parts_shin")));
      player.menu.choices.Add(($"Pied gauche.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.LeftFoot, "parts_foot")));
      player.menu.choices.Add(($"PIed droit.", () => ValidateArmorPartChoice(ItemAppearanceArmorModel.RightFoot, "parts_foot")));

      player.menu.choices.Add(("Retour.", () => DrawArmorModificationMenu()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private void ValidateArmorPartChoice(ItemAppearanceArmorModel choice, string part)
    {
      armorPartChoice = choice;
      file = part;
      ApplyArmorModifications(-2);
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
      else if (LocationTypeColorChoice == 1)
        ApplyArmorModifications(-2);
      else
        ApplyHelmetCloakModification(-2);
    }

    private void ApplyArmorModifications(int modification)
    {
      if(item == null || item.Possessor != player.oid)
      {
        player.oid.SendServerMessage($"L'objet que vous essayez de modifier n'existe plus ou n'est plus en votre possession.", Color.RED);
        player.menu.Close();
        return;
      }

      if (modification == -2)
        player.menu.Clear();

      byte currentValue = 0;

      if (LocationTypeColorChoice > 0)
      {
        player.menu.titleLines = new List<string> {
        "Faites défiler les couleurs à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur de couleur à l'oral (entre 0 et 64)"
        };

        if(LocationTypeColorChoice == 1)
        {
          currentValue = item.Appearance.GetArmorColor((ItemAppearanceArmorColor)colorChannelChoice);

          if (modification > -2)
          {
            DisableFeedbackMessages();

            if (player.setValue != Config.invalidInput)
            {
              currentValue = (byte)player.setValue;
              player.setValue = Config.invalidInput;
            }
            else if (modification == 1)
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
            NwItem newItem = item.Clone(player.oid);
            player.oid.ActionEquipItem(newItem, InventorySlot.Chest);
            item.Destroy();
            item = newItem;

            Task waitDestruction = NwTask.Run(async () =>
            {
              await NwTask.Delay(TimeSpan.FromSeconds(0.4));
              EnableFeedbackMessages();
            });
          }

          player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        }
        else
        {
          currentValue = item.Appearance.GetArmorPieceColor((ItemAppearanceArmorModel)armorPartChoice, (ItemAppearanceArmorColor)colorChannelChoice);

          DisableFeedbackMessages();

          if (modification > -2)
          {
            if (player.setValue != Config.invalidInput)
            {
              currentValue = (byte)player.setValue;
              player.setValue = Config.invalidInput;
            }
            else if (modification == 1)
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
            NwItem newItem = item.Clone(player.oid);
            player.oid.ActionEquipItem(newItem, InventorySlot.Chest);
            item.Destroy();
            item = newItem;

            Task waitDestruction = NwTask.Run(async () =>
            {
              await NwTask.Delay(TimeSpan.FromSeconds(0.4));
              EnableFeedbackMessages();
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

        if(armorPartChoice == ItemAppearanceArmorModel.Torso)
          HandleTorsoModelModification(modification);
        else
          HandleDefaultModelModification(modification);
      }

      if (modification > -2)
      {
        player.menu.DrawText();
      }
      else
      {
        player.menu.choices.Add(($"Suivant", () => ApplyArmorModifications(1)));
        player.menu.choices.Add(($"Précédent.", () => ApplyArmorModifications(-1)));

        if (armorPartChoice == ItemAppearanceArmorModel.LeftBicep || armorPartChoice == ItemAppearanceArmorModel.LeftFoot
          || armorPartChoice == ItemAppearanceArmorModel.LeftForearm || armorPartChoice == ItemAppearanceArmorModel.LeftHand
          || armorPartChoice == ItemAppearanceArmorModel.LeftShin || armorPartChoice == ItemAppearanceArmorModel.LeftShoulder
          || armorPartChoice == ItemAppearanceArmorModel.LeftThigh || armorPartChoice == ItemAppearanceArmorModel.RightBicep
          || armorPartChoice == ItemAppearanceArmorModel.RightFoot || armorPartChoice == ItemAppearanceArmorModel.RightThigh
          || armorPartChoice == ItemAppearanceArmorModel.RightForearm || armorPartChoice == ItemAppearanceArmorModel.RightHand
          || armorPartChoice == ItemAppearanceArmorModel.RightShin || armorPartChoice == ItemAppearanceArmorModel.RightShoulder)
        {
          player.menu.choices.Add(("Copier vers le côté opposé.", () => HandleToSymmetry()));
          player.menu.choices.Add(("Copier à partir du côté opposé.", () => HandleFromSymmetry()));
        }

        player.menu.choices.Add(("Retour.", () => DrawArmorModificationMenu()));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      Task waitPlayerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        player.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
        ApplyArmorModifications(player.setValue);
        player.setValue = Config.invalidInput;
      });
    }
    private void HandleToSymmetry()
    {
      DisableFeedbackMessages();

      switch(armorPartChoice)
      {
        case ItemAppearanceArmorModel.LeftBicep:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.RightBicep, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftBicep));
          foreach(ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.RightBicep, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.LeftBicep, channel));
          break;
        case ItemAppearanceArmorModel.LeftFoot:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.RightFoot, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftFoot));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.RightFoot, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.LeftFoot, channel));
          break;
        case ItemAppearanceArmorModel.LeftForearm:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.RightForearm, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftForearm));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.RightForearm, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.LeftForearm, channel));
          break;
        case ItemAppearanceArmorModel.LeftHand:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.RightHand, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftHand));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.RightHand, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.LeftHand, channel));
          break;
        case ItemAppearanceArmorModel.LeftShin:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.RightShin, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShin));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.RightShin, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.LeftShin, channel));
          break;
        case ItemAppearanceArmorModel.LeftShoulder:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.RightShoulder, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShoulder));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.RightShoulder, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.LeftShoulder, channel));
          break;
        case ItemAppearanceArmorModel.LeftThigh:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.RightThigh, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftThigh));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.RightThigh, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.LeftThigh, channel));
          break;
        case ItemAppearanceArmorModel.RightBicep:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.LeftBicep, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightBicep));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.LeftBicep, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.RightBicep, channel));
          break;
        case ItemAppearanceArmorModel.RightFoot:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.LeftFoot, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightFoot));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.LeftFoot, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.RightFoot, channel));
          break;
        case ItemAppearanceArmorModel.RightForearm:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.LeftForearm, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightForearm));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.LeftForearm, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.RightForearm, channel));
          break;
        case ItemAppearanceArmorModel.RightHand:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.LeftHand, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightHand));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.LeftHand, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.RightHand, channel));
          break;
        case ItemAppearanceArmorModel.RightShin:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.LeftShin, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShin));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.LeftShin, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.RightShin, channel));
          break;
        case ItemAppearanceArmorModel.RightShoulder:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.LeftShoulder, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShoulder));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.LeftShoulder, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.RightShoulder, channel));
          break;
        case ItemAppearanceArmorModel.RightThigh:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.LeftThigh, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightThigh));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.LeftThigh, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.RightThigh, channel));
          break;
      }

      NwItem newItem = item.Clone(player.oid);
      player.oid.ActionEquipItem(newItem, InventorySlot.Chest);
      item.Destroy();
      item = newItem;

      Task waitDestruction = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.4));
        EnableFeedbackMessages();
      });

      ApplyArmorModifications(-2);
    }
    private void HandleFromSymmetry()
    {
      DisableFeedbackMessages();

      switch (armorPartChoice)
      {
        case ItemAppearanceArmorModel.LeftBicep:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.LeftBicep, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightBicep));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.LeftBicep, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.RightBicep, channel));
          break;
        case ItemAppearanceArmorModel.LeftFoot:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.LeftFoot, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightFoot));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.LeftFoot, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.RightFoot, channel));
          break;
        case ItemAppearanceArmorModel.LeftForearm:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.LeftForearm, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightForearm));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.LeftForearm, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.RightForearm, channel));
          break;
        case ItemAppearanceArmorModel.LeftHand:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.LeftHand, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightHand));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.LeftHand, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.RightHand, channel));
          break;
        case ItemAppearanceArmorModel.LeftShin:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.LeftShin, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShin));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.LeftShin, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.RightShin, channel));
          break;
        case ItemAppearanceArmorModel.LeftShoulder:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.LeftShoulder, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShoulder));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.LeftShoulder, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.RightShoulder, channel));
          break;
        case ItemAppearanceArmorModel.LeftThigh:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.LeftThigh, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightThigh));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.LeftThigh, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.RightThigh, channel));
          break;
        case ItemAppearanceArmorModel.RightBicep:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.RightBicep, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftBicep));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.RightBicep, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.LeftBicep, channel));
          break;
        case ItemAppearanceArmorModel.RightFoot:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.RightFoot, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftFoot));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.RightFoot, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.LeftFoot, channel));
          break;
        case ItemAppearanceArmorModel.RightForearm:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.RightForearm, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftForearm));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.RightForearm, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.LeftForearm, channel));
          break;
        case ItemAppearanceArmorModel.RightHand:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.RightHand, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftHand));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.RightHand, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.LeftHand, channel));
          break;
        case ItemAppearanceArmorModel.RightShin:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.RightShin, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShin));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.RightShin, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.LeftShin, channel));
          break;
        case ItemAppearanceArmorModel.RightShoulder:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.RightShoulder, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShoulder));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.RightShoulder, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.LeftShoulder, channel));
          break;
        case ItemAppearanceArmorModel.RightThigh:
          item.Appearance.SetArmorModel(ItemAppearanceArmorModel.RightThigh, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftThigh));
          foreach (ItemAppearanceArmorColor channel in colorChannelList)
            item.Appearance.SetArmorPieceColor(ItemAppearanceArmorModel.RightThigh, channel, item.Appearance.GetArmorPieceColor(ItemAppearanceArmorModel.LeftThigh, channel));
          break;
      }

      NwItem newItem = item.Clone(player.oid);
      player.oid.ActionEquipItem(newItem, InventorySlot.Chest);
      item.Destroy();
      item = newItem;

      Task waitDestruction = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.4));
        EnableFeedbackMessages();
      });

      ApplyArmorModifications(-2);
    }
    private void HandleTorsoModelModification(int modification)
    {
      byte currentValue = item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Torso);

      if (modification > -2)
      {
        DisableFeedbackMessages();

        if (player.setValue != Config.invalidInput)
        {
          currentValue = (byte)player.setValue;
          player.setValue = Config.invalidInput;
        }
        else if (modification == 1)
          currentValue++;
        else if (modification == -1)
          currentValue--;

        int currentAC = ItemPlugin.GetBaseArmorClass(item);
        int gender = (int)player.oid.Gender;

        while ((!float.TryParse(NWScript.Get2DAString(file, "ACBONUS", currentValue), out float hasModel) || (int)hasModel != currentAC)
          || (float.TryParse(NWScript.Get2DAString(file, "GENDER", currentValue), out float modelGender) && modelGender != gender))
        {
          if (modification == 1)
            currentValue++;
          else if (modification == -1)
            currentValue--;
        }

        item.Appearance.SetArmorModel(ItemAppearanceArmorModel.Torso, currentValue);
        NwItem newItem = item.Clone(player.oid);

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
          EnableFeedbackMessages();
        });
      }

      player.menu.titleLines.Add($"Apparence actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
    }
    private void HandleDefaultModelModification(int modification)
    {
      byte currentValue = item.Appearance.GetArmorModel((ItemAppearanceArmorModel)armorPartChoice);

      if (modification > -2)
      {
        DisableFeedbackMessages();

        if (player.setValue != Config.invalidInput)
        {
          currentValue = (byte)player.setValue;
          player.setValue = Config.invalidInput;
        }
        else if (modification == 1)
          currentValue++;
        else if (modification == -1)
          currentValue--;

        int gender = (int)player.oid.Gender;

        while (!float.TryParse(NWScript.Get2DAString(file, "ACBONUS", currentValue), out float hasModel) 
          || (float.TryParse(NWScript.Get2DAString(file, "GENDER", currentValue), out float modelGender) && modelGender != gender))
        {
          if (modification == 1)
            currentValue++;
          else if (modification == -1)
            currentValue--;
        }

        item.Appearance.SetArmorModel((ItemAppearanceArmorModel)armorPartChoice, currentValue);
        NwItem newItem = item.Clone(player.oid);
        player.oid.ActionEquipItem(newItem, InventorySlot.Chest);

        item.Destroy();
        item = newItem;

        Task waitDestruction = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.4));
          EnableFeedbackMessages();
        });
      }

      player.menu.titleLines.Add($"Apparence actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
    }
    private void DisableFeedbackMessages()
    {
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.ItemReceived, player.oid);
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.ItemLost, player.oid);
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.EquipWeaponSwappedOut, player.oid);
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.EquipSkillSpellModifiers, player.oid);
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.InventoryFull, player.oid);
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedToRun, player.oid);
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedWalkSlow, player.oid);
      ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.SendMessageToPc, player.oid);
    }
    private void EnableFeedbackMessages()
    {
      ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.ItemReceived, player.oid);
      ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.ItemLost, player.oid);
      ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.EquipWeaponSwappedOut, player.oid);
      ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.EquipSkillSpellModifiers, player.oid);
      ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.InventoryFull, player.oid);
      ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedToRun, player.oid);
      ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedWalkSlow, player.oid);
      ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.SendMessageToPc, player.oid);
    }
    private void GetNewName()
    {
      player.menu.titleLines = new List<string>() {
        $"Nom actuel : {item.Name.ColorString(Color.GREEN)}",
        "Veuillez prononcer le nouveau nom à l'oral."
      };

      player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Delete();

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Value = 1;
        player.setString = "";
        await NwTask.WaitUntil(() => player.setString != "");
        item.Name = player.setString;
        player.oid.SendServerMessage($"Votre objet est désormais nommé {player.setString.ColorString(Color.GREEN)}.");
        player.setString = "";
        player.menu.Close();
      });

      player.menu.Draw();
    }
    private void GetNewDescription()
    {
      player.menu.titleLines = new List<string>() {
        "Veuillez prononcer la nouvelle description à l'oral."
      };

      player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Delete();

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Value = 1;
        player.setString = "";
        await NwTask.WaitUntil(() => player.setString != "");
        item.Description = player.setString;
        player.oid.SendServerMessage($"La description de votre objet a été modifiée.", Color.ROSE);
        player.setString = "";
        player.menu.Close();
      });

      player.menu.Draw();
    }
    private void HandleReinitialisation()
    {
      if (item == null || item.Possessor != player.oid)
      {
        player.oid.SendServerMessage($"L'objet que vous essayez de modifier n'existe plus ou n'est plus en votre possession.", Color.RED);
        player.menu.Close();
        return;
      }

      DisableFeedbackMessages();
      item.Destroy();
      NwItem newItem = NwItem.Deserialize(serializedInitialItem.ToByteArray());
      player.oid.AcquireItem(newItem);
      
      for(int i = (int)InventorySlot.Head; i == (int)InventorySlot.Bolts; i++)
      {
        if (player.oid.GetItemInSlot((InventorySlot)i) == item)
        {
          player.oid.ActionEquipItem(newItem, (InventorySlot)i);
          break;
        }
      }

      Task waitDestruction = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.4));
        EnableFeedbackMessages();
      });
    }
    private void DrawWeaponModificationMenu()
    {
      weaponColorChoice = null;
      weaponPartChoice = null;

      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Quel type de modification souhaitez-vous effectuer sur votre arme {item.Name.ColorString(Color.GREEN)} ?"
      };
      
      player.menu.choices.Add(($"Modifier l'apparence.".ColorString(Color.ORANGE), () => HandleWeaponPartChoice()));
      player.menu.choices.Add(($"Modifier les couleurs.".ColorString(Color.PINK), () => HandleWeaponColorChoice()));
      player.menu.choices.Add(($"Modifier le nom.".ColorString(Color.ORANGE), () => GetNewName()));
      player.menu.choices.Add(($"Modifier la description.".ColorString(Color.PINK), () => GetNewDescription()));
      player.menu.choices.Add(($"Annuler toutes les modifications en cours.".ColorString(Color.ORANGE), () => HandleReinitialisation()));

      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));
      player.menu.Draw();
    }

    private void HandleWeaponPartChoice()
    {
      weaponPartChoice = null;

      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Quelle partie de l'arme souhaitez-vous modifier ?"
      };
      
      player.menu.choices.Add(($"Supérieure.", () => ValidateWeaponPartChoice(ItemAppearanceWeaponModel.Top)));
      player.menu.choices.Add(($"Médiane.", () => ValidateWeaponPartChoice(ItemAppearanceWeaponModel.Middle)));
      player.menu.choices.Add(($"Inférieure.", () => ValidateWeaponPartChoice(ItemAppearanceWeaponModel.Bottom)));

      player.menu.choices.Add(("Retour.", () => DrawWeaponModificationMenu()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private void HandleWeaponColorChoice()
    {
      weaponColorChoice = null;

      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Quelle partie de l'arme souhaitez-vous modifier ?"
      };

      player.menu.choices.Add(($"Supérieure.", () => ValidateWeaponColorChoice(ItemAppearanceWeaponColor.Top)));
      player.menu.choices.Add(($"Médiane.", () => ValidateWeaponColorChoice(ItemAppearanceWeaponColor.Middle)));
      player.menu.choices.Add(($"Inférieure.", () => ValidateWeaponColorChoice(ItemAppearanceWeaponColor.Bottom)));

      player.menu.choices.Add(("Retour.", () => DrawWeaponModificationMenu()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private void ValidateWeaponPartChoice(ItemAppearanceWeaponModel choice)
    {
      weaponPartChoice = choice;
      ApplyWeaponModifications(-2);
    }
    private void ValidateWeaponColorChoice(ItemAppearanceWeaponColor choice)
    {
      weaponColorChoice = choice;
      ApplyWeaponModifications(-2);
    }
    private void ApplyWeaponModifications(int modification)
    {
      if(modification == -2)
        player.menu.Clear();

      if (item == null || item.Possessor != player.oid)
      {
        player.oid.SendServerMessage($"L'objet que vous essayez de modifier n'existe plus ou n'est plus en votre possession.", Color.RED);
        player.menu.Close();
        return;
      }

      if (weaponColorChoice != null)
      {
        player.menu.titleLines = new List<string> {
        "Faites défiler les couleurs à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur de couleur à l'oral entre 1 et 8"
        };

        byte currentValue = item.Appearance.GetWeaponColor((ItemAppearanceWeaponColor)weaponColorChoice);

        if (modification > -2)
        {
          DisableFeedbackMessages();

          if (player.setValue != Config.invalidInput)
          {
            currentValue = (byte)player.setValue;
            player.setValue = Config.invalidInput;
          }
          else if (modification == 1)
          {
            currentValue++;
            //if (currentValue > 8)
              //currentValue = 0;
          }
          else if (modification == -1)
          {
            currentValue--;
            //if (currentValue > 8)
              //currentValue = 8;
          }

          item.Appearance.SetWeaponColor((ItemAppearanceWeaponColor)weaponColorChoice, currentValue);
          NwItem newItem = item.Clone(player.oid);
          player.oid.ActionEquipItem(newItem, InventorySlot.RightHand);
          item.Destroy();
          item = newItem;

          Task waitDestruction = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.4));
            EnableFeedbackMessages();
          });
        }

        player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
      }
      else
      {
        player.menu.titleLines = new List<string> {
        "Faites défiler les apparences à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur d'apparence à l'oral (entre 1 et 8)"
        };

        byte currentValue = item.Appearance.GetWeaponModel((ItemAppearanceWeaponModel)weaponPartChoice);

        if (modification > -2)
        {
          DisableFeedbackMessages();

          if (player.setValue != Config.invalidInput)
          {
            currentValue = (byte)player.setValue;
            player.setValue = Config.invalidInput;
          }
          else if (modification == 1)
          {
            currentValue++;
            //if (currentValue > 8)
              //currentValue = 0;
          }
          else if (modification == -1)
          {
            currentValue--;
            //if (currentValue > 8)
              //currentValue = 8;
          }

          item.Appearance.SetWeaponModel((ItemAppearanceWeaponModel)weaponPartChoice, currentValue);
          NwItem newItem = item.Clone(player.oid);
          player.oid.ActionEquipItem(newItem, InventorySlot.RightHand);
          item.Destroy();
          item = newItem;

          Task waitDestruction = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.4));
            EnableFeedbackMessages();
          });
        }

        player.menu.titleLines.Add($"Modèle actuel : {currentValue.ToString().ColorString(Color.LIME)}");
      }

      if (modification > -2)
      {
        player.menu.DrawText();
      }
      else
      {
        player.menu.choices.Add(($"Suivant", () => ApplyWeaponModifications(1)));
        player.menu.choices.Add(($"Précédent.", () => ApplyWeaponModifications(-1)));

        player.menu.choices.Add(("Retour.", () => DrawWeaponModificationMenu()));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      Task waitPlayerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        player.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
        ApplyWeaponModifications(player.setValue);
        player.setValue = Config.invalidInput;
      });
    }
    private void DrawSimpleModificationMenu()
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Quel type de modification souhaitez-vous effectuer sur votre objet {item.Name.ColorString(Color.GREEN)} ?"
      };

      player.menu.choices.Add(($"Modifier l'apparence.".ColorString(Color.ORANGE), () => ApplySimpleModification(-2)));
      player.menu.choices.Add(($"Modifier le nom.".ColorString(Color.ORANGE), () => GetNewName()));
      player.menu.choices.Add(($"Modifier la description.".ColorString(Color.PINK), () => GetNewDescription()));
      player.menu.choices.Add(($"Annuler toutes les modifications en cours.".ColorString(Color.ORANGE), () => HandleReinitialisation()));

      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void ApplySimpleModification(int modification)
    {
      if(modification == -2)
        player.menu.Clear();

      if (item == null || item.Possessor != player.oid)
      {
        player.oid.SendServerMessage($"L'objet que vous essayez de modifier n'existe plus ou n'est plus en votre possession.", Color.RED);
        player.menu.Close();
        return;
      }

      player.menu.titleLines = new List<string> {
        "Faites défiler les apparences à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur d'apparence à l'oral (entre 1 et 255)"
        };

      byte currentValue = item.Appearance.GetSimpleModel();

      if (modification > -2)
      {
        DisableFeedbackMessages();

        if (player.setValue != Config.invalidInput)
        {
          currentValue = (byte)player.setValue;
          player.setValue = Config.invalidInput;
        }
        else if (modification == 1)
          currentValue++;
        else if (modification == -1)
          currentValue--;

        item.Appearance.SetSimpleModel(currentValue);
        NwItem newItem = item.Clone(player.oid);
    
        for (int i = (int)InventorySlot.Head; i == (int)InventorySlot.Bolts; i++)
        {
          if (player.oid.GetItemInSlot((InventorySlot)i) == item)
          {
            player.oid.ActionEquipItem(newItem, (InventorySlot)i);
            break;
          }
        }

        item.Destroy();
        item = newItem;

        player.menu.titleLines.Add($"Modèle actuel : {currentValue.ToString().ColorString(Color.LIME)}");

        Task waitDestruction = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.4));
          EnableFeedbackMessages();
        });
      }

      if (modification > -2)
      {
        player.menu.DrawText();
      }
      else
      {
        player.menu.choices.Add(($"Suivant", () => ApplySimpleModification(1)));
        player.menu.choices.Add(($"Précédent.", () => ApplySimpleModification(-1)));

        player.menu.choices.Add(("Retour.", () => DrawSimpleModificationMenu()));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      Task waitPlayerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        player.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
        ApplySimpleModification(player.setValue);
        player.setValue = Config.invalidInput;
      });
    }
    private void DrawHelmetCloakModificationMenu()
    {
      colorChannelChoice = null;
      LocationTypeColorChoice = 0;

      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Quel type de modification souhaitez-vous effectuer sur votre objet {item.Name.ColorString(Color.GREEN)} ?"
      };

      player.menu.choices.Add(($"Modifier l'apparence.".ColorString(Color.ORANGE), () => ApplyHelmetCloakModification(-2)));
      player.menu.choices.Add(($"Modifier les couleurs.".ColorString(Color.PINK), () => HandleColorChannelChoice()));
      player.menu.choices.Add(($"Modifier le nom.".ColorString(Color.ORANGE), () => GetNewName()));
      player.menu.choices.Add(($"Modifier la description.".ColorString(Color.PINK), () => GetNewDescription()));
      player.menu.choices.Add(($"Annuler toutes les modifications en cours.".ColorString(Color.ORANGE), () => HandleReinitialisation()));

      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void ApplyHelmetCloakModification(int modification)
    {
      if(modification == -2)
        player.menu.Clear();

      if (item == null || item.Possessor != player.oid)
      {
        player.oid.SendServerMessage($"L'objet que vous essayez de modifier n'existe plus ou n'est plus en votre possession.", Color.RED);
        player.menu.Close();
        return;
      }

      if (colorChannelChoice != null)
      {
        player.menu.titleLines = new List<string> {
        "Faites défiler les couleurs à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur de couleur à l'oral (entre 1 et 255)"
        };

        byte currentValue = item.Appearance.GetArmorColor((ItemAppearanceArmorColor)colorChannelChoice);

        if (modification > -2)
        {
          DisableFeedbackMessages();

          if (player.setValue != Config.invalidInput)
          {
            currentValue = (byte)player.setValue;
            player.setValue = Config.invalidInput;
          }
          else if (modification == 1)
            currentValue++;
          else if (modification == -1)
            currentValue--;

          item.Appearance.SetArmorColor((ItemAppearanceArmorColor)colorChannelChoice, currentValue);
          NwItem newItem = item.Clone(player.oid);

          if (item.BaseItemType == BaseItemType.Cloak)
            player.oid.ActionEquipItem(newItem, InventorySlot.Cloak);
          else
            player.oid.ActionEquipItem(newItem, InventorySlot.Head);

          item.Destroy();
          item = newItem;

          Task waitDestruction = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.4));
            EnableFeedbackMessages();
          });
        }

        player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
      }
      else
      {
        byte currentValue = item.Appearance.GetSimpleModel();

        if (modification > -2)
        {
          player.menu.titleLines = new List<string> {
        "Faites défiler les apparences à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur d'apparence à l'oral (entre 0 et 255)"
        };

          DisableFeedbackMessages();

          if (player.setValue != Config.invalidInput)
          {
            currentValue = (byte)player.setValue;
            player.setValue = Config.invalidInput;
          }
          else if (modification == 1)
            currentValue++;
          else if (modification == -1)
            currentValue--;

          item.Appearance.SetSimpleModel(currentValue);
          NwItem newItem = item.Clone(player.oid);

          if (item.BaseItemType == BaseItemType.Cloak)
            player.oid.ActionEquipItem(newItem, InventorySlot.Cloak);
          else
            player.oid.ActionEquipItem(newItem, InventorySlot.Head);

          item.Destroy();
          item = newItem;

          Task waitDestruction = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.4));
            EnableFeedbackMessages();
          });
        }

        player.menu.titleLines.Add($"Modèle actuel : {currentValue.ToString().ColorString(Color.LIME)}");
      }

      if (modification > -2)
      {
        player.menu.DrawText();
      }
      else
      {
        player.menu.choices.Add(($"Suivant", () => ApplyHelmetCloakModification(1)));
        player.menu.choices.Add(($"Précédent.", () => ApplyHelmetCloakModification(-1)));

        player.menu.choices.Add(("Retour.", () => DrawArmorModificationMenu()));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      Task waitPlayerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        player.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
        ApplyHelmetCloakModification(player.setValue);
        player.setValue = Config.invalidInput;
      });
    }
  }
}
