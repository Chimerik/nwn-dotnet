

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

using Anvil.API;

using static NWN.Systems.Craft.Collect.Config;

namespace NWN.Systems.Alchemy
{
  class AlchemyTableDialog
  {
    PlayerSystem.Player player;
    string[,] clonedAlchemyTable;
    Vector2 tablePosition;
    List<Vector2> ingredientVector;
    List<string> effectList;
    int nBrowsedCases;

    public AlchemyTableDialog(PlayerSystem.Player player)
    {
      this.player = player;
      tablePosition = AlchemySystem.center;
      ingredientVector = new List<Vector2>();
      effectList = new List<string>();
      nBrowsedCases = 0;

      clonedAlchemyTable = (string[,])AlchemySystem.alchemyTable.Clone();

      // Eventuellement : check si le joueur a déjà un chaudron en cours, si oui, charger le chaudron, sinon initialiser le chaudron vide

      this.DrawWelcomePage();
    }
    private void DrawWelcomePage()
    {
      player.menu.Clear();

      player.menu.choices.Add(("Ajouter des ingrédients", () => DisplayAvailableIngredients()));

      if (tablePosition == AlchemySystem.center)
      {
        if (ingredientVector.Count < 1)
        {
          player.menu.titleLines = new List<string> {
            $"Le chaudron est rempli d'une eau neutre et limpide.",
            "Que souhaitez-vous faire ?"
          };
        }
        else
        {
          player.menu.titleLines = new List<string> {
            $"L'eau du chaudron chantonne doucement la mélodie des ingrédients qui y surnagent.",
            "Que souhaitez-vous faire ?"
          };

          player.menu.choices.Add(("Remuer le mélange", () => HandleMixStrength()));
        }
      }
      else
      {
        player.menu.titleLines = new List<string> {
          $"L'eau du chaudron chantonne doucement la mélodie des ingrédients mélangés qui y surnagent.",
          "Que souhaitez-vous faire ?"
        };
        
        player.menu.choices.Add(("Remuer le mélange", () => HandleMixStrength()));

        if(player.oid.LoginCreature.KnowsFeat(CustomFeats.AlchemistCareful))
          player.menu.choices.Add(("Ajouter de l 'eau au mélange", () => AddWater()));

        player.menu.choices.Add(("Activer le soufflet", () => ActivateBellows()));
        player.menu.choices.Add(("Finaliser la potion", () => BrewPotion()));
      }

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void DisplayAvailableIngredients()
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Voici la liste des ingrédients en votre possession.",
        "Lesquels souhaitez-vous ajouter ?"
      };

      foreach (KeyValuePair<string, int> materialEntry in player.materialStock)
      {
        string ingredientKey = materialEntry.Key;
        string displayName = materialEntry.Key;
        int multiplier = 1;

        if (ingredientKey.StartsWith("PoudreDe"))
        {
          ingredientKey = ingredientKey.Replace("PoudreDe", "");
          displayName = "Poudre de " + ingredientKey;
          multiplier = 2;
        }

        if (Enum.TryParse(ingredientKey, out PlantType myOreType) && myOreType != PlantType.Invalid)
          player.menu.choices.Add(($"{displayName} - {materialEntry.Value} unité(s).", () => HandleQuantity(myOreType, multiplier, materialEntry.Key)));
      }

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private async void HandleQuantity(PlantType plantType, int multiplier, string materialKey)
    {
      player.menu.Clear();
      Plant addedPlant = plantDictionnary[plantType];
      player.menu.titleLines = new List<string> {
        $"Quelle quantité de {addedPlant.name} souhaitez-vous ajouter au chaudron parmi vos {player.materialStock[addedPlant.type.ToString()]} disponibles ?",
        "(Prononcez simplement la quantité à l'oral.)"
      };

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        AddToCauldron(addedPlant.gridEffect, multiplier, materialKey);
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
      }
    }
    private void AddToCauldron(Vector2 gridEffect, int multiplier, string materialKey)
    {
      player.menu.Clear();
      int input = int.Parse(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT"));

      if (input < 1)
        input = 1;

      if (input > player.materialStock[materialKey])
        input = player.materialStock[materialKey];

      player.materialStock[materialKey] -= input;
      for(int i = 0; i < multiplier; i++)
      ingredientVector.Add(gridEffect);

      player.oid.SendServerMessage("Votre ingrédient disparaît dans l'eau du chaudron après un petit plouf.");
      DrawWelcomePage();
    }
    private async void HandleMixStrength()
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Quel nombre de tours de cuillière souhaitez-vous effectuer ?",
        "(Prononcez simplement le nombre à l'oral.)"
      };

