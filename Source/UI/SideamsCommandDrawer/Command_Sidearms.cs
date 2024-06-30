using PeteTimesSix.SimpleSidearms.UI;
using PeteTimesSix.SimpleSidearms.UI.SideamsCommandDrawer;
using PeteTimesSix.SimpleSidearms.Utilities;
using RimWorld;
using SimpleSidearms.rimworld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using static PeteTimesSix.SimpleSidearms.Utilities.Enums;

namespace PeteTimesSix.SimpleSidearms.Rimworld
{
    public enum SidearmsButtonStyle
    {
        None,
        Gather,
        Inspect,
    }

    public class WeaponStuffDefPair: WeaponStuffDefPairBase
    {
        public ThingDef WeaponDef;
        public ThingDef StuffDef;
        public Color WeaponColor;

        public WeaponStuffDefPair(ThingWithComps weapon)
        {
            this.WeaponDef = weapon.def;
            this.StuffDef = weapon.Stuff;
            this.WeaponColor = weapon.DrawColor;
        }
    }

    public class WeaponStuffDefPairBase
    {
        public bool Unarmed = false;
    }

    public class WeaponStuffDefPairComparer : IComparer<WeaponStuffDefPair>
    {
        public static WeaponStuffDefPairComparer Instance = new WeaponStuffDefPairComparer();

        public int Compare(WeaponStuffDefPair x, WeaponStuffDefPair y)
        {
            if (x.WeaponDef == y.WeaponDef && x.StuffDef == y.StuffDef)
                return 0;

            int result = (int)((x.WeaponDef.BaseMarketValue - y.WeaponDef.BaseMarketValue) * 1000);
            if (result == 0)
            {
                result = (int)(( (x.StuffDef == null ? 0f : x.StuffDef.BaseMarketValue) - (y.StuffDef == null ? 0f : y.StuffDef.BaseMarketValue) ) * 1000);
                if (result == 0)
                    return 1;
            }

            return result ;
        }
    }

    public class Command_Sidearms_SharedData
    {


        public class PawnData
        {
            public IEnumerable<ThingWithComps> carriedWeapons;
            public List<ThingDefStuffDefPair> rememberedWeapons;
        }

        public Dictionary<Pawn, PawnData> pawnDic = new Dictionary<Pawn, PawnData>();

        public Command_Sidearms_SharedData(Func<IEnumerable<Pawn>> getPawns, int redundantNums) {
            _redundantGetPawns = getPawns;
            _redundantNums = redundantNums;
        }


        Func<IEnumerable<Pawn>> _redundantGetPawns;
        int _redundantNums;
        public void RedundantCleanForce(IEnumerable<Pawn> pawns)
        {
            foreach (var k in pawnDic.Keys.Where(f => pawns.Contains(f)).ToList())
            {
                pawnDic.Remove(k);
            }
        }
        internal void Add(Pawn instance, IEnumerable<ThingWithComps> carriedWeapons, List<ThingDefStuffDefPair> rememberedWeapons)
        {
            var data = new PawnData()
            {
                carriedWeapons = carriedWeapons,
                rememberedWeapons = rememberedWeapons
            };
            if (pawnDic.ContainsKey(instance))
                pawnDic[instance] = data;
            else
                pawnDic.Add(instance, data);

            if(_redundantNums > 0 && pawnDic.Count > _redundantNums)
            {
                RedundantCleanForce(_redundantGetPawns());
            }
        }

        public IEnumerable<WeaponStuffDefPair> GatherCarriedWeapons(IEnumerable<Pawn> pawns)
        {
            SortedSet<WeaponStuffDefPair> weapons = new SortedSet<WeaponStuffDefPair>(WeaponStuffDefPairComparer.Instance);

            foreach (var p in pawns)
            {
                var data = pawnDic.TryGetValue(p);
                if (data == null) continue;

                foreach (var w in data.carriedWeapons)
                {
                    var weaponStuff = new WeaponStuffDefPair(w);  
                    if (weapons.Contains(weaponStuff))
                        continue;
                    weapons.Add(weaponStuff);
                }
            }

            return weapons;
        }

        internal IEnumerable<Tuple<Pawn, ThingWithComps>> GetPawnsOfWeaponCarried(WeaponStuffDefPair weapon, IEnumerable<Pawn> pawns)
        {
            foreach (var p in pawns)
            {
                var data = pawnDic.TryGetValue(p);
                if (data == null) continue;
                var weaponInstance = data.carriedWeapons.FirstOrDefault(t => t.def == weapon.WeaponDef && t.Stuff == weapon.StuffDef);
                if (weaponInstance != null)
                    yield return new Tuple<Pawn, ThingWithComps>(p, weaponInstance);
            }
        }
    }


    public class Command_Sidearms : Gizmo_SidearmsList
    {
        static List<Pawn> GetUserSelectedPawns()
        {
            return Find.Selector.SelectedPawns;
        }
        public static Command_Sidearms_SharedData Shared { get; private set; } = new Command_Sidearms_SharedData(GetUserSelectedPawns, 300);



