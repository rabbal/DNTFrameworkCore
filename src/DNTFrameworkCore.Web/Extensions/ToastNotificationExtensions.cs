using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DNTFrameworkCore.Web.Extensions
{
    public static class ToastNotificationExtensions
    {
        private const string TempDataKey = "TOAST_NOTIFICATION_TEMPDATA_KEY";
        internal static void AddToastNotification(this ITempDataDictionary tempData, string type, string message)
        {
            var notifications = tempData.ContainsKey(TempDataKey)
                ? (List<KeyValuePair<string, string>>)tempData[TempDataKey]
                : new List<KeyValuePair<string, string>>();

            notifications.Add(new KeyValuePair<string, string>(type, message));

            tempData[TempDataKey] = notifications;
        }

        public static void AddInfoToastNotification(this ITempDataDictionary tempData, string message)
        {
            tempData.AddToastNotification("info", message);
        }
        public static void AddSuccessToastNotification(this ITempDataDictionary tempData, string message)
        {
            tempData.AddToastNotification("success", message);
        }
        public static void AddErrorToastNotification(this ITempDataDictionary tempData, string message)
        {
            tempData.AddToastNotification("error", message);
        }
        public static void AddWarningToastNotification(this ITempDataDictionary tempData, string message)
        {
            tempData.AddToastNotification("warning", message);
        }
    }
}