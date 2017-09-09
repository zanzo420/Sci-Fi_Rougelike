namespace LlockhamIndustries.Decals
{
    public class DecalsAssetProcessor : UnityEditor.AssetModificationProcessor
    {
        //Callback for when the project or scene is saved
        private static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (var path in paths)
            //Scene
                if (path.Contains(".unity"))
                    DynamicDecals.System.RestoreDepthTextureModes();
            return paths;
        }
    }
}