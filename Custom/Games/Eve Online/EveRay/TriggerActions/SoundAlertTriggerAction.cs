using System.Media;
using Radiant.Common.Screen.Watcher.PixelsInZone.TriggerActions;
using File = System.IO.File;

namespace Radiant.Custom.Games.EveOnline.EveRay.TriggerActions
{
    public class SoundAlertTriggerAction : ITriggerAction
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public void Trigger()
        {
            if (!File.Exists(this.SoundFilePath))
                return;

            fSoundPlayer?.Play();
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        SoundPlayer fSoundPlayer;
        private string fSoundFilePath = "";

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string SoundFilePath
        {
            get => fSoundFilePath;
            set
            {
                fSoundFilePath = value;
                fSoundPlayer = new SoundPlayer(fSoundFilePath);
            }
        }
    }
}
