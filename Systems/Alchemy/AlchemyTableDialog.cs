

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Anvil.API;

using Newtonsoft.Json;

using static NWN.Systems.Craft.Collect.Config;

namespace NWN.Systems.Alchemy
{
  class AlchemyTableDialog
  {
    string[,] clonedAlchemyTable;
    PlayerSystem.Player player;
    NwPlaceable plcCauldron;

    public AlchemyTableDialog(PlayerSystem.Player player, NwPlaceable plcCauldron)
    {
      this.player = player;
      this.plcCauldron = plcCauldron;
      _ = this.plcCauldron.PlayAnimation(Animation.PlaceableActivate, 1);

      clonedAlchemyTable = (string[,])AlchemySystem.alchemyTable.Clone();

      if (player.alchemyCauldron == null)
      {
        player.alchemyCauldron = new Cauldron(player.characterId);

        foreach (Vector2 consumedGridEffect in player.alchemyCauldron.consumedGridEffects)
          clonedAlchemyTable[(int)consumedGridEffect.X, (int)consumedGridEffect.Y] = "";
      }     

      this.DrawWelcomePage();
    }
    private void DrawWelcomePage()
    {
      player.menu.Clear();

      player.menu.choices.Add(("Ajouter des ingrédients", () => DisplayAvailableIngredients()));

      if (player.alchemyCauldron.tablePosition == AlchemySystem.center)
      {
        if (player.alchemyCauldron.ingredientVector.Count < 1)
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
          player.menu.choices.Add(("Vider le chaudron et recommencer", () => HandleReinit()));
        }
      }
      else
      {
        player.menu.titleLines = new List<string> {
          $"L'eau du chaudron chantonne doucement la mélodie des ingrédients mélangés qui y surnagent.",
          "Que souhaitez-vous faire ?"
        };
        
        player.menu.choices.Add(("Remuer le mélange", () => HandleMixStrength()));

        if (player.oid.LoginCreature.KnowsFeat(CustomFeats.AlchemistAware))
          player.menu.choices.Add(("Examiner les nuances de couleur", () => HandleExamineColors()));

        if (player.oid.LoginCreature.KnowsFeat(CustomFeats.AlchemistAccurate))
          player.menu.choices.Add(("Humer le mélange", () => HandleSmell()));

        if (player.oid.LoginCreature.KnowsFeat(CustomFeats.AlchemistCareful))
          player.menu.choices.Add(("Ajouter de l 'eau au mélange", () => AddWater()));

        player.menu.choices.Add(("Activer le soufflet", () => ActivateBellows()));
        player.menu.choices.Add(("Finaliser la potion", () => BrewPotion()));

        player.menu.choices.Add(("Vider le chaudron et recommencer", () => HandleReinit()));
      }

      player.menu.choices.Add(("Consulter mes recettes", () => DisplayPlayerRecipes()));

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
        int input = int.Parse(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value);

        if (input < 1)
          input = 1;

        if (input > player.materialStock[materialKey])
          input = player.materialStock[materialKey];

        player.materialStock[materialKey] -= input;

        int total = input * multiplier;

        player.alchemyCauldron.addedIngredients.Add(new AddedIngredient(addedPlant, total));

