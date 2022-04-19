using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using Newtonsoft.Json;

using static NWN.Core.NWScript;

using json = System.IntPtr;

namespace Test
{
  [ServiceBinding(typeof(Example))]
  public sealed partial class Example
  {
    const int NUI_DIRECTION_HORIZONTAL = 0;
    const int NUI_DIRECTION_VERTICAL = 1;

    const int NUI_MOUSE_BUTTON_LEFT = 0;
    const int NUI_MOUSE_BUTTON_MIDDLE = 1;
    const int NUI_MOUSE_BUTTON_RIGHT = 2;

    const int NUI_SCROLLBARS_NONE = 0;
    const int NUI_SCROLLBARS_X = 1;
    const int NUI_SCROLLBARS_Y = 2;
    const int NUI_SCROLLBARS_BOTH = 3;
    const int NUI_SCROLLBARS_AUTO = 4;

    const int NUI_ASPECT_FIT = 0;
    const int NUI_ASPECT_FILL = 1;
    const int NUI_ASPECT_FIT100 = 2;
    const int NUI_ASPECT_EXACT = 3;
    const int NUI_ASPECT_EXACTSCALED = 4;
    const int NUI_ASPECT_STRETCH = 5;

    const int NUI_HALIGN_CENTER = 0;
    const int NUI_HALIGN_LEFT = 1;
    const int NUI_HALIGN_RIGHT = 2;

    const int NUI_VALIGN_MIDDLE = 0;
    const int NUI_VALIGN_TOP = 1;
    const int NUI_VALIGN_BOTTOM = 2;

    // -----------------------
    // Style

    const float NUI_STYLE_PRIMARY_WIDTH = 150.0f;
    const float NUI_STYLE_PRIMARY_HEIGHT = 50.0f;

    const float NUI_STYLE_SECONDARY_WIDTH = 150.0f;
    const float NUI_STYLE_SECONDARY_HEIGHT = 35.0f;

    const float NUI_STYLE_TERTIARY_WIDTH = 100.0f;
    const float NUI_STYLE_TERTIARY_HEIGHT = 30.0f;

    const float NUI_STYLE_ROW_HEIGHT = 25.0f;

    const int NUI_DRAW_LIST_ITEM_TYPE_POLYLINE = 0;
    const int NUI_DRAW_LIST_ITEM_TYPE_CURVE = 1;
    const int NUI_DRAW_LIST_ITEM_TYPE_CIRCLE = 2;
    const int NUI_DRAW_LIST_ITEM_TYPE_ARC = 3;
    const int NUI_DRAW_LIST_ITEM_TYPE_TEXT = 4;
    const int NUI_DRAW_LIST_ITEM_TYPE_IMAGE = 5;

    json
      NuiWindow(
        json jRoot,
        json jTitle,
        json jGeometry,
        json jResizable,
        json jCollapsed,
        json jClosable,
        json jTransparent,
        json jBorder
      )
    {
      json ret = JsonObject();
      // Currently hardcoded and here to catch backwards-incompatible data in the future.
      ret = JsonObjectSet(ret, "version", JsonInt(1));
      ret = JsonObjectSet(ret, "title", jTitle);
      ret = JsonObjectSet(ret, "root", jRoot);
      ret = JsonObjectSet(ret, "geometry", jGeometry);
      ret = JsonObjectSet(ret, "resizable", jResizable);
      ret = JsonObjectSet(ret, "collapsed", jCollapsed);
      ret = JsonObjectSet(ret, "closable", jClosable);
      ret = JsonObjectSet(ret, "transparent", jTransparent);
      ret = JsonObjectSet(ret, "border", jBorder);
      return ret;
    }

    json
      NuiElement(
        string sType,
        json jLabel,
        json jValue
      )
    {
      json ret = JsonObject();
      ret = JsonObjectSet(ret, "type", JsonString(sType));
      ret = JsonObjectSet(ret, "label", jLabel);
      ret = JsonObjectSet(ret, "value", jValue);
      return ret;
    }

    json
      NuiBind(
        string sId
      )
    {
      return JsonObjectSet(JsonObject(), "bind", JsonString(sId));
    }

