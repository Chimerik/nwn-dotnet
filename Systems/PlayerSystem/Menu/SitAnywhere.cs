using System;
using System.Collections.Generic;
using System.Numerics;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class SitAnywhereWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private ScheduledTask checkPositionScheduler { get; set; }
        private Vector3 playerPosition { get; set; }

        public SitAnywhereWindow(Player player) : base(player)
        {
          windowId = "sitAnywhere";
          rootColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ief_impinvis") { Id = "ground", Height = 40, Width = 40, Tooltip = "Animation au sol." },
            new NuiButtonImage("ief_sanctuary") { Id = "chair", Height = 40, Width = 40, Tooltip = "Animation chaise." },
            new NuiSpacer()
          }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiButtonImage("menu_down") { Id = "down", Height = 40, Width = 40, Tooltip = "Attention, seule la position affichée change. La position réelle du personnage reste la même. Ne pas en abuser." },
            new NuiButtonImage("menu_up") { Id = "up", Height = 40, Width = 40, Tooltip = "Attention, seule la position affichée change. La position réelle du personnage reste la même. Ne pas en abuser." },
            new NuiButtonImage("right") { Id = "right", Height = 40, Width = 40, Tooltip = "Attention, seule la position affichée change. La position réelle du personnage reste la même. Ne pas en abuser." },
            new NuiButtonImage("left") { Id = "left", Height = 40, Width = 40, Tooltip = "Attention, seule la position affichée change. La position réelle du personnage reste la même. Ne pas en abuser." },
            new NuiButtonImage("forward") { Id = "forward", Height = 40, Width = 40, Tooltip = "Attention, seule la position affichée change. La position réelle du personnage reste la même. Ne pas en abuser." },
            new NuiButtonImage("backward") { Id = "backward", Height = 40, Width = 40, Tooltip = "Attention, seule la position affichée change. La position réelle du personnage reste la même. Ne pas en abuser." },
            new NuiButtonImage("rotate_right") { Id = "rotate_right", Height = 40, Width = 40, Tooltip = "Attention, seule la position affichée change. La position réelle du personnage reste la même. Ne pas en abuser." },
            new NuiButtonImage("rotate_left") { Id = "rotate_left", Height = 40, Width = 40, Tooltip = "Attention, seule la position affichée change. La position réelle du personnage reste la même. Ne pas en abuser." },
            new NuiButtonImage("menu_exit") { Id = "cancel", Height = 40, Width = 40, Tooltip = "Réinitialiser l'affichage de la position du personnage controlé." },
          }
          });

          CreateWindow();
        }
        public void CreateWindow()
        {
          playerPosition = player.oid.ControlledCreature.Position;
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(450, 600, 400, 140);

          window = new NuiWindow(rootColumn, "Réorientation")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleSitAnywhereEvents;

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            checkPositionScheduler = player.scheduler.ScheduleRepeating(() => { CheckPlayerMovement(); }, TimeSpan.FromSeconds(2));
          }
        }

        private void HandleSitAnywhereEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              Vector3 translation = player.oid.ControlledCreature.VisualTransform.Translation;
              Vector3 rotation = player.oid.ControlledCreature.VisualTransform.Rotation;

              switch (nuiEvent.ElementId)
              {
                case "down":

                  player.oid.ControlledCreature.VisualTransform.Translation = new Vector3(translation.X, translation.Y, translation.Z - 0.1f);
                  if (translation.Z < player.oid.ControlledCreature.Location.GroundHeight)
                    Utils.LogMessageToDMs($"SIT COMMAND - Player {player.oid.PlayerName} - Z translation = {translation.Z}");

                  player.oid.CameraHeight = 1 + translation.Z;

                  return;

                case "up":

                  player.oid.ControlledCreature.VisualTransform.Translation = new Vector3(translation.X, translation.Y, translation.Z + 0.1f);
                  if (translation.Z > 5)
                    Utils.LogMessageToDMs($"SIT COMMAND - Player {player.oid.PlayerName} - Z translation = {translation.Z}");

                  player.oid.CameraHeight = 1 + translation.Z;

                  return;

                case "right":
                  player.oid.ControlledCreature.VisualTransform.Translation = new Vector3(translation.X + 0.1f, translation.Y, translation.Z);
                  return;

                case "left":
                  player.oid.ControlledCreature.VisualTransform.Translation = new Vector3(translation.X - 0.1f, translation.Y, translation.Z);
                  return;

                case "forward":
                  player.oid.ControlledCreature.VisualTransform.Translation = new Vector3(translation.X, translation.Y + 0.1f, translation.Z);
                  return;

                case "backward":
                  player.oid.ControlledCreature.VisualTransform.Translation = new Vector3(translation.X, translation.Y - 0.1f, translation.Z);
                  return;

                case "rotate_right":
                  player.oid.ControlledCreature.VisualTransform.Rotation = new Vector3(rotation.X + 20, rotation.Y, rotation.Z);
                  return;

                case "rotate_left":
                  player.oid.ControlledCreature.VisualTransform.Rotation = new Vector3(rotation.X - 20, rotation.Y, rotation.Z);
                  return;

                case "cancel":
                  Utils.ResetVisualTransform(player.oid.ControlledCreature);
                  return;

                case "ground":
                  _ = player.oid.ControlledCreature.PlayAnimation(Animation.LoopingSitCross, 1, false, TimeSpan.FromDays(1));
                  return;

                case "chair":
                  _ = player.oid.ControlledCreature.PlayAnimation(Animation.LoopingSitChair, 1, false, TimeSpan.FromDays(1));
                  return;
              }

              return;

            case NuiEventType.Close:
              checkPositionScheduler.Dispose();
              return;
          }
        }
        private void CheckPlayerMovement()
        {
          if (player.oid == null || player.oid.ControlledCreature == null)
          {
            checkPositionScheduler.Dispose();
            return;
          }

          if (player.oid.ControlledCreature.Position != playerPosition)
          {
            player.oid.SendServerMessage("Déplacement détecté. Réinitialisation de l'affichage de la position.", ColorConstants.Orange);
            Utils.ResetVisualTransform(player.oid.ControlledCreature);
          }
        }
      }
    }
  }
}
