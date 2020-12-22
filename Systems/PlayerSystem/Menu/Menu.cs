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
      public bool isOpen = false;

      public Menu(Player player)
      {
        this.player = player;
        ResetConfig();
      }

      public void Draw()
      {
        if (!isOpen)
        {
          player.LoadMenuQuickbar(QuickbarType.Menu);
          player.OnKeydown += HandleMenuFeatUsed;
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
          player.OnKeydown -= HandleMenuFeatUsed;
          player.UnloadMenuQuickbar();

          if(NWScript.GetTag(NWScript.GetArea(player.oid)).StartsWith("entry_scene_"))
          {
            uint oClone = NWScript.GetNearestObjectByTag($"clone_{NWScript.GetPCPublicCDKey(player.oid)}", player.oid);
            VisibilityPlugin.SetVisibilityOverride(player.oid, NWScript.GetNearestObjectByTag("intro_mirror", player.oid), VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
            CreaturePlugin.JumpToLimbo(oClone);
            NWScript.DestroyObject(oClone);
          }
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
          NWScript.PostString(player.oid, "", X, Y, 0, 0.000001f, 0, 0, ID);
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
          0,
          0.000001f,
          0,
          0,
          drawnSelectionIds.ID
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

      public void HandleMenuFeatUsed(object sender, Player.MenuFeatEventArgs e)
      {
        switch (player.loadedQuickBar)
        {
          case QuickbarType.Invalid:
            return;
          case QuickbarType.Menu:
            switch (e.feat)
            {
              default: return;

              case Feat.CustomMenuUP:
                selectedChoiceID = (selectedChoiceID + choices.Count - 1) % choices.Count;
                EraseLastSelection();
                PlayerPlugin.PlaySound(player.oid, "gui_select", NWScript.OBJECT_INVALID);
                DrawSelection();
                return;

              case Feat.CustomMenuDOWN:
                selectedChoiceID = (selectedChoiceID + 1) % choices.Count;
                EraseLastSelection();
                PlayerPlugin.PlaySound(player.oid, "gui_select", NWScript.OBJECT_INVALID);
                DrawSelection();
                return;

              case Feat.CustomMenuSELECT:
                var handler = choices.ElementAtOrDefault(selectedChoiceID).handler;
                PlayerPlugin.PlaySound(player.oid, "gui_picklockopen", NWScript.OBJECT_INVALID);
                handler?.Invoke();
                return;
              case Feat.CustomMenuEXIT:
                player.menu.Close();
                return;
            }
          case QuickbarType.Sit:
            float zPos;
            float newValue;
            NWScript.SendMessageToPC(NWScript.GetFirstPC(), $"feat : {e.feat}");

            switch(e.feat)
            {
              default: return;

              case Feat.CustomMenuUP:
                newValue = 0.1f;
                if (player.setValue > 0)
                  newValue = player.setValue;

                NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z) + newValue);
                  zPos = NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z);
                  if (zPos > 5)
                    Utils.LogMessageToDMs($"SIT COMMAND - Player {NWScript.GetName(player.oid)} - Z translation = {zPos}");

                break;

              case Feat.CustomMenuDOWN:
                newValue = -0.1f;
                if (player.setValue > 0)
                  newValue = -player.setValue;

                NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z) + newValue);
                zPos = NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z);
                if (zPos < NWScript.GetGroundHeight(NWScript.GetLocation(player.oid)))
                  Utils.LogMessageToDMs($"SIT COMMAND - Player {NWScript.GetName(player.oid)} - Z translation = {zPos}");
                break;

              case Feat.CustomPositionRotateRight:
                newValue = 20.0f;
                if (player.setValue > 0)
                  newValue = player.setValue;

                NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_X, NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_X) + newValue);
                break;

              case Feat.CustomPositionRotateLeft:
                newValue = -20.0f;
                if (player.setValue > 0)
                  newValue = -player.setValue;

                NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_X, NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_X) + newValue);
                break;

              case Feat.CustomPositionRight:
                newValue = 0.1f;
                if (player.setValue > 0)
                  newValue = player.setValue;

                NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X,
                NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X) + newValue);
                break;

              case Feat.CustomPositionLeft:
                newValue = 0.1f;
                if (player.setValue > 0)
                  newValue = player.setValue;

                NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X,
                NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X) - newValue);
                break;

              case Feat.CustomPositionForward:
                newValue = 0.1f;
                if (player.setValue > 0)
                  newValue = player.setValue;

                NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y,
                NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y) + newValue);
                break;

              case Feat.CustomPositionBackward:
                newValue = 0.1f;
                if (player.setValue > 0)
                  newValue = player.setValue;

                NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y,
                NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y) - newValue);
                break;

              case Feat.CustomMenuEXIT:
                player.UnloadMenuQuickbar();
                NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_X, 0.0f);
                NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, 0.0f);
                NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, 0.0f);
                NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, 0.0f);
                player.setValue = 0;
                player.OnKeydown -= HandleMenuFeatUsed;
                return;
            }
            break;
        }
      }
    }
  }
}
