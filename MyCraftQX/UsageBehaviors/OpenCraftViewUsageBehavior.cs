using ItemStatsSystem;
using System;
using System.Linq;

namespace MyCraftQX.UsageBehaviors
{
    public class OpenCraftViewUsageBehavior : UsageBehavior
    {
        public override bool CanBeUsed(Item item, object user)
        {
            if (user is CharacterMainControl character && character != null && character.IsMainCharacter)
            {
                return true;
            }
            return false;
        }

        protected override void OnUse(Item item, object user)
        {
            CraftView.SetupAndOpenView((static (craftingFormula) =>
            {
                return craftingFormula.tags.Contains("WorkBenchAdvanced");
            }));
        }
    }
}
