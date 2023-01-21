using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PeteTimesSix.SimpleSidearms.UI.SideamsCommandDrawer
{
    public class SidearmsPreferenceCommandDrawer : SidearmsWeaponCommandDrawer
    {
        protected static readonly float preference_icon_rows = 3.0f;
        protected float preference_icon_height;
        protected float preference_icon_width;

        protected static readonly Color preference_color_base = new Color(0.5f, 0.5f, 0.5f, 1f);
        protected static readonly Color preference_color_set = new Color(0.5f, 1.0f, 0.5f, 1f);
        protected static readonly Color preference_color_skill = new Color(0.5f, 1.0f, 0.5f, 1f);
        protected static readonly Color preference_color_highlight = new Color(0.7f, 0.7f, 0.4f, 1f);
        protected static readonly Color preference_color_highlight_set = new Color(0.7f, 1.0f, 0.4f, 1f);

        protected float preference_lefttop_x;
        protected float preference_lefttop_y;

        protected float preferenceIconsWidth()
        {
            return preference_icon_width;
        }

        public override void Reset(int weaponColumns, float maxWidth)
        {
            _dawingRects.Clear();

            panel_rect.width = 0;
            panel_rect.width += content_padding * 2.0f;
            panel_rect.width += preferenceIconsWidth();
            panel_rect.width += weaponIconsWidth(weaponColumns);
            panel_rect.width = Math.Min(maxWidth, panel_rect.width);

            preference_icon_height = (panel_rect.height - content_padding * 2.0f - icon_gap * (preference_icon_rows - 1.0f)) / preference_icon_rows;
            preference_icon_width = weapon_icon_width;
        }

        public void BeginDrawPreference(int rowIndex)
        {
            var rect = DrawingRect;

            _dawingRects.Push(new Rect(
                preference_lefttop_x,
                preference_lefttop_y + preference_icon_height * (float)(rowIndex + 1) + icon_gap * (float)rowIndex,
                preference_icon_width,
                preference_icon_height));
        }

        public void EndDrawPreference()
        {
            _dawingRects.Pop();
        }

        public static void SetIconHighlightColor(bool selected)
        {
            GUI.color = selected ? preference_color_highlight_set : preference_color_highlight;
        }

        //public static void SetPreferenceIconColor(PrimaryWeaponMode iconType, PrimaryWeaponMode primaryType, PrimaryWeaponMode skillPref)
        //{
        //    if (iconType == primaryType)
        //        GUI.color = preference_color_set;
        //    else if (iconType == skillPref && iconType == PrimaryWeaponMode.BySkill)
        //        GUI.color = preference_color_skill;
        //    else
        //        GUI.color = preference_color_base;
        //}

        //public static void DrawPreferenceIconWithTip(Rect iconRect, Texture2D texture, string tip)
        //{
        //    GUI.DrawTexture(iconRect, texture);
        //    TooltipHandler.TipRegion(iconRect, tip);
        //    MouseoverSounds.DoRegion(iconRect, SoundDefOf.Mouseover_Command);
        //}
    }
}
