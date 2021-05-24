﻿using NWN.API.Constants;
using NWN.API;
using static NWN.Systems.PlayerSystem;
using NWN.Core;
using System.Threading.Tasks;
using NWN.Core.NWNX;
using System.Collections.Generic;
using System;
using NWN.API.Events;

namespace NWN.Systems
{
  class PNJFactory
  {
    public PNJFactory(Player player)
    {
      DrawPNJFactoryWelcome(player);
    }
    private static void DrawPNJFactoryWelcome(Player player)
    {
      player.menu.Clear();

      player.menu.titleLines.Add("Bienvenue dans la PNJ factory. Que souhaitez-vous faire ?");

      player.menu.choices.Add(("Sélectionner un PNJ existant", () => ActivateSelectionMode(player.oid)));
      player.menu.choices.Add(("Parcourir les listes de PNJs enregistrés", () => DisplayPNJCreatorsList(player)));

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private static void ActivateSelectionMode(NwPlayer oPC)
    {
      cursorTargetService.EnterTargetMode(oPC, OnPNJSelected, ObjectTypes.Creature, MouseCursor.Create);
    }
    public static void OnPNJSelected(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.TargetObject == null)
        return;

      if (selection.TargetObject is NwPlayer && selection.Player.PlayerName != "Chim")
      {
        selection.Player.SendServerMessage("Seuls les pnjs peuvent être modifiés à l'aide de cet outil.");
        return;
      }

      DrawPNJSelectionWelcome(selection.Player, (NwCreature)selection.TargetObject);
    }
    private static void DrawPNJSelectionWelcome(NwPlayer oPC, NwCreature oPNJ)
    {
      if (!Players.TryGetValue(oPC.LoginCreature, out Player player))
        return;

      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)} Que souhaitez-vous faire ?");

      player.menu.choices.Add(("Modifier le prénom", () => ModifyPNJName(player, oPNJ, 0)));
      player.menu.choices.Add(("Modifier le nom", () => ModifyPNJName(player, oPNJ, 1)));
      player.menu.choices.Add(("Modifier la description", () => ModifyPNJDescription(player, oPNJ)));
      player.menu.choices.Add(("Modifier l'apparence", () => DrawPNJAppearancePage(player, oPNJ)));
      player.menu.choices.Add(("Modifier la CA de base", () => ModifyPNJBaseArmorClass(player, oPNJ)));
      player.menu.choices.Add(("Modifier les caractéristiques", () => DrawAbilityList(player, oPNJ)));
      player.menu.choices.Add(("Modifier les points de vie", () => ModifyPNJHealthByLevel(player, oPNJ)));
      player.menu.choices.Add(("Modifier la vitesse", () => DrawSpeedList(player, oPNJ)));
      player.menu.choices.Add(("Modifier la voix", () => ModifyPNJSoundset(player, oPNJ)));
      player.menu.choices.Add(("Modifier l'attaque de base", () => ModifyPNJBaseAttack(player, oPNJ)));
      player.menu.choices.Add(("Modifier le sexe", () => DrawGenderList(player, oPNJ)));
      player.menu.choices.Add(("Modifier le type de taille", () => DrawSizeList(player, oPNJ)));
      player.menu.choices.Add(("Modifier la race", () => DrawRacialTypeList(player, oPNJ)));
      player.menu.choices.Add(("Modifier les jets de sauvegarde", () => DrawSavingThrowList(player, oPNJ)));
      player.menu.choices.Add(("Modifier la résistance aux sorts", () => ModifyPNJSpellResistance(player, oPNJ)));

