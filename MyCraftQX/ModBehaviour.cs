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


            CreateCraftingTableItem();
            // 配方Tag “WorkBenchAdvanced”

            // 添加所有已创建的物品到物品系统中
            foreach (var item in _items)
            {
                ItemAssetsCollection.AddDynamicEntry(item);
            }

        }

        protected override void OnBeforeDeactivate()
        {
            base.OnBeforeDeactivate();

            // 销毁并移除所有创建的物品
            foreach (var item in _items)
            {
                ItemAssetsCollection.RemoveDynamicEntry(item);
                GameObject.Destroy(item.gameObject);
            }
            // 移除所有增加的合成配方
        }

        private void CreateCraftingTableItem()
        {
            // 创建物品“工作台”
            Item craftingTable = ItemBuilder.Create()
                 .WithTypeID(ItemAssetsCollection.Instance.NextTypeID)
                 .WithItemNameKey(CraftingTableItemName)
                 .LoadIconFromFilePath(GetItemIconFilePath(CraftingTableItemName))
                 .WithWeight(1)
                 .WithValue(58)
                 .WithQuality(6, DisplayQuality.Red)
                 .WithOrder(0)
                 .AddTag("Tool")
                 .AsNonStackable()
                 .WithUsageUtilities(static (usageUtilities) =>
                 {
                     ReflectionHelper.SetFieldValue(usageUtilities, "useTime", 0.5f);
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
