using Assets.Code.DataPipeline.Providers;
using Assets.Code.Utilities;

namespace Assets.Code.DataPipeline.Loading
{
    public static class TextureLoader
    {
        public static void LoadTextures(TextureProvider textureProvider, SpriteProvider spriteProvider, string folderLocation)
        {
            var texturePaths = FileServices.GetResourceFiles(folderLocation, ".png", ".jpg");

            // build textures and sprite
            foreach (var path in texturePaths)
            {
                var texture = FileServices.LoadTextureResource(path);
                var sprite = FileServices.CreateSpriteFromTexture(texture);

                textureProvider.AddTexture(texture);
                spriteProvider.AddSprite(sprite);
            }
        }
    }
}
