using HarmonyLib;
using PeterHan.PLib.Options;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;


namespace FastAirFilter
{
    public class AirFilterConfigFix
    {
        [HarmonyPatch(typeof(AirFilterConfig), "ConfigureBuildingTemplate")]
        public class AirFilterConfig_ConfigureBuildingTemplate_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Debug.Log("FastAirFilter Transpiler called!");
                // SingletonOptions<ArgumentSet>.Instance.AirFilterSpeed
                Debug.Log("FastAirFilter SingletonOptions<ArgumentSet>.Instance.AirFilterSpeed = " + SingletonOptions<ArgumentSet>.Instance.AirFilterSpeed);
                float airFilterSpeed = SingletonOptions<ArgumentSet>.Instance.AirFilterSpeed;

                // 获取对应字段的 FieldInfo
                FieldInfo elementConsumerFieldInfo = typeof(ElementConsumer).GetField("capacityKG");

                List<CodeInstruction> list = instructions.ToList<CodeInstruction>();

                float oxygenConsumeKg = 0.1f * airFilterSpeed;
                for (int i = 0; i <= list.Count - 1; ++i)
                {
                    CodeInstruction instruction = list[i];
                    if (i == (list.Count - 1))
                    {
                        yield return instruction;
                    }
                    else
                    {
                        CodeInstruction nextInstruction = list[i + 1];
                        // 检查当前指令是否是加载特定浮点数的操作
                        if (instruction.opcode == OpCodes.Ldc_R4)
                        {
                            if ((float)instruction.operand == 0.143333346f)
                            {
                                // 修改为新的浮点数值
                                float oldValue = (float)instruction.operand;
                                instruction.operand = oldValue * airFilterSpeed; // 将值修改为原值乘以 airFilterSpeed
                                Debug.Log("FastAirFilter Modify Clay output instruction.operand = " + instruction.operand);
                            }
                            else if ((float)instruction.operand == 0.0899999961f)
                            {
                                // 修改为新的浮点数值
                                float oldValue = (float)instruction.operand;
                                instruction.operand = oldValue * airFilterSpeed; // 将值修改为原值乘以 airFilterSpeed
                                Debug.Log("FastAirFilter Modify Oxygen output instruction.operand = " + instruction.operand);
                            }
                            else if ((float)instruction.operand == 0.13333334f)
                            {
                                float oldValue = (float)instruction.operand;
                                instruction.operand = oldValue * airFilterSpeed; // 将值修改为原值乘以 airFilterSpeed
                                Debug.Log("FastAirFilter Modify Filter instruction.operand = " + instruction.operand);
                            }
                            else if ((float)instruction.operand == 0.1f)
                            {
                                // 修改为新的浮点数值
                                float oldValue = (float)instruction.operand;
                                instruction.operand = oldValue * airFilterSpeed; // 将值修改为原值乘以 airFilterSpeed
                                //oxygenConsumeKg = oldValue * airFilterSpeed;
                                Debug.Log("FastAirFilter Modify ContaminatedOxygen consume instruction.operand = " + instruction.operand);
                            }

                            if (instruction.operand is float value && nextInstruction.opcode == OpCodes.Stfld)
                            {
                                // 如果指令的目标字段是 capacityKG
                                if ((FieldInfo)nextInstruction.operand == elementConsumerFieldInfo)
                                {
                                    // 在这里修改值，例如将 0.5f 修改为 1.0f
                                    if (value == 0.5f)
                                    {
                                        instruction.operand = oxygenConsumeKg * 2;  // 修改值为 1.0f
                                        Debug.Log($"FastAirFilter Modified ElementConsumer::capacityKG value to {instruction.operand}");
                                    }
                                }
                            }
                        }
                        if (instruction.opcode == OpCodes.Ldc_I4_3)
                        {
                            int radius = SingletonOptions<ArgumentSet>.Instance.AirFilterRadius;
                            if (radius == 3)
                            {

                            }
                            else if (radius == 4)
                            {
                                instruction.opcode = OpCodes.Ldc_I4_4;
                            }
                            else if (radius == 5)
                            {
                                instruction.opcode = OpCodes.Ldc_I4_5;
                            }
                            else if (radius == 6)
                            {
                                instruction.opcode = OpCodes.Ldc_I4_6;
                            }
                            Debug.Log("FastAirFilter Modify AirFilterRadius instruction.opcode = " + instruction.opcode);

                        }
                        //Debug.Log("Orgin: instruction.opcode = " + instruction.opcode + ", instruction.operand = " + instruction.operand);

                    }
                    yield return instruction;
                }
            }
        }
    }

    [HarmonyPatch(typeof(AirFilterConfig), "CreateBuildingDef")]
    public class AirFilterConfig_CreateBuildingDef_Patch
    {
        public static void Postfix(BuildingDef __result)
        {
            bool AirFilterUsePower = SingletonOptions<ArgumentSet>.Instance.AirFilterUsePower;
            if (!AirFilterUsePower)
            {
                __result.RequiresPowerInput = false;
                Debug.Log("FastAirFilter AirFilterConfig_CreateBuildingDef_Patch: Set AirFilter RequiresPowerInput to false");
            }
        }
    }

    [HarmonyPatch(typeof(AirFilter))]
    public static class AirFilterPatches
    {
        // 为 HasFilter 方法创建后缀补丁
        //[HarmonyPatch(nameof(AirFilter.HasFilter))]
        //[HarmonyPostfix]
        //public static void HasFilterPostfix(bool __result)
        //{
        //    Debug.Log($"HasFilter 返回值: {__result}");
        //}

        //// 为 IsConvertable 方法创建后缀补丁
        //[HarmonyPatch(nameof(AirFilter.IsConvertable))]
        //[HarmonyPostfix]
        //public static void IsConvertablePostfix(AirFilter __instance, GameObject __state, bool __result)
        //{
        //    Debug.Log($"IsConvertable 返回值: {__result}");
        //    //AirFilterPatches.TryCallHasEnoughMassToStartConverting(__instance);
        //}
        /*
        public bool HasEnoughMassToStartConverting(bool includeInactive = false)
        {
          float num1 = 1f * this.GetSpeedMultiplier();
          bool flag1 = includeInactive || this.consumedElements.Length == 0;
          bool flag2 = true;
          List<GameObject> items = this.storage.items;
          for (int index1 = 0; index1 < this.consumedElements.Length; ++index1)
          {
            ElementConverter.ConsumedElement consumedElement = this.consumedElements[index1];
            flag1 |= consumedElement.IsActive;
            if (includeInactive || consumedElement.IsActive)
            {
              float num2 = 0.0f;
              for (int index2 = 0; index2 < items.Count; ++index2)
              {
                GameObject go = items[index2];
                if (!((UnityEngine.Object) go == (UnityEngine.Object) null) && go.HasTag(consumedElement.Tag))
                  num2 += go.GetComponent<PrimaryElement>().Mass;
              }
              if ((double) num2 < (double) consumedElement.MassConsumptionRate * (double) num1)
              {
                flag2 = false;
                break;
              }
            }
          }
          return flag1 & flag2;
        }
      */
        public static bool TryCallHasEnoughMassToStartConverting(AirFilter __instance)
        {
            Debug.Log("TryCallHasEnoughMassToStartConverting");
            // elementConverter
            ElementConverter elementConverter = __instance.GetComponent<ElementConverter>();
            // 获取私有方法的 MethodInfo
            MethodInfo privateMethod = AccessTools.Method(typeof(ElementConverter), "GetSpeedMultiplier");
            // 调用私有方法
            float num1 = (float)privateMethod.Invoke(elementConverter, null);
            Debug.Log("num1 = " + num1);
            bool flag1 = elementConverter.consumedElements.Length == 0;
            Debug.Log($"flag1 = {flag1}, elementConverter.consumedElements.Length = {elementConverter.consumedElements.Length}");
            float a1 = 1f;
            Debug.Log($"flag1 = {flag1}, a1 = {a1}");
            bool flag2 = true;
            // 获取私友的成员 storage
            FieldInfo storageField = AccessTools.Field(typeof(ElementConverter), "storage");
            Storage storageValue = (Storage)storageField.GetValue(elementConverter);
            List<GameObject> items = storageValue.items;
            Debug.Log($"items.Count = {items.Count}");
            for (int index1 = 0; index1 < elementConverter.consumedElements.Length; ++index1)
            {
                ElementConverter.ConsumedElement consumedElement = elementConverter.consumedElements[index1];
                flag1 |= consumedElement.IsActive;
                Debug.Log($"flag1 = {flag1}, consumedElement.IsActive = {consumedElement.IsActive}");
                if (consumedElement.IsActive)
                {
                    float num2 = 0.0f;
                    for (int index2 = 0; index2 < items.Count; ++index2)
                    {
                        GameObject go = items[index2];
                        if (!((UnityEngine.Object)go == (UnityEngine.Object)null) && go.HasTag(consumedElement.Tag))
                        {
                            num2 += go.GetComponent<PrimaryElement>().Mass;
                            Debug.Log($"num2 = {num2}, go.GetComponent<PrimaryElement>().Mass = {go.GetComponent<PrimaryElement>().Mass}");
                        }
                    }
                    Debug.Log($"num2 = {num2}, consumedElement.MassConsumptionRate = {consumedElement.MassConsumptionRate}, num1 = {num1}");
                    if ((double)num2 < (double)consumedElement.MassConsumptionRate * (double)num1)
                    {
                        flag2 = false;
                        break;
                    }
                }
            }
            Debug.Log($"flag = {flag1}, flag2 = {flag2}");
            return true;
        }
    }

    //public class Patches
    //{
    //    [HarmonyPatch(typeof(Db))]
    //    [HarmonyPatch("Initialize")]
    //    public class Db_Initialize_Patch
    //    {
    //        public static void Prefix()
    //        {
    //            Debug.Log("FastAirFilter： I execute before Db.Initialize!");
    //        }

    //        public static void Postfix()
    //        {
    //            Debug.Log("FastAirFilter：I execute after Db.Initialize!");
    //        }
    //    }
    //}
}
