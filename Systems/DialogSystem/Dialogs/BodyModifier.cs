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
  class BodyModifier
  {
    NwPlaceable mirror;
    NwCreature clone { get; set; }
    public BodyModifier(Player player, NwPlaceable oMirror)
    {
      mirror = oMirror;
      this.DrawWelcomePage(player);
    }
    private void DrawWelcomePage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        "Vous contemplez votre reflet.",
      };
      player.menu.choices.Add(($"Me refaire une beauté.", () => HandleBodyCloneSpawn(player)));
      player.menu.choices.Add(("M'éloigner du miroir.", () => player.menu.Close()));
      player.menu.Draw();

      RestoreMirror(player);
    }
    private void HandleBodyCloneSpawn(Player player)
    {
      clone = player.oid.ControlledCreature.Clone(mirror.Location, "clone");
      clone.ApplyEffect(EffectDuration.Permanent, API.Effect.CutsceneGhost());
      clone.HiliteColor = ColorConstants.Silver;
      clone.Name = $"Reflet de {player.oid.ControlledCreature.Name}";
      clone.Rotation += 180;

      VisibilityPlugin.SetVisibilityOverride(player.oid.ControlledCreature, mirror, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);

      HandleBodyModification(player);

      Task waitMenuClosed = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => player.oid == null || !player.menu.isOpen);
        RestoreMirror(player);
      });
    }
    private void HandleBodyModification(Player player)
    {
      clone.GetLocalVariable<int>("_CURRENT_HEAD").Value = NWScript.GetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, clone);

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
    private void RestoreMirror(Player player)
    {
      if (clone != null)
      {
        clone.Destroy();
        VisibilityPlugin.SetVisibilityOverride(player.oid.ControlledCreature, mirror, VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
      }
    }
    private void ChangeCloneHead(Player player, int model)
    {
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, NWScript.GetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, clone) + model, clone);
      clone.GetLocalVariable<int>("_CURRENT_HEAD").Value = NWScript.GetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, clone);
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
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, NWScript.GetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, clone), player.oid.ControlledCreature);
      NWScript.SetColor(player.oid.ControlledCreature, NWScript.COLOR_CHANNEL_TATTOO_1, NWScript.GetColor(clone, NWScript.COLOR_CHANNEL_TATTOO_1));
      NWScript.SetColor(player.oid.ControlledCreature, NWScript.COLOR_CHANNEL_TATTOO_2, NWScript.GetColor(clone, NWScript.COLOR_CHANNEL_TATTOO_2));
      NWScript.SetColor(player.oid.ControlledCreature, NWScript.COLOR_CHANNEL_HAIR, NWScript.GetColor(clone, NWScript.COLOR_CHANNEL_HAIR));
      player.oid.ControlledCreature.VisualTransform = clone.VisualTransform;
      HandleBodyModification(player);
    }
    private void ChangeCloneEyeColor(Player player, int color)
    {
      NWScript.SetColor(clone, NWScript.COLOR_CHANNEL_TATTOO_1, NWScript.GetColor(clone, NWScript.COLOR_CHANNEL_TATTOO_1) + color);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, 0, clone);

      Task waitHeadUpdate = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, clone.GetLocalVariable<int>("_CURRENT_HEAD").Value, clone);
      });
    }
    private void ChangeCloneHairColor(Player player, int color)
    {
      NWScript.SetColor(clone, NWScript.COLOR_CHANNEL_HAIR, NWScript.GetColor(clone, NWScript.COLOR_CHANNEL_HAIR) + color);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, 0, clone);

      Task waitHeadUpdate = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, clone.GetLocalVariable<int>("_CURRENT_HEAD").Value, clone);
      });
    }
    private void ChangeCloneLipsColor(Player player, int color)
    {
      NWScript.SetColor(clone, NWScript.COLOR_CHANNEL_TATTOO_2, NWScript.GetColor(clone, NWScript.COLOR_CHANNEL_TATTOO_2) + color);
      NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, 0, clone);

      Task waitHeadUpdate = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        NWScript.SetCreatureBodyPart(NWScript.CREATURE_PART_HEAD, clone.GetLocalVariable<int>("_CURRENT_HEAD").Value, clone);
      });
    }
  }
}
