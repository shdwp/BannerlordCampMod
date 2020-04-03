using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using HarmonyLib;
using StoryMode.GameModels;
using TaleWorlds.CampaignSystem;
using UIExtenderLib;

namespace CampMod
{
    public class PlayerCampMod: MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            
            UIExtender.Register();
            
            var harmony = new Harmony("net.shdwp.CampMod");
            harmony.PatchAll();
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            Campaign.Current.CampaignBehaviorManager.AddBehavior(new PlayerCampBehavior());
        }
    }
}