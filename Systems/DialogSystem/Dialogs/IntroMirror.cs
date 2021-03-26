using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;
using Skill = NWN.Systems.SkillSystem.Skill;

namespace NWN.Systems
{
  class IntroMirror
  {
    NwPlaceable mirror;
    NwCreature clone { get; set; }
    public IntroMirror(Player player, NwPlaceable oMirror)
    {
      mirror = oMirror;
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
      
      player.menu.choices.Add(("M'éloigner du miroir.", () => player.menu.Close()));
      player.menu.Draw();

      RestoreMirror(player);
    }
    private void HandleBodyCloneSpawn(Player player)
    {
      clone = player.oid.Clone(mirror.Location, "clone");
      clone.ApplyEffect(EffectDuration.Permanent, API.Effect.CutsceneGhost());
      clone.HiliteColor = Color.SILVER;
      clone.Name = $"Reflet de {player.oid.Name}";
      clone.Rotation += 180;

      VisibilityPlugin.SetVisibilityOverride(player.oid, mirror, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
      
      HandleBodyModification(player);

      Task waitMenuClosed = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => player.oid == null || !player.menu.isOpen);
        RestoreMirror(player);
      });
    }
    private void HandleBodyModification(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Vous vous concentrez sur le miroir de façon à mieux vous mirer.",
        "Qu'ajusteriez-vous dans ce reflet ?"
      };
      player.menu.choices.Add(($"Apparence suivante.", () => ChangeCloneHead(player, 1)));
      player.menu.choices.Add(($"Apparence précédente.", () => ChangeCloneHead(player, -1)));
      
      if (clone.VisualTransform.Scale > 0.75)
        player.menu.choices.Add(($"Réduire ma taille.", () => ChangeCloneHeight(player, -0.02f)));
      
      if (clone.VisualTransform.Scale < 1.25)
        player.menu.choices.Add(($"Augmenter ma taille.", () => ChangeCloneHeight(player, 0.02f)));

      player.menu.choices.Add(($"Yeux, couleur suivante.", () => ChangeCloneEyeColor(player, 1)));
      player.menu.choices.Add(($"Yeux, couleur précédente.", () => ChangeCloneEyeColor(player, -1)));
      player.menu.choices.Add(($"Cheveux, couleur suivante.", () => ChangeCloneHairColor(player, 1)));
      player.menu.choices.Add(($"Cheveux, couleur précédente.", () => ChangeCloneHairColor(player, -1)));
      player.menu.choices.Add(($"Lèvres, couleur suivante.", () => ChangeCloneLipsColor(player, 1)));
      player.menu.choices.Add(($"Lèvres, couleur précédente.", () => ChangeCloneLipsColor(player, -1)));
      player.menu.choices.Add(($"Appliquer les modifications.", () => ApplyBodyChangesOnPlayer(player)));
      player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleBackgroundChoice(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"L'espace de quelques instants, la galère disparaît de votre esprit.",
        "Vous vous revoyez ..."
      };

      player.menu.choices.Add(($"véritable brute, parcourant les rues de votre ville en recherche d'un prochain écolier à martyriser.", () => AddTrait(player, API.Constants.Feat.Thug)));
      player.menu.choices.Add(($"au temple, en tant que simple adepte, entouré de vos semblables en communion.", () => AddTrait(player, API.Constants.Feat.Strongsoul)));
      player.menu.choices.Add(($"entouré de votre famille, issue d'une noble et ancienne lignée aristocratique sombrée en désuétude.", () => AddTrait(player, API.Constants.Feat.SilverPalm)));
      player.menu.choices.Add(($"occupé à contempler les oeuvres artistiques de votre précédent maître.", () => AddTrait(player, API.Constants.Feat.Artist)));
      player.menu.choices.Add(($"athlète acclamé, auréolé de plusieurs victoires.", () => AddTrait(player, API.Constants.Feat.Bullheaded)));
      player.menu.choices.Add(($"gamin des rues, arpentant les venelles mal famées de votre ville natale.", () => AddTrait(player, API.Constants.Feat.Snakeblood)));
      player.menu.choices.Add(($"milicien volontaire de votre ville ou village natal, garant de la paix en ses rues.", () => AddTrait(player, API.Constants.Feat.Blooded)));
      player.menu.choices.Add(($"jeune apprenti, étudiant parchemins et grimoires anciens afin d'appréhender les bases mêmes de la magie.", () => AddTrait(player, API.Constants.Feat.CourtlyMagocracy)));
      player.menu.choices.Add(($"né sous une bonne étoile, Tymora vous ayant jusque là sourit plus que de raison.", () => AddTrait(player, API.Constants.Feat.LuckOfHeroes)));

      player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleSkillSelection(Player player)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string> {
        $"Vous disposez actuellement de {ObjectPlugin.GetInt(player.oid, "_STARTING_SKILL_POINTS")} points de compétence.",
        "Quelles capacités initiales votre personnage possède-t-il ?"
      };

      foreach (KeyValuePair< API.Constants.Feat, Skill > SkillListEntry in player.learnableSkills)
      {
        Skill skill = SkillListEntry.Value;

        if (!skill.trained)
        {
          player.menu.choices.Add(($"{skill.name} - Coût : {skill.pointsToNextLevel}", () => HandleSkillSelected(player, skill)));
        }
      }

      player.menu.choices.Add(("Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void RestoreMirror(Player player)
    {   
      if (clone != null)
      {
        clone.Destroy();
        VisibilityPlugin.SetVisibilityOverride(player.oid, mirror, VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
      }
    }
    private void ChangeCloneHead(Player player, int model)
    {
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, NWScript.GetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, clone) + model, clone);
    }
    private void ChangeCloneHeight(Player player, float size)
    {
      if ((size < 0 && clone.VisualTransform.Scale > 0.75) || (size > 0 && clone.VisualTransform.Scale < 1.25))
      {
        VisualTransform scale = clone.VisualTransform;
        scale.Scale += size;
        clone.VisualTransform = scale;
      }
    }
    private void ApplyBodyChangesOnPlayer(Player player)
    {
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, NWScript.GetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, clone), player.oid);
      NWScript.SetColor(player.oid, NWScript.COLOR_CHANNEL_TATTOO_1, NWScript.GetColor(clone, NWScript.COLOR_CHANNEL_TATTOO_1));
      NWScript.SetColor(player.oid, NWScript.COLOR_CHANNEL_TATTOO_2, NWScript.GetColor(clone, NWScript.COLOR_CHANNEL_TATTOO_2));
      NWScript.SetColor(player.oid, NWScript.COLOR_CHANNEL_HAIR, NWScript.GetColor(clone, NWScript.COLOR_CHANNEL_HAIR));
      player.oid.VisualTransform.Scale = clone.VisualTransform.Scale;
      HandleBodyModification(player);
    }
    private void ChangeCloneEyeColor(Player player, int color)
    {
      NWScript.SetColor(clone, NWScript.COLOR_CHANNEL_TATTOO_1, NWScript.GetColor(clone, NWScript.COLOR_CHANNEL_TATTOO_1) + color);
      //HandleBodyModification(player, clone);
    }
    private void ChangeCloneHairColor(Player player, int color)
    {
      NWScript.SetColor(clone, NWScript.COLOR_CHANNEL_HAIR, NWScript.GetColor(clone, NWScript.COLOR_CHANNEL_HAIR) + color);
      //HandleBodyModification(player, clone);
    }
    private void ChangeCloneLipsColor(Player player, int color)
    {
      NWScript.SetColor(clone, NWScript.COLOR_CHANNEL_TATTOO_2, NWScript.GetColor(clone, NWScript.COLOR_CHANNEL_TATTOO_2) + color);
      //HandleBodyModification(player, clone);
    }
    private void HandleSkillSelected(Player player, Skill skill)
    {
      int remainingPoints = ObjectPlugin.GetInt(player.oid, "_STARTING_SKILL_POINTS");

      if (remainingPoints >= skill.pointsToNextLevel)
      {
        ObjectPlugin.SetInt(player.oid, "_STARTING_SKILL_POINTS", remainingPoints -= skill.pointsToNextLevel, 1);

        if (customFeatsDictionnary.ContainsKey(skill.oid)) // Il s'agit d'un Custom Feat
        {
          skill.acquiredPoints = skill.pointsToNextLevel;

          if (player.learntCustomFeats.ContainsKey(skill.oid))
            player.learntCustomFeats[skill.oid] = (int)skill.acquiredPoints;
          else
            player.learntCustomFeats.Add(skill.oid, (int)skill.acquiredPoints);

          string customFeatName = customFeatsDictionnary[skill.oid].name;
          skill.name = customFeatName;

          skill.currentLevel = GetCustomFeatLevelFromSkillPoints(skill.oid, (int)skill.acquiredPoints);
          skill.pointsToNextLevel = (int)(250 * skill.multiplier * Math.Pow(5, skill.currentLevel));

          if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", (int)skill.oid), out int nameValue))
            PlayerPlugin.SetTlkOverride(player.oid, nameValue, $"{customFeatName} - {skill.currentLevel}");
          //player.oid.SetTlkOverride(nameValue, $"{customFeatName} - {skill.currentLevel}");
          else
            Utils.LogMessageToDMs($"CUSTOM SKILL SYSTEM ERROR - Skill {customFeatName} - {(int)skill.oid} : no available custom name StrRef");
         
          if (skill.currentLevel >= customFeatsDictionnary[skill.oid].maxLevel)
            skill.trained = true;
        }
        else
        {
          skill.trained = true;

          if (skill.successorId > 0)
          {
            player.learnableSkills.Add((Feat)skill.successorId, new Skill((Feat)skill.successorId, 0, player));
          }
        }

        player.oid.AddFeat(skill.oid);
        skill.CreateSkillJournalEntry();
        skill.PlayNewSkillAcquiredEffects();
        HandleSkillSelection(player);

        if (RegisterAddCustomFeatEffect.TryGetValue(skill.oid, out Func<Player, Feat, int> handler))
        {
          try
          {
            handler.Invoke(player, skill.oid);
          }
          catch (Exception e)
          {
            Utils.LogMessageToDMs(e.Message);
          }
        }
      }
      else
      {
        skill.acquiredPoints += remainingPoints;
        ObjectPlugin.DeleteInt(player.oid, "_STARTING_SKILL_POINTS");
        skill.currentJob = true;
        player.currentSkillJob = (int)skill.oid;
        skill.CreateSkillJournalEntry();
        DrawWelcomePage(player);
      }
    }
    private void AddTrait(Player player, Feat trait)
    {
      player.oid.AddFeat(trait);
      mirror.GetLocalVariable<int>("_TRAIT_SELECTED").Value = 1;
      DrawWelcomePage(player);
    }
  }
}
