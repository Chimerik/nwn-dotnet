using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class IntroMirroWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new NuiColumn();
        private readonly List<NuiElement> rootChidren = new();
        private readonly NuiBind<string> mirrorText = new("mirrorText");

        public IntroMirroWindow(Player player) : base(player)
        {
          windowId = "introMirror";
          rootColumn.Children = rootChidren;

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = new NuiRect(0, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.02f, 540, 340);

          rootChidren.Clear();

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiImage(player.oid.LoginCreature.PortraitResRef + "M") { ImageAspect = NuiAspect.ExactScaled, Width = 60, Height = 80 },
            new NuiText(mirrorText) { Width = 450, Height = 110 }
          } });

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiButton("Se refaire une beauté.") { Id = "beauty", Width = 510 } } });

          if (!player.learnableSkills.Values.Any(s => s.category == SkillSystem.Category.StartingTraits))
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiButton("Se perdre brièvement dans le passé.") { Id = "past", Width = 510 } } });
          
          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiButton("Se préparer à l'avenir.") { Id = "future", Width = 510 } } });

          window = new NuiWindow(rootColumn, "Votre reflet")
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
            nuiToken.OnNuiEvent += HandleIntroMirrorEvents;

            mirrorText.SetBindValue(player.oid, nuiToken.Token, "Houla, y a pas à dire, vous avez connu de meilleurs jours.\n\n" +
              "C'est quoi cette mine de déterré ?\n\n" +
              "On va mettre ça sur le compte du mal de mer.");
            
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleIntroMirrorEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "beauty":

                  CloseWindow();

                  if (!player.windows.ContainsKey("bodyAppearanceModifier")) player.windows.Add("bodyAppearanceModifier", new BodyAppearanceWindow(player, player.oid.LoginCreature));
                  else ((BodyAppearanceWindow)player.windows["bodyAppearanceModifier"]).CreateWindow(player.oid.LoginCreature);

                  break;

                case "past":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introBackground")) player.windows.Add("introBackground", new IntroBackgroundWindow(player));
                  else ((IntroBackgroundWindow)player.windows["introBackground"]).CreateWindow();

                  break;

                case "future":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introLearnables")) player.windows.Add("introLearnables", new IntroLearnableWindow(player));
                  else ((IntroLearnableWindow)player.windows["introLearnables"]).CreateWindow();

                  break;
              }
              break;
          }
        }
      }
    }
  }
}