    json
      NuiId(
        json jElem,
        string sId
      )
    {
      return JsonObjectSet(jElem, "id", JsonString(sId));
    }

    json
      NuiCol(
        json jList
      )
    {
      return JsonObjectSet(NuiElement("col", JsonNull(), JsonNull()), "children", jList);
    }

    json
      NuiRow(
        json jList
      )
    {
      return JsonObjectSet(NuiElement("row", JsonNull(), JsonNull()), "children", jList);
    }

    json
      NuiGroup(
        json jChild,
        int bBorder = TRUE,
        int nScroll = NUI_SCROLLBARS_AUTO
      )
    {
      json ret = NuiElement("group", JsonNull(), JsonNull());
      ret = JsonObjectSet(ret, "children", JsonArrayInsert(JsonArray(), jChild));
      ret = JsonObjectSet(ret, "border", JsonBool(bBorder));
      ret = JsonObjectSet(ret, "scrollbars", JsonInt(nScroll));
      return ret;
    }

    json
      NuiWidth(json jElem, float fWidth)
    {
      return JsonObjectSet(jElem, "width", JsonFloat(fWidth));
    }

    json
      NuiHeight(json jElem, float fHeight)
    {
      return JsonObjectSet(jElem, "height", JsonFloat(fHeight));
    }

    json
      NuiAspect(json jElem, float fAspect)
    {
      return JsonObjectSet(jElem, "aspect", JsonFloat(fAspect));
    }

    json
      NuiMargin(
        json jElem,
        float fMargin
      )
    {
      return JsonObjectSet(jElem, "margin", JsonFloat(fMargin));
    }

    json
      NuiPadding(
        json jElem,
        float fPadding
      )
    {
      return JsonObjectSet(jElem, "padding", JsonFloat(fPadding));
    }

    json
      NuiEnabled(
        json jElem,
        json jEnabler
      )
    {
      return JsonObjectSet(jElem, "enabled", jEnabler);
    }

    json
      NuiVisible(
        json jElem,
        json jVisible
      )
    {
      return JsonObjectSet(jElem, "visible", jVisible);
    }

    json
      NuiTooltip(
        json jElem,
        json jTooltip
      )
    {
      return JsonObjectSet(jElem, "tooltip", jTooltip);
    }

    json
      NuiVec(float x, float y)
    {
      json ret = JsonObject();
      ret = JsonObjectSet(ret, "x", JsonFloat(x));
      ret = JsonObjectSet(ret, "y", JsonFloat(y));
      return ret;
    }

    json
      NuiRect(float x, float y, float w, float h)
    {
      json ret = JsonObject();
      ret = JsonObjectSet(ret, "x", JsonFloat(x));
      ret = JsonObjectSet(ret, "y", JsonFloat(y));
      ret = JsonObjectSet(ret, "w", JsonFloat(w));
      ret = JsonObjectSet(ret, "h", JsonFloat(h));
      return ret;
    }

    json
      Color(int r, int g, int b, int a = 255)
    {
      json ret = JsonObject();
      ret = JsonObjectSet(ret, "r", JsonInt(r));
      ret = JsonObjectSet(ret, "g", JsonInt(g));
      ret = JsonObjectSet(ret, "b", JsonInt(b));
      ret = JsonObjectSet(ret, "a", JsonInt(a));
      return ret;
    }

    json
      NuiStyleForegroundColor(
        json jElem,
        json jColor
      )
    {
      return JsonObjectSet(jElem, "foreground_color", jColor);
    }

    json
      NuiSpacer()
    {
      return NuiElement("spacer", JsonNull(), JsonNull());
    }

    json
      NuiLabel(
        json jValue,
        json jHAlign,
        json jVAlign
      )
    {
      json ret = NuiElement("label", JsonNull(), jValue);
      ret = JsonObjectSet(ret, "text_halign", jHAlign);
      ret = JsonObjectSet(ret, "text_valign", jVAlign);
      return ret;
    }

    json
      NuiText(
        json jValue
      )
    {
      return NuiElement("text", JsonNull(), jValue);
    }

