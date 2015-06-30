using Assets.Code.DataPipeline.Providers;
using Assets.Code.Utilities;
using UnityEngine;

namespace Assets.Code.DataPipeline.Loading
{
    public class SoundLoader
    {
        public static void LoadSounds(SoundProvider soundProvider, string folderLocation)
        {
            foreach (var fileName in FileServices.GetResourceFiles(folderLocation, ".ogg", ".wav", ".flac", ".mp3"))
            {
                var sound = FileServices.LoadAudioResource(fileName);

                if(sound != null){
                    sound.name = FileServices.GetEndOfResourcePath(fileName);
                    soundProvider.AddSound(sound);
				}
				else
					Debug.Log("WARNING! tried to load a sound into the provider but it was null! (file : " + fileName + ")");
            }
        }
    }
}
