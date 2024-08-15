using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class EnsoMetaTransmutationSelectionWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        public DamageType selectedDamageType { get; set; }

        public EnsoMetaTransmutationSelectionWindow(Player player) : base(player)
        {
          windowId = "ensoMetaTransmutationSelection";
          windowWidth = player.guiScaledWidth * 0.2f;
          windowHeight = player.guiScaledHeight * 0.1f;
          selectedDamageType =  DamageType.BaseWeapon;

          rootColumn.Children = rootChildren;

          List<NuiElement> rowChildren = new();
          NuiRow row = new() { Children = rowChildren };
          rootChildren.Add(row);

          rowChildren.Add(new NuiButtonImage("acid") { Id = "16", Tooltip = "Les dégâts de votre prochain sort seront de type acide", Height = 40, Width = 40 });
          rowChildren.Add(new NuiButtonImage("cold") { Id = "32", Tooltip = "Les dégâts de votre prochain sort seront de type froid", Height = 40, Width = 40 });
          rowChildren.Add(new NuiButtonImage("fire") { Id = "256", Tooltip = "Les dégâts de votre prochain sort seront de type feu", Height = 40, Width = 40 });
          rowChildren.Add(new NuiButtonImage("electricity") { Id = "128", Tooltip = "Les dégâts de votre prochain sort seront de type électrique", Height = 40, Width = 40 });
          rowChildren.Add(new NuiButtonImage("thunder") { Id = "2048", Tooltip = "Les dégâts de votre prochain sort seront de type tonnerre", Height = 40, Width = 40 });
          rowChildren.Add(new NuiButtonImage("poison") { Id = "8192", Tooltip = "Les dégâts de votre prochain sort seront de type poison", Height = 40, Width = 40 });
          
          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiWidth * 0.35f, player.guiHeight * 0.85f, windowWidth, windowHeight);

          window = new NuiWindow(rootColumn, "")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = false,
            Transparent = true,
            Border = false
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleEnsoTransmuEvents;

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, windowWidth, windowHeight));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }

        private void HandleEnsoTransmuEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              selectedDamageType = (DamageType)int.Parse(nuiEvent.ElementId);

              var vfxType = selectedDamageType switch
              {
                DamageType.Cold => VfxType.ImpPulseCold,
                DamageType.Fire => VfxType.ImpPulseFire,
                DamageType.Electrical => VfxType.ImpHeadElectricity,
                DamageType.Sonic => VfxType.ImpPulseWind,
                DamageType.Custom1 => VfxType.ImpHeadNature,
                _ => VfxType.ImpHeadAcid,
              };

              player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(vfxType));

              CloseWindow();

              return;
          }
        }
      }
    }
  }
}