    json
      NuiButton(
        json jLabel
      )
    {
      return NuiElement("button", jLabel, JsonNull());
    }

    json
      NuiButtonImage(
        json jResRef
      )
    {
      return NuiElement("button_image", jResRef, JsonNull());
    }

    json
      NuiButtonSelect(
        json jLabel,
        json jValue
      )
    {
      return NuiElement("button_select", jLabel, jValue);
    }

    json
      NuiCheck(
        json jLabel,
        json jBool
      )
    {
      return NuiElement("check", jLabel, jBool);
    }

    json
      NuiImage(
        json jResRef,
        json jAspect,
        json jHAlign,
        json jVAlign
      )
    {
      json img = NuiElement("image", JsonNull(), jResRef);
      img = JsonObjectSet(img, "image_aspect", jAspect);
      img = JsonObjectSet(img, "image_halign", jHAlign);
      img = JsonObjectSet(img, "image_valign", jVAlign);
      return img;
    }

    json
      NuiCombo(
        json jElements,
        json jSelected
      )
    {
      return JsonObjectSet(NuiElement("combo", JsonNull(), jSelected), "elements", jElements);
    }

    json
      NuiComboEntry(
        string sLabel,
        int nValue
      )
    {
      return JsonArrayInsert(JsonArrayInsert(JsonArray(), JsonString(sLabel)), JsonInt(nValue));
    }

    json
      NuiSliderFloat(
        json jValue,
        json jMin,
        json jMax,
        json jStepSize
      )
    {
      json ret = NuiElement("sliderf", JsonNull(), jValue);
      ret = JsonObjectSet(ret, "min", jMin);
      ret = JsonObjectSet(ret, "max", jMax);
      ret = JsonObjectSet(ret, "step", jStepSize);
      return ret;
    }

    json
      NuiSlider(
        json jValue,
        json jMin,
        json jMax,
        json jStepSize
      )
    {
      json ret = NuiElement("slider", JsonNull(), jValue);
      ret = JsonObjectSet(ret, "min", jMin);
      ret = JsonObjectSet(ret, "max", jMax);
      ret = JsonObjectSet(ret, "step", jStepSize);
      return ret;
    }

    json
      NuiProgress(
        json jValue
      )
    {
      return NuiElement("progress", JsonNull(), jValue);
    }

    json
      NuiTextEdit(
        json jPlaceholder,
        json jValue,
        int nMaxLength,
        int bMultiline
      )
    {
      json ret = NuiElement("textedit", jPlaceholder, jValue);
      ret = JsonObjectSet(ret, "max", JsonInt(nMaxLength));
      ret = JsonObjectSet(ret, "multiline", JsonBool(bMultiline));
      return ret;
    }

    json
      NuiList(
        json jTemplate,
        json jRowCount,
        float fRowHeight = NUI_STYLE_ROW_HEIGHT
      )
    {
      json ret = NuiElement("list", JsonNull(), JsonNull());
      ret = JsonObjectSet(ret, "row_template", jTemplate);
      ret = JsonObjectSet(ret, "row_count", jRowCount);
      ret = JsonObjectSet(ret, "row_height", JsonFloat(fRowHeight));
      return ret;
    }

    json
      NuiListTemplateCell(
        json jElem,
        float fWidth,
        int bVariable
      )
    {
      json ret = JsonArray();
      ret = JsonArrayInsert(ret, jElem);
      ret = JsonArrayInsert(ret, JsonFloat(fWidth));
      ret = JsonArrayInsert(ret, JsonBool(bVariable));
      return ret;
    }

    json
      ColorPicker(
        json jColor
      )
    {
      json ret = NuiElement("color_picker", JsonNull(), jColor);
      return ret;
    }

    json
      NuiOptions(
        int nDirection,
        json jElements,
        json jValue
      )
    {
      json ret = NuiElement("options", JsonNull(), jValue);
      ret = JsonObjectSet(ret, "direction", JsonInt(nDirection));
      ret = JsonObjectSet(ret, "elements", jElements);
      return ret;
    }

