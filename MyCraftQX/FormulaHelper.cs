using Duckov.Economy;
using MyCraftQX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyCraftQX
{
    public class FormulaHelper
    {
        private static HashSet<string> addedFormulaIDs = new HashSet<string>();

        public static void AddCraftingFormula(string formulaID, Cost costInfo, CraftingFormula.ItemEntry resultItemInfo, string[] tags = null, string requirePerk = "", bool unlockByDefault = true, bool hideInIndex = false, bool lockInDemo = false)
        {
            try
            {
                CraftingFormulaCollection instance = CraftingFormulaCollection.Instance;
                // 获取配方列表
                List<CraftingFormula> list = ReflectionHelper.GetFieldValue<List<CraftingFormula>>(instance, "list");
                if (list.Any((craftingFormula) => craftingFormula.id == formulaID))
                {
                    Debug.LogWarning($"配方ID: {formulaID} 已存在，跳过添加");
                    return;
                }

                if (tags == null)
                {
                    tags = new string[] { "WorkBenchAdvanced" };
                }

                CraftingFormula craftingFormula = new CraftingFormula
                {
                    id = formulaID,
                    unlockByDefault = unlockByDefault,
                    cost = costInfo,
                    result = resultItemInfo,
                    requirePerk = requirePerk,
                    tags = tags,
                    hideInIndex = hideInIndex,
                    lockInDemo = lockInDemo
                };

                list.Add(craftingFormula);
                addedFormulaIDs.Add(formulaID);
                ReflectionHelper.SetFieldValue(instance, "_entries_ReadOnly", null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"添加合成配方失败: {ex.Message}");
            }
        }

        public static void RemoveAllAddedFormulas()
        {
            try
            {
                CraftingFormulaCollection instance = CraftingFormulaCollection.Instance;
                // 获取配方列表
                List<CraftingFormula> list = ReflectionHelper.GetFieldValue<List<CraftingFormula>>(instance, "list");
                list.RemoveAll(craftingFormula => addedFormulaIDs.Contains(craftingFormula.id));

                addedFormulaIDs.Clear();
                ReflectionHelper.SetFieldValue(instance, "_entries_ReadOnly", null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"移除配方时出错: {ex.Message}");
            }
        }
    }
}
