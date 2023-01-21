using PeteTimesSix.SimpleSidearms.Rimworld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PeteTimesSix.SimpleSidearms.UI.SideamsCommandDrawer
{

    public class SidearmsWeaponCommandDrawer : SidearmsCommandDrawer
    {

        protected readonly float icon_gap = 3.0f;

        protected readonly int weapon_icon_rows = 2;
        protected float weapon_icon_height; 
        protected float weapon_icon_width;

        public static readonly Color weapon_color_base = new Color(0.5f, 0.5f, 0.5f, 1f);
        public static readonly Color weapon_color_mouseover = new Color(0.6f, 0.6f, 0.4f, 1f);


        WeaponStuffDefPair _lastInteractedWeapon;

        public SidearmsWeaponCommandDrawer()
        { 
            this.weapon_icon_height =
                (panel_rect.height -content_padding * 2 - icon_gap * (weapon_icon_rows - 1)) / weapon_icon_rows;
            this.weapon_icon_width = this.weapon_icon_height;
        }

        protected float weaponIconsWidth(int weaponColumns)
        {
            return (float)weaponColumns * weapon_icon_width + (float)(weaponColumns - 1) * icon_gap;
        }

        public virtual void Reset(int weaponColumns, float maxWidth)
        {
            _dawingRects.Clear();
            _lastInteractedWeapon = null;

            panel_rect.width = 0;
            panel_rect.width += content_padding * 2.0f;
            panel_rect.width += weaponIconsWidth(weaponColumns);
            panel_rect.width = Math.Min(maxWidth, panel_rect.width);


        }

        public virtual void BeginDrawWeapon(int rowIndex, int columnIndex)
        {
            var rect = DrawingRect;

            _dawingRects.Push(
                new Rect(
                    rect.x + weapon_icon_width * columnIndex + icon_gap * columnIndex,
                    rect.y + weapon_icon_height * rowIndex + icon_gap * rowIndex,
                    weapon_icon_width,
                    weapon_icon_height
                ));
        }

        public void DrawWeaponIcons(IEnumerable<WeaponStuffDefPair> weapons, int rowIndex)
        {
            int columnIndex = 0;
            foreach (var w in weapons)
            {
                BeginDrawWeapon(rowIndex, columnIndex);
                DrawWeapon(w);
                EndDrawWeapon();
                columnIndex++;
            }
        }

        public virtual void DrawWeapon(WeaponStuffDefPair weapon)
        {
            var rect = DrawingRect;
            if (Mouse.IsOver(rect))
                GUI.color = weapon_color_mouseover;
            else
                GUI.color = weapon_color_base;
            GUI.DrawTexture(rect, TextureResources.drawPocket);

            var graphic = weapon.WeaponDef.graphic;
            if (graphic is Graphic_StackCount)
                graphic = (graphic as Graphic_StackCount).SubGraphicForStackCount(1, weapon.WeaponDef);

            Texture resolvedIcon = (Texture2D)graphic.MatEast.mainTexture;
            GUI.color = weapon.WeaponColor;
            GUI.DrawTexture(rect, resolvedIcon);

            if (Widgets.ButtonInvisible(rect))
            {
                if (Event.current.button == 0)
                {
                    _lastInteractedWeapon = weapon;
                    Event.current.Use();
                }
            }
        }

        

        public WeaponStuffDefPair GetLastInteractedWeapon() 
        { 
            return _lastInteractedWeapon;  
        }

        public void EndDrawWeapon()
        {
            _dawingRects.Pop();
        }
    }
}
