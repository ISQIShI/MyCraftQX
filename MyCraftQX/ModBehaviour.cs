using Duckov.Economy;
using ItemStatsSystem;
using MyCraftQX.UsageBehaviors;
using MyCraftQX.Utils;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MyCraftQX
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        public const string CraftingTableItemName = "crafting_table";

        private LinkedList<Item> _items = new LinkedList<Item>();

        protected override void OnAfterSetup()
        {
            base.OnAfterSetup();

            LocalizationHelper.Init();

            // 创建物品
            CreateCraftingTableItem();

            // 添加所有已创建的物品到物品系统中
            foreach (var item in _items)
            {
                ItemAssetsCollection.AddDynamicEntry(item);
            }
        }

        protected override void OnBeforeDeactivate()
        {
            base.OnBeforeDeactivate();

            LocalizationHelper.Release();

            // 销毁并移除所有创建的物品
            foreach (var item in _items)
            {
                ItemAssetsCollection.RemoveDynamicEntry(item);
                GameObject.Destroy(item.gameObject);
            }
            // 移除所有增加的合成配方
            FormulaHelper.RemoveAllAddedFormulas();
        }

        private void CreateCraftingTableItem()
        {
            var typeID = ItemAssetsCollection.Instance.NextTypeID;
            // 创建物品“工作台”
            Item craftingTable = ItemBuilder.Create()
                 .WithTypeID(typeID)
                 .WithItemNameKey(CraftingTableItemName)
                 .LoadIconFromFilePath(GetItemIconFilePath(CraftingTableItemName))
                 .WithWeight(0.5f)
                 .WithValue(400)
                 .WithQuality(9, DisplayQuality.None)
                 .WithOrder(0)
                 .AddTag("Tool")
                 .AsNonStackable()
                 .WithDurability(1)
                 .WithUsageUtilities(static (usageUtilities) =>
                 {
                     ReflectionHelper.SetFieldValue(usageUtilities, "useTime", 0.2f);
                     // 不消耗耐久度
                     usageUtilities.useDurability = false;
                     // 没有使用声音
                     usageUtilities.hasSound = false;
                     usageUtilities.behaviors.Clear();
                     var openCraftViewBehavior = usageUtilities.gameObject.AddComponent<OpenCraftViewUsageBehavior>();
                     usageUtilities.behaviors.Add(openCraftViewBehavior);
                 })
                 .Build();
            craftingTable.transform.SetParent(this.transform);
            _items.AddLast(craftingTable);
            LogHelper.Instance.LogTest($"物品 '{craftingTable.DisplayName}' 已创建并添加到列表中。");

            // 添加工作台合成配方
            // 配方Tag “WorkBenchAdvanced” 
            FormulaHelper.AddCraftingFormula(
                $"{CraftingTableItemName}_formula",
                new Cost
                {
                    money = 0,
                    items = new Cost.ItemEntry[]
                    {
                        // 木板(4)
                        new Cost.ItemEntry
                        {
                            id = 361,
                            amount = 4
                        }
                    }
                },
                new CraftingFormula.ItemEntry
                {
                    id = typeID,
                    amount = 1
                },
                new string[] { "WorkBenchAdvanced" },
                requirePerk: "",
                unlockByDefault: true,
                hideInIndex: false,
                lockInDemo: false
            );
        }


        private static string GetItemIconFilePath(string itemName)
        {
            // 获取当前执行程序集所在文件夹路径
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (directoryName == null) return null;
            return Path.Combine(directoryName, "ItemIcon", $"{itemName}.png");
        }
    }
}