      player.menu.choices.Add(("1", () => HandleMix(1)));

      if (ingredientVector.Count > 10)
        player.menu.choices.Add(("10", () => HandleMix(10)));

      if (ingredientVector.Count > 50)
        player.menu.choices.Add(("50", () => HandleMix(50)));

      if (ingredientVector.Count > 100)
        player.menu.choices.Add(("100", () => HandleMix(100)));

      player.menu.choices.Add(("Jusqu'à miscibilité totale du mélange", () => HandleMix(ingredientVector.Count)));

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        HandleMix(int.Parse(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value));
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
      }
    }
    private void HandleMix(int nbTurns)
    {
      if (player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").HasValue)
        player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_PLAYER_INPUT_CANCELLED").Value = 1;

      if (nbTurns > ingredientVector.Count)
        nbTurns = ingredientVector.Count;

      if (nbTurns < 1)
        nbTurns = 1;

      if (ingredientVector.Count > 0)
      {
        for (int i = 0; i < nbTurns; i++)
          tablePosition += ingredientVector[i];

        nBrowsedCases += nbTurns;
        ingredientVector.RemoveRange(0, nbTurns);

        player.oid.SendServerMessage($"Vous faites tourner {nbTurns} fois la cuilllère dans le chaudron, mélangeant peu à peu les ingrédients.");
      }

      if (ingredientVector.Count < 1)
        player.oid.SendServerMessage("Les ingrédients sont totalement dissous dans l'eau du chaudron.");

      // TODO : ajouter ici les effets de Alchimiste attentif et Alchimiste précis

      DrawWelcomePage();
    }
    private void AddWater()
    {
      Vector2 diff = AlchemySystem.center - tablePosition;

      if (diff.X > 0)
        diff.X = -1;
      else
        diff.X = 1;

      if (diff.Y > 0)
        diff.Y = -1;
      else
        diff.Y = 1;

      if (diff.X == 0 && diff.Y == 0)
        player.oid.SendServerMessage("Le mélange est déjà parfaitement neutre.");
      else
      {
        tablePosition += diff;
        player.oid.SendServerMessage("Vous ajoutez une dose d'eau. Le mélange s'éclaircit légèrement.");
      }

      DrawWelcomePage();
    }
    private void ActivateBellows()
    {
      try
      {
        string foundEffect = clonedAlchemyTable[(int)tablePosition.X, (int)tablePosition.Y];
        
        if (foundEffect == "")
          foundEffect = clonedAlchemyTable[(int)tablePosition.X, (int)tablePosition.Y - 1];
        else
        {
          clonedAlchemyTable[(int)tablePosition.X, (int)tablePosition.Y] = "";
          AddEffectToPotionList(foundEffect, "3");
          return;
        }

        if (foundEffect == "")
          foundEffect = clonedAlchemyTable[(int)tablePosition.X + 1, (int)tablePosition.Y];
        else
        {
          clonedAlchemyTable[(int)tablePosition.X, (int)tablePosition.Y - 1] = "";
          AddEffectToPotionList(foundEffect, "2");
          return;
        }

        if (foundEffect == "")
          foundEffect = clonedAlchemyTable[(int)tablePosition.X, (int)tablePosition.Y + 1];
        else
        {
          clonedAlchemyTable[(int)tablePosition.X + 1, (int)tablePosition.Y] = "";
          AddEffectToPotionList(foundEffect, "2");
          return;
        }

        if (foundEffect == "")
          foundEffect = clonedAlchemyTable[(int)tablePosition.X - 1, (int)tablePosition.Y];
        else
        {
          clonedAlchemyTable[(int)tablePosition.X, (int)tablePosition.Y + 1] = "";
          AddEffectToPotionList(foundEffect, "2");
          return;
        }

        if (foundEffect == "")
          foundEffect = clonedAlchemyTable[(int)tablePosition.X, (int)tablePosition.Y + 2];
        else
        {
          clonedAlchemyTable[(int)tablePosition.X - 1, (int)tablePosition.Y] = "";
          AddEffectToPotionList(foundEffect, "1");
          return;
        }

        if (foundEffect == "")
          foundEffect = clonedAlchemyTable[(int)tablePosition.X + 1, (int)tablePosition.Y + 1];
        else
        {
          clonedAlchemyTable[(int)tablePosition.X, (int)tablePosition.Y + 2] = "";
          AddEffectToPotionList(foundEffect, "1");
          return;
        }

        if (foundEffect == "")
          foundEffect = clonedAlchemyTable[(int)tablePosition.X + 2, (int)tablePosition.Y];
        else
        {
          clonedAlchemyTable[(int)tablePosition.X + 1, (int)tablePosition.Y + 1] = "";
          AddEffectToPotionList(foundEffect, "1");
          return;
        }

        if (foundEffect == "")
          foundEffect = clonedAlchemyTable[(int)tablePosition.X + 1, (int)tablePosition.Y - 1];
        else
        {
          clonedAlchemyTable[(int)tablePosition.X + 2, (int)tablePosition.Y] = "";
          AddEffectToPotionList(foundEffect, "1");
          return;
        }

        if (foundEffect == "")
          foundEffect = clonedAlchemyTable[(int)tablePosition.X, (int)tablePosition.Y - 2];
        else
        {
          clonedAlchemyTable[(int)tablePosition.X + 1, (int)tablePosition.Y - 1] = "";
          AddEffectToPotionList(foundEffect, "1");
          return;
        }

        if (foundEffect == "")
          foundEffect = clonedAlchemyTable[(int)tablePosition.X - 1, (int)tablePosition.Y - 1];
        else
        {
          clonedAlchemyTable[(int)tablePosition.X, (int)tablePosition.Y - 2] = "";
          AddEffectToPotionList(foundEffect, "1");
          return;
        }

        if (foundEffect == "")
          foundEffect = clonedAlchemyTable[(int)tablePosition.X - 2, (int)tablePosition.Y];
        else
        {
          clonedAlchemyTable[(int)tablePosition.X - 1, (int)tablePosition.Y - 1] = "";
          AddEffectToPotionList(foundEffect, "1");
          return;
        }

        if (foundEffect == "")
          foundEffect = clonedAlchemyTable[(int)tablePosition.X - 1, (int)tablePosition.Y + 1];
        else
        {
          clonedAlchemyTable[(int)tablePosition.X - 2, (int)tablePosition.Y] = "";
          AddEffectToPotionList(foundEffect, "1");
          return;
        }

        if (foundEffect == "")
        {
          player.oid.SendServerMessage("Souffler sur les braises ne semble précipiter aucune réaction particulière.");
          DrawWelcomePage();
        }
        else
        {
          clonedAlchemyTable[(int)tablePosition.X - 1, (int)tablePosition.Y + 1] = "";
          AddEffectToPotionList(foundEffect, "1");
        }
      }
      catch(Exception e)
      {
        effectList.Add(@"{ 'tag': 'CUSTOM_EFFECT_POISON' }");

        PlayerSystem.Log.Info(e.Message);
        PlayerSystem.Log.Info(e.StackTrace);

        DrawWelcomePage();
      }
    }
    private void AddEffectToPotionList(string effect, string power)
    {
      switch (effect)
      {
        default:
          effect = effect.Replace("'power': ,", $"'power': '{power}',");
          effectList.Add(effect);
          break;

        case "Range":
        case "Power":
        case "AoE":
        case "Uses":
        case "Duration":
          effectList.Add(effect + power);
          break;
      }
      player.oid.SendServerMessage("Le feu crépite, le mélange bout, au vue de l'odeur et des fumées dégagées, nul doute que le mélange réagit !");
      DrawWelcomePage();
    }
    private async void BrewPotion()
    {
      // La potion est ruinée si elle n'a pas d'effet ou si elle a plus d'effets que le niveau de compétence en alchimiste expert + 1 ou elle dispose déjà d'un effet avec tag = poison
      // Le temps de confection de la potion dépend du nombre de cases parcourues

      NwItem oItem = await NwItem.Create("potionalchimique");
      oItem.Appearance.SetWeaponModel(ItemAppearanceWeaponModel.Bottom, (byte)Utils.random.Next(11));
      oItem.Appearance.SetWeaponModel(ItemAppearanceWeaponModel.Middle, (byte)Utils.random.Next(8));
      oItem.Appearance.SetWeaponModel(ItemAppearanceWeaponModel.Top, (byte)Utils.random.Next(8));
      oItem.Appearance.SetWeaponColor(ItemAppearanceWeaponColor.Bottom, (byte)Utils.random.Next(10));
      oItem.Appearance.SetWeaponColor(ItemAppearanceWeaponColor.Middle, (byte)Utils.random.Next(10));
      oItem.Appearance.SetWeaponColor(ItemAppearanceWeaponColor.Top, (byte)Utils.random.Next(10));

      int expertLevel = 1;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.AlchemistExpert))
        expertLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.AlchemistExpert, player.learntCustomFeats[CustomFeats.AlchemistExpert]);

      string properties = "";

      if(effectList.Count < 1 || effectList.Any(s => s.Contains("CUSTOM_EFFECT_POISON")) || effectList.Count > expertLevel)
        properties = @"{ 'tag': 'CUSTOM_EFFECT_POISON' }";
      else
      {
        int nbUse = 1;

        foreach (string use in effectList.Where(s => s.StartsWith("Uses")).ToList())
        {
          nbUse += (int)use.Last();
          effectList.Remove(use);
        }

        oItem.ItemCharges = nbUse;

        if (effectList.Any(s => s.StartsWith("Range")))
        {
          foreach (ItemProperty ip in oItem.ItemProperties)
            oItem.RemoveItemProperty(ip);

          int range = 0;
          
          foreach (string rangeString in effectList.Where(s => s.StartsWith("Range")).ToList())
          {
            range += (int)rangeString.Last();
            effectList.Remove(rangeString);
          }

          if (range > 1)
            oItem.AddItemProperty(ItemProperty.Custom(15, 513, 6), EffectDuration.Permanent); // propriété activate item long range 1 charge / utilisation
          else
            oItem.AddItemProperty(ItemProperty.Custom(15, 537, 6), EffectDuration.Permanent); // propriété activate item touch 1 charge / utilisation
        }

        int power = 1;

        foreach (string powerString in effectList.Where(s => s.StartsWith("Power")).ToList())
        {
          power *= (int)powerString.Last();
          effectList.Remove(powerString);
        }

        oItem.GetObjectVariable<LocalVariableInt>("POTION_POWER").Value = power;

        int duration = 1;

        foreach (string durationString in effectList.Where(s => s.StartsWith("Duration")).ToList())
        {
          duration *= (int)durationString.Last();
          effectList.Remove(durationString);
        }

        oItem.GetObjectVariable<LocalVariableInt>("POTION_DURATION").Value = duration;

        int aoe = 0;

        foreach (string aoeString in effectList.Where(s => s.StartsWith("AoE")).ToList())
        {
          aoe += (int)aoeString.Last();
          effectList.Remove(aoeString);
        }

        oItem.GetObjectVariable<LocalVariableInt>("POTION_AOE").Value = aoe;


        foreach (string eff in effectList)
          properties += eff + "|";

        properties = properties.SkipLast(1).ToString();
      }

      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_INGREDIENT_COUNT").Value = nBrowsedCases;
      player.craftJob.Start(Craft.Job.JobType.Alchemy, null, player, null, oItem, properties);
      player.menu.Close();
    }
  }
}
