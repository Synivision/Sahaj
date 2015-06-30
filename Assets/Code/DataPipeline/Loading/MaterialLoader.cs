using Assets.Code.DataPipeline.Providers;
using Assets.Code.Utilities;

namespace Assets.Code.DataPipeline.Loading
{
    class MaterialLoader
    {
        public static void LoadMaterial(MaterialProvider materialProvider, string folderLocation)
        {
            var materialPaths = FileServices.GetResourceFiles(folderLocation, ".mat");

            //build textures and sprite
            foreach (var path in materialPaths)
            {
                var material = FileServices.LoadMaterialResources(path);

                materialProvider.AddMaterial(material);
            }
        }
    }
}