      player.menu.choices.Add(("Sauvegarder ce PNJ", () => HandleSaveNPC(player, oPNJ)));

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private static async void ModifyPNJName(Player player, NwCreature oPNJ, int isLastName)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}.");
      player.menu.titleLines.Add($"Veuillez indiquer le nouveau nom à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        CreaturePlugin.SetOriginalName(oPNJ, player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value, isLastName);
        oPNJ.Name = player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value;
        player.oid.SendServerMessage($"Le nom du PNJ a été modifié à {oPNJ.Name.ColorString(Color.WHITE)}", Color.BLUE);
        DrawPNJSelectionWelcome(player.oid, oPNJ);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private static async void ModifyPNJDescription(Player player, NwCreature oPNJ)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}.");
      player.menu.titleLines.Add($"Veuillez indiquer la nouvelle description à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        oPNJ.Description = player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value;
        player.oid.SendServerMessage($"La description de {oPNJ.Name.ColorString(Color.WHITE)} a été modifiée.", Color.BLUE);
        DrawPNJSelectionWelcome(player.oid, oPNJ);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private static async void ModifyPNJBaseArmorClass(Player player, NwCreature oPNJ)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}.");
      player.menu.titleLines.Add($"Veuillez indiquer la valeur de CA de base à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        CreaturePlugin.SetBaseAC(oPNJ, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        player.oid.SendServerMessage($"La CA de base de {oPNJ.Name.ColorString(Color.WHITE)} a été modifiée à {int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value)}", Color.BLUE);
        DrawPNJSelectionWelcome(player.oid, oPNJ);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private static void DrawAbilityList(Player player, NwCreature oPNJ)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}. Quel caractéristique souhaitez-vous modifier ?");

      player.menu.choices.Add(("Force", () => ModifySelectedAbility(player, oPNJ, NWScript.ABILITY_STRENGTH)));
      player.menu.choices.Add(("Dextérité", () => ModifySelectedAbility(player, oPNJ, NWScript.ABILITY_DEXTERITY)));
      player.menu.choices.Add(("Constitution", () => ModifySelectedAbility(player, oPNJ, NWScript.ABILITY_CONSTITUTION)));
      player.menu.choices.Add(("Intelligence", () => ModifySelectedAbility(player, oPNJ, NWScript.ABILITY_INTELLIGENCE)));
      player.menu.choices.Add(("Sagesse", () => ModifySelectedAbility(player, oPNJ, NWScript.ABILITY_WISDOM)));
      player.menu.choices.Add(("Charisme", () => ModifySelectedAbility(player, oPNJ, NWScript.ABILITY_CHARISMA)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome(player.oid, oPNJ)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private static async void ModifySelectedAbility(Player player, NwCreature oPNJ, int ability)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}.");
      player.menu.titleLines.Add($"Veuillez indiquer la valeur de la caractéristique à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        CreaturePlugin.SetRawAbilityScore(oPNJ, ability, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        player.oid.SendServerMessage($"La caractéristique de base de {oPNJ.Name.ColorString(Color.WHITE)} a été modifiée à {int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value)}", Color.BLUE);
        DrawPNJSelectionWelcome(player.oid, oPNJ);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private static async void ModifyPNJHealthByLevel(Player player, NwCreature oPNJ)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}.");
      player.menu.titleLines.Add($"Veuillez indiquer le nombre de points de vie par niveau à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        if (oPNJ is NwPlayer)
          CreaturePlugin.SetMaxHitPointsByLevel(oPNJ, 1, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        else
          ObjectPlugin.SetMaxHitPoints(oPNJ, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));

        player.oid.SendServerMessage($"Le nombre de points de vie de {oPNJ.Name.ColorString(Color.WHITE)} a été modifiée à {int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value)}", Color.BLUE);
        DrawPNJSelectionWelcome(player.oid, oPNJ);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private static void DrawSpeedList(Player player, NwCreature oPNJ)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}. Quelle vitesse souhaitez-vous appliquer ?");

      player.menu.choices.Add(("Par défaut", () => ModifySelectedSpeed(player, oPNJ, MovementRate.CreatureDefault)));
      player.menu.choices.Add(("Immobile", () => ModifySelectedSpeed(player, oPNJ, MovementRate.Immobile)));
      player.menu.choices.Add(("Très lent", () => ModifySelectedSpeed(player, oPNJ, MovementRate.VerySlow)));
      player.menu.choices.Add(("Lent", () => ModifySelectedSpeed(player, oPNJ, MovementRate.Slow)));
      player.menu.choices.Add(("Normal", () => ModifySelectedSpeed(player, oPNJ, MovementRate.Normal)));
      player.menu.choices.Add(("Rapide", () => ModifySelectedSpeed(player, oPNJ, MovementRate.Fast)));
      player.menu.choices.Add(("Très rapide", () => ModifySelectedSpeed(player, oPNJ, MovementRate.VeryFast)));
      player.menu.choices.Add(("Vitesse DM", () => ModifySelectedSpeed(player, oPNJ, MovementRate.DM)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome(player.oid, oPNJ)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private static void ModifySelectedSpeed(Player player, NwCreature oPNJ, MovementRate speed)
    {
      oPNJ.MovementRate = speed;
      player.oid.SendServerMessage($"La vitesse de {oPNJ.Name.ColorString(Color.WHITE)} a bien été modifiée.", Color.BLUE);
      DrawPNJSelectionWelcome(player.oid, oPNJ);
    }
    private static async void ModifyPNJSoundset(Player player, NwCreature oPNJ)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}.");
      player.menu.titleLines.Add($"Veuillez indiquer la voix à utiliser à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        CreaturePlugin.SetSoundset(oPNJ, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        player.oid.SendServerMessage($"La voix de {oPNJ.Name.ColorString(Color.WHITE)} a été modifiée à {int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value)}", Color.BLUE);
        DrawPNJSelectionWelcome(player.oid, oPNJ);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private static async void ModifyPNJBaseAttack(Player player, NwCreature oPNJ)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}.");
      player.menu.titleLines.Add($"Veuillez indiquer la valeur d'attaque de base à utiliser à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        CreaturePlugin.SetBaseAttackBonus(oPNJ, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        player.oid.SendServerMessage($"L'attaque de base de {oPNJ.Name.ColorString(Color.WHITE)} a été modifiée à {int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value)}", Color.BLUE);
        DrawPNJSelectionWelcome(player.oid, oPNJ);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private static void DrawGenderList(Player player, NwCreature oPNJ)
    {
      player.menu.Clear();
      
      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}. Quel genre souhaitez-vous appliquer ?");
      
      player.menu.choices.Add(("Masculin", () => ModifySelectedGender(player, oPNJ, NWScript.GENDER_MALE)));
      player.menu.choices.Add(("Féminin", () => ModifySelectedGender(player, oPNJ, NWScript.GENDER_FEMALE)));
      player.menu.choices.Add(("Les deux", () => ModifySelectedGender(player, oPNJ, NWScript.GENDER_BOTH)));
      player.menu.choices.Add(("Aucun", () => ModifySelectedGender(player, oPNJ, NWScript.GENDER_NONE)));
      player.menu.choices.Add(("Autre", () => ModifySelectedGender(player, oPNJ, NWScript.GENDER_OTHER)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome(player.oid, oPNJ)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private static void ModifySelectedGender(Player player, NwCreature oPNJ, int gender)
    {
      CreaturePlugin.SetGender(oPNJ, gender);
      player.oid.SendServerMessage($"Le genre de {oPNJ.Name.ColorString(Color.WHITE)} a bien été modifié.", Color.BLUE);
      DrawPNJSelectionWelcome(player.oid, oPNJ);
    }
    private static void DrawSizeList(Player player, NwCreature oPNJ)
    {
      player.menu.Clear();
      
      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}. Quel genre souhaitez-vous appliquer ?");

      player.menu.choices.Add(("Minuscule", () => ModifySelectedSize(player, oPNJ, NWScript.CREATURE_SIZE_TINY)));
      player.menu.choices.Add(("Petite", () => ModifySelectedSize(player, oPNJ, NWScript.CREATURE_SIZE_SMALL)));
      player.menu.choices.Add(("Medium", () => ModifySelectedSize(player, oPNJ, NWScript.CREATURE_SIZE_MEDIUM)));
      player.menu.choices.Add(("Large", () => ModifySelectedSize(player, oPNJ, NWScript.CREATURE_SIZE_LARGE)));
      player.menu.choices.Add(("Enorme", () => ModifySelectedSize(player, oPNJ, NWScript.CREATURE_SIZE_HUGE)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome(player.oid, oPNJ)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private static void ModifySelectedSize(Player player, NwCreature oPNJ, int size)
    {
      CreaturePlugin.SetSize(oPNJ, size);
      player.oid.SendServerMessage($"Le type de taille de {oPNJ.Name.ColorString(Color.WHITE)} a bien été modifié.", Color.BLUE);
      DrawPNJSelectionWelcome(player.oid, oPNJ);
    }
    private static void DrawRacialTypeList(Player player, NwCreature oPNJ)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}. Quel genre souhaitez-vous appliquer ?");

      player.menu.choices.Add(("Aberration", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_ABERRATION)));
      player.menu.choices.Add(("Animal", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_ANIMAL)));
      player.menu.choices.Add(("Monstre", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_BEAST)));
      player.menu.choices.Add(("Créature artificielle", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_CONSTRUCT)));
      player.menu.choices.Add(("Dragon", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_DRAGON)));
      player.menu.choices.Add(("Nain", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_DWARF)));
      player.menu.choices.Add(("Elémentaire", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_ELEMENTAL)));
      player.menu.choices.Add(("Elfe", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_ELF)));
      player.menu.choices.Add(("Féerique", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_FEY)));
      player.menu.choices.Add(("Géant", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_GIANT)));
      player.menu.choices.Add(("Gnome", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_GNOME)));
      player.menu.choices.Add(("Demi-elfe", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_HALFELF)));
      player.menu.choices.Add(("Halfelin", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_HALFLING)));
      player.menu.choices.Add(("Demi-orc", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_HALFORC)));
      player.menu.choices.Add(("Humain", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_HUMAN)));
      player.menu.choices.Add(("Gobelinoïde", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_HUMANOID_GOBLINOID)));
      player.menu.choices.Add(("Humainoïde monstrueux", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_HUMANOID_MONSTROUS)));
      player.menu.choices.Add(("Orc", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_HUMANOID_ORC)));
      player.menu.choices.Add(("Reptilien", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_HUMANOID_REPTILIAN)));
      player.menu.choices.Add(("Créature magique", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_MAGICAL_BEAST)));
      player.menu.choices.Add(("Vase", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_OOZE)));
      player.menu.choices.Add(("Extérieur", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_OUTSIDER)));
      player.menu.choices.Add(("Métamorphe", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_SHAPECHANGER)));
      player.menu.choices.Add(("Mort-vivant", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_UNDEAD)));
      player.menu.choices.Add(("Vermine", () => ModifySelectedRacialType(player, oPNJ, NWScript.RACIAL_TYPE_VERMIN)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome(player.oid, oPNJ)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private static void ModifySelectedRacialType(Player player, NwCreature oPNJ, int racialType)
    {
      CreaturePlugin.SetRacialType(oPNJ, racialType);
      player.oid.SendServerMessage($"La race de {oPNJ.Name.ColorString(Color.WHITE)} a bien été modifié.", Color.BLUE);
      DrawPNJSelectionWelcome(player.oid, oPNJ);
    }
    private static void DrawSavingThrowList(Player player, NwCreature oPNJ)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}. Quel jet de sauvegarde souhaitez-vous modifier ?");

      player.menu.choices.Add(("Vigueur", () => ModifySelectedSavingThrow(player, oPNJ, NWScript.SAVING_THROW_FORT)));
      player.menu.choices.Add(("Réflexes", () => ModifySelectedSavingThrow(player, oPNJ, NWScript.SAVING_THROW_REFLEX)));
      player.menu.choices.Add(("Volonté", () => ModifySelectedSavingThrow(player, oPNJ, NWScript.SAVING_THROW_WILL)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome(player.oid, oPNJ)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private static async void ModifySelectedSavingThrow(Player player, NwCreature oPNJ, int savingThrow)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}.");
      player.menu.titleLines.Add($"Veuillez indiquer la valeur du jet de sauvegarde à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        CreaturePlugin.SetBaseSavingThrow(oPNJ, savingThrow, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        player.oid.SendServerMessage($"Le jet de sauvegarde de base de {oPNJ.Name.ColorString(Color.WHITE)} a été modifiée à {int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value)}", Color.BLUE);
        DrawSavingThrowList(player, oPNJ);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private static async void ModifyPNJSpellResistance(Player player, NwCreature oPNJ)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Cible de la sélection : {oPNJ.Name.ColorString(Color.ORANGE)}.");
      player.menu.titleLines.Add($"Veuillez indiquer la valeur de résistance aux sortsà utiliser à l'oral.");

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        CreaturePlugin.SetSpellResistance(oPNJ, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        player.oid.SendServerMessage($"La résistance aux sorts de {oPNJ.Name.ColorString(Color.WHITE)} a été modifiée à {int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value)}", Color.BLUE);
        DrawPNJSelectionWelcome(player.oid, oPNJ);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private static void DrawPNJAppearancePage(Player player, NwCreature oPNJ)
    {
      oPNJ.GetLocalVariable<int>("_CURRENT_HEAD").Value = NWScript.GetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, oPNJ);

      player.menu.Clear();

      player.menu.titleLines.Add($"Modification d'apparence de : {oPNJ.Name.ColorString(Color.ORANGE)}. Que souhaitez-vous modifier ?");

      player.menu.choices.Add(("Le type d'apparence", () => DrawModifyApparenceTypePage(player, oPNJ, -2)));

      if (NWScript.Get2DAString("appearance", "RACE", (int)oPNJ.CreatureAppearanceType).Length == 1)
      {
        player.menu.choices.Add(("La tête", () => DrawModifyHeadPage(player, oPNJ, -2)));
        player.menu.choices.Add(("Les cheveux", () => DrawModifyHairPage(player, oPNJ, -2)));
        player.menu.choices.Add(("Les yeux", () => DrawModifyEyesPage(player, oPNJ, -2)));
        player.menu.choices.Add(("Les lèvres", () => DrawModifyLipsPage(player, oPNJ, -2)));
        player.menu.choices.Add(("Les ailes", () => DrawModifyWingsPage(player, oPNJ, -2)));
        player.menu.choices.Add(("La queue", () => DrawModifyTailPage(player, oPNJ, -2)));
        player.menu.choices.Add(("Ré-appliquer le corps par défaut pour modèle complexe", () => HandleApplyDefaultModel(player, oPNJ)));
      }

      player.menu.choices.Add(("Le portrait", () => DrawModifyPortraitPage(player, oPNJ, -2)));
      player.menu.choices.Add(("La taille", () => DrawModifySizePage(player, oPNJ, -2)));

      player.menu.choices.Add(("Retour", () => DrawPNJSelectionWelcome(player.oid, oPNJ)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private static async void DrawModifyApparenceTypePage(Player player, NwCreature oPNJ, int modification)
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
        player.menu.titleLines.Add($"Apparence actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Apparence actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        player.menu.choices.Add(($"Suivant", () => DrawModifyApparenceTypePage(player, oPNJ, 1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyApparenceTypePage(player, oPNJ, -1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage(player, oPNJ)));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyApparenceTypePage(player, oPNJ, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
    }
    private static async void DrawModifyPortraitPage(Player player, NwCreature oPNJ, int modification)
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
        player.menu.titleLines.Add($"Portrait actuel : {currentValue.ToString().ColorString(Color.LIME)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Portrait actuel : {currentValue.ToString().ColorString(Color.LIME)}");
        player.menu.choices.Add(($"Suivant", () => DrawModifyPortraitPage(player, oPNJ, 1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyPortraitPage(player, oPNJ, -1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage(player, oPNJ)));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyPortraitPage(player, oPNJ, int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
    }
    private static async void DrawModifySizePage(Player player, NwCreature oPNJ, int modification)
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

        VisualTransform scale = oPNJ.VisualTransform;
        scale.Scale = currentValue;
        oPNJ.VisualTransform = scale;

        player.menu.titleLines.Add($"Taille actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Taille actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        player.menu.choices.Add(($"Suivant", () => DrawModifySizePage(player, oPNJ, 1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifySizePage(player, oPNJ, -1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage(player, oPNJ)));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifySizePage(player, oPNJ, 0);
    }
    private static async void DrawModifyHeadPage(Player player, NwCreature oPNJ, int modification)
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
        player.menu.titleLines.Add($"Tête actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Tête actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        player.menu.choices.Add(($"Suivant", () => DrawModifyHeadPage(player, oPNJ, 1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyHeadPage(player, oPNJ, -1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage(player, oPNJ)));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyHeadPage(player, oPNJ, 0);
    }
    private static async void DrawModifyEyesPage(Player player, NwCreature oPNJ, int modification)
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

        player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(Color.LIME)}");

        player.menu.choices.Add(($"Suivant", () => DrawModifyEyesPage(player, oPNJ, 1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyEyesPage(player, oPNJ, -1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage(player, oPNJ)));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyEyesPage(player, oPNJ, 0);
    }
    private static async void DrawModifyLipsPage(Player player, NwCreature oPNJ, int modification)
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

        player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(Color.LIME)}");

        player.menu.choices.Add(($"Suivant", () => DrawModifyLipsPage(player, oPNJ, 1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyLipsPage(player, oPNJ, -1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage(player, oPNJ)));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyLipsPage(player, oPNJ, 0);
    }
    private static async void DrawModifyHairPage(Player player, NwCreature oPNJ, int modification)
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
        
        player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Couleur actuelle : {currentValue.ToString().ColorString(Color.LIME)}");

        player.menu.choices.Add(($"Suivant", () => DrawModifyHairPage(player, oPNJ, 1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyHairPage(player, oPNJ, -1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage(player, oPNJ)));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyHairPage(player, oPNJ, 0);
    }
    private static async void DrawModifyWingsPage(Player player, NwCreature oPNJ, int modification)
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
        player.menu.titleLines.Add($"Ailes actuelles : {currentValue.ToString().ColorString(Color.LIME)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Ailes actuelles : {currentValue.ToString().ColorString(Color.LIME)}");

        player.menu.choices.Add(($"Suivant", () => DrawModifyWingsPage(player, oPNJ, 1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyWingsPage(player, oPNJ, -1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage(player, oPNJ)));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyWingsPage(player, oPNJ, 0);
    }
    private static async void DrawModifyTailPage(Player player, NwCreature oPNJ, int modification)
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
        player.menu.titleLines.Add($"Queue actuelle : {currentValue.ToString().ColorString(Color.LIME)}");
        player.menu.DrawText();
      }
      else
      {
        player.menu.titleLines.Add($"Queue actuelle : {currentValue.ToString().ColorString(Color.LIME)}");

        player.menu.choices.Add(($"Suivant", () => DrawModifyTailPage(player, oPNJ, 1)));
        player.menu.choices.Add(($"Précédent.", () => DrawModifyTailPage(player, oPNJ, -1)));

        player.menu.choices.Add(("Retour.", () => DrawPNJAppearancePage(player, oPNJ)));
        player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

        player.menu.Draw();
      }

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
        DrawModifyTailPage(player, oPNJ, 0);
    }
    private static void HandleSaveNPC(Player player, NwCreature oPNJ)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO savedNPC (accountName, name, serializedCreature) VALUES (@accountName, @name, @serializedCreature)" +
          $"ON CONFLICT (accountName, name) DO UPDATE SET serializedCreature = @serializedCreature;");
      NWScript.SqlBindString(query, "@accountName", player.oid.PlayerName);
      NWScript.SqlBindString(query, "@name", oPNJ.Name);
      NWScript.SqlBindString(query, "@serializedCreature", ObjectPlugin.Serialize(oPNJ));
      NWScript.SqlStep(query);

      player.oid.SendServerMessage($"Votre PNJ {oPNJ.Name.ColorString(Color.WHITE)} a bien été enregistré.", Color.BLUE);
    }
    private static void DisplayPNJCreatorsList(Player player)
    {
      player.menu.Clear();

      player.menu.titleLines.Add("Quelle liste de PNJ souhaitez-vous explorer ?");

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT distinct accountName from savedNPC");
      while (NWScript.SqlStep(query) > 0)
      {
        string accountName = NWScript.SqlGetString(query, 0);
        player.menu.choices.Add((accountName, () => DrawPNJList(player, accountName)));
      }

      player.menu.choices.Add(("Retour", () => DrawPNJFactoryWelcome(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));

      player.menu.Draw();
    }
    private static void DrawPNJList(Player player, string accountName)
    {
      player.menu.Clear();

      player.menu.titleLines.Add("Quel PNJ souhaitez-vous sélectionner ?");

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT name from savedNPC where accountName = @accountName");
      NWScript.SqlBindString(query, "@accountName", accountName);

      while (NWScript.SqlStep(query) > 0)
      {
        string npcName = NWScript.SqlGetString(query, 0);
        player.menu.choices.Add((npcName, () => HandleNPCSelection(player, npcName, accountName)));
      }

      player.menu.choices.Add(("Retour", () => DisplayPNJCreatorsList(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));

      player.menu.Draw();
    }
    private static void HandleNPCSelection(Player player, string npcName, string accountName)
    {
      player.menu.Clear();

      player.menu.titleLines.Add($"Sélection : {npcName.ColorString(Color.LIME)}. Que souhaitez-vous faire ?");

      player.menu.choices.Add(("Spawner ce pnj", () => ActivateSpawnLocationSelectionMode(player.oid, npcName, accountName)));

      if(player.oid.PlayerName == accountName)
        player.menu.choices.Add(("Supprimer ce pnj", () => HandleDeleteNPC(player, npcName)));

      player.menu.choices.Add(("Retour", () => DrawPNJList(player, accountName)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));

      player.menu.Draw();
    }
    private static void HandleDeleteNPC(Player player, string npcName)
    {
      var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE FROM savedNPC where accountName = @accountName AND name = @name");
      NWScript.SqlBindString(deletionQuery, "@accountName", player.oid.PlayerName);
      NWScript.SqlBindString(deletionQuery, "@name", npcName);
      NWScript.SqlStep(deletionQuery);

      player.oid.SendServerMessage($"Votre PNJ {npcName.ColorString(Color.WHITE)} a bien été supprimé", Color.BLUE);
    }
    private static void ActivateSpawnLocationSelectionMode(NwPlayer oPC, string npcName, string accountName)
    {
      oPC.LoginCreature.GetLocalVariable<string>("_SPAWNING_NPC").Value = npcName;
      oPC.LoginCreature.GetLocalVariable<string>("_SPAWNING_NPC_ACCOUNT").Value = accountName;
      cursorTargetService.EnterTargetMode(oPC, OnPNJSpawnLocationSelected, ObjectTypes.All, MouseCursor.Create);
    }
    private static void OnPNJSpawnLocationSelected(ModuleEvents.OnPlayerTarget selection)
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
    private static void HandleApplyDefaultModel(Player player, NwCreature oPNJ)
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
  }
}
