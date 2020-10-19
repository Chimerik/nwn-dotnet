using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    private class PrivateMenu : Menu
    {
      public PrivateMenu(Player player) : base(player) { }
    }
    public abstract partial class Menu
    {
      public string title { get; set; } = "";
      public List<(string text, Action handler)> choices = new List<(string text, Action handler)>();
      public int originTop { get; set; }
      public int originLeft { get; set; }

      private const int borderSize = 1;
      private const int widthPadding = 2;
      private const int heightPadding = 1;
      private const int titleBottomMargin = 1;

      private readonly Player player;
      
      private int titleHeight
      {
        get
        {
          if (title == "") return 0;
          return 1 + titleBottomMargin;
        }
      }

      private List<(int X, int Y, int ID)> drawnLineBackgroundIds = new List<(int X, int Y, int ID)>();
      private List<(int X, int Y, int ID)> drawnLineTextIds = new List<(int X, int Y, int ID)>();
      private (int X, int Y, int ID) drawnSelectionIds;
      private const int windowBaseID = 9000;
      private const int textBaseID = 8500;
      private const int arrowID = 8499;

      private int selectedChoiceID = 0;
      private bool isOpen = false;

      public Menu(Player player)
      {
        this.player = player;
        ResetConfig();
      }

      public void Draw()
      {
        if (!isOpen)
        {
          player.BoulderBlock();
          player.OnKeydown += HandleKeydown;
        }

        DrawWindow();
        DrawText();
        DrawSelection();

        isOpen = true;
      }

      public void Close ()
      {
        EraseDrawing();
        Clear();

        if (isOpen)
        {
          player.OnKeydown -= HandleKeydown;
          player.BoulderUnblock();
        }

        isOpen = false;
      }

      public void ResetConfig ()
      {
        originTop = 4;
        originLeft = 2;
      }

      public void Clear()
      {
        title = "";
        choices.Clear();
        selectedChoiceID = 0;
      }

      private void EraseDrawing()
      {
        EraseDrawing(drawnLineBackgroundIds);
        EraseDrawing(drawnLineTextIds);
        EraseLastSelection();
      }

      private void EraseDrawing(List<(int X, int Y, int ID)> drawnLines)
      {
        foreach (var (X, Y, ID) in drawnLines)
        {
          NWScript.PostString(player.oid, "", X, Y, ID, 0.000001f);
        }
        drawnLines.Clear();
      }

      private int CalcWindowWidth()
      {
        var longestText = title.Length;
        foreach (var (text, _) in choices)
        {
          if (text.Length > longestText) longestText = text.Length;
        }

        return (2 * borderSize) + longestText + (2 * widthPadding) + 1;
      }

      private int CalcWindowHeight()
      {
        var choicesHeight = choices.Count;
        return (2 * borderSize) + (2 * heightPadding) + titleHeight + choicesHeight;
      }

      public void DrawWindow()
      {
        EraseDrawing(drawnLineBackgroundIds);
        var width = CalcWindowWidth();
        var height = CalcWindowHeight();

        string top = Config.Glyph.WindowTopLeft;
        string middle = Config.Glyph.WindowMiddleLeft;
        string bottom = Config.Glyph.WindowBottomLeft;

        for (var i = 1; i < width - 1; i++)
        {
          top += Config.Glyph.WindowTopMiddle;
          middle += Config.Glyph.WindowMiddleBlank;
          bottom += Config.Glyph.WindowBottomMiddle;
        }

        top += Config.Glyph.WindowTopRight;
        middle += Config.Glyph.WindowMiddleRight;
        bottom += Config.Glyph.WindowBottomRight;

        DrawLine(top, originLeft, originTop, windowBaseID, Config.Font.Gui, drawnLineBackgroundIds);
        for (var i = 1; i < height - 1; i++)
        {
          DrawLine(middle, originLeft, originTop + i, windowBaseID + i, Config.Font.Gui, drawnLineBackgroundIds);
        }
        DrawLine(bottom, originLeft, originTop + height - 1, windowBaseID + height - 1, Config.Font.Gui, drawnLineBackgroundIds);
      }

      public void DrawText()
      {
        EraseDrawing(drawnLineTextIds);
        var textX = originLeft + widthPadding + borderSize;
        var textY = originTop + heightPadding + borderSize;
        var textID = textBaseID;

        if (title != "")
        {
          DrawLine(title, textX, textY++, textID++, Config.Font.Text, drawnLineTextIds);
          textY += titleBottomMargin;
        }

        for (var i = 0; i < choices.Count; i++)
        {
          var (text, _) = choices[i];
          DrawLine(text, textX, textY++, textID++, Config.Font.Text, drawnLineTextIds);
        }
      }

      public void DrawSelection()
      {
        EraseLastSelection();
        var x = originLeft + widthPadding + borderSize - 1;
        var y = originTop + heightPadding + borderSize + titleHeight + selectedChoiceID;
        DrawLine(Config.Glyph.Arrow, x, y, arrowID, Config.Font.Gui);
        drawnSelectionIds = (x, y, arrowID);
      }

      private void EraseLastSelection()
      {
        NWScript.PostString(
          player.oid, "",
          drawnSelectionIds.X,
          drawnSelectionIds.Y,
          drawnSelectionIds.ID,
          0.000001f
        );
      }

      private void DrawLine(string text, int x, int y, int id, string font, List<(int X, int Y, int ID)> drawnLines = null)
      {
        int color = unchecked((int)Config.Color.White);
        NWScript.PostString(
            player.oid, text, x, y, 0, 0f,
            color, color, id, font
        );
        if (drawnLines != null)
        {
          drawnLines.Add((X: x, Y: x, ID: id));
        }
      }

      private void HandleKeydown(object sender, PlayerSystem.Player.KeydownEventArgs e)
      {
        switch (e.key)
        {
          default: return;

          case "W":
            selectedChoiceID = (selectedChoiceID + choices.Count - 1) % choices.Count;
            EraseLastSelection();
            PlayerPlugin.PlaySound(player.oid, "gui_select", NWScript.OBJECT_INVALID);
            DrawSelection();
            return;

          case "S":
            selectedChoiceID = (selectedChoiceID + 1) % choices.Count;
            EraseLastSelection();
            PlayerPlugin.PlaySound(player.oid, "gui_select", NWScript.OBJECT_INVALID);
            DrawSelection();
            return;

          case "E":
            var handler = choices.ElementAtOrDefault(selectedChoiceID).handler;
            PlayerPlugin.PlaySound(player.oid, "gui_picklockopen", NWScript.OBJECT_INVALID);
            handler?.Invoke();
            return;
        }
      }
    }
  }
}
