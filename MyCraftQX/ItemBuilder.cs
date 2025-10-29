using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using MyCraftQX.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace MyCraftQX
{
    /// <summary>
    /// Item 建造者类，用于流式构建 Item 对象
    /// </summary>
    public class ItemBuilder
    {
        private Item _item;
        private GameObject _gameObject;

        private ItemBuilder()
        {
            _gameObject = new GameObject("NewItem");
            _item = _gameObject.AddComponent<Item>();
        }

        /// <summary>
        /// 开始构建一个新的 Item
        /// </summary>
        public static ItemBuilder Create()
        {
            return new ItemBuilder();
        }

        /// <summary>
        /// 从现有 Item 创建副本
        /// </summary>
        public static ItemBuilder FromTemplate(Item template)
        {
            var builder = new ItemBuilder();
            var newItem = UnityEngine.Object.Instantiate(template);
            UnityEngine.Object.DestroyImmediate(builder._gameObject);
            builder._gameObject = newItem.gameObject;
            builder._item = newItem;
            return builder;
        }

        #region 基础属性设置

        /// <summary>
        /// 设置类型 ID
        /// </summary>
        public ItemBuilder WithTypeID(int typeId)
        {
            ReflectionHelper.SetFieldValue(_item, "typeID", typeId);
            return this;
        }

        /// <summary>
        /// 设置排序顺序
        /// </summary>
        public ItemBuilder WithOrder(int order)
        {
            _item.Order = order;
            return this;
        }

        /// <summary>
        /// 设置显示名称的键
        /// </summary>
        public ItemBuilder WithDisplayNameRaw(string displayNameRaw)
        {
            _item.DisplayNameRaw = displayNameRaw;
            return this;
        }

        /// <summary>
        /// 设置图标
        /// </summary>
        public ItemBuilder WithIcon(Sprite icon)
        {
            _item.Icon = icon;
            return this;
        }

        /// <summary>
        /// 设置价值
        /// </summary>
        public ItemBuilder WithValue(int value)
        {
            _item.Value = value;
            return this;
        }

        /// <summary>
        /// 设置品质
        /// </summary>
        public ItemBuilder WithQuality(int quality, DisplayQuality displayQuality)
        {
            _item.Quality = quality;
            _item.DisplayQuality = displayQuality;
            return this;
        }

        /// <summary>
        /// 设置重量
        /// </summary>
        public ItemBuilder WithWeight(float weight)
        {
            // 需要通过反射设置私有字段
            var field = typeof(Item).GetField("weight",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            field?.SetValue(_item, weight);
            return this;
        }

        /// <summary>
        /// 设置声音键
        /// </summary>
        public ItemBuilder WithSoundKey(string soundKey)
        {
            _item.soundKey = soundKey;
            return this;
        }

        #endregion

        #region 堆叠设置

        /// <summary>
        /// 设置为可堆叠物品
        /// </summary>
        public ItemBuilder AsStackable(int maxStackCount, int initialCount = 1)
        {
            if (maxStackCount <= 1)
            {
                LogHelper.Instance.LogWarning($"尝试将物品 '{_item.DisplayName}' 设置为可堆叠物品，但提供的最大堆叠数量 {maxStackCount} 小于等于 1，操作已忽略。");
                return this;
            }
            if (initialCount < 1 || initialCount > maxStackCount)
            {
                LogHelper.Instance.LogWarning($"尝试将物品 '{_item.DisplayName}' 设置为可堆叠物品，但提供的初始数量 {initialCount} 不在有效范围内（1 到 {maxStackCount}），操作已忽略。");
                return this;
            }
            _item.MaxStackCount = maxStackCount;
            _item.SetInt("Count", initialCount, true);
            return this;
        }

        /// <summary>
        /// 设置为不可堆叠物品
        /// </summary>
        public ItemBuilder AsNonStackable()
        {
            _item.MaxStackCount = 1;
            return this;
        }

        #endregion

        #region 耐久度设置

        /// <summary>
        /// 设置耐久度
        /// </summary>
        public ItemBuilder WithDurability(float maxDurability, float currentDurability = -1)
        {
            _item.MaxDurability = maxDurability;
            _item.Durability = currentDurability < 0 ? maxDurability : currentDurability;
            return this;
        }

        /// <summary>
        /// 设置耐久度损失
        /// </summary>
        public ItemBuilder WithDurabilityLoss(float loss)
        {
            _item.DurabilityLoss = loss;
            return this;
        }

        #endregion

        #region 标签设置

        public ItemBuilder ClearTags()
        {
            _item.Tags.Clear();
            return this;
        }

        public ItemBuilder AddTag(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                LogHelper.Instance.LogWarning("尝试添加空标签名称，操作已忽略。");
                return this;
            }

            Tag[] array = Resources.FindObjectsOfTypeAll<Tag>();
            var tag = array.FirstOrDefault((Tag t) => t.name == tagName);
            if (tag != null)
            {
                AddTag(tag);
            }
            else
            {
                LogHelper.Instance.LogWarning($"标签 '{tagName}' 未找到，无法添加到物品 '{_item.DisplayName}'。");
            }
            return this;
        }

        public ItemBuilder AddTags(params string[] tagNames)
        {
            Tag[] array = Resources.FindObjectsOfTypeAll<Tag>();
            foreach (var tagName in tagNames)
            {
                if (string.IsNullOrEmpty(tagName))
                {
                    LogHelper.Instance.LogWarning("尝试添加空标签名称，操作已忽略。");
                    continue;
                }

                var tag = array.FirstOrDefault((Tag t) => t.name == tagName);
                if (tag != null)
                {
                    AddTag(tag);
                }
                else
                {
                    LogHelper.Instance.LogWarning($"标签 '{tagName}' 未找到，无法添加。");
                }
            }
            return this;
        }


        /// <summary>
        /// 添加标签
        /// </summary>
        public ItemBuilder AddTag(Tag tag)
        {
            if (!_item.Tags.Contains(tag))
            {
                _item.Tags.Add(tag);
            }
            return this;
        }

        /// <summary>
        /// 添加多个标签
        /// </summary>
        public ItemBuilder AddTags(params Tag[] tags)
        {
            foreach (var tag in tags)
            {
                AddTag(tag);
            }
            return this;
        }

        /// <summary>
        /// 设置为粘性物品（不可丢弃）
        /// </summary>
        public ItemBuilder AsSticky()
        {
            // 需要添加 "Sticky" 标签
            return this;
        }

        /// <summary>
        /// 设置为可修复
        /// </summary>
        public ItemBuilder AsRepairable()
        {
            // 需要添加 "Repairable" 标签
            return this;
        }

        #endregion

        #region 统计属性设置

        /// <summary>
        /// 添加统计属性组件
        /// </summary>
        public ItemBuilder WithStats()
        {
            if (_item.Stats == null)
            {
                var stats = _gameObject.AddComponent<StatCollection>();
                ReflectionHelper.SetFieldValue(stats, "master", _item);
                typeof(Item).GetField("stats",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance)
                    ?.SetValue(_item, stats);
            }
            return this;
        }

        /// <summary>
        /// 添加统计属性
        /// </summary>
        public ItemBuilder AddStat(Stat stat)
        {
            WithStats();
            _item.Stats.Add(stat);
            return this;
        }

        #endregion

        #region 插槽设置

        /// <summary>
        /// 添加插槽系统
        /// </summary>
        public ItemBuilder WithSlots()
        {
            if (_item.Slots == null)
            {
                _item.CreateSlotsComponent();
            }
            return this;
        }

        /// <summary>
        /// 添加插槽
        /// </summary>
        public ItemBuilder AddSlot(Slot slot)
        {
            WithSlots();
            _item.Slots.Add(slot);
            return this;
        }

        #endregion

        #region 修改器设置

        /// <summary>
        /// 添加修改器系统
        /// </summary>
        public ItemBuilder WithModifiers()
        {
            if (_item.Modifiers == null)
            {
                _item.CreateModifiersComponent();
            }
            return this;
        }

        #endregion

        #region 背包设置

        /// <summary>
        /// 添加背包系统
        /// </summary>
        public ItemBuilder WithInventory(int capacity = 10)
        {
            if (_item.Inventory == null)
            {
                var inventory = _gameObject.AddComponent<Inventory>();
                ReflectionHelper.SetFieldValue(inventory, "attachedToItem", _item);
                typeof(Item).GetField("inventory",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance)
                    ?.SetValue(_item, inventory);
            }
            return this;
        }

        #endregion

        #region 自定义数据设置

        /// <summary>
        /// 添加变量
        /// </summary>
        public ItemBuilder WithVariable(string key, int value)
        {
            _item.SetInt(key, value);
            return this;
        }

        public ItemBuilder WithVariable(string key, float value)
        {
            _item.SetFloat(key, value);
            return this;
        }

        public ItemBuilder WithVariable(string key, bool value)
        {
            _item.SetBool(key, value);
            return this;
        }

        public ItemBuilder WithVariable(string key, string value)
        {
            _item.SetString(key, value);
            return this;
        }

        /// <summary>
        /// 添加常量
        /// </summary>
        public ItemBuilder WithConstant(string key, float value)
        {
            _item.Constants.SetFloat(key, value, true);
            return this;
        }

        #endregion

        #region 效果设置

        /// <summary>
        /// 添加效果
        /// </summary>
        public ItemBuilder AddEffect(Effect effect)
        {
            _item.AddEffect(effect);
            return this;
        }

        #endregion

        #region 使用工具设置

        /// <summary>
        /// 设置使用工具
        /// </summary>
        public ItemBuilder WithUsageUtilities(Action<UsageUtilities> usageUtilitiesConfiguration)
        {
            var usageUtilities = ReflectionHelper.GetFieldValue<UsageUtilities>(_item, "usageUtilities");
            if (usageUtilities != null)
            {
                usageUtilitiesConfiguration?.Invoke(usageUtilities);
            }
            else
            {
                usageUtilities = _item.gameObject.AddComponent<UsageUtilities>();
                ReflectionHelper.SetFieldValue(_item, "usageUtilities", usageUtilities);
                usageUtilitiesConfiguration?.Invoke(usageUtilities);
            }

            return this;
        }

        #endregion

        #region 初始化和构建

        /// <summary>
        /// 设置物品为已检查状态
        /// </summary>
        public ItemBuilder AsInspected(bool inspected = true)
        {
            _item.Inspected = inspected;
            return this;
        }

        /// <summary>
        /// 构建并返回 Item
        /// </summary>
        public Item Build()
        {
            _item.Initialize();
            return _item;
        }

        /// <summary>
        /// 构建并设置名称
        /// </summary>
        public Item Build(string gameObjectName)
        {
            _gameObject.name = gameObjectName;
            return Build();
        }

        #endregion
    }
}