        AddToCauldron(addedPlant.gridEffect, total, materialKey);
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
      }
    }
    private void AddToCauldron(Vector2 gridEffect, int total, string materialKey)
    {
      player.menu.Clear();

      for (int i = 0; i < total; i++)
        player.alchemyCauldron.ingredientVector.Add(gridEffect);

      player.oid.SendServerMessage("Votre ingrédient disparaît dans l'eau du chaudron après un léger frémissement de sa surface.");
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

      if (player.alchemyCauldron.ingredientVector.Count > 10)
        player.menu.choices.Add(("10", () => HandleMix(10)));

      if (player.alchemyCauldron.ingredientVector.Count > 50)
        player.menu.choices.Add(("50", () => HandleMix(50)));

      if (player.alchemyCauldron.ingredientVector.Count > 100)
        player.menu.choices.Add(("100", () => HandleMix(100)));

      player.menu.choices.Add(("Jusqu'à miscibilité totale du mélange", () => HandleMix(player.alchemyCauldron.ingredientVector.Count)));

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

      if (nbTurns > player.alchemyCauldron.ingredientVector.Count)
        nbTurns = player.alchemyCauldron.ingredientVector.Count;

      if (nbTurns < 1)
        nbTurns = 1;

      if (player.alchemyCauldron.ingredientVector.Count > 0)
      {
        for (int i = 0; i < nbTurns; i++)
          player.alchemyCauldron.tablePosition += player.alchemyCauldron.ingredientVector[i];

        player.alchemyCauldron.nBrowsedCases += nbTurns;
        player.alchemyCauldron.ingredientVector.RemoveRange(0, nbTurns);

        player.oid.SendServerMessage($"Vous faites tourner {nbTurns} fois la cuilllère dans le chaudron, mélangeant peu à peu les ingrédients.");
      }

      if (player.alchemyCauldron.ingredientVector.Count < 1)
        player.oid.SendServerMessage("Les ingrédients sont totalement dissous dans l'eau du chaudron.");

      Instruction lastInstruction = player.alchemyCauldron.instructions.LastOrDefault();

      if (lastInstruction != null && lastInstruction.instruction == InstructionType.Mix)
        lastInstruction.quantity += nbTurns;
      else
        player.alchemyCauldron.instructions.Add(new Instruction(InstructionType.Mix, nbTurns));

      DrawWelcomePage();
    }
    private void AddWater()
    {
      Vector2 diff = AlchemySystem.center - player.alchemyCauldron.tablePosition;

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
        player.alchemyCauldron.tablePosition += diff;
        player.oid.SendServerMessage("Vous ajoutez une dose d'eau. Le mélange s'éclaircit légèrement.");

        Instruction lastInstruction = player.alchemyCauldron.instructions.LastOrDefault();

        if (lastInstruction != null && lastInstruction.instruction == InstructionType.Distill)
          lastInstruction.quantity += 1;
        else
          player.alchemyCauldron.instructions.Add(new Instruction(InstructionType.Distill, 1));
      }

      DrawWelcomePage();
    }
    private void ActivateBellows()
    {
      Vector2? result = GetClosestEffectCoordinates((int)player.alchemyCauldron.tablePosition.X, (int)player.alchemyCauldron.tablePosition.Y, 3);

      if (!result.HasValue)
        player.oid.SendServerMessage("Souffler sur les braises ne semble précipiter aucune réaction particulière.");
      else
      {
        AddEffectToPotionList(clonedAlchemyTable[(int)result.Value.X, (int)result.Value.Y], ((int)Vector2.Distance(result.Value, player.alchemyCauldron.tablePosition)).ToString());
        clonedAlchemyTable[(int)result.Value.X, (int)result.Value.Y] = "";
        player.alchemyCauldron.consumedGridEffects.Add(result.Value);
      }

      DrawWelcomePage();
    }
    private void AddEffectToPotionList(string effect, string power)
    {
      switch (effect)
      {
        default:
          effect = effect.Replace("'power': ,", $"'power': '{power}',");
          player.alchemyCauldron.effectList.Add(effect);
          break;

        case "Range":
        case "Power":
        case "AoE":
        case "Uses":
        case "Duration":
          player.alchemyCauldron.effectList.Add(effect + power);
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

      if(player.alchemyCauldron.effectList.Count < 1 || player.alchemyCauldron.effectList.Any(s => s.Contains("CUSTOM_EFFECT_POISON")) || player.alchemyCauldron.effectList.Count > expertLevel)
        properties = @"{ 'tag': 'CUSTOM_EFFECT_POISON' }";
      else
      {
        int nbUse = 1;

        foreach (string use in player.alchemyCauldron.effectList.Where(s => s.StartsWith("Uses")).ToList())
        {
          nbUse += (int)use.Last();
          player.alchemyCauldron.effectList.Remove(use);
        }

        oItem.ItemCharges = nbUse;

        if (player.alchemyCauldron.effectList.Any(s => s.StartsWith("Range")))
        {
          foreach (ItemProperty ip in oItem.ItemProperties)
            oItem.RemoveItemProperty(ip);

          int range = 0;
          
          foreach (string rangeString in player.alchemyCauldron.effectList.Where(s => s.StartsWith("Range")).ToList())
          {
            range += (int)rangeString.Last();
            player.alchemyCauldron.effectList.Remove(rangeString);
          }

          if (range > 1)
            oItem.AddItemProperty(ItemProperty.Custom(15, 513, 6), EffectDuration.Permanent); // propriété activate item long range 1 charge / utilisation
          else
            oItem.AddItemProperty(ItemProperty.Custom(15, 537, 6), EffectDuration.Permanent); // propriété activate item touch 1 charge / utilisation
        }

        int power = 1;

        foreach (string powerString in player.alchemyCauldron.effectList.Where(s => s.StartsWith("Power")).ToList())
        {
          power *= (int)powerString.Last();
          player.alchemyCauldron.effectList.Remove(powerString);
        }

        oItem.GetObjectVariable<LocalVariableInt>("POTION_POWER").Value = power;

        int duration = 1;

        foreach (string durationString in player.alchemyCauldron.effectList.Where(s => s.StartsWith("Duration")).ToList())
        {
          duration *= (int)durationString.Last();
          player.alchemyCauldron.effectList.Remove(durationString);
        }

        oItem.GetObjectVariable<LocalVariableInt>("POTION_DURATION").Value = duration;

        int aoe = 0;

        foreach (string aoeString in player.alchemyCauldron.effectList.Where(s => s.StartsWith("AoE")).ToList())
        {
          aoe += (int)aoeString.Last();
          player.alchemyCauldron.effectList.Remove(aoeString);
        }

        oItem.GetObjectVariable<LocalVariableInt>("POTION_AOE").Value = aoe;

        foreach (string eff in player.alchemyCauldron.effectList)
          properties += eff + "|";

        properties = properties.SkipLast(1).ToString();
      }

      player.craftJob.Start(Craft.Job.JobType.Alchemy, null, player, null, oItem, properties);

      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"La concotion de votre potion va prendre un certain temps.",
        "Souhaitez-vous inscrire cette recette dans vos notes ?"
      };

      player.menu.choices.Add(("Noter la recette", () => GetRecipeName()));
      player.menu.choices.Add(("Non, nettoyer le chaudron", () => player.menu.Close()));

      player.alchemyCauldron = null;
    }
    private Vector2? GetClosestEffectCoordinates(int xCenter, int yCenter, int max)
    {
      try
      {
        for (int i = 0; i < max; i++)
        {
          for (int x = -i; x == 0; x++)
          {
            if (clonedAlchemyTable[xCenter + x, yCenter - i] != "")
              return new Vector2(xCenter + x, yCenter - i);
            if (clonedAlchemyTable[xCenter - i, yCenter + x] != "")
              return new Vector2(xCenter - i, yCenter + x);
            if (clonedAlchemyTable[xCenter + x, yCenter + i] != "")
              return new Vector2(xCenter + x, yCenter + i);
            if (clonedAlchemyTable[xCenter + i, yCenter + x] != "")
              return new Vector2(xCenter + i, yCenter + x);
          }
        }
      }
      catch (Exception){}

      return null;
    }
    private void HandleExamineColors()
    {
      Vector2? result = GetClosestEffectCoordinates((int)player.alchemyCauldron.tablePosition.X, (int)player.alchemyCauldron.tablePosition.Y, SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.AlchemistAware, player.learntCustomFeats[CustomFeats.AlchemistAware]) + 4);

      if (!result.HasValue)
        player.oid.SendServerMessage("Vous avez beau concentrer toute votre attention sur les changements de couleur du mélange, ceux-ci ne vous apportent aucune indication utile.", ColorConstants.Lime);
      else
      {
         switch(((int)Vector2.Distance(result.Value, player.alchemyCauldron.tablePosition)))
        {
          case 0:
          case 1:
          case 2:
            player.oid.SendServerMessage("D'après les changements de couleur du mélange, vous estimez que la solution est à un stade parfait pour obtenir un effet positif.", new Color(32, 255, 32));
            break;
          case 3:
            player.oid.SendServerMessage("D'après les changements de couleur du mélange, vous estimez que la solution est à un stade extrêmement proche d'obtenir un effet positif.", new Color(32, 255, 32));
            break;
          case 4:
            player.oid.SendServerMessage("D'après les changements de couleur du mélange, vous estimez que la solution est à un stade très proche d'obtenir un effet positif.", new Color(32, 255, 32));
            break;
          case 5:
            player.oid.SendServerMessage("D'après les changements de couleur du mélange, vous estimez que la solution est à un stade proche d'obtenir un effet positif.", new Color(32, 255, 32));
            break;
          case 6:
            player.oid.SendServerMessage("D'après les changements de couleur du mélange, vous estimez que la solution est à un stade relativement proche d'obtenir un effet positif.", new Color(32, 255, 32));
            break;
          case 7:
            player.oid.SendServerMessage("D'après les changements de couleur du mélange, vous estimez que la solution est à un stade presque proche d'obtenir un effet positif.", new Color(32, 255, 32));
            break;
        }
      }

      DrawWelcomePage();
    }
    private void HandleSmell()
    {
      Vector2? result = GetClosestEffectCoordinates((int)player.alchemyCauldron.tablePosition.X, (int)player.alchemyCauldron.tablePosition.Y, SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.AlchemistAware, player.learntCustomFeats[CustomFeats.AlchemistAware]) + 4);

      if (!result.HasValue)
        player.oid.SendServerMessage("Vous avez beau concentrer toute votre attention sur l'odeur du mélange, celui-ci ne vous apporte aucune indication utile.", ColorConstants.Lime);
      else
      {
        float xDiff = player.alchemyCauldron.tablePosition.X - result.Value.X;
        float yDiff = player.alchemyCauldron.tablePosition.Y - result.Value.Y;

        if (xDiff == 0 && yDiff == 0)
        {
          player.oid.SendServerMessage("D'après l'odeur du mélange, vous estimez que la solution est à un stade parfait pour obtenir un effet positif.", new Color(32, 255, 32));
          DrawWelcomePage();
          return;
        }

        if (yDiff > 0 && xDiff == 0)
        {
          player.oid.SendServerMessage("Humer le mélange vous permet de déterminer que votre décoction serait plus efficace si elle avait une odeur plus sucrée.", ColorConstants.Orange);
          DrawWelcomePage();
          return;
        }

        if (yDiff < 0 && xDiff == 0)
        {
          player.oid.SendServerMessage("Humer le mélange vous permet de déterminer que votre décoction serait plus efficace si elle avait une odeur plus citronnée.", ColorConstants.Orange);
          DrawWelcomePage();
          return;
        }

        if (xDiff > 0 && yDiff == 0)
        {
          player.oid.SendServerMessage("Humer le mélange vous permet de déterminer que votre décoction serait plus efficace si elle avait une odeur plus écoeurante.", ColorConstants.Orange);
          DrawWelcomePage();
          return;
        }

        if (xDiff < 0 && yDiff == 0)
        {
          player.oid.SendServerMessage("Humer le mélange vous permet de déterminer que votre décoction serait plus efficace si elle avait une odeur plus brûlée.", ColorConstants.Orange);
          DrawWelcomePage();
          return;
        }

        if (xDiff > 0 && yDiff > 0)
        {
          player.oid.SendServerMessage("Humer le mélange vous permet de déterminer que votre décoction serait plus efficace si elle avait une odeur plus chimique.", ColorConstants.Orange);
          DrawWelcomePage();
          return;
        }

        if (xDiff > 0 && yDiff < 0)
        {
          player.oid.SendServerMessage("Humer le mélange vous permet de déterminer que votre décoction serait plus efficace si elle avait une odeur plus mentholée.", ColorConstants.Orange);
          DrawWelcomePage();
          return;
        }

        if (xDiff < 0 && yDiff > 0)
        {
          player.oid.SendServerMessage("Humer le mélange vous permet de déterminer que votre décoction serait plus efficace si elle avait une odeur plus fruitée.", ColorConstants.Orange);
          DrawWelcomePage();
          return;
        }

        if (xDiff < 0 && yDiff < 0)
        {
          player.oid.SendServerMessage("Humer le mélange vous permet de déterminer que votre décoction serait plus efficace si elle avait une odeur plus boisée.", ColorConstants.Orange);
        }
      }

      DrawWelcomePage();
    }
    private void HandleReinit()
    {
      player.alchemyCauldron = null;
      player.menu.Close();
      player.oid.SendServerMessage("Vous videz le chaudron de son contenu actuel et le remplissez à nouveau d'une eau neutre.", ColorConstants.Orange);
    }
    private async void GetRecipeName()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string> {
        $"Quel nom souhaitez-vous donner à cette recette ?",
        "(Prononcez simplement le nom à l'oral.)"
      };

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        SaveRecipe();
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
      }
    }
    private async void SaveRecipe()
    {
      string input = player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value;

      bool awaitedQuery = await SqLiteUtils.InsertQueryAsync("playerAlchemyRecipe",
        new List<string[]>() {
            new string[] { "characterId", player.characterId.ToString() },
            new string[] { "recipeName", input },
            new string[] { "serializedRecipe", JsonConvert.SerializeObject(new CurrentRecipe(player.alchemyCauldron.addedIngredients, player.alchemyCauldron.effectList, player.alchemyCauldron.instructions)) } },
        new List<string>() { "characterId", "recipeName" },
        new List<string[]>() { new string[] { "serializedRecipe" } },
        new List<string>() { "characterId", "recipeName" });

      player.HandleAsyncQueryFeedback(awaitedQuery, $"Vous notez scrupuleuse votre recette {input.ColorString(ColorConstants.White)} dans votre carnet d'alchimiste.", "Erreur technique - votre recette n'a pas été enregistrée.");

      player.menu.Close();
    }
    private async void DisplayPlayerRecipes()
    {
      player.menu.Clear();

      var query = await SqLiteUtils.SelectQueryAsync("playerAlchemyRecipe",
        new List<string>() { { "recipeName" }, { "serializedRecipe" } },
        new List<string[]>() { new string[] { "characterId", player.characterId.ToString() } });

      player.menu.titleLines = new List<string> {
        "Voici la liste de vos recettes.",
        "Laquelle souhaitez-vous consulter ?"
      };

      foreach (var result in query)
      {
        string recipeName = $"- {result[0]}".ColorString(ColorConstants.Cyan);
        string serializedRecipe = result[1];

        player.menu.choices.Add((
          recipeName,
          () => HandleSelectedRecipe(recipeName, serializedRecipe)
        ));
      }

      player.menu.choices.Add(("Retour.", () => CommandSystem.DrawCommandList(player)));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      await NwTask.SwitchToMainThread();
      player.menu.Draw();
    }
    private void HandleSelectedRecipe(string recipeName, string serializedRecipe)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string> {
        $"La recette {recipeName} stipule les instructions ci-dessous :",
        "Que souhaitez-vous faire de cette recette ?"
      };

      CurrentRecipe recipe = JsonConvert.DeserializeObject<CurrentRecipe>(serializedRecipe);

      foreach (AddedIngredient ingredient in recipe.addedIngredients)
        player.menu.titleLines.Add($"Ajouter {ingredient.quantity} doses de {ingredient.ingredient.name}");
