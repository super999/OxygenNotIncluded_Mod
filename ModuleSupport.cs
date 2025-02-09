using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;

namespace FastAirFilter
{
    public class ModuleSupport : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            PUtil.InitLibrary();
            base.OnLoad(harmony);
            Debug.Log("FastAirCondition loaded!");
            new POptions().RegisterOptions((UserMod2)this, typeof(ArgumentSet));
        }
    }
}
