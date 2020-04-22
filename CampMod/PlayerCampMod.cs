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
        private UIExtender _extender;
        
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            
            _extender = new UIExtender("CampMod");
            _extender.Register();
            
            var harmony = new Harmony("net.shdwp.CampMod");
            harmony.PatchAll();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            _extender.Verify();
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            
            Campaign.Current.CampaignBehaviorManager.AddBehavior(new PlayerCampBehavior());
        }
    }
}