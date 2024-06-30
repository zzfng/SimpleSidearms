using PeteTimesSix.SimpleSidearms.Rimworld;
using RimWorld;
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
        const float icon_row_gap_default = 3.0f;
        const float icon_column_gap_default = 3.0f;

        protected float icon_row_gap;
        protected readonly float icon_column_gap = icon_column_gap_default;

        protected readonly int weapon_icon_rows = 2;
        protected float weapon_icon_side;


        public static readonly Color weapon_color_base = new Color(0.5f, 0.5f, 0.5f, 1f);
        public static readonly Color weapon_color_mouseover = new Color(0.6f, 0.6f, 0.4f, 1f);


        WeaponStuffDefPairBase _lastInteractedWeapon;

        public SidearmsWeaponCommandDrawer()
        { 
        }


        public virtual void Reset(int weaponColumns, float maxWidth)
        {
            _dawingRects.Clear();
            _lastInteractedWeapon = null;

            icon_row_gap = icon_row_gap_default;
            weapon_icon_side = (panel_rect.height - content_padding * 2 - icon_row_gap * (weapon_icon_rows - 1)) / weapon_icon_rows;
            panel_rect.width = 0;
            panel_rect.width += content_padding * 2.0f;
            panel_rect.width += weaponColumns * weapon_icon_side + (weaponColumns - 1) * icon_column_gap;
            if (panel_rect.width > maxWidth)
            {
                panel_rect.width = maxWidth;
                weapon_icon_side = panel_rect.width;
                weapon_icon_side -= content_padding * 2.0f;
                weapon_icon_side = (weapon_icon_side - (weaponColumns - 1) * icon_column_gap) / weaponColumns ;
                icon_row_gap = ((panel_rect.height - weapon_icon_rows * weapon_icon_side) - content_padding * 2) / (weapon_icon_rows - 1);
            }
        }

        public virtual void BeginDrawWeapon(int rowIndex, int columnIndex)
        {
            var rect = DrawingRect;

            _dawingRects.Push(
                new Rect(
                    rect.x + weapon_icon_side * columnIndex + icon_column_gap * columnIndex,
                    rect.y + weapon_icon_side * rowIndex + icon_row_gap * rowIndex,
                    weapon_icon_side,
                    weapon_icon_side
                ));
        }

        public void DrawWeaponIcons(IEnumerable<WeaponStuffDefPairBase> weapons, int rowIndex, int columnIndex = 0)
        {
            foreach (var w in weapons)
            {
                BeginDrawWeapon(rowIndex, columnIndex);
                DrawWeapon(w);
                EndDrawWeapon();
                columnIndex++;
            }
        }

        public virtual void DrawWeapon(WeaponStuffDefPairBase weaponBase)
        {
            var rect = DrawingRect;
            if (Mouse.IsOver(rect))
                GUI.color = weapon_color_mouseover;
            else
                GUI.color = weapon_color_base;
            GUI.DrawTexture(rect, TextureResources.drawPocket);

            Texture resolvedIcon;
            
            if (weaponBase.Unarmed)
            {
                resolvedIcon = TexCommand.AttackMelee;
                GUI.color = Color.white;
            }
            else
            {
                var weapon = weaponBase as WeaponStuffDefPair;
                var graphic = weapon.WeaponDef.graphic;
                if (graphic is Graphic_StackCount)
                    graphic = (graphic as Graphic_StackCount).SubGraphicForStackCount(1, weapon.WeaponDef);

                resolvedIcon = (Texture2D)graphic.MatEast.mainTexture;
                GUI.color = weapon.WeaponColor;
            }

            GUI.DrawTexture(rect, resolvedIcon);

            if (Widgets.ButtonInvisible(rect))
            {
                if (Event.current.button == 0)
                {
                    _lastInteractedWeapon = weaponBase;
                    Event.current.Use();
                }
            }
        }
        

        public WeaponStuffDefPairBase GetLastInteractedWeapon() 
        { 
            return _lastInteractedWeapon;  
        }

        public void EndDrawWeapon()
        {
            _dawingRects.Pop();
        }
    }
}
