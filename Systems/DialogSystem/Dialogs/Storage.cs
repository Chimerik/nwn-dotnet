using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.Craft.Collect.System;
using static NWN.Systems.Craft.Collect.Config;
using NWN.API;

namespace NWN.Systems
{
  class Storage
  {
    private Dictionary<uint, string> inventoryMaterials;
    public Storage(Player player)
    {
      this.inventoryMaterials = new Dictionary<uint, string>();
      this.DrawWelcomePage(player);
    }
    private void DrawWelcomePage(PlayerSystem.Player player)
    {
      inventoryMaterials.Clear();
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        "Yop, tu veux déposer tes matières premières quelque part ?",
        "Vas-y file moi ça. Oublie pas qu'on prend 5 % pour le service."
      };
      player.menu.choices.Add(($"Tout déposer.", () => HandleDropAll(player)));
      //player.menu.choices.Add(($"Déposer une matière en particulier.", () => HandleDropMaterialSelection(player)));
      //player.menu.choices.Add(($"A vrai dire, je suis là pour un retrait.", () => HandleWithdrawMaterialSelection(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleDropAll(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        "Voilà qui est fait.",
        "Merci pour ta contribution à la cause !"
    };

      foreach (NwItem item in player.oid.Items.Where(i => IsItemCraftMaterial(i.Tag) == true))
      {
        int addedOre = item.StackSize * 95 / 100;

        PeltType peltType = GetPeltTypeFromItemTag(item.Tag);
        if (peltType != PeltType.Invalid)
          item.Tag = Enum.GetName(typeof(PeltType), peltType) ?? "";

        if (player.materialStock.ContainsKey(item.Tag))
          player.materialStock[item.Tag] += addedOre;
        else
          player.materialStock.Add(item.Tag, addedOre);

        item.Destroy();
      }

      player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleDropMaterialSelection(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        "D'ac. Dépôt de quelle matière première ?"
      };

      foreach (NwItem item in player.oid.Items.Where(i => IsItemCraftMaterial(i.Tag) == true))
        if (IsItemCraftMaterial(item.Tag))
          inventoryMaterials.Add(item, item.Tag);

      foreach (string value in inventoryMaterials.Values.Distinct())
        player.menu.choices.Add(($"{value}.", () => HandleValidateDropMaterial(player, value)));

      player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleValidateDropMaterial(Player player, string material)
    {
      player.menu.Clear();

      if (player.setValue <= 0)
      {
        player.menu.titleLines = new List<string> {
          "Plait-il ? Je n'ai pas bien compris.",
          "(Utilisez la commande !set X avant de valider votre choix)"
        };
        player.menu.choices.Add(($"Valider.", () => HandleValidateDropMaterial(player, material)));
      }
      else
      {
        int valueToStock = player.setValue;
        foreach (KeyValuePair<uint, string> materialEntry in inventoryMaterials.Where(v => v.Value == material))
        {
          NwItem item = materialEntry.Key.ToNwObject<NwItem>();
          if (item != null)
          {
            int stackSize = item.StackSize;

            if (stackSize >= valueToStock)
            {
              player.materialStock[material] += valueToStock * 95 / 100;
              if (stackSize == valueToStock)
                item.Destroy();
              else
                item.StackSize -= valueToStock;

              break;
            }
            else
            {
              player.materialStock[material] += stackSize * 95 / 100;
              item.Destroy();
            }
          }
        }
        player.menu.titleLines.Add("Voilà qui est fait !");
      }

      player.setValue = 0;
      player.menu.choices.Add(("Retour", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleWithdrawMaterialSelection(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        "D'ac. Retrait de quelle matière première ?",
        "(Utilisez !set X pour préciser la quantité avant de valider votre choix)"
      };

      foreach (KeyValuePair<string, int> stockEntry in player.materialStock.Where(v => v.Value > 0))
        player.menu.choices.Add(($"{stockEntry.Key} - {stockEntry.Value}.", () => HandleValidateWithdrawMaterial(player, stockEntry.Key)));

      player.menu.choices.Add(("Retour", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleValidateWithdrawMaterial(Player player, string material)
    {
      player.menu.Clear();

      if (player.setValue <= 0)
      {
        player.menu.titleLines = new List<string> {
          "Plait-il ? Je n'ai pas bien compris.",
          "(Utilisez la commande !set X avant de valider votre choix)"
        };
        player.menu.choices.Add(("Valider.", () => HandleValidateDropMaterial(player, material)));
      }
      else
      {
        string itemTemplate = GetCraftMaterialItemTemplate(material);
        if (itemTemplate != "")
        {
          int remainingValue = 0;

          if (player.setValue >= player.materialStock[material])
          {
            player.menu.titleLines.Add($"Ouais, j'te file tout en gros. D'ac, démerde toi avec ça.");
            remainingValue = player.materialStock[material];
            player.materialStock[material] = 0;
          }
          else
          {
            player.menu.titleLines.Add($"{player.setValue} de {material} ? C'est parti !");
            remainingValue = player.setValue;
            player.materialStock[material] -= player.setValue;
          }

          while (remainingValue > 0)
          {
            if (remainingValue >= 50000)
            {
              NwItem item = NwItem.Create(itemTemplate, player.oid, 50000, material);
              item.Name = material;
              remainingValue -= 50000;
            }
            else
            {
              NwItem item = NwItem.Create(itemTemplate, player.oid, remainingValue, material);
              item.Name = material;
              break;
            }
          }
        }
      }

      player.setValue = 0;
      player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
  }
}
