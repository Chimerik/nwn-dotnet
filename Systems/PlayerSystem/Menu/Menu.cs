using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Enums;

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
      
      private int currentTitleHeight
      {
        get
        {
          if (currentTitle == "") return 0;
          return 1 + titleBottomMargin;
        }
      }

      private List<(int X, int Y, int ID)> drawnLineIds = new List<(int X, int Y, int ID)>();
      private (int X, int Y, int ID) drawnSelectionIds;
      private const int windowBaseID = 9000;
      private const int textBaseID = 8500;
      private const int arrowID = 8499;

      private int selectedChoiceID = 0;
      private bool isOpen = false;
      private List<(string text, Action handler)> currentChoices = new List<(string text, Action handler)>();
      private string currentTitle = "";

      public Menu(PlayerSystem.Player player)
      {
        this.player = player;
        ResetConfig();
      }

      public void Draw()
      {
        EraseDrawing();
        if (!isOpen)
        {
          player.BoulderBlock();
          player.OnKeydown += HandleKeydown;
        }

        currentChoices = choices.ToList();
        currentTitle = title;

        Clear();

        var width = CalcWindowWidth();
        var height = CalcWindowHeight();
        DrawWindow(width, height);
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

      private void Clear()
      {
        title = "";
        choices.Clear();
        selectedChoiceID = 0;
      }

      private void EraseDrawing()
      {
        foreach (var (X, Y, ID) in drawnLineIds)
        {
          NWScript.PostString(PC: player.oid, Msg: "", X: X, Y: Y, ID: ID, life: 0.000001f);
        }
        drawnLineIds.Clear();
        EraseLastSelection();
      }

      private int CalcWindowWidth()
      {
        var longestText = currentTitle.Length;
        foreach (var (text, _) in currentChoices)
        {
          if (text.Length > longestText) longestText = text.Length;
        }

        return (2 * borderSize) + longestText + (2 * widthPadding) + 1;
      }

      private int CalcWindowHeight()
      {
        var choicesHeight = currentChoices.Count;
        return (2 * borderSize) + (2 * heightPadding) + currentTitleHeight + choicesHeight;
      }

      private void DrawWindow(int width, int height)
      {
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

        DrawLine(top, originLeft, originTop, windowBaseID, Config.Font.Gui);
        for (var i = 1; i < height - 1; i++)
        {
          DrawLine(middle, originLeft, originTop + i, windowBaseID + i, Config.Font.Gui);
        }
        DrawLine(bottom, originLeft, originTop + height - 1, windowBaseID + height - 1, Config.Font.Gui);
      }

      private void DrawText()
      {
        var textX = originLeft + widthPadding + borderSize;
        var textY = originTop + heightPadding + borderSize;
        var textID = textBaseID;

        if (currentTitle != "")
        {
          DrawLine(currentTitle, textX, textY++, textID++, Config.Font.Text);
          textY += titleBottomMargin;
        }

        for (var i = 0; i < currentChoices.Count; i++)
        {
          var (text, _) = currentChoices[i];
          DrawLine(text, textX, textY++, textID++, Config.Font.Text);
        }
      }

      private void DrawSelection()
      {
        var x = originLeft + widthPadding + borderSize - 1;
        var y = originTop + heightPadding + borderSize + currentTitleHeight + selectedChoiceID;
        DrawLine(Config.Glyph.Arrow, x, y, arrowID, Config.Font.Gui);
        drawnSelectionIds = (x, y, arrowID);
      }

      private void EraseLastSelection()
      {
        NWScript.PostString(
          PC: player.oid, Msg: "",
          X: drawnSelectionIds.X,
          Y: drawnSelectionIds.Y,
          ID: drawnSelectionIds.ID,
          life: 0.000001f
        );
      }

      private void DrawLine(string text, int x, int y, int id, string font)
      {
        int color = unchecked((int)Config.Color.White);
        NWScript.PostString(
            PC: player.oid, Msg: text, X: x, Y: y, ID: id, life: 0f,
            RGBA: color, RGBA2: color, font: font
        );
        drawnLineIds.Add((x, y, id));
      }

      private void HandleKeydown(object sender, PlayerSystem.Player.KeydownEventArgs e)
      {
        switch (e.key)
        {
          default: return;

          case "W":
            selectedChoiceID = (selectedChoiceID + currentChoices.Count - 1) % currentChoices.Count;
            EraseLastSelection();
            NWNX.Player.PlaySound(player.oid, "gui_select", NWObject.OBJECT_INVALID);
            DrawSelection();
            return;

          case "S":
            selectedChoiceID = (selectedChoiceID + 1) % currentChoices.Count;
            EraseLastSelection();
            NWNX.Player.PlaySound(player.oid, "gui_select", NWObject.OBJECT_INVALID);
            DrawSelection();
            return;

          case "E":
            var handler = currentChoices.ElementAtOrDefault(selectedChoiceID).handler;
            NWNX.Player.PlaySound(player.oid, "gui_picklockopen", NWObject.OBJECT_INVALID);
            handler?.Invoke();
            return;
        }
      }
    }
  }
}
