using static Android.Provider.Settings;

namespace MauiAppStart.Platforms
{
    public class DeviceInfoService : IDeviceInfoService
    {
        public string GetDeviceModel()
        {
            var context = Android.App.Application.Context;

            string id = Android.Provider.Settings.Secure.GetString(context.ContentResolver, Secure.AndroidId);

            return id;
        }
    }
}