        SidearmsButtonStyle _style;
        static SidearmsWeaponCommandDrawer sharedWeaponDrawer = new SidearmsWeaponCommandDrawer();
        SidearmsWeaponCommandDrawer _drawer;

        private static bool isSidearmsGatherStyle()
        {
            if (GetUserSelectedPawns().Count > 1)
            {
                int sidearmsPawnCount = 0;
                foreach (var p in GetUserSelectedPawns())
                {
                    if (p.IsColonistPlayerControlled && p.IsValidSidearmsCarrier() && p.inventory != null && p.equipment != null)
                        sidearmsPawnCount++;
                    if (sidearmsPawnCount > 1)
                        return true;
                }
            }
            return false;
        }

        public Command_Sidearms(Pawn pawn, List<ThingWithComps> carriedWeapons, List<ThingDefStuffDefPair> RememberedWeapons, CompSidearmMemory pawnMemory) : base(pawn, carriedWeapons, RememberedWeapons, pawnMemory)
        {
        }

        List<WeaponStuffDefPair> _gatherWeaponsRange;
        List<WeaponStuffDefPair> _gatherWeaponsMelee;

        void collectGatherWeapons()
        {
            _gatherWeaponsRange = new List<WeaponStuffDefPair>();
            _gatherWeaponsMelee = new List<WeaponStuffDefPair>();
            foreach (var w in Shared.GatherCarriedWeapons(GetUserSelectedPawns()))
            {
                if (w.WeaponDef.IsRangedWeapon)
                    _gatherWeaponsRange.Add(w);
                else if (w.WeaponDef.IsMeleeWeapon)
                    _gatherWeaponsMelee.Add(w);
                else
                    throw new NotImplementedException();
            }
        }

        private bool initOnce(float maxWidth)
        {
            if (_drawer != null) return true;

            _style = isSidearmsGatherStyle() ? SidearmsButtonStyle.Gather : SidearmsButtonStyle.Inspect;
            if (_style == SidearmsButtonStyle.Gather)
            {
                _drawer = sharedWeaponDrawer;
                collectGatherWeapons();
                _drawer.Reset(Math.Max( _gatherWeaponsMelee.Count + 1, _gatherWeaponsRange.Count), maxWidth);
                return true;
            }
            return false;
        }

        public override float GetWidth(float maxWidth)
        {
            if(!initOnce(maxWidth) )
                return base.GetWidth(maxWidth);
            return _drawer.PanelWidth;
        }



        WeaponStuffDefPairBase _lastInteractdWeapon;

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            initOnce(maxWidth);

            if (_style == SidearmsButtonStyle.Gather)
            {
                if (_gatherWeaponsMelee.Count == 0 && _gatherWeaponsRange.Count == 0) return new GizmoResult(GizmoState.Clear);

                var d = _drawer as SidearmsWeaponCommandDrawer;
                {
                    d.BeginDrawPanel(topLeft, maxWidth);
                    d.DrawBackground();
                    d.BeginDrawContent();
                    d.DrawWeaponIcons(_gatherWeaponsRange, rowIndex: 0);
                    d.DrawWeaponIcons(new List<WeaponStuffDefPairBase>() { new WeaponStuffDefPairBase() { Unarmed = true } }, rowIndex: 1, columnIndex: 0);
                    d.DrawWeaponIcons(_gatherWeaponsMelee, rowIndex: 1, columnIndex: 1);
                    d.EndDrawContent();
                    d.EndDrawPanel();
                }

                _lastInteractdWeapon = _drawer.GetLastInteractedWeapon();
                if (_lastInteractdWeapon != null)
                {
                    var weapon = _lastInteractdWeapon as WeaponStuffDefPair;
                    if (weapon != null) {
                        foreach (var (pawnInstance, weaponInstance) in Shared.GetPawnsOfWeaponCarried(weapon, GetUserSelectedPawns()))
                        {
                            ThingDefStuffDefPair weaponType = weaponInstance.toThingDefStuffDefPair();
                            CompSidearmMemory.GetMemoryCompForPawn(pawnInstance).SetWeaponAsForced(weaponType, pawnInstance.Drafted);
                            var dropMode = pawnInstance.Drafted ? DroppingModeEnum.Combat : DroppingModeEnum.Calm;
                            if (pawnInstance.equipment.Primary != weaponInstance)
                                WeaponAssingment.equipSpecificWeaponTypeFromInventory(pawnInstance, weaponType, MiscUtils.shouldDrop(pawnInstance, dropMode, false), false);
                        }
                    }
                    else if (_lastInteractdWeapon.Unarmed)
                    {

                        foreach (var pawnInstance in  GetUserSelectedPawns())
                        {
                            CompSidearmMemory.GetMemoryCompForPawn(pawnInstance)?.SetUnarmedAsForced(true);
                            WeaponAssingment.equipSpecificWeapon(pawnInstance, null, false, false);
                        }
                    }
                    _lastInteractdWeapon = null;
                }

                return new GizmoResult(GizmoState.Clear);
            }
            else
            {
                return base.GizmoOnGUI(topLeft, maxWidth, parms);
            }
        }

    }
}
