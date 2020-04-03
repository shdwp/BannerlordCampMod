using System.Reflection;
using TaleWorlds.CampaignSystem.GameMenus;

namespace CampMod
{
    public static class Utils
    {
        public static GameMenuOption.OnConditionDelegate positiveLeaveType(GameMenuOption.LeaveType type = GameMenuOption.LeaveType.Submenu)
        {
            return delegate(MenuCallbackArgs args)
            {
                args.optionLeaveType = type;
                return true;
            };
        }
    }
    
    public static class ReflectionHelpers
    {
        public static T PrivateValue<T>(this object o, string fieldName) where T: class
        {
            var field = o.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return field.GetValue(o) as T;
        }

        public static object PrivateValue(this object o, string fieldName)
        {
            var field = o.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return field.GetValue(o);
        }

        public static void PrivateValueSet<T>(this object o, string fieldName, T value) where T : class
        {
            var field = o.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            field.SetValue(o, value);
        }
    }
}