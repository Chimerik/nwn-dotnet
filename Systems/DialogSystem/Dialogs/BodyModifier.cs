using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

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
      clone.HighlightColor = ColorConstants.Silver;
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
      player.menu.choices.Add(($"Cheveux, couleur suivante.", () => clone.SetColor(ColorChannel.Tattoo1, clone.GetColor(ColorChannel.Hair) + 1)));
      player.menu.choices.Add(($"Cheveux, couleur précédente.", () => clone.SetColor(ColorChannel.Tattoo1, clone.GetColor(ColorChannel.Hair) - 1)));
      player.menu.choices.Add(($"Lèvres, couleur suivante.", () => clone.SetColor(ColorChannel.Tattoo2, clone.GetColor(ColorChannel.Tattoo2) + 1)));
      player.menu.choices.Add(($"Lèvres, couleur précédente.", () => clone.SetColor(ColorChannel.Tattoo2, clone.GetColor(ColorChannel.Tattoo2) - 1)));
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
      clone.SetCreatureBodyPart(CreaturePart.Head, clone.GetCreatureBodyPart(CreaturePart.Head) + model);
      clone.GetObjectVariable<LocalVariableInt>("_CURRENT_HEAD").Value = clone.GetCreatureBodyPart(CreaturePart.Head);
    }
    private void ChangeCloneHeight(Player player, float size)
    {
      if ((size < 0 && clone.VisualTransform.Scale > 0.75) || (size > 0 && clone.VisualTransform.Scale < 1.25))
      {
        clone.VisualTransform.Scale += size;
      }
    }
    private void ApplyBodyChangesOnPlayer(Player player)
    {
      player.oid.ControlledCreature.SetCreatureBodyPart(CreaturePart.Head, clone.GetCreatureBodyPart(CreaturePart.Head));
      player.oid.ControlledCreature.SetColor(ColorChannel.Tattoo1, clone.GetColor(ColorChannel.Tattoo1));
      player.oid.ControlledCreature.SetColor(ColorChannel.Tattoo2, clone.GetColor(ColorChannel.Tattoo2));
      player.oid.ControlledCreature.SetColor(ColorChannel.Hair, clone.GetColor(ColorChannel.Hair));
      player.oid.ControlledCreature.VisualTransform.Scale = clone.VisualTransform.Scale;
      HandleBodyModification(player);
    }
  }
}
