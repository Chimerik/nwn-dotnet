﻿using NWN.API.Constants;
using NWN.API;
using NWN.Core;
using System.Threading.Tasks;
using NWN.Core.NWNX;
using System.Collections.Generic;
using System;
using NWN.API.Events;
using ItemProperty = NWN.API.ItemProperty;
using System.Linq;

namespace NWN.Systems
{
  class PNJFactory
  {
    PlayerSystem.Player player;
    NwCreature oPNJ;
    public PNJFactory(PlayerSystem.Player player)
    {
      this.player = player;
      DrawPNJFactoryWelcome();
    }
    private void DrawPNJFactoryWelcome()
    {
      player.menu.Clear();

      player.menu.titleLines.Add("Bienvenue dans la PNJ factory. Que souhaitez-vous faire ?");

      player.menu.choices.Add(("Sélectionner un PNJ existant", () => PlayerSystem.cursorTargetService.EnterTargetMode(player.oid, OnPNJSelected, ObjectTypes.Creature, MouseCursor.Create)));
      player.menu.choices.Add(("Parcourir les listes de PNJs enregistrés", () => DisplayPNJCreatorsList()));

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    public void OnPNJSelected(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.TargetObject == null || !(selection.TargetObject is NwCreature oCreature))
        return;

      if (oCreature.IsLoginPlayerCharacter && selection.Player.PlayerName != "Chim")
      {
        selection.Player.SendServerMessage("Seuls les pnjs peuvent être modifiés à l'aide de cet outil.");
        return;
      }

      this.oPNJ = oCreature;

      DrawPNJSelectionWelcome();
    }
    private void DrawPNJSelectionWelcome()
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)} Que souhaitez-vous faire ?");

      player.menu.choices.Add(("Modifier le prénom", () => ModifyPNJName(0)));
      player.menu.choices.Add(("Modifier le nom", () => ModifyPNJName(1)));
      player.menu.choices.Add(("Modifier la description", () => ModifyPNJDescription()));
      player.menu.choices.Add(("Modifier l'apparence", () => DrawPNJAppearancePage()));
      player.menu.choices.Add(("Modifier la CA de base", () => ModifyPNJBaseArmorClass()));
      player.menu.choices.Add(("Modifier les caractéristiques", () => DrawAbilityList()));
      player.menu.choices.Add(("Modifier les points de vie", () => ModifyPNJHealthByLevel()));
      player.menu.choices.Add(("Modifier la vitesse", () => DrawSpeedList()));
      player.menu.choices.Add(("Modifier la voix", () => ModifyPNJSoundset()));
      player.menu.choices.Add(("Modifier l'attaque de base", () => ModifyPNJBaseAttack()));
      player.menu.choices.Add(("Modifier le sexe", () => DrawGenderList()));
      player.menu.choices.Add(("Modifier le type de taille", () => DrawSizeList()));
      player.menu.choices.Add(("Modifier la race", () => DrawRacialTypeList()));
      player.menu.choices.Add(("Modifier les jets de sauvegarde", () => DrawSavingThrowList()));
      player.menu.choices.Add(("Modifier la résistance aux sorts", () => ModifyPNJSpellResistance()));
      player.menu.choices.Add(("Afficher les résistances de la créature", () => DisplayPNJSkin()));
      player.menu.choices.Add(("Modifier la résistance aux dégâts spécifiques", () => SelectDamageType()));

      player.menu.choices.Add(("Sauvegarder ce PNJ", () => HandleSaveNPC()));

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private async void ModifyPNJName(int isLastName)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}.");
      player.menu.titleLines.Add($"Veuillez indiquer le nouveau nom à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        CreaturePlugin.SetOriginalName(oPNJ, player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value, isLastName);
        oPNJ.Name = player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value;
        player.oid.SendServerMessage($"Le nom du PNJ a été modifié à {oPNJ.Name.ColorString(ColorConstants.White)}", ColorConstants.Blue);
        DrawPNJSelectionWelcome();
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private async void ModifyPNJDescription()
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}.");
      player.menu.titleLines.Add($"Veuillez indiquer la nouvelle description à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        oPNJ.Description = player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value;
        player.oid.SendServerMessage($"La description de {oPNJ.Name.ColorString(ColorConstants.White)} a été modifiée.", ColorConstants.Blue);
        DrawPNJSelectionWelcome();
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private async void ModifyPNJBaseArmorClass()
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}.");
      player.menu.titleLines.Add($"Veuillez indiquer la valeur de CA de base à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        CreaturePlugin.SetBaseAC(oPNJ, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        player.oid.SendServerMessage($"La CA de base de {oPNJ.Name.ColorString(ColorConstants.White)} a été modifiée à {int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value)}", ColorConstants.Blue);
        DrawPNJSelectionWelcome();
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private void DrawAbilityList()
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}. Quel caractéristique souhaitez-vous modifier ?");

      player.menu.choices.Add(("Force", () => ModifySelectedAbility(NWScript.ABILITY_STRENGTH)));
      player.menu.choices.Add(("Dextérité", () => ModifySelectedAbility(NWScript.ABILITY_DEXTERITY)));
      player.menu.choices.Add(("Constitution", () => ModifySelectedAbility(NWScript.ABILITY_CONSTITUTION)));
      player.menu.choices.Add(("Intelligence", () => ModifySelectedAbility(NWScript.ABILITY_INTELLIGENCE)));
      player.menu.choices.Add(("Sagesse", () => ModifySelectedAbility(NWScript.ABILITY_WISDOM)));
      player.menu.choices.Add(("Charisme", () => ModifySelectedAbility(NWScript.ABILITY_CHARISMA)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome()));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private async void ModifySelectedAbility(int ability)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}.");
      player.menu.titleLines.Add($"Veuillez indiquer la valeur de la caractéristique à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        CreaturePlugin.SetRawAbilityScore(oPNJ, ability, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        player.oid.SendServerMessage($"La caractéristique de base de {oPNJ.Name.ColorString(ColorConstants.White)} a été modifiée à {int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value)}", ColorConstants.Blue);
        DrawPNJSelectionWelcome();
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private async void ModifyPNJHealthByLevel()
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}.");
      player.menu.titleLines.Add($"Veuillez indiquer le nombre de points de vie par niveau à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        if (oPNJ.IsLoginPlayerCharacter)
          CreaturePlugin.SetMaxHitPointsByLevel(oPNJ, 1, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        else
          ObjectPlugin.SetMaxHitPoints(oPNJ, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));

        player.oid.SendServerMessage($"Le nombre de points de vie de {oPNJ.Name.ColorString(ColorConstants.White)} a été modifiée à {int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value)}", ColorConstants.Blue);
        DrawPNJSelectionWelcome();
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private void DrawSpeedList()
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}. Quelle vitesse souhaitez-vous appliquer ?");

      player.menu.choices.Add(("Par défaut", () => ModifySelectedSpeed(MovementRate.CreatureDefault)));
      player.menu.choices.Add(("Immobile", () => ModifySelectedSpeed(MovementRate.Immobile)));
      player.menu.choices.Add(("Très lent", () => ModifySelectedSpeed(MovementRate.VerySlow)));
      player.menu.choices.Add(("Lent", () => ModifySelectedSpeed(MovementRate.Slow)));
      player.menu.choices.Add(("Normal", () => ModifySelectedSpeed(MovementRate.Normal)));
      player.menu.choices.Add(("Rapide", () => ModifySelectedSpeed(MovementRate.Fast)));
      player.menu.choices.Add(("Très rapide", () => ModifySelectedSpeed(MovementRate.VeryFast)));
      player.menu.choices.Add(("Vitesse DM", () => ModifySelectedSpeed(MovementRate.DM)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome()));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void ModifySelectedSpeed(MovementRate speed)
    {
      oPNJ.MovementRate = speed;
      player.oid.SendServerMessage($"La vitesse de {oPNJ.Name.ColorString(ColorConstants.White)} a bien été modifiée.", ColorConstants.Blue);
      DrawPNJSelectionWelcome();
    }
    private async void ModifyPNJSoundset()
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}.");
      player.menu.titleLines.Add($"Veuillez indiquer la voix à utiliser à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        CreaturePlugin.SetSoundset(oPNJ, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        player.oid.SendServerMessage($"La voix de {oPNJ.Name.ColorString(ColorConstants.White)} a été modifiée à {int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value)}", ColorConstants.Blue);
        DrawPNJSelectionWelcome();
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private async void ModifyPNJBaseAttack()
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}.");
      player.menu.titleLines.Add($"Veuillez indiquer la valeur d'attaque de base à utiliser à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        CreaturePlugin.SetBaseAttackBonus(oPNJ, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        player.oid.SendServerMessage($"L'attaque de base de {oPNJ.Name.ColorString(ColorConstants.White)} a été modifiée à {int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value)}", ColorConstants.Blue);
        DrawPNJSelectionWelcome();
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private void DrawGenderList()
    {
      player.menu.Clear();
      
      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}. Quel genre souhaitez-vous appliquer ?");
      
      player.menu.choices.Add(("Masculin", () => ModifySelectedGender(NWScript.GENDER_MALE)));
      player.menu.choices.Add(("Féminin", () => ModifySelectedGender(NWScript.GENDER_FEMALE)));
      player.menu.choices.Add(("Les deux", () => ModifySelectedGender(NWScript.GENDER_BOTH)));
      player.menu.choices.Add(("Aucun", () => ModifySelectedGender(NWScript.GENDER_NONE)));
      player.menu.choices.Add(("Autre", () => ModifySelectedGender(NWScript.GENDER_OTHER)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome()));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void ModifySelectedGender(int gender)
    {
      CreaturePlugin.SetGender(oPNJ, gender);
      player.oid.SendServerMessage($"Le genre de {oPNJ.Name.ColorString(ColorConstants.White)} a bien été modifié.", ColorConstants.Blue);
      DrawPNJSelectionWelcome();
    }
    private void DrawSizeList()
    {
      player.menu.Clear();
      
      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}. Quel genre souhaitez-vous appliquer ?");

      player.menu.choices.Add(("Minuscule", () => ModifySelectedSize(NWScript.CREATURE_SIZE_TINY)));
      player.menu.choices.Add(("Petite", () => ModifySelectedSize(NWScript.CREATURE_SIZE_SMALL)));
      player.menu.choices.Add(("Medium", () => ModifySelectedSize(NWScript.CREATURE_SIZE_MEDIUM)));
      player.menu.choices.Add(("Large", () => ModifySelectedSize(NWScript.CREATURE_SIZE_LARGE)));
      player.menu.choices.Add(("Enorme", () => ModifySelectedSize(NWScript.CREATURE_SIZE_HUGE)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome()));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void ModifySelectedSize(int size)
    {
      CreaturePlugin.SetSize(oPNJ, size);
      player.oid.SendServerMessage($"Le type de taille de {oPNJ.Name.ColorString(ColorConstants.White)} a bien été modifié.", ColorConstants.Blue);
      DrawPNJSelectionWelcome();
    }
    private void DrawRacialTypeList()
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}. Quel genre souhaitez-vous appliquer ?");

      player.menu.choices.Add(("Aberration", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_ABERRATION)));
      player.menu.choices.Add(("Animal", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_ANIMAL)));
      player.menu.choices.Add(("Monstre", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_BEAST)));
      player.menu.choices.Add(("Créature artificielle", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_CONSTRUCT)));
      player.menu.choices.Add(("Dragon", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_DRAGON)));
      player.menu.choices.Add(("Nain", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_DWARF)));
      player.menu.choices.Add(("Elémentaire", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_ELEMENTAL)));
      player.menu.choices.Add(("Elfe", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_ELF)));
      player.menu.choices.Add(("Féerique", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_FEY)));
      player.menu.choices.Add(("Géant", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_GIANT)));
      player.menu.choices.Add(("Gnome", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_GNOME)));
      player.menu.choices.Add(("Demi-elfe", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_HALFELF)));
      player.menu.choices.Add(("Halfelin", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_HALFLING)));
      player.menu.choices.Add(("Demi-orc", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_HALFORC)));
      player.menu.choices.Add(("Humain", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_HUMAN)));
      player.menu.choices.Add(("Gobelinoïde", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_HUMANOID_GOBLINOID)));
      player.menu.choices.Add(("Humainoïde monstrueux", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_HUMANOID_MONSTROUS)));
      player.menu.choices.Add(("Orc", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_HUMANOID_ORC)));
      player.menu.choices.Add(("Reptilien", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_HUMANOID_REPTILIAN)));
      player.menu.choices.Add(("Créature magique", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_MAGICAL_BEAST)));
      player.menu.choices.Add(("Vase", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_OOZE)));
      player.menu.choices.Add(("Extérieur", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_OUTSIDER)));
      player.menu.choices.Add(("Métamorphe", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_SHAPECHANGER)));
      player.menu.choices.Add(("Mort-vivant", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_UNDEAD)));
      player.menu.choices.Add(("Vermine", () => ModifySelectedRacialType(NWScript.RACIAL_TYPE_VERMIN)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome()));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void ModifySelectedRacialType(int racialType)
    {
      CreaturePlugin.SetRacialType(oPNJ, racialType);
      player.oid.SendServerMessage($"La race de {oPNJ.Name.ColorString(ColorConstants.White)} a bien été modifié.", ColorConstants.Blue);
      DrawPNJSelectionWelcome();
    }
    private void DrawSavingThrowList()
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}. Quel jet de sauvegarde souhaitez-vous modifier ?");

      player.menu.choices.Add(("Vigueur", () => ModifySelectedSavingThrow(NWScript.SAVING_THROW_FORT)));
      player.menu.choices.Add(("Réflexes", () => ModifySelectedSavingThrow(NWScript.SAVING_THROW_REFLEX)));
      player.menu.choices.Add(("Volonté", () => ModifySelectedSavingThrow(NWScript.SAVING_THROW_WILL)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome()));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private async void ModifySelectedSavingThrow(int savingThrow)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}.");
      player.menu.titleLines.Add($"Veuillez indiquer la valeur du jet de sauvegarde à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        CreaturePlugin.SetBaseSavingThrow(oPNJ, savingThrow, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        player.oid.SendServerMessage($"Le jet de sauvegarde de base de {oPNJ.Name.ColorString(ColorConstants.White)} a été modifiée à {int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value)}", ColorConstants.Blue);
        DrawSavingThrowList();
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private async void ModifyPNJSpellResistance()
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}.");
      player.menu.titleLines.Add($"Veuillez indiquer la valeur de résistance aux sortsà utiliser à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        CreaturePlugin.SetSpellResistance(oPNJ, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        player.oid.SendServerMessage($"La résistance aux sorts de {oPNJ.Name.ColorString(ColorConstants.White)} a été modifiée à {int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value)}", ColorConstants.Blue);
        DrawPNJSelectionWelcome();
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private void DrawPNJAppearancePage()
    {
      oPNJ.GetLocalVariable<int>("_CURRENT_HEAD").Value = NWScript.GetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, oPNJ);

      player.menu.Clear();

      player.menu.titleLines.Add($"Modification d'apparence de : {oPNJ.Name.ColorString(ColorConstants.Orange)}. Que souhaitez-vous modifier ?");

      player.menu.choices.Add(("Le type d'apparence", () => DrawModifyApparenceTypePage(-2)));

      if (NWScript.Get2DAString("appearance", "RACE", (int)oPNJ.CreatureAppearanceType).Length == 1)
      {
        player.menu.choices.Add(("La tête", () => DrawModifyHeadPage(-2)));
        player.menu.choices.Add(("Les cheveux", () => DrawModifyHairPage(-2)));
        player.menu.choices.Add(("Les yeux", () => DrawModifyEyesPage(-2)));
        player.menu.choices.Add(("Les lèvres", () => DrawModifyLipsPage(-2)));
        player.menu.choices.Add(("Les ailes", () => DrawModifyWingsPage(-2)));
        player.menu.choices.Add(("La queue", () => DrawModifyTailPage(-2)));
        player.menu.choices.Add(("Ré-appliquer le corps par défaut pour modèle complexe", () => HandleApplyDefaultModel()));
      }

      player.menu.choices.Add(("Le portrait", () => DrawModifyPortraitPage(-2)));
      player.menu.choices.Add(("La taille", () => DrawModifySizePage(-2)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome()));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private async void DrawModifyApparenceTypePage(int modification)
    {
      if (modification == -2)
        player.menu.Clear();

      player.menu.titleLines = new List<string> {
        "Faites défiler les apparences à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur d'apparence à l'oral (se référer à appearance.2da)"
        };

      int currentValue = (int)oPNJ.CreatureAppearanceType;

      if (modification > -2)
      {
        int choice = -1;
        if (player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").HasValue)
        {
          choice = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
          player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        }

        if (player.oid.LoginCreature.GetLocalVariable<int>("_AWAITING_PLAYER_INPUT").HasValue)
          player.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Value = 1;

        if (choice > -1)
          currentValue = choice;
        else if(modification == 1)
          currentValue++;
        else if (modification == -1)
          currentValue--;

        while (NWScript.Get2DAString("appearance", "RACE", currentValue).Length == 0)
        {
          if (modification == 1)
            currentValue++;
          else if (modification == -1)
            currentValue--;

          if (currentValue > 15100)
            currentValue = 0;
          if (currentValue < 0)
            currentValue = 15100;
        }

        oPNJ.CreatureAppearanceType = (AppearanceType)currentValue;
        player.menu.titleLines.Add($"Apparence actuelle : {currentValue.ToString().ColorString(ColorConstants.Lime)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Apparence actuelle : {currentValue.ToString().ColorString(ColorConstants.Lime)}");
        player.menu.choices.Add(($"Suivant", () => DrawModifyApparenceTypePage(1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyApparenceTypePage(-1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage()));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyApparenceTypePage(int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
    }
    private async void DrawModifyPortraitPage(int modification)
    {
      if (modification == -2)
        player.menu.Clear();

      player.menu.titleLines = new List<string> {
        "Faites défiler les apparences à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur d'apparence à l'oral (se référer à portrait.2da)"
        };

      int currentValue = oPNJ.PortraitId;

      if (modification > -2)
      {
        int choice = -1;
        if (player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").HasValue)
        {
          choice = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
          player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        }

        if (player.oid.LoginCreature.GetLocalVariable<int>("_AWAITING_PLAYER_INPUT").HasValue)
          player.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Value = 1;

        if (choice > -1)
          currentValue = choice;
        else if (modification == 1)
          currentValue++;
        else if (modification == -1)
          currentValue--;

        while (NWScript.Get2DAString("portraits", "BaseResRef", currentValue).Length == 0)
        {
          if (modification == 1)
            currentValue++;
          else if (modification == -1)
            currentValue--;

          if (currentValue > 12000)
            currentValue = 0;
          if (currentValue < 0)
            currentValue = 12000;
        }

        oPNJ.PortraitId = currentValue;
        player.menu.titleLines.Add($"Portrait actuel : {currentValue.ToString().ColorString(ColorConstants.Lime)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Portrait actuel : {currentValue.ToString().ColorString(ColorConstants.Lime)}");
        player.menu.choices.Add(($"Suivant", () => DrawModifyPortraitPage(1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyPortraitPage(-1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage()));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyPortraitPage(int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
    }
    private async void DrawModifySizePage(int modification)
    {
      if (modification == -2)
        player.menu.Clear();

      player.menu.titleLines = new List<string> {
        "Augmentez ou diminuez la taille.",
        "Ou bien prononcez directement une valeur de taille à l'oral."
        };

      float currentValue = oPNJ.VisualTransform.Scale;

      if (modification > -2)
      {
        int choice = -1;
        if (player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").HasValue)
        {
          choice = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
          player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        }

        if (player.oid.LoginCreature.GetLocalVariable<int>("_AWAITING_PLAYER_INPUT").HasValue)
          player.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Value = 1;

        if (choice > -1)
          currentValue = choice;
        else if (modification == 1)
          currentValue += 0.02f;
        else if (modification == -1)
          currentValue -= 0.02f;

        oPNJ.VisualTransform.Scale = currentValue;

        player.menu.titleLines.Add($"Taille actuelle : {currentValue.ToString().ColorString(ColorConstants.Lime)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Taille actuelle : {currentValue.ToString().ColorString(ColorConstants.Lime)}");
        player.menu.choices.Add(($"Suivant", () => DrawModifySizePage(1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifySizePage(-1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage()));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifySizePage(0);
    }
    private async void DrawModifyHeadPage(int modification)
    {
      if (modification == -2)
        player.menu.Clear();

      player.menu.titleLines = new List<string> {
        "Faites défiler les apparences à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur d'apparence à l'oral."
        };
      
      int currentValue = NWScript.GetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, oPNJ);
      
      if (modification > -2)
      {
        int choice = -1;
        if (player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").HasValue)
        {
          choice = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
          player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        }

        if (player.oid.LoginCreature.GetLocalVariable<int>("_AWAITING_PLAYER_INPUT").HasValue)
          player.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Value = 1;

        if (choice > -1)
          currentValue = choice;
        else if (modification == 1)
          currentValue++;
        else if (modification == -1)
          currentValue--;

        NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, currentValue, oPNJ);
        player.menu.titleLines.Add($"Tête actuelle : {currentValue.ToString().ColorString(ColorConstants.Lime)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Tête actuelle : {currentValue.ToString().ColorString(ColorConstants.Lime)}");
        player.menu.choices.Add(($"Suivant", () => DrawModifyHeadPage(1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyHeadPage(-1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage()));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyHeadPage(0);
    }
    private async void DrawModifyEyesPage(int modification)
    {
      if (modification == -2)
        player.menu.Clear();

      player.menu.titleLines = new List<string> {
        "Faites défiler les apparences à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur d'apparence à l'oral."
        };

      int currentValue = NWScript.GetColor(oPNJ, NWScript.COLOR_CHANNEL_TATTOO_1);

      if (modification > -2)
      {
        int choice = -1;
        if (player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").HasValue)
        {
          choice = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
          player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        }

        if (player.oid.LoginCreature.GetLocalVariable<int>("_AWAITING_PLAYER_INPUT").HasValue)
          player.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Value = 1;

        if (choice > -1)
          currentValue = choice;
        else if (modification == 1)
          currentValue++;
        else if (modification == -1)
          currentValue--;

        NWScript.SetColor(oPNJ, NWScript.COLOR_CHANNEL_TATTOO_1,  currentValue);
        NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, 0, oPNJ);

        Task waitHeadUpdate = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, oPNJ.GetLocalVariable<int>("_CURRENT_HEAD").Value, oPNJ);
        });

        player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(ColorConstants.Lime)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(ColorConstants.Lime)}");

        player.menu.choices.Add(($"Suivant", () => DrawModifyEyesPage(1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyEyesPage(-1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage()));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyEyesPage(0);
    }
    private async void DrawModifyLipsPage(int modification)
    {
      if (modification == -2)
        player.menu.Clear();

      player.menu.titleLines = new List<string> {
        "Faites défiler les apparences à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur d'apparence à l'oral."
        };

      int currentValue = NWScript.GetColor(oPNJ, NWScript.COLOR_CHANNEL_TATTOO_2);

      if (modification > -2)
      {
        int choice = -1;
        if (player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").HasValue)
        {
          choice = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
          player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        }

        if (player.oid.LoginCreature.GetLocalVariable<int>("_AWAITING_PLAYER_INPUT").HasValue)
          player.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Value = 1;

        if (choice > -1)
          currentValue = choice;
        else if (modification == 1)
          currentValue++;
        else if (modification == -1)
          currentValue--;

        NWScript.SetColor(oPNJ, NWScript.COLOR_CHANNEL_TATTOO_2, currentValue);
        NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, 0, oPNJ);
        
        Task waitHeadUpdate = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, oPNJ.GetLocalVariable<int>("_CURRENT_HEAD").Value, oPNJ);
        });

        player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(ColorConstants.Lime)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(ColorConstants.Lime)}");

        player.menu.choices.Add(($"Suivant", () => DrawModifyLipsPage(1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyLipsPage(-1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage()));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyLipsPage(0);
    }
    private async void DrawModifyHairPage(int modification)
    {
      if (modification == -2)
        player.menu.Clear();

      player.menu.titleLines = new List<string> {
        "Faites défiler les apparences à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur d'apparence à l'oral."
        };

      int currentValue = NWScript.GetColor(oPNJ, NWScript.COLOR_CHANNEL_HAIR);

      if (modification > -2)
      {
        int choice = -1;
        if (player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").HasValue)
        {
          choice = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
          player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        }

        if (player.oid.LoginCreature.GetLocalVariable<int>("_AWAITING_PLAYER_INPUT").HasValue)
          player.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Value = 1;

        if (choice > -1)
          currentValue = choice;
        else if (modification == 1)
          currentValue++;
        else if (modification == -1)
          currentValue--;

        NWScript.SetColor(oPNJ, NWScript.COLOR_CHANNEL_HAIR, currentValue);
        NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, 0, oPNJ);

        Task waitHeadUpdate = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, oPNJ.GetLocalVariable<int>("_CURRENT_HEAD").Value, oPNJ);
        });
        
        player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(ColorConstants.Lime)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(ColorConstants.Lime)}");

        player.menu.choices.Add(($"Suivant", () => DrawModifyHairPage(1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyHairPage(-1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage()));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyHairPage(0);
    }
    private async void DrawModifyWingsPage(int modification)
    {
      if (modification == -2)
        player.menu.Clear();

      player.menu.titleLines = new List<string> {
        "Faites défiler les apparences à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur d'apparence à l'oral."
        };
      
      int currentValue = NWScript.GetCreatureWingType(oPNJ);

      if (modification > -2)
      {
        int choice = -1;
        if (player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").HasValue)
        {
          choice = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
          player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        }

        if (player.oid.LoginCreature.GetLocalVariable<int>("_AWAITING_PLAYER_INPUT").HasValue)
          player.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Value = 1;

        if (choice > -1)
          currentValue = choice;
        else if (modification == 1)
          currentValue++;
        else if (modification == -1)
          currentValue--;

        NWScript.SetCreatureWingType(currentValue, oPNJ);
        player.menu.titleLines.Add($"Ailes actuelles : {currentValue.ToString().ColorString(ColorConstants.Lime)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Ailes actuelles : {currentValue.ToString().ColorString(ColorConstants.Lime)}");

        player.menu.choices.Add(($"Suivant", () => DrawModifyWingsPage(1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyWingsPage(-1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage()));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyWingsPage(0);
    }
    private async void DrawModifyTailPage(int modification)
    {
      if (modification == -2)
        player.menu.Clear();

      player.menu.titleLines = new List<string> {
        "Faites défiler les apparences à l'aide de Suivant et Précédent.",
        "Ou bien prononcez directement une valeur d'apparence à l'oral."
        };

      int currentValue = NWScript.GetCreatureTailType(oPNJ);

      if (modification > -2)
      {
        int choice = -1;
        if (player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").HasValue)
        {
          choice = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
          player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        }

        if (player.oid.LoginCreature.GetLocalVariable<int>("_AWAITING_PLAYER_INPUT").HasValue)
          player.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Value = 1;

        if (choice > -1)
          currentValue = choice;
        else if (modification == 1)
          currentValue++;
        else if (modification == -1)
          currentValue--;

        NWScript.SetCreatureTailType(currentValue, oPNJ);
        player.menu.titleLines.Add($"Queue actuelle : {currentValue.ToString().ColorString(ColorConstants.Lime)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Queue actuelle : {currentValue.ToString().ColorString(ColorConstants.Lime)}");

        player.menu.choices.Add(($"Suivant", () => DrawModifyTailPage(1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyTailPage(-1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage()));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyTailPage(0);
    }
    private void HandleSaveNPC()
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO savedNPC (accountName, name, serializedCreature) VALUES (@accountName, @name, @serializedCreature)" +
          $"ON CONFLICT (accountName, name) DO UPDATE SET serializedCreature = @serializedCreature;");
      NWScript.SqlBindString(query, "@accountName", player.oid.PlayerName);
      NWScript.SqlBindString(query, "@name", oPNJ.Name);
      NWScript.SqlBindString(query, "@serializedCreature", ObjectPlugin.Serialize(oPNJ));
      NWScript.SqlStep(query);

      player.oid.SendServerMessage($"Votre PNJ {oPNJ.Name.ColorString(ColorConstants.White)} a bien été enregistré.", ColorConstants.Blue);
    }
    private void DisplayPNJCreatorsList()
    {
      player.menu.Clear();

      player.menu.titleLines.Add("Quelle liste de PNJ souhaitez-vous explorer ?");

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT distinct accountName from savedNPC");
      while (NWScript.SqlStep(query) > 0)
      {
        string accountName = NWScript.SqlGetString(query, 0);
        player.menu.choices.Add((accountName, () => DrawPNJList(accountName)));
      }

      player.menu.choices.Add(("Retour", () => DrawPNJFactoryWelcome()));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));

      player.menu.Draw();
    }
    private void DrawPNJList(string accountName)
    {
      player.menu.Clear();

      player.menu.titleLines.Add("Quel PNJ souhaitez-vous sélectionner ?");

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT name from savedNPC where accountName = @accountName");
      NWScript.SqlBindString(query, "@accountName", accountName);

      while (NWScript.SqlStep(query) > 0)
      {
        string npcName = NWScript.SqlGetString(query, 0);
        player.menu.choices.Add((npcName, () => HandleNPCSelection(npcName, accountName)));
      }

      player.menu.choices.Add(("Retour", () => DisplayPNJCreatorsList()));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));

      player.menu.Draw();
    }
    private void HandleNPCSelection(string npcName, string accountName)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Sélection : {npcName.ColorString(ColorConstants.Lime)}. Que souhaitez-vous faire ?");

      player.menu.choices.Add(("Spawner ce pnj", () => ActivateSpawnLocationSelectionMode(player.oid, npcName, accountName)));

      if(player.oid.PlayerName == accountName)
        player.menu.choices.Add(("Supprimer ce pnj", () => HandleDeleteNPC(npcName)));

      player.menu.choices.Add(("Retour", () => DrawPNJList(accountName)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));

      player.menu.Draw();
    }
    private void HandleDeleteNPC(string npcName)
    {
      var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE FROM savedNPC where accountName = @accountName AND name = @name");
      NWScript.SqlBindString(deletionQuery, "@accountName", player.oid.PlayerName);
      NWScript.SqlBindString(deletionQuery, "@name", npcName);
      NWScript.SqlStep(deletionQuery);

      player.oid.SendServerMessage($"Votre PNJ {npcName.ColorString(ColorConstants.White)} a bien été supprimé", ColorConstants.Blue);
    }
    private void ActivateSpawnLocationSelectionMode(NwPlayer oPC, string npcName, string accountName)
    {
      oPC.LoginCreature.GetLocalVariable<string>("_SPAWNING_NPC").Value = npcName;
      oPC.LoginCreature.GetLocalVariable<string>("_SPAWNING_NPC_ACCOUNT").Value = accountName;
      PlayerSystem.cursorTargetService.EnterTargetMode(oPC, OnPNJSpawnLocationSelected, ObjectTypes.All, MouseCursor.Create);
    }
    private void OnPNJSpawnLocationSelected(ModuleEvents.OnPlayerTarget selection)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT serializedCreature from savedNPC where accountName = @accountName and name = @name");
      NWScript.SqlBindString(query, "@accountName", selection.Player.LoginCreature.GetLocalVariable<string>("_SPAWNING_NPC_ACCOUNT").Value);
      NWScript.SqlBindString(query, "@name", selection.Player.LoginCreature.GetLocalVariable<string>("_SPAWNING_NPC").Value);

      if (NWScript.SqlStep(query) > 0)
      {
        NwCreature oNPC = NwCreature.Deserialize(NWScript.SqlGetString(query, 0).ToByteArray());
        oNPC.Location = API.Location.Create(selection.Player.ControlledCreature.Area, selection.TargetPosition, selection.Player.ControlledCreature.Rotation);
      }
    }
    private void HandleApplyDefaultModel()
    {
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_BELT, 0, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_LEFT_BICEP, 1, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_RIGHT_BICEP, 1, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_LEFT_FOOT, 1, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_RIGHT_FOOT, 1, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_LEFT_FOREARM, 1, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_RIGHT_FOREARM, 1, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_LEFT_HAND, 1, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_RIGHT_HAND, 1, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_LEFT_SHIN, 1, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_RIGHT_SHIN, 1, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_LEFT_SHOULDER, 0, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_RIGHT_SHOULDER, 0, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_LEFT_THIGH, 1, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_RIGHT_THIGH, 1, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_NECK, 1, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_PELVIS, 1, oPNJ);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_TORSO, 1, oPNJ);
    }
    private async void DisplayPNJSkin()
    {
      NwItem skin = oPNJ.GetItemInSlot(InventorySlot.CreatureSkin);

      if (skin == null)
       skin = await CreateGenericSkin();

      await player.oid.ControlledCreature.ClearActionQueue();
      await player.oid.ActionExamine(skin);
    }
    private void SelectDamageType()
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}.");
      player.menu.titleLines.Add($"Contre quel type de dégâts la créature doit-elle être protégée ?");

      player.menu.choices.Add(("Physique", () => SelectResistanceType(4)));
      player.menu.choices.Add(("Elémentaire", () => SelectResistanceType(14)));
      player.menu.choices.Add(("Contondant", () => SelectResistanceType(1)));
      player.menu.choices.Add(("Perçant", () => SelectResistanceType(2)));
      player.menu.choices.Add(("Tranchant", () => SelectResistanceType(3)));
      player.menu.choices.Add(("Magique", () => SelectResistanceType(5)));
      player.menu.choices.Add(("Froid", () => SelectResistanceType(7)));
      player.menu.choices.Add(("Acide", () => SelectResistanceType(6)));
      player.menu.choices.Add(("Electricité", () => SelectResistanceType(9)));
      player.menu.choices.Add(("Feu", () => SelectResistanceType(10)));
      player.menu.choices.Add(("Sonique", () => SelectResistanceType(13)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome()));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void SelectResistanceType(int damageType)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}.");
      player.menu.titleLines.Add($"Quel type de résistance doit être prise en compte ?");
      
      player.menu.choices.Add(("Résistance normale", () => AskResistanceValue(damageType)));
      player.menu.choices.Add(("Immunité", () => AskImmunityValue(damageType)));
      player.menu.choices.Add(("Absorption", () => AskAbsorptionValue(damageType)));

      player.menu.choices.Add(("Retour", () => SelectDamageType()));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }

    private async void AskResistanceValue(int damageType)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}.");
      player.menu.titleLines.Add($"Quelle valeur de résistance (doit être compris entre 1 et 100) ?");
      player.menu.titleLines.Add($"0 pour supprimer la résistance.");
      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputByte();

      if (awaitedValue)
        SetResistance(damageType);
    }
    private async void SetResistance(int damageType)
    {
      int playerInput = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT"));
      player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();

      NwItem skin = oPNJ.GetItemInSlot(InventorySlot.CreatureSkin);

      if (skin == null)
        skin = await CreateGenericSkin();

      if (playerInput > 100)
        playerInput = 100;

      if (playerInput == 0)
      {
        foreach (ItemProperty ip in skin.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.AcBonusVsDamageType && i.SubType == damageType))
          skin.RemoveItemProperty(ip);

        player.oid.SendServerMessage("La résistance a bien été supprimée.", ColorConstants.Rose);
      }
      else
      {
        skin.AddItemProperty(ItemProperty.ACBonusVsDmgType((IPDamageType)damageType, playerInput), EffectDuration.Permanent);
        player.oid.SendServerMessage("La résistance a bien été ajoutée.", ColorConstants.Rose);
      }

      SelectDamageType();
    }
    private async void AskImmunityValue(int damageType)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}.");
      player.menu.titleLines.Add($"Quelle valeur d'immunité (doit être compris entre 1 et 7) ?");
      player.menu.titleLines.Add($"1 = 5 %, 2 = 10, 3 = 25, 4 = 50, 5 = 75, 6 = 90, 7 = 100 %.");
      player.menu.titleLines.Add($"0 pour supprimer l'immunité.");
      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputByte();

      if (awaitedValue)
        SetImmunity(damageType);
    }
    private async void SetImmunity(int damageType)
    {
      int playerInput = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT"));
      player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();

      NwItem skin = oPNJ.GetItemInSlot(InventorySlot.CreatureSkin);

      if (skin == null)
        skin = await CreateGenericSkin();

      if (playerInput > 7)
        playerInput = 7;

      if (playerInput == 0)
      {
        foreach (ItemProperty ip in skin.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && i.SubType == damageType))
          skin.RemoveItemProperty(ip);

        player.oid.SendServerMessage("L'immunité a bien été supprimée.", ColorConstants.Rose);
      }
      else
      {
        skin.AddItemProperty(ItemProperty.DamageImmunity((IPDamageType)damageType, (IPDamageImmunityType)playerInput), EffectDuration.Permanent);
        player.oid.SendServerMessage("L'immunité a bien été ajoutée.", ColorConstants.Rose);
      }

      SelectDamageType();
    }
    private async void AskAbsorptionValue(int damageType)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(ColorConstants.Orange)}.");
      player.menu.titleLines.Add($"Quelle valeur d'absorption (doit être compris entre 8 et 11) ?");
      player.menu.titleLines.Add($"8 = 25 %, 9 = 50, 10 = 75, 11 = 100 %.");
      player.menu.titleLines.Add($"0 pour supprimer l'absorption.");
      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputByte();

      if (awaitedValue)
        SetAbsorption(damageType);
    }
    private async void SetAbsorption(int damageType)
    {
      int playerInput = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT"));
      player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();

      NwItem skin = oPNJ.GetItemInSlot(InventorySlot.CreatureSkin);

      if (skin == null)
        skin = await CreateGenericSkin();

      if (playerInput > 0 && playerInput < 8)
        playerInput = 8;

      if (playerInput > 11)
        playerInput = 11;

      if (playerInput == 0)
      {
        foreach (ItemProperty ip in skin.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && i.SubType == damageType))
          skin.RemoveItemProperty(ip);

        player.oid.SendServerMessage("L'absorption a bien été supprimée.", ColorConstants.Rose);
      }
      else
      {
        skin.AddItemProperty(ItemProperty.DamageImmunity((IPDamageType)damageType, (IPDamageImmunityType)playerInput), EffectDuration.Permanent);
        player.oid.SendServerMessage("L'absorption a bien été ajoutée.", ColorConstants.Rose);
      }

      SelectDamageType();
    }
    private async Task<NwItem> CreateGenericSkin()
    {
      NwItem skin = await NwItem.Create("peaudejoueur", oPNJ);
      skin.Name = $"Propriétés de {oPNJ.Name}";
      CreaturePlugin.RunEquip(oPNJ, skin, (int)InventorySlot.CreatureSkin);
      return skin;
    }
  }
}