    json
      NuiChartSlot(
        int nType,
        json jLegend,
        json jColor,
        json jData
      )
    {
      json ret = JsonObject();
      ret = JsonObjectSet(ret, "type", JsonInt(nType));
      ret = JsonObjectSet(ret, "legend", jLegend);
      ret = JsonObjectSet(ret, "color", jColor);
      ret = JsonObjectSet(ret, "data", jData);
      return ret;
    }

    json
      NuiChart(
        json jSlots
      )
    {
      json ret = NuiElement("chart", JsonNull(), jSlots);
      return ret;
    }

    json
      NuiDrawListItem(
        int nType,
        json jEnabled,
        json jColor,
        json jFill,
        json jLineThickness
      )
    {
      json ret = JsonObject();
      ret = JsonObjectSet(ret, "type", JsonInt(nType));
      ret = JsonObjectSet(ret, "enabled", jEnabled);
      ret = JsonObjectSet(ret, "color", jColor);
      ret = JsonObjectSet(ret, "fill", jFill);
      ret = JsonObjectSet(ret, "line_thickness", jLineThickness);
      return ret;
    }

    json
      NuiDrawListPolyLine(
        json jEnabled,
        json jColor,
        json jFill,
        json jLineThickness,
        json jPoints
      )
    {
      json ret = NuiDrawListItem(NUI_DRAW_LIST_ITEM_TYPE_POLYLINE, jEnabled, jColor, jFill, jLineThickness);
      ret = JsonObjectSet(ret, "points", jPoints);
      return ret;
    }

    json
      NuiDrawListCurve(
        json jEnabled,
        json jColor,
        json jLineThickness,
        json jA,
        json jB,
        json jCtrl0,
        json jCtrl1
      )
    {
      json ret = NuiDrawListItem(NUI_DRAW_LIST_ITEM_TYPE_CURVE, jEnabled, jColor, JsonBool(0), jLineThickness);
      ret = JsonObjectSet(ret, "a", jA);
      ret = JsonObjectSet(ret, "b", jB);
      ret = JsonObjectSet(ret, "ctrl0", jCtrl0);
      ret = JsonObjectSet(ret, "ctrl1", jCtrl1);
      return ret;
    }

    json
      NuiDrawListCircle(
        json jEnabled,
        json jColor,
        json jFill,
        json jLineThickness,
        json jRect
      )
    {
      json ret = NuiDrawListItem(NUI_DRAW_LIST_ITEM_TYPE_CIRCLE, jEnabled, jColor, jFill, jLineThickness);
      ret = JsonObjectSet(ret, "rect", jRect);
      return ret;
    }

    json
      NuiDrawListArc(
        json jEnabled,
        json jColor,
        json jFill,
        json jLineThickness,
        json jCenter,
        json jRadius,
        json jAMin,
        json jAMax
      )
    {
      json ret = NuiDrawListItem(NUI_DRAW_LIST_ITEM_TYPE_ARC, jEnabled, jColor, jFill, jLineThickness);
      ret = JsonObjectSet(ret, "c", jCenter);
      ret = JsonObjectSet(ret, "radius", jRadius);
      ret = JsonObjectSet(ret, "amin", jAMin);
      ret = JsonObjectSet(ret, "amax", jAMax);
      return ret;
    }

    json
      NuiDrawListText(
        json jEnabled,
        json jColor,
        json jRect,
        json jText
      )
    {
      json ret = NuiDrawListItem(NUI_DRAW_LIST_ITEM_TYPE_TEXT, jEnabled, jColor, JsonNull(), JsonNull());
      ret = JsonObjectSet(ret, "rect", jRect);
      ret = JsonObjectSet(ret, "text", jText);
      return ret;
    }

    json
      NuiDrawListImage(
        json jEnabled,
        json jResRef,
        json jRect,
        json jAspect,
        json jHAlign,
        json jVAlign
      )
    {
      json ret = NuiDrawListItem(NUI_DRAW_LIST_ITEM_TYPE_IMAGE, jEnabled, JsonNull(), JsonNull(), JsonNull());
      ret = JsonObjectSet(ret, "image", jResRef);
      ret = JsonObjectSet(ret, "rect", jRect);
      ret = JsonObjectSet(ret, "image_aspect", jAspect);
      ret = JsonObjectSet(ret, "image_halign", jHAlign);
      ret = JsonObjectSet(ret, "image_valign", jVAlign);
      return ret;
    }

