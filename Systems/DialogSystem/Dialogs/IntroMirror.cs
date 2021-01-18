using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  class IntroMirror
  {
    public IntroMirror(Player player)
    {
      this.DrawWelcomePage(player);
    }
    private void DrawWelcomePage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Houla, y a pas à dire, vous avez connu de meilleurs jours.",
        "C'est quoi cette mine que vous me tirez ?",
        "On va mettre ça sur le compte du mal de mer."
      };
      player.menu.choices.Add(($"Me refaire une beauté.", () => HandleBodyCloneSpawn(player)));

      if(!Convert.ToBoolean(NWScript.GetLocalInt(NWScript.GetNearestObjectByTag("intro_mirror", player.oid), "_TRAIT_SELECTED")))
        player.menu.choices.Add(($"Me perdre brièvement dans le passé.", () => HandleBackgroundChoice(player)));
      
      if(ObjectPlugin.GetInt(player.oid, "_STARTING_SKILL_POINTS") > 0)
        player.menu.choices.Add(($"Me préparer à l'avenir.", () => HandleSkillSelection(player)));
      
      player.menu.choices.Add(("M'éloigner du miroir.", () => ExitDialog(player)));
      player.menu.Draw();

      RestoreMirror(player);
    }
    private void HandleBodyCloneSpawn(Player player)
    {
      uint oMirror = NWScript.GetNearestObjectByTag("intro_mirror", player.oid);
      uint oClone = NWScript.CopyObject(player.oid, NWScript.GetLocation(oMirror), NWScript.OBJECT_INVALID, $"clone_{NWScript.GetPCPublicCDKey(player.oid)}");
      VisibilityPlugin.SetVisibilityOverride(player.oid, oMirror, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
      NWScript.SetName(oClone, $"Reflet de {NWScript.GetName(player.oid)}");
      ObjectPlugin.SetFacing(oClone, NWScript.GetFacing(oMirror) + 180);
      ObjectPlugin.SetPosition(oClone, NWScript.GetPosition(oMirror));

      HandleBodyModification(player, oClone);
    }
    private void HandleBodyModification(Player player, uint oClone)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Vous vous concentrez sur le miroir de façon à mieux vous mirer.",
        "Qu'ajusteriez-vous dans ce reflet ?"
      };
      player.menu.choices.Add(($"Apparence suivante.", () => ChangeCloneHead(player, oClone, 1)));
      player.menu.choices.Add(($"Apparence précédente.", () => ChangeCloneHead(player, oClone, -1)));

      if (NWScript.GetObjectVisualTransform(oClone, NWScript.OBJECT_VISUAL_TRANSFORM_SCALE) > 0.75f)
        player.menu.choices.Add(($"Réduire ma taille.", () => ChangeCloneHeight(player, oClone, -0.02f)));

      if (NWScript.GetObjectVisualTransform(oClone, NWScript.OBJECT_VISUAL_TRANSFORM_SCALE) < 1.25f)
        player.menu.choices.Add(($"Augmenter ma taille.", () => ChangeCloneHeight(player, oClone, 0.02f)));

      player.menu.choices.Add(($"Yeux, couleur suivante.", () => ChangeCloneEyeColor(player, oClone, 1)));
      player.menu.choices.Add(($"Yeux, couleur précédente.", () => ChangeCloneEyeColor(player, oClone, -1)));
      player.menu.choices.Add(($"Cheveux, couleur suivante.", () => ChangeCloneHairColor(player, oClone, 1)));
      player.menu.choices.Add(($"Cheveux, couleur précédente.", () => ChangeCloneHairColor(player, oClone, -1)));
      player.menu.choices.Add(($"Lèvres, couleur suivante.", () => ChangeCloneLipsColor(player, oClone, 1)));
      player.menu.choices.Add(($"Lèvres, couleur précédente.", () => ChangeCloneLipsColor(player, oClone, -1)));
      player.menu.choices.Add(($"Appliquer les modifications.", () => ApplyBodyChangesOnPlayer(player, oClone)));
      player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => ExitDialog(player)));
      player.menu.Draw();
    }
    private void HandleBackgroundChoice(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"L'espace de quelques instants, la galère disparaît de votre esprit.",
        "Vous vous revoyez ..."
      };

      player.menu.choices.Add(($"véritable brute, parcourant les rues de votre ville en recherche d'un prochain écolier à martyriser.", () => AddTrait(player, Feat.Thug)));
      player.menu.choices.Add(($"au temple, en tant que simple adepte, entouré de vos semblables en communion.", () => AddTrait(player, Feat.Strongsoul)));
      player.menu.choices.Add(($"entouré de votre famille, issue d'une noble et ancienne lignée aristocratique sombrée en désuétude.", () => AddTrait(player, Feat.SilverPalm)));
      player.menu.choices.Add(($"occupé à contempler les oeuvres artistiques de votre précédent maître.", () => AddTrait(player, Feat.Artist)));
      player.menu.choices.Add(($"athlète acclamé, auréolé de plusieurs victoires.", () => AddTrait(player, Feat.Bullheaded)));
      player.menu.choices.Add(($"gamin des rues, arpentant les venelles mal famées de votre ville natale.", () => AddTrait(player, Feat.Snakeblood)));
      player.menu.choices.Add(($"milicien volontaire de votre ville ou village natal, garant de la paix en ses rues.", () => AddTrait(player, Feat.Blooded)));
      player.menu.choices.Add(($"jeune apprenti, étudiant parchemins et grimoires anciens afin d'appréhender les bases mêmes de la magie.", () => AddTrait(player, Feat.CourtlyMagocracy)));
      player.menu.choices.Add(($"né sous une bonne étoile, Tymora vous ayant jusque là sourit plus que de raison.", () => AddTrait(player, Feat.LuckOfHeroes)));

      player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => ExitDialog(player)));
      player.menu.Draw();
    }
    private void HandleSkillSelection(Player player)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string> {
        $"Vous disposez actuellement de {ObjectPlugin.GetInt(player.oid, "_STARTING_SKILL_POINTS")} points de compétence.",
        "Quelles capacités initiales votre personnage possède-t-il ?"
      };

      foreach (KeyValuePair<int, SkillSystem.Skill> SkillListEntry in player.learnableSkills)
      {
        SkillSystem.Skill skill = SkillListEntry.Value;

        if (!skill.trained)
        {
          player.menu.choices.Add(($"{skill.name} - Coût : {skill.pointsToNextLevel}", () => HandleSkillSelected(player, skill)));
        }
      }

      player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => ExitDialog(player)));
      player.menu.Draw();
    }
    private void ExitDialog(Player player)
    {
      player.menu.Close();
      RestoreMirror(player);
    }
    private void RestoreMirror(Player player)
    {
      uint oClone = NWScript.GetNearestObjectByTag($"clone_{NWScript.GetPCPublicCDKey(player.oid)}", player.oid);
      if (Convert.ToBoolean(NWScript.GetIsObjectValid(oClone)))
      {
        VisibilityPlugin.SetVisibilityOverride(player.oid, NWScript.GetNearestObjectByTag("intro_mirror", player.oid), VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
        CreaturePlugin.JumpToLimbo(oClone);
        NWScript.DestroyObject(oClone);
      }
    }
    private void ChangeCloneHead(Player player, uint oClone, int model)
    {
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, NWScript.GetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, oClone) + model, oClone);
      //HandleBodyModification(player, oClone);
    }
    private void ChangeCloneHeight(Player player, uint oClone, float size)
    {
      NWScript.SetObjectVisualTransform(oClone, NWScript.OBJECT_VISUAL_TRANSFORM_SCALE, NWScript.GetObjectVisualTransform(oClone, NWScript.OBJECT_VISUAL_TRANSFORM_SCALE) + size);
      //HandleBodyModification(player, oClone);
    }
    private void ApplyBodyChangesOnPlayer(Player player, uint oClone)
    {
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, NWScript.GetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, oClone), player.oid);
      NWScript.SetColor(player.oid, NWScript.COLOR_CHANNEL_TATTOO_1, NWScript.GetColor(oClone, NWScript.COLOR_CHANNEL_TATTOO_1));
      NWScript.SetColor(player.oid, NWScript.COLOR_CHANNEL_TATTOO_2, NWScript.GetColor(oClone, NWScript.COLOR_CHANNEL_TATTOO_2));
      NWScript.SetColor(player.oid, NWScript.COLOR_CHANNEL_HAIR, NWScript.GetColor(oClone, NWScript.COLOR_CHANNEL_HAIR));
      NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_SCALE, NWScript.GetObjectVisualTransform(oClone, NWScript.OBJECT_VISUAL_TRANSFORM_SCALE));
      HandleBodyModification(player, oClone);
    }
    private void ChangeCloneEyeColor(Player player, uint oClone, int color)
    {
      NWScript.SetColor(oClone, NWScript.COLOR_CHANNEL_TATTOO_1, NWScript.GetColor(oClone, NWScript.COLOR_CHANNEL_TATTOO_1) + color);
      //HandleBodyModification(player, oClone);
    }
    private void ChangeCloneHairColor(Player player, uint oClone, int color)
    {
      NWScript.SetColor(oClone, NWScript.COLOR_CHANNEL_HAIR, NWScript.GetColor(oClone, NWScript.COLOR_CHANNEL_HAIR) + color);
      //HandleBodyModification(player, oClone);
    }
    private void ChangeCloneLipsColor(Player player, uint oClone, int color)
    {
      NWScript.SetColor(oClone, NWScript.COLOR_CHANNEL_TATTOO_2, NWScript.GetColor(oClone, NWScript.COLOR_CHANNEL_TATTOO_2) + color);
      //HandleBodyModification(player, oClone);
    }
    private void HandleSkillSelected(Player player, Skill skill)
    {
      int remainingPoints = ObjectPlugin.GetInt(player.oid, "_STARTING_SKILL_POINTS");

      if (remainingPoints >= skill.pointsToNextLevel)
      {
        CreaturePlugin.AddFeat(player.oid, skill.oid);
        skill.trained = true;

        if (skill.successorId > 0)
        {
          player.learnableSkills.Add(skill.successorId, new SkillSystem.Skill(skill.successorId, 0, player));
        }

        if (CreaturePlugin.GetHighestLevelOfFeat(player.oid, skill.oid - 1) == skill.oid) // Suppression du prédécesseur
          CreaturePlugin.RemoveFeat(player.oid, skill.oid -1);

        ObjectPlugin.SetInt(player.oid, "_STARTING_SKILL_POINTS", remainingPoints -= skill.pointsToNextLevel, 1);
        skill.CreateSkillJournalEntry();
        skill.PlayNewSkillAcquiredEffects();
        HandleSkillSelection(player);

        if (RegisterAddCustomFeatEffect.TryGetValue(skill.oid, out Func<Player, int, int> handler))
        {
          try
          {
            handler.Invoke(player, skill.oid);
          }
          catch (Exception e)
          {
            Utils.LogException(e);
          }
        }
      }
      else
      {
        skill.acquiredPoints += remainingPoints;
        ObjectPlugin.DeleteInt(player.oid, "_STARTING_SKILL_POINTS");
        skill.currentJob = true;
        player.currentSkillJob = skill.oid;
        skill.CreateSkillJournalEntry();
        DrawWelcomePage(player);
      }
    }
    private void AddTrait(Player player, Feat trait)
    {
      CreaturePlugin.AddFeat(player.oid, (int)trait);
      NWScript.SetLocalInt(NWScript.GetNearestObjectByTag("intro_mirror", player.oid), "_TRAIT_SELECTED", 1);
      DrawWelcomePage(player);
    }
  }
}
