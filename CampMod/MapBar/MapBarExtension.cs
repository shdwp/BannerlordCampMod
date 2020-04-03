using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using UIExtenderLib;
using UIExtenderLib.Prefab;
using UIExtenderLib.ViewModel;

namespace CampMod.MapBar
{
    [PrefabExtension("MapBar", "descendant::ListPanel[@Sprite='mapbar_left_canvas']/Children")]
    public class MapBarExtension : PrefabExtensionInsertPatch
    {
        public override int Position => 3;
        public override string Name => "CampButton";
    }
    
    [ViewModelMixin]
    public class MapNavigationVMExtension : BaseViewModelMixin<MapNavigationVM>
    {
        private HintViewModel _campHint = new HintViewModel("Establish camp");
        
        [DataSourceProperty] public bool IsCampEnabled => MobileParty.MainParty.Party.Settlement == null;
        [DataSourceProperty] public HintViewModel CampHintText => _campHint;

        public MapNavigationVMExtension(MapNavigationVM vm) : base(vm)
        {
        }

        [DataSourceMethod]
        public void ExecuteOpenCamp()
        {
            Campaign.Current.GetCampaignBehavior<PlayerCampBehavior>().EstablishCamp(MobileParty.MainParty);
        }
    }
}
