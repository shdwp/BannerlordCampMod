using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CampMod
{
    public class PlayerCampBehavior: CampaignBehaviorBase
    {
        protected class PlayerEstablishedCamp
        {
            public MobileParty Party;
            public MobileParty ParentParty;
            public bool IsTraining;
        }
        
        public static string PlayerCampMenu = "player_camp_menu";

        private PlayerEstablishedCamp _establishedCamp = null;
        
        public override void RegisterEvents()
        {
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnSessionLaunched));
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(OnHourlyTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
            
        }

        public void EstablishCamp(MobileParty parentParty)
        {
            var party = MobileParty.Create("main_player_camp:0");
            party.StringId = "main_player_camp:0";
            party.InitializeMobileParty(new TextObject("Player Camp"), new TroopRoster(), new TroopRoster(), MobileParty.MainParty.Position2D, 1f, 0.5f);
            
            var roster = MobileParty.MainParty.MemberRoster;
            roster.RemoveTroop(Hero.MainHero.CharacterObject);
            party.Party.AddMembers(roster.ToFlattenedRoster());
            roster.Clear();

            party.Party.ItemRoster.Add(MobileParty.MainParty.ItemRoster);
            MobileParty.MainParty.ItemRoster.RemoveAllItems();
            
            MobileParty.MainParty.AddElementToMemberRoster(Hero.MainHero.CharacterObject, 1);
            party.DisableAi();
            party.Party.Owner = Hero.MainHero;

            _establishedCamp = new PlayerEstablishedCamp
            {
                Party = party,
                ParentParty = parentParty,
                IsTraining = false,
            };
            
            MBTextManager.SetTextVariable("PLAYER_CAMP_RESOURCES", _establishedCamp.Party.GetNumDaysForFoodToLast());
        }

        public void RemoveCamp()
        {
            if (_establishedCamp == null)
            {
                return;
            }

            var party = _establishedCamp.Party;
            var parent = _establishedCamp.ParentParty;
            
            parent.MemberRoster.Add(party.MemberRoster);
            parent.ItemRoster.Add(party.ItemRoster);
            
            party.RemoveParty();

            _establishedCamp = null;
        }

        private void OnHourlyTick()
        {
            if (_establishedCamp == null)
            {
                return;
            }
            
            if (_establishedCamp.IsTraining)
            {
                for (int i = 0; i < _establishedCamp.Party.MemberRoster.Count; i++)
                {
                    _establishedCamp.Party.MemberRoster.AddXpToTroopAtIndex(100, i);
                }
            }
            
            MBTextManager.SetTextVariable("PLAYER_CAMP_RESOURCES", _establishedCamp.Party.GetNumDaysForFoodToLast());

            if (_establishedCamp.Party.GetNumDaysForFoodToLast() <= 1)
            {
                _establishedCamp.IsTraining = false;
                InformationManager.DisplayMessage(new InformationMessage("Your camp is running out of food!'"));
            }
        }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            MBTextManager.SetTextVariable("PLAYER_CAMP_STATE", "idling");
            MBTextManager.SetTextVariable("PLAYER_CAMP_START_STOP", "Start");
            
            starter.AddGameMenu(
                PlayerCampMenu, 
                "Player camp.\n \nTroops are currently {PLAYER_CAMP_STATE}.\nResources left for {PLAYER_CAMP_RESOURCES} days.", 
                null
            );
            
            starter.AddGameMenuOption(
                PlayerCampMenu,
                "player_camp_train",
                "{PLAYER_CAMP_START_STOP} training",
                Utils.positiveLeaveType(GameMenuOption.LeaveType.Recruit),
                _ =>
                {
                    if (_establishedCamp != null)
                    {
                        _establishedCamp.IsTraining = !_establishedCamp.IsTraining;
                        
                        MBTextManager.SetTextVariable("PLAYER_CAMP_STATE", _establishedCamp.IsTraining ? "training" : "idling");
                        MBTextManager.SetTextVariable("PLAYER_CAMP_START_STOP", _establishedCamp.IsTraining ? "Stop" : "Start");
                    }
                }
            );
            
            starter.AddGameMenuOption(
                PlayerCampMenu,
                "player_camp_remove",
                "Remove camp",
                Utils.positiveLeaveType(GameMenuOption.LeaveType.Continue),
                _ =>
                {
                    RemoveCamp();
                    PlayerEncounter.Finish();
                },
                true
            );
            
            starter.AddGameMenuOption(
                PlayerCampMenu, 
                "player_camp_wait", 
                "Wait", 
                Utils.positiveLeaveType(GameMenuOption.LeaveType.Wait),
                _ => GameMenu.ActivateGameMenu("player_camp_menu_wait")
            );

            starter.AddGameMenuOption(
                PlayerCampMenu, 
                "player_camp_leave", 
                "Leave",
                Utils.positiveLeaveType(GameMenuOption.LeaveType.Leave),
                _ => PlayerEncounter.Finish()
            );
            
            // wait menu
            starter.AddWaitGameMenu("player_camp_menu_wait", "Waiting.",
                null,
                args =>
                {
                    args.MenuContext.GameMenu.AllowWaitingAutomatically();
                    args.optionLeaveType = GameMenuOption.LeaveType.Wait;
                    return true;
                },
                null,
                null,
                GameMenu.MenuAndOptionType.WaitMenuHideProgressAndHoursOption
            );

            starter.AddGameMenuOption(
                "player_camp_menu_wait", 
                "wait_leave", 
                "{=UqDNAZqM}Stop waiting",
                Utils.positiveLeaveType(GameMenuOption.LeaveType.Leave),
                _ => GameMenu.ActivateGameMenu(PlayerCampMenu)
            );
        }
    }
}