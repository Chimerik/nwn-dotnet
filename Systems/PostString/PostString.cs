using NWN.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NWN.Systems.PostString
{
    class PostString
    {
        private readonly static string GUI_FONT_TEXT_NAME = "fnt_es_text";
        private readonly static string GUI_FONT_GUI_NAME = "fnt_es_gui";
        private readonly static string GUI_FONT_GUI_GLYPH_WINDOW_TOP_LEFT = "a";
        private readonly static string GUI_FONT_GUI_GLYPH_WINDOW_TOP_RIGHT = "c";
        private readonly static string GUI_FONT_GUI_GLYPH_WINDOW_TOP_MIDDLE = "b";
        private readonly static string GUI_FONT_GUI_GLYPH_WINDOW_MIDDLE_LEFT = "d";
        private readonly static string GUI_FONT_GUI_GLYPH_WINDOW_MIDDLE_RIGHT = "f";
        private readonly static string GUI_FONT_GUI_GLYPH_WINDOW_MIDDLE_BLANK = "i";
        private readonly static string GUI_FONT_GUI_GLYPH_WINDOW_BOTTOM_LEFT = "h";
        private readonly static string GUI_FONT_GUI_GLYPH_WINDOW_BOTTOM_RIGHT = "g";
        private readonly static string GUI_FONT_GUI_GLYPH_WINDOW_BOTTOM_MIDDLE = "e";
        private readonly static string GUI_FONT_GUI_GLYPH_ARROW = "j";
        private readonly static string GUI_FONT_GUI_GLYPH_BLANK_WHITE = "k";

        public readonly static uint GUI_COLOR_TRANSPARENT = 0xFFFFFF00;
        public readonly static uint GUI_COLOR_WHITE = 0xFFFFFFFF;
        public readonly static uint GUI_COLOR_SILVER = 0xC0C0C0FF;
        public readonly static uint GUI_COLOR_GRAY = 0x808080FF;
        public readonly static uint GUI_COLOR_DARK_GRAY = 0x303030FF;
        public readonly static uint GUI_COLOR_BLACK = 0x000000FF;
        public readonly static uint GUI_COLOR_RED = 0xFF0000FF;
        public readonly static uint GUI_COLOR_MAROON = 0x800000FF;
        public readonly static uint GUI_COLOR_ORANGE = 0xFFA500FF;
        public readonly static uint GUI_COLOR_YELLOW = 0xFFFF00FF;
        public readonly static uint GUI_COLOR_OLIVE = 0x808000FF;
        public readonly static uint GUI_COLOR_LIME = 0x00FF00FF;
        public readonly static uint GUI_COLOR_GREEN = 0x008000FF;
        public readonly static uint GUI_COLOR_AQUA = 0x00FFFFFF;
        public readonly static uint GUI_COLOR_TEAL = 0x008080FF;
        public readonly static uint GUI_COLOR_BLUE = 0x0000FFFF;
        public readonly static uint GUI_COLOR_NAVY = 0x000080FF;
        public readonly static uint GUI_COLOR_FUSCHIA = 0xFF00FFFF;
        public readonly static uint GUI_COLOR_PURPLE = 0x800080FF;

        public static void GUI_Draw(NWObject oPlayer, string sMessage, int nX, int nY, int nAnchor, int nID, float fLifeTime = 0.0f)
        {
            if (fLifeTime > 0.0f)
                NWScript.DeleteLocalInt(oPlayer, "_DISPLAY_HEALTHBAR");
            else
                NWScript.SetLocalInt(oPlayer, "_DISPLAY_HEALTHBAR", 1);

            NWScript.PostString(oPlayer, sMessage, nX, nY, (ScreenAnchor)nAnchor, fLifeTime, (int)GUI_COLOR_WHITE, (int)GUI_COLOR_WHITE, nID, GUI_FONT_GUI_NAME);
        }

        public static int GUI_DrawWindow(NWObject oPlayer, int nStartID, int nAnchor, int nX, int nY, int nWidth, int nHeight, float fLifetime = 0.0f)
        {
            string sTop = GUI_FONT_GUI_GLYPH_WINDOW_TOP_LEFT;
            string sMiddle = GUI_FONT_GUI_GLYPH_WINDOW_MIDDLE_LEFT;
            string sBottom = GUI_FONT_GUI_GLYPH_WINDOW_BOTTOM_LEFT;

            int i;
            for (i = 0; i < nWidth; i++)
            {
                sTop += GUI_FONT_GUI_GLYPH_WINDOW_TOP_MIDDLE;
                sMiddle += GUI_FONT_GUI_GLYPH_WINDOW_MIDDLE_BLANK;
                sBottom += GUI_FONT_GUI_GLYPH_WINDOW_BOTTOM_MIDDLE;
            }

            sTop += GUI_FONT_GUI_GLYPH_WINDOW_TOP_RIGHT;
            sMiddle += GUI_FONT_GUI_GLYPH_WINDOW_MIDDLE_RIGHT;
            sBottom += GUI_FONT_GUI_GLYPH_WINDOW_BOTTOM_RIGHT;

            GUI_Draw(oPlayer, sTop, nX, nY, nAnchor, nStartID++, fLifetime);
            for (i = 0; i < nHeight; i++)
            {
                GUI_Draw(oPlayer, sMiddle, nX, ++nY, nAnchor, nStartID++, fLifetime);
            }
            GUI_Draw(oPlayer, sBottom, nX, ++nY, nAnchor, nStartID, fLifetime);

            return nHeight + 2;
        }
        public static int GUI_CenterStringInWindow(string sString, int nWindowX, int nWindowWidth)
        {
            return (nWindowX + (nWindowWidth / 2)) - ((sString.Length + 2) / 2);
        }

        public static void Menu_UpdateGUI(NWObject oPlayer)
        {
            int nID = 3000;
            int nTextColor = (int)GUI_COLOR_WHITE;
            string sTextFont = GUI_FONT_TEXT_NAME;
            float fLifeTime = 0.0f;

            int nCurrentGUISelection = NWScript.GetLocalInt(oPlayer, "CurrentGUISelection");
            int nInvasionMode = NWScript.GetLocalInt(oPlayer, "InvasionMode");

            if (nCurrentGUISelection.Equals(0))
                NWScript.PostString(oPlayer, "> Start", 3, 4, ScreenAnchor.TopLeft, fLifeTime, nTextColor, nTextColor, nID++, sTextFont);
            else
                NWScript.PostString(oPlayer, "Start", 3, 4, ScreenAnchor.TopLeft, fLifeTime, nTextColor, nTextColor, nID++, sTextFont);

            if (nCurrentGUISelection == 1)
                NWScript.PostString(oPlayer, "> Stop", 3, 5, ScreenAnchor.TopLeft, fLifeTime, nTextColor, nTextColor, nID++, sTextFont);
            else
                NWScript.PostString(oPlayer, "Stop", 3, 5, ScreenAnchor.TopLeft, fLifeTime, nTextColor, nTextColor, nID++, sTextFont);

            if (nCurrentGUISelection == 2)
                NWScript.PostString(oPlayer, "> Exit", 3, 6, ScreenAnchor.TopLeft, fLifeTime, nTextColor, nTextColor, nID++, sTextFont);
            else
                NWScript.PostString(oPlayer, "Exit", 3, 6, ScreenAnchor.TopLeft, fLifeTime, nTextColor, nTextColor, nID++, sTextFont);
        }

        public static void Menu_DrawStaticGUI(NWObject oPlayer)
        {
            int nID = 3050;
            int nTextColor = (int)GUI_COLOR_WHITE;
            string sTextFont = GUI_FONT_TEXT_NAME;
            //float fLifeTime = 0.0f;

            // Menu Window
            GUI_DrawWindow(oPlayer, nID, (int)ScreenAnchor.TopLeft, 1, 3, 12, 4);
        }
    }
}
