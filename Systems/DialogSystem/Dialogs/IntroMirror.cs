using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anvil.API;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

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
        "Houla, y a pas à dire, vous avez connu de meilleurs jours.",
        "C'est quoi cette mine que vous me tirez ?",
        "On va mettre ça sur le compte du mal de mer."
      };
      player.menu.choices.Add(($"Me refaire une beauté.", () => HandleBodyCloneSpawn(player)));
      
      if(mirror.GetObjectVariable<LocalVariableInt>("_TRAIT_SELECTED").HasNothing)
        player.menu.choices.Add(($"Me perdre brièvement dans le passé.", () => HandleBackgroundChoice(player)));
      
      /*if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").HasValue)
        player.menu.choices.Add(($"Me préparer à l'avenir.", () => HandleSkillSelection(player)));*/
      
      player.menu.choices.Add(("M'éloigner du miroir.", () => player.menu.Close()));
      player.menu.Draw();

      RestoreMirror(player);
    }
    private void HandleBodyCloneSpawn(Player player)
    {
      clone = player.oid.LoginCreature.Clone(mirror.Location, "clone");
      clone.ApplyEffect(EffectDuration.Permanent, Effect.CutsceneGhost());
      clone.HighlightColor = ColorConstants.Silver;
      clone.Name = $"Reflet de {player.oid.LoginCreature.Name}";
      clone.Rotation += 180;

      VisibilityPlugin.SetVisibilityOverride(player.oid.LoginCreature, mirror, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
      
      HandleBodyModification(player);

      Task waitMenuClosed = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => player.oid == null || !player.menu.isOpen);
        RestoreMirror(player);
      });
    }
    private void HandleBodyModification(Player player)
    {
      clone.GetObjectVariable<LocalVariableInt>("_CURRENT_HEAD").Value = clone.GetCreatureBodyPart(CreaturePart.Head);

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

      player.menu.choices.Add(($"Yeux, couleur suivante.", () => clone.SetColor(ColorChannel.Tattoo1, clone.GetColor(ColorChannel.Tattoo1) + 1)));
      player.menu.choices.Add(($"Yeux, couleur précédente.", () => clone.SetColor(ColorChannel.Tattoo1, clone.GetColor(ColorChannel.Tattoo1) - 1)));
      player.menu.choices.Add(($"Cheveux, couleur suivante.", () => clone.SetColor(ColorChannel.Hair, clone.GetColor(ColorChannel.Hair) + 1)));
      player.menu.choices.Add(($"Cheveux, couleur précédente.", () => clone.SetColor(ColorChannel.Hair, clone.GetColor(ColorChannel.Hair) - 1)));
      player.menu.choices.Add(($"Lèvres, couleur suivante.", () => clone.SetColor(ColorChannel.Tattoo2, clone.GetColor(ColorChannel.Tattoo2) + 1)));
      player.menu.choices.Add(($"Lèvres, couleur précédente.", () => clone.SetColor(ColorChannel.Tattoo2, clone.GetColor(ColorChannel.Tattoo2) - 1)));
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
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    /*private void HandleSkillSelection(Player player)
    {
      player.menu.Clear();
      
      player.menu.titleLines = new List<string> {
        $"Vous disposez actuellement de {player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value} points de compétence.",
        "Quelles capacités initiales votre personnage possède-t-il ?"
      };

      foreach (KeyValuePair<string, Learnable> SkillListEntry in player.learnables.Where(k => k.Value.type == LearnableType.Feat))
      {
        Learnable skill = SkillListEntry.Value;

        if (!skill.trained)
        {
          player.menu.choices.Add(($"{skill.name} - Coût : {skill.pointsToNextLevel}", () => HandleSkillSelected(player, skill)));
        }
      }

      player.menu.choices.Add(("Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }*/
    private void RestoreMirror(Player player)
    {   
      if (clone != null)
      {
        clone.Destroy();
        VisibilityPlugin.SetVisibilityOverride(player.oid.LoginCreature, mirror, VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
      }
    }
    private void ChangeCloneHead(Player player, int model)
    {
      clone.SetCreatureBodyPart(CreaturePart.Head, clone.GetCreatureBodyPart(CreaturePart.Head) + model);
      clone.GetObjectVariable<LocalVariableInt>("_CURRENT_HEAD").Value = clone.GetCreatureBodyPart(CreaturePart.Head);
    }
    private void ChangeCloneHeight(Player player, float size)
    {
      if ((size < 0 && clone.VisualTransform.Scale > 0.75) || (size > 0 && clone.VisualTransform.Scale < 1.25))
        clone.VisualTransform.Scale += size;
    }
    private void ApplyBodyChangesOnPlayer(Player player)
    {
      player.oid.LoginCreature.SetCreatureBodyPart(CreaturePart.Head, clone.GetCreatureBodyPart(CreaturePart.Head));
      player.oid.LoginCreature.SetColor(ColorChannel.Tattoo1, clone.GetColor(ColorChannel.Tattoo1));
      player.oid.LoginCreature.SetColor(ColorChannel.Tattoo2, clone.GetColor(ColorChannel.Tattoo2));
      player.oid.LoginCreature.SetColor(ColorChannel.Hair, clone.GetColor(ColorChannel.Hair));
      player.oid.LoginCreature.VisualTransform.Scale = clone.VisualTransform.Scale;
      HandleBodyModification(player);
    }
    /*private void HandleSkillSelected(Player player, Learnable skill)
    {
      int remainingPoints = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value;

      if (remainingPoints >= skill.pointsToNextLevel)
      {
        player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value = remainingPoints -= skill.pointsToNextLevel;

        if (customFeatsDictionnary.ContainsKey(skill.featId)) // Il s'agit d'un Custom Feat
        {
          skill.acquiredPoints = skill.pointsToNextLevel;

          if (player.learntCustomFeats.ContainsKey(skill.featId))
            player.learntCustomFeats[skill.featId] = (int)skill.acquiredPoints;
          else
            player.learntCustomFeats.Add(skill.featId, (int)skill.acquiredPoints);

          string customFeatName = customFeatsDictionnary[skill.featId].name;
          skill.name = customFeatName;

          skill.currentLevel = GetCustomFeatLevelFromSkillPoints(skill.featId, (int)skill.acquiredPoints);
          skill.pointsToNextLevel = (int)(250 * skill.multiplier * Math.Pow(5, skill.currentLevel));
          
          player.oid.SetTlkOverride((int)Feat2da.featTable.GetFeatDataEntry(skill.featId).tlkName, $"{customFeatName} - {skill.currentLevel}");

          if (skill.currentLevel >= customFeatsDictionnary[skill.featId].maxLevel)
            skill.trained = true;
        }
        else
        {
          skill.trained = true;

          if (skill.successorId > 0)
          {
            player.learnables.Add($"F{skill.successorId}", new Learnable(LearnableType.Feat, skill.successorId, 0).InitializeLearnableLevel(player));
          }
        }

        player.oid.LoginCreature.AddFeat(skill.featId);
        player.CreateSkillJournalEntry(skill);
        player.PlayNewSkillAcquiredEffects(skill);
        HandleSkillSelection(player);

        if (RegisterAddCustomFeatEffect.TryGetValue(skill.featId, out Func<Player, Feat, int> handler))
        {
          try
          {
            handler.Invoke(player, skill.featId);
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
        skill.active = true;
        player.AwaitPlayerStateChangeToCalculateSPGain(skill);
        player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Delete();
        player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_GO").Value = 1;
        player.CreateSkillJournalEntry(skill);
        DrawWelcomePage(player);
      }
    }*/
    private void AddTrait(Player player, Feat trait)
    {
      player.oid.LoginCreature.AddFeat(trait);
      mirror.GetObjectVariable<LocalVariableInt>("_TRAIT_SELECTED").Value = 1;
      DrawWelcomePage(player);
    }
  }
}
