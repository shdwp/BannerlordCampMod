using HarmonyLib;
using StoryMode.GameModels;
using TaleWorlds.CampaignSystem;

namespace CampMod
{
    [HarmonyPatch(typeof(StoryModeEncounterGameMenuModel), "GetEncounterMenu")]
    public class EncounterMenuPatch
    {
        static void Postfix(PartyBase attackerParty, PartyBase defenderParty, ref bool startBattle, ref bool joinBattle, ref string __result)
        {
            if (defenderParty.Id.StartsWith("main_player_camp:"))
            {
                startBattle = false;
                joinBattle = false;
                __result = PlayerCampBehavior.PlayerCampMenu;
            }
        }
    }
}