    json
      NuiDrawList(
        json jElem,
        json jScissor,
        json jList
      )
    {
      json ret = JsonObjectSet(jElem, "draw_list", jList);
      ret = JsonObjectSet(ret, "draw_list_scissor", jScissor);
      return ret;
    }

    // json
    // NuiCanvas(
    //   json jList
    // )
    // {
    //   json ret = NuiElement("canvas", JsonNull(), jList);
    //   return ret;
    // }

    public Example()
    {

    }
    public void testDrawList(NwPlayer player)
    {
      json col = JsonArray();
      json group = JsonArray();

      /*json drawList = JsonArray();
        drawList = JsonArrayInsert(drawList, NuiDrawListImage(JsonBool(TRUE), JsonString("menu_exit"), NuiRect(0.0f, 0.0f, 25, 25), JsonInt(NUI_ASPECT_FILL), JsonInt(NUI_HALIGN_CENTER), JsonInt(NUI_VALIGN_TOP)));
        drawList = JsonArrayInsert(drawList, NuiDrawListImage(JsonBool(TRUE), JsonString("menu_up"), NuiRect(25, 25, 25, 25), JsonInt(NUI_ASPECT_FILL), JsonInt(NUI_HALIGN_CENTER), JsonInt(NUI_VALIGN_TOP)));
 */       //drawList = JsonArrayInsert(drawList, NuiDrawListCircle(JsonBool(TRUE), JsonString("menu_up"), NuiRect(25, 25, 25, 25), JsonInt(NUI_ASPECT_FILL), JsonInt(NUI_HALIGN_CENTER), JsonInt(NUI_VALIGN_TOP)));

      json jButton = NuiButton(JsonString("Test Update"));
      jButton = NuiId(jButton, "testUpdate");
      jButton = NuiGroup(jButton, TRUE, NUI_SCROLLBARS_NONE);
      jButton = NuiId(jButton, "somegroupid");
      /*jImage = NuiDrawList(jImage, JsonBool(TRUE), drawList);
      jImage = NuiWidth(jImage, 64.0f);
      jImage = NuiHeight(jImage, 192.0f);*/

      //row = JsonArrayInsert(row, jButton);
      //group = JsonArrayInsert(group, jButton);
      //col = NuiId(col, "testUpdateGroup");
      col = JsonArrayInsert(col, jButton);

      json root = NuiCol(col);
      json nui = NuiWindow(
      root,
      JsonString("test"),
      NuiRect(0.0f, 0.0f, 900.0f, 600.0f),
      JsonBool(FALSE),
      JsonBool(FALSE),
      JsonBool(TRUE),
      JsonBool(FALSE),
      JsonBool(TRUE));

      int token = NuiCreate(player.ControlledCreature, nui, "poviewer");

      player.OnNuiEvent -= HandleTestUpdateEvents;
      player.OnNuiEvent += HandleTestUpdateEvents;

      NWN.Systems.ModuleSystem.Log.Info(JsonDump(nui, 0));
    }
    private void HandleTestUpdateEvents(ModuleEvents.OnNuiEvent nuiEvent)
    {
      switch (nuiEvent.ElementId)
      {
        case "testUpdate":

          if (nuiEvent.EventType == NuiEventType.Click)
          {
            int nbClick = nuiEvent.Player.LoginCreature.GetObjectVariable<LocalVariableInt>("NUI_TEST_UPDATE").Value + 1;
            nuiEvent.Player.LoginCreature.GetObjectVariable<LocalVariableInt>("NUI_TEST_UPDATE").Value = nbClick;

            json jButton = NuiButton(JsonString($"Test Update {nbClick}"));
            jButton = NuiId(jButton, "testUpdate");

            NuiSetGroupLayout(nuiEvent.Player.ControlledCreature, nuiEvent.WindowToken, "somegroupid", jButton);
          }

          break;
      }
    }
  }
}
