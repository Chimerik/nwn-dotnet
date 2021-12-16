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
        NuiGroup rootGroup { get; }
        NuiColumn rootColumn { get; }
        List<NuiElement> rootChidren { get; }
        NuiRow introTextRow { get;  }
        NuiRow beautyRow { get;  }
        NuiRow pastRow { get; }
        NuiRow futureRow { get; }

        public IntroMirroWindow(Player player) : base(player)
        {
          windowId = "introMirror";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          introTextRow = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiImage(player.oid.LoginCreature.PortraitResRef + "M") { ImageAspect = NuiAspect.ExactScaled, Width = 60, Height = 100 },
              new NuiText("Houla, y a pas à dire, vous avez connu de meilleurs jours.\n\n" +
              "C'est quoi cette mine de déterré ?\n\n" +
              "On va mettre ça sur le compte du mal de mer.") { Width = 450, Height = 110 }
            }
          };

          beautyRow = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiButton("Se refaire une beauté.") { Id = "beauty", Width = 510 }
            }
          };

          pastRow = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiButton("Se perdre brièvement dans le passé.") { Id = "past", Width = 510 }
            }
          };

          futureRow = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiButton("Se préparer à l'avenir.") { Id = "future", Width = 510 }
            }
          };

          CreateWindow();
        }
        public void CreateWindow()
        {
          rootChidren.Clear();

          rootChidren.Add(introTextRow);
          rootChidren.Add(beautyRow);

          if(!player.learnableSkills.Values.Any(s => s.category == SkillSystem.Category.StartingTraits))
            rootChidren.Add(pastRow);
          
          rootChidren.Add(futureRow);

          NuiRect windowRectangle = new NuiRect(0, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.02f, 540, 340);

          window = new NuiWindow(rootGroup, "Votre reflet")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleIntroMirrorEvents;
          player.oid.OnNuiEvent += HandleIntroMirrorEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
        private void HandleIntroMirrorEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch(nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch(nuiEvent.ElementId)
              {
                case "beauty":

                  player.oid.NuiDestroy(token);

                  if (player.windows.ContainsKey("bodyAppearanceModifier"))
                    ((BodyAppearanceWindow)player.windows["bodyAppearanceModifier"]).CreateWindow();
                  else
                    player.windows.Add("bodyAppearanceModifier", new BodyAppearanceWindow(player));

                  break;

                case "past":

                  player.oid.NuiDestroy(token);

                  if (player.windows.ContainsKey("introBackground"))
                    ((LearnableWindow)player.windows["introBackground"]).CreateWindow();
                  else
                    player.windows.Add("introBackground", new IntroBackgroundWindow(player));

                  break;

                case "future":

                  player.oid.NuiDestroy(token);

                  if (player.windows.ContainsKey("introLearnables"))
                    ((IntroLearnableWindow)player.windows["introLearnables"]).CreateWindow();
                  else
                    player.windows.Add("introLearnables", new IntroLearnableWindow(player));
                  
                  break;
              }
              break;
          }
        }
      }
    }
  }
}
