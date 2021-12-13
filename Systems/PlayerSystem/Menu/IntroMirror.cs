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
              new NuiSpacer(),
              new NuiImage(player.oid.LoginCreature.PortraitResRef),
              new NuiText("Houla, y a pas à dire, vous avez connu de meilleurs jours.\n" +
              "C'est quoi cette mine que vous me tirez ?\n" +
              "On va mettre ça sur le compte du mal de mer."),
              new NuiSpacer()
            }
          };

          beautyRow = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Se refaire une beauté.") { Id = "beauty" },
              new NuiSpacer()
            }
          };

          pastRow = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Se perdre brièvement dans le passé.") { Id = "past" },
              new NuiSpacer()
            }
          };

          futureRow = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Se préparer à l'avenir.") { Id = "future" },
              new NuiSpacer()
            }
          };

          CreateWindow();
        }
        public void CreateWindow()
        {
          rootChidren.Clear();

          //potentiellement : relire le dialogue d'intro de Disco Elysium
          //portrait
          //texte
          // option : me refaire une beauté
          // option : me perdre dans le passé (niveaux de compétences de départ en fonction du background) + éventuellement quelques éléments de classe de base
          // option : me préparer à l'avenir

          rootChidren.Add(introTextRow);
          rootChidren.Add(beautyRow);

          if(!player.learnableSkills.Values.Any(s => s.category == SkillSystem.Category.StartingTraits))
            rootChidren.Add(pastRow);
          
          rootChidren.Add(futureRow);

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 500, 300);

          window = new NuiWindow(rootGroup, "Votre reflet")
          {
            Geometry = geometry,
            Resizable = true,
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
                    ((LearnableWindow)player.windows["introLearnables"]).CreateWindow();
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
