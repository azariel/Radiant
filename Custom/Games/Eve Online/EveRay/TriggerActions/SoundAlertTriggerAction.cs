using System.Media;
using File = System.IO.File;

namespace EveRay.TriggerActions
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
