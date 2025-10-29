using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using MyCraftQX.Utils;
using UnityEngine;

namespace MyCraftQX
{
    /// <summary>
    /// Item 类的扩展方法
    /// </summary>
    public static class ItemExtensions
    {
        #region 字段设置扩展方法

        /// <summary>
        /// 设置私有字段值（泛型版本）
        /// </summary>
        public static bool SetField<T>(this Item item, string fieldName, T value)
        {
            return ReflectionHelper.SetFieldValue(item, fieldName, value);
        }

        /// <summary>
        /// 获取私有字段值（泛型版本）
        /// </summary>
        public static T GetField<T>(this Item item, string fieldName, T defaultValue = default(T))
        {
            T result = ReflectionHelper.GetFieldValue<T>(item, fieldName);
            return result ?? defaultValue;
        }

        /// <summary>
        /// 设置重量
        /// </summary>
        public static Item SetWeight(this Item item, float weight)
        {
            item.SetField("weight", weight);
            return item;
        }

        /// <summary>
        /// 设置最大堆叠数量
        /// </summary>
        public static Item SetMaxStackCount(this Item item, int maxStackCount)
        {
            item.SetField("maxStackCount", maxStackCount);
            return item;
        }

        /// <summary>
        /// 设置 Stats 组件
        /// </summary>
        public static Item SetStats(this Item item, StatCollection stats)
        {
            item.SetField("stats", stats);
            if (stats != null)
            {
                ReflectionHelper.SetFieldValue(stats, "master", item);
            }
            return item;
        }

        /// <summary>
        /// 设置 Slots 组件
        /// </summary>
        public static Item SetSlots(this Item item, SlotCollection slots)
        {
            item.SetField("slots", slots);
            if (slots != null)
            {
                ReflectionHelper.SetFieldValue(slots, "master", item);
            }
            return item;
        }

        /// <summary>
        /// 设置 Modifiers 组件
        /// </summary>
        public static Item SetModifiers(this Item item, ModifierDescriptionCollection modifiers)
        {
            item.SetField("modifiers", modifiers);
            if (modifiers != null)
            {
                ReflectionHelper.SetFieldValue(modifiers, "master", item);
            }
            return item;
        }

        /// <summary>
        /// 设置 Inventory 组件
        /// </summary>
        public static Item SetInventory(this Item item, Inventory inventory)
        {
            item.SetField("inventory", inventory);
            if (inventory != null)
            {
                ReflectionHelper.SetFieldValue(inventory, "attachedToItem", item);
            }
            return item;
        }

        /// <summary>
        /// 设置 UsageUtilities
        /// </summary>
        public static Item SetUsageUtilities(this Item item, UsageUtilities usageUtilities)
        {
            item.SetField("usageUtilities", usageUtilities);
            return item;
        }

        /// <summary>
        /// 设置 AgentUtilities
        /// </summary>
        public static Item SetAgentUtilities(this Item item, ItemAgentUtilities agentUtilities)
        {
            item.SetField("agentUtilities", agentUtilities);
            return item;
        }

        /// <summary>
        /// 设置 ItemGraphicInfo
        /// </summary>
        public static Item SetItemGraphic(this Item item, ItemGraphicInfo itemGraphic)
        {
            item.SetField("itemGraphic", itemGraphic);
            return item;
        }

        #endregion

        #region 便捷操作方法