;
      foreach (Instruction instruction in recipe.instructions)
      {
        if (instruction.instruction == InstructionType.Mix)
          player.menu.titleLines.Add($"Remuer {instruction.quantity} fois.");
        else if (instruction.instruction == InstructionType.Distill)
          player.menu.titleLines.Add($"Ajouter {instruction.quantity} dose(s) d'eau.");
      }

      player.menu.choices.Add(("Produire.", () => CraftPotionFromRecipe(serializedRecipe)));
      player.menu.choices.Add(("Supprimer.", () => DeleteRecipe(recipeName)));

      player.menu.choices.Add(("Retour.", () => DrawWelcomePage()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }

    private void DeleteRecipe(string recipeName)
    {
      SqLiteUtils.DeletionQuery("playerAlchemyRecipe",
          new Dictionary<string, string>() { { "characterId", player.characterId.ToString() }, { "recipeName", recipeName } });

      player.oid.SendServerMessage($"Votre recette {recipeName.ColorString(ColorConstants.White)} a été supprimée de vos notes d'alchimie.", ColorConstants.Orange);
      DisplayPlayerRecipes();
    }
    private void CraftPotionFromRecipe(string serializedRecipe)
    {
      CurrentRecipe recipe = JsonConvert.DeserializeObject<CurrentRecipe>(serializedRecipe);

      foreach(AddedIngredient ingredient in recipe.addedIngredients)
      {
        if(player.materialStock[ingredient.ingredient.ToString()] < ingredient.quantity)
        {
          int missingIngredients = ingredient.quantity - player.materialStock[ingredient.ingredient.ToString()];
          player.oid.SendServerMessage($"Il vous manque {missingIngredients.ToString().ColorString(ColorConstants.White)} de {ingredient.ingredient.name.ColorString(ColorConstants.White)} pour réaliser cette recette.", ColorConstants.Red);
          return;
        }
      }

      foreach (AddedIngredient ingredient in recipe.addedIngredients)
        player.materialStock[ingredient.ingredient.ToString()] -= ingredient.quantity;

      player.alchemyCauldron.effectList = recipe.effectList;

      BrewPotion();

      player.oid.SendServerMessage("Votre potion est en cours de concotion, ce qui va prendre un certain temps.", new Color(32, 255, 32));
      player.menu.Close();
    }
  }
}
