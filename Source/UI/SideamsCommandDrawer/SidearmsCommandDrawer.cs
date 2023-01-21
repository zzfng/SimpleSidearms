using RimWorld;
using SimpleSidearms.rimworld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PeteTimesSix.SimpleSidearms.UI.SideamsCommandDrawer
{
    public abstract class SidearmsCommandDrawer
    {
        protected Rect panel_rect = new Rect(0,0,Gizmo.Height, Gizmo.Height);
        protected static readonly float content_padding = 4.0f;

        protected Stack<Rect> _dawingRects = new Stack<Rect>();

        protected Rect DrawingRect { get { return _dawingRects.Peek(); } }

        public virtual void BeginDrawPanel(Vector2 topleft, float maxWidth)
        {
            panel_rect.width = Math.Min(maxWidth, panel_rect.width);
            panel_rect.x = topleft.x;
            panel_rect.y = topleft.y;
            _dawingRects.Push(panel_rect);
        }

        public float PanelWidth { get { return panel_rect.width; } }


        public void MouseOverTeachOpportunity(ConceptDef conc, OpportunityType type)
        {
            if (Mouse.IsOver(DrawingRect))
            {
                LessonAutoActivator.TeachOpportunity(conc, type);
            }
        }

        public void EndDrawPanel()
        {
            _dawingRects.Pop();
        }

        public void BeginDrawContent()
        {
            var rect = DrawingRect;
            rect.x += content_padding;
            rect.y += content_padding;
            rect.width  -= content_padding * 2;
            rect.height -= content_padding * 2;
            _dawingRects.Push(rect);
        }

        public void DrawBackground()
        {
            Widgets.DrawWindowBackground(DrawingRect);
        }

        public void EndDrawContent()
        {
            _dawingRects.Pop();
        }




    }
}