        /// <summary>
        /// 添加多个标签
        /// </summary>
        public static Item AddTags(this Item item, params Tag[] tags)
        {
            foreach (var tag in tags)
            {
                if (tag != null && !item.Tags.Contains(tag))
                {
                    item.Tags.Add(tag);
                }
            }
            return item;
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        public static Item RemoveTag(this Item item, Tag tag)
        {
            if (item.Tags.Contains(tag))
            {
                item.Tags.Remove(tag);
            }
            return item;
        }

        /// <summary>
        /// 设置为已检查
        /// </summary>
        public static Item MarkAsInspected(this Item item)
        {
            item.Inspected = true;
            return item;
        }

        /// <summary>
        /// 设置为未检查
        /// </summary>
        public static Item MarkAsNotInspected(this Item item)
        {
            item.Inspected = false;
            return item;
        }

        /// <summary>
        /// 完全修复耐久度
        /// </summary>
        public static Item FullyRepair(this Item item)
        {
            if (item.UseDurability)
            {
                item.Durability = item.MaxDurability;
                item.DurabilityLoss = 0f;
            }
            return item;
        }

        /// <summary>
        /// 修复耐久度（百分比）
        /// </summary>
        public static Item Repair(this Item item, float percentage)
        {
            if (item.UseDurability)
            {
                float repairAmount = item.MaxDurability * Mathf.Clamp01(percentage);
                item.Durability = Mathf.Min(item.Durability + repairAmount, item.MaxDurability);
            }
            return item;
        }

        /// <summary>
        /// 损坏耐久度
        /// </summary>
        public static Item Damage(this Item item, float amount)
        {
            if (item.UseDurability)
            {
                item.Durability = Mathf.Max(0f, item.Durability - amount);
            }
            return item;
        }

        /// <summary>
        /// 复制变量到另一个物品
        /// </summary>
        public static Item CopyVariablesTo(this Item source, Item target)
        {
            if (source == null || target == null) return target;

            foreach (var variable in source.Variables)
            {
                target.Variables.SetRaw(variable.Key, variable.DataType, variable.GetRawCopied(), true);
            }

            return target;
        }

        /// <summary>
        /// 复制常量到另一个物品
        /// </summary>
        public static Item CopyConstantsTo(this Item source, Item target)
        {
            if (source == null || target == null) return target;

            foreach (var constant in source.Constants)
            {
                target.Constants.SetRaw(constant.Key, constant.DataType, constant.GetRawCopied(), true);
            }

            return target;
        }

        /// <summary>
        /// 清空所有插槽
        /// </summary>
        public static Item ClearAllSlots(this Item item)
        {
            if (item.Slots != null)
            {
                foreach (var slot in item.Slots)
                {
                    if (slot != null && slot.Content != null)
                    {
                        slot.Unplug();
                    }
                }
            }
            return item;
        }

        /// <summary>
        /// 检查是否有特定标签
        /// </summary>
        public static bool HasTag(this Item item, string tagName)
        {
            return item.Tags.Contains(tagName);
        }

        /// <summary>
        /// 检查是否有任意一个标签
        /// </summary>
        public static bool HasAnyTag(this Item item, params string[] tagNames)
        {
            foreach (var tagName in tagNames)
            {
                if (item.Tags.Contains(tagName))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查是否有所有标签
        /// </summary>
        public static bool HasAllTags(this Item item, params string[] tagNames)
        {
            foreach (var tagName in tagNames)
            {
                if (!item.Tags.Contains(tagName))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region 安全访问方法

        /// <summary>
        /// 安全获取 Stat 值
        /// </summary>
        public static float GetStatValueSafe(this Item item, string statKey, float defaultValue = 0f)
        {
            if (item == null || item.Stats == null)
            {
                return defaultValue;
            }

            var stat = item.GetStat(statKey);
            return stat != null ? stat.Value : defaultValue;
        }

        /// <summary>
        /// 安全获取 Stat 值（使用哈希）
        /// </summary>
        public static float GetStatValueSafe(this Item item, int statHash, float defaultValue = 0f)
        {
            if (item == null || item.Stats == null)
            {
                return defaultValue;
            }

            var stat = item.GetStat(statHash);
            return stat != null ? stat.Value : defaultValue;
        }

        /// <summary>
        /// 安全获取插槽内容
        /// </summary>
        public static Item GetSlotContentSafe(this Item item, string slotKey)
        {
            if (item == null || item.Slots == null)
            {
                return null;
            }

            var slot = item.Slots[slotKey];
            return slot?.Content;
        }

        #endregion

        #region 调试和诊断

        /// <summary>
        /// 打印物品详细信息
        /// </summary>
        public static void PrintDebugInfo(this Item item)
        {
            if (item == null)
            {
                Debug.Log("Item is null");
                return;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== Item Debug Info: {item.DisplayName} ===");
            sb.AppendLine($"TypeID: {item.TypeID}");
            sb.AppendLine($"Value: {item.Value}");
            sb.AppendLine($"Quality: {item.Quality} ({item.DisplayQuality})");
            sb.AppendLine($"Weight: {item.UnitSelfWeight} (Total: {item.TotalWeight})");
            sb.AppendLine($"Stackable: {item.Stackable} (Max: {item.MaxStackCount}, Current: {item.StackCount})");

            if (item.UseDurability)
            {
                sb.AppendLine($"Durability: {item.Durability}/{item.MaxDurability} (Loss: {item.DurabilityLoss:P})");
            }

            sb.AppendLine($"Tags: {string.Join(", ", item.Tags)}");
            sb.AppendLine($"Inspected: {item.Inspected}");
            sb.AppendLine($"CanBeSold: {item.CanBeSold}");
            sb.AppendLine($"CanDrop: {item.CanDrop}");

            if (item.Stats != null && item.Stats.Count > 0)
            {
                sb.AppendLine($"Stats Count: {item.Stats.Count}");
            }

            if (item.Slots != null && item.Slots.Count > 0)
            {
                sb.AppendLine($"Slots Count: {item.Slots.Count}");
            }

            if (item.Inventory != null)
            {
                sb.AppendLine($"Has Inventory: Yes (Capacity: {item.Inventory.Capacity})");
            }

            Debug.Log(sb.ToString());
        }

        #endregion
    }
}