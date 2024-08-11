using System.Collections;
using System.IO;
using UnityEngine;

public class AssetBundleLoader : MonoBehaviour
{
    public delegate void LoadAssetCallback(GameObject loadedModGameObject);
    public event LoadAssetCallback LoadedAssetEvent;
    public IEnumerator LoadAsset(string path, string bundleName)
    {
        string bundlePath = Path.Combine(path, bundleName);

        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return bundleRequest;

        AssetBundle bundle = bundleRequest.assetBundle;
        if (bundle != null)
        {
            AssetBundleRequest assetRequest = bundle.LoadAssetAsync<GameObject>(bundle.GetAllAssetNames()[0]);
            yield return assetRequest;

            GameObject obj = assetRequest.asset as GameObject;
            if (obj != null)
            {
                LoadedAssetEvent?.Invoke(obj);
            }
            else
            {
                Debug.LogError("Failed to load asset: " + bundleName);
            }

            bundle.Unload(false);
        }
        else
        {
            Debug.LogError("Failed to load AssetBundle from path: " + bundlePath);
        }
    }
}