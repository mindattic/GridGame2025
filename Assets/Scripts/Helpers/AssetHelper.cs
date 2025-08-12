using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using g = Assets.Helpers.GameHelper;

namespace Assets.Helpers
{
    public static class AssetHelper
    {
        public static async Task<T> LoadAssetAsync<T>(string address)
        {
            var handle = Addressables.LoadAssetAsync<T>(address);
            await handle.Task;

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }

            Debug.LogError($"Failed to load {typeof(T)} at address: {address}");
            return default(T);
        }

        public static T LoadAsset<T>(string address)
        {
            var handle = Addressables.LoadAssetAsync<T>(address);
            handle.WaitForCompletion(); // Block until the asset is fully loaded


            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }

            Debug.LogError($"Failed to load {typeof(T)} at address: {address}");
            return default(T);
        }
    }

    /*
     * Alternative: Let the Caller Handle the Release
    If you want the caller to manage the handle (e.g., for long-term use of the asset), you can return the AsyncOperationHandle<Sprite> instead of the Sprite itself:

    public static async Task<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Sprite>> LoadSpriteHandleAsync(string address)
    {
        var handle = Addressables.LoadAssetAsync<Sprite>(address);
        await handle.Task;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return handle;
        }

        Debug.LogError($"Failed to load sprite at address: {address}");
        return default; // Return an empty handle if loading fails
    }

    */

}
