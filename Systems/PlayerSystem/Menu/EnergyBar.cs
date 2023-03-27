﻿using System.Threading;
using System.Collections.Generic;

using Anvil.API;
using System.Threading.Tasks;
using System;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class EnergyBarWindow : PlayerWindow
      {
        private readonly NuiRow root;
        private readonly NuiBind<float> energy = new("energy");
        private readonly NuiBind<string> readableEnergy = new("readableEnergy");
        private readonly Color white = new(255, 255, 255);
        private readonly NuiBind<NuiRect> drawListRect = new("drawListRect");

        private readonly NuiProgress energyBar;

        public EnergyBarWindow(Player player) : base(player)
        {
          windowId = "energyBar";

          energyBar = new NuiProgress(energy) { Width = 485, Height = 35, ForegroundColor = ColorConstants.Blue, DrawList = new List<NuiDrawListItem>() {
              new NuiDrawListText(white, drawListRect, readableEnergy)
          } };

          root = new NuiRow() { Children = new List<NuiElement>() };

          CreateWindow();
        }
        public void CreateWindow()
        {
          bool closableBind = IsOpen && !closable.GetBindValue(player.oid, nuiToken.Token);
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? new NuiRect(player.windowRectangles[windowId].X, player.windowRectangles[windowId].Y, player.windowRectangles[windowId].Width, 45) : new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2 - 250, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 495, 45);

          root.Children.Clear();
          energyBar.Width = windowRectangle.Width - 8;
          root.Children.Add(energyBar);

          window = new NuiWindow(root, "")
          {
            Geometry = geometry,
            Resizable = resizable,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = false,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;

            readableEnergy.SetBindValue(player.oid, nuiToken.Token, ((int)Math.Round(player.endurance.currentMana, MidpointRounding.ToZero)).ToString());
            energy.SetBindValue(player.oid, nuiToken.Token, (float)(player.endurance.currentMana / (double)player.endurance.maxMana));
            drawListRect.SetBindValue(player.oid, nuiToken.Token, new((float)(energyBar.Width.Value / 1.3), 15, 151, 20));

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            closable.SetBindValue(player.oid, nuiToken.Token, closableBind);
            resizable.SetBindValue(player.oid, nuiToken.Token, closableBind);

            IsOpen = true;
            UpdateCurrentEnergy();
          }
        }
        private async void UpdateCurrentEnergy()
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          double previousEnergy = player.endurance.currentMana;
          double previousRegen = player.energyRegen;

          Task windowClosed = NwTask.WaitUntil(() => !player.oid.IsValid || !IsOpen, tokenSource.Token);
          Task energyChanged = NwTask.WaitUntil(() => !player.oid.IsValid || previousEnergy != player.endurance.currentMana || previousRegen != player.energyRegen, tokenSource.Token);

          await NwTask.WhenAny(windowClosed, energyChanged);
          tokenSource.Cancel();

          if (windowClosed.IsCompletedSuccessfully)
            return;

          string currentEnergy = ((int)Math.Round(player.endurance.currentMana, MidpointRounding.ToZero)).ToString() + " ";
          
          if (player.endurance.regenerableMana > 1)
            for (int i = 0; i < player.energyRegen; i++)
              currentEnergy += ">";

          readableEnergy.SetBindValue(player.oid, nuiToken.Token, currentEnergy);
          energy.SetBindValue(player.oid, nuiToken.Token, (float)((double)player.endurance.currentMana / (double)player.endurance.maxMana));

          UpdateCurrentEnergy();
        }
      }
    }
  }
}
