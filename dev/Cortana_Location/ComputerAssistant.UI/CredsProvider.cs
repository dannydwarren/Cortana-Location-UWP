using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace ComputerAssistant.UI
{
    public static class CredsProvider
    {
        private static bool _isInitialized;

        public static async Task Initialize()
        {
            if (_isInitialized)
                return;

            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///app.creds"));
            DirectLineKey = await FileIO.ReadTextAsync(file);

            _isInitialized = true;
        }

        public static string DirectLineKey { get; private set; }
    }
}
