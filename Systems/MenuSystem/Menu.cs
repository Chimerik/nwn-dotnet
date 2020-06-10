using System;
using System.Collections.Generic;
using System.Linq;

namespace NWN.Systems.MenuSystem
{
  public class Menu
  {
    public string title { get; set; } = "";
    public List<(string text, Action handler)> choices = new List<(string text, Action handler)>();

    private readonly PlayerSystem.Player player;

    private const int originTop = 4;
    private const int originLeft = 2;
    private const int borderSize = 1;
    private const int widthPadding = 2;
    private const int heightPadding = 1;
    private const int titleBottomMargin = 1;

    private int titleHeight
    {
      get
      {
        if (title == "") return 0;
        return 1 + titleBottomMargin;
      }
    }

    private List<(int X, int Y, int ID)> drawnLineIds = new List<(int X, int Y, int ID)>();
    private const int windowBaseID = 9000;
    private const int textBaseID = 8500;
    private const int arrowID = 8499;

    private int selectedChoiceID = 0;
    private bool isDrawn = false;

    public Menu(PlayerSystem.Player player)
    {
      this.player = player;
    }

    public void Draw()
    {
      if (!isDrawn)
      {
        player.OnKeydown += HandleKeydown;
      }

      var width = CalcWindowWidth();
      var height = CalcWindowHeight();
      DrawWindow(width, height);
      DrawText();
      DrawSelection();

      isDrawn = true;
    }

    public void Hide()
    {
      if (isDrawn)
      {
        player.OnKeydown -= HandleKeydown;
      }

      foreach (var (X, Y, ID) in drawnLineIds)
      {
        NWScript.PostString(PC: player.oid, Msg: "", X: X, Y: Y, ID: ID, life: 0.000001f);
      }
      drawnLineIds.Clear();

      isDrawn = false;
    }

    public void Clear()
    {
      title = "";
      choices.Clear();
      selectedChoiceID = 0;
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

      if (title != "")
      {
        DrawLine(title, textX, textY++, textID++, Config.Font.Text);
        textY += titleBottomMargin;
      }

      for (var i = 0; i < choices.Count; i++)
      {
        var (text, _) = choices[i];
        DrawLine(text, textX, textY++, textID++, Config.Font.Text);
      }
    }

    private void DrawSelection()
    {
      var x = originLeft + widthPadding + borderSize - 1;
      var y = originTop + heightPadding + borderSize + titleHeight + selectedChoiceID;
      NWNX.Player.PlaySound(player.oid, "gui_select", NWObject.OBJECT_INVALID);
      DrawLine(Config.Glyph.Arrow, x, y, arrowID, Config.Font.Gui);
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

    private void HandleKeydown (object sender, PlayerSystem.Player.KeydownEventArgs e)
    {
      Console.WriteLine($"HandleKeydown key={e.key}");

      switch (e.key)
      {
        default: return;

        case "W":
          selectedChoiceID = (selectedChoiceID + choices.Count - 1) % choices.Count;
          DrawSelection();
          return;

        case "S":
          selectedChoiceID = (selectedChoiceID + 1) % choices.Count;
          DrawSelection();
          return;

        case "E":
          var handler = choices.ElementAtOrDefault(selectedChoiceID).handler;
          Hide();
          Clear();
          NWNX.Player.PlaySound(player.oid, "gui_picklockopen", NWObject.OBJECT_INVALID);
          handler?.Invoke();
          return;
      }
    }
  }
}
