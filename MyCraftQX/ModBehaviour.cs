using ItemStatsSystem;
using MyCraftQX.UsageBehaviors;
using MyCraftQX.Utils;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MyCraftQX
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        public const string CraftingTableItemNameKey = LocalizationHelper.KeyPrefix + "crafting_table";
        protected override void OnAfterSetup()
        {
            base.OnAfterSetup();

            // 加载物品“工作台”的图标
            Texture2D texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            // 加载图片数据
            byte[] imageData = File.ReadAllBytes(GetItemIconFilePath(CraftingTableItemNameKey));
            // 将图片数据加载到Texture2D对象中
            if (!texture2D.LoadImage(imageData))
            {
                Debug.LogError($"加载物品图标到Texture2D失败：{CraftingTableItemNameKey}");
            }
            texture2D.filterMode = FilterMode.Bilinear;
            texture2D.Apply();
            Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f), 100f);
            if (sprite == null)
            {
                Debug.LogError($"为物品图标创建Sprite失败：{CraftingTableItemNameKey}");
            }

            // 配方Tag “WorkBenchAdvanced”
            // 创建物品“工作台”
            ItemBuilder.Create()
                .WithTypeID(ItemAssetsCollection.Instance.NextTypeID)
                .WithDisplayNameRaw(CraftingTableItemNameKey)
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
                .WithIcon(sprite);

        }

        protected override void OnBeforeDeactivate()
        {
            base.OnBeforeDeactivate();

        }

        private static string GetItemIconFilePath(string itemNameKey)
        {
            // 获取当前执行程序集所在文件夹路径
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (directoryName == null) return null;
            return Path.Combine(directoryName, "ItemIcon", $"{itemNameKey}.png");
        }
    }
}
