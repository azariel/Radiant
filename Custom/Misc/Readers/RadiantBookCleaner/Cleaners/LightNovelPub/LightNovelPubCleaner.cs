using System.Text.RegularExpressions;

namespace Radiant.Custom.Readers.RadiantBookCleaner.Cleaners.LightNovelPub
{
    public class LightNovelPubCleaner : ICleaner
    {
        /// <inheritdoc />
        public string Clean(string aRawText)
        {
            // TODO: Replace this by a beautiful regex matching these use cases
            string[] _TextToReplace = 
            {
                "Foll_ow current_novel on lightno​velpub.c‍om",
                "Foll_ow current_novel on lightnovelpub.com",
                "Foll_ow new_episo_des on the lightnovelpub.com platform.",
                "For more_novel, visit lightno‍velpub.c‍om",
                "For more_novel, visit lightnovelpub.com",
                "New novel_chap_ters are published here: lightno‍velpub.c‌om",
                "New novel_chap_ters are published here: lightnovelpub.com",
                "New_chap_ters are pub_lished on lightnovelpub.com",
                "The latest_epi_sodes are on_the lightno‌velpub.c‍om website.",
                "The latest_epi_sodes are on_the lightnovelpub.com website.",
                "The latest_epi_sodes are on_the ʟɪɢʜᴛɴᴏᴠᴇʟᴘᴜʙ.ᴄᴏᴍ website.",
                "The most up-to-date nov_els are published_here &gt; lightnovelpub.c‌om",
                "The most up-to-date nov_els are published_here &gt; lightnovelpub.com",
                "The source of this_chapter; lightnovelpub.c‌om",
                "The source of this_chapter; lightnovelpub.com",
                "This_content is taken from lightnovelpub.com",
                "Try the lightnovelpub.c‌om platform_for the most advanced_reading experience.",
                "Try the lightnovelpub.com platform_for the most advanced_reading experience.",
                "Visit ʟɪɢʜᴛɴᴏᴠᴇʟᴘᴜʙ.ᴄᴏᴍ, for the best no_vel_read_ing experience",
                "Updated_at lightnovelpub.com",
                "Visit to lightno​velpub.c­om discover_new novels.",
                "Visit to lightnovelpub.com discover_new novels.",
                "Visit lightnovelpub.c‍om for a better_user experience",
                "Visit lightnovelpub.com for a better_user experience",
                "Visit lightnovelpub.com for a better_user experience",
                "Visit ʟɪɢʜᴛɴᴏᴠᴇʟᴘᴜʙ.ᴄᴏᴍ for a better_user experience",
                "Visit lightnovelpub.com for a better_reading experience",
                "Visit lightnovelpub.c‌om, for the best no_vel_read_ing experience",
                "Visit lightnovelpub.com, for the best no_vel_read_ing experience",
                "You can_find the rest of this_content on the lightnovelpub.com platform."
            };

            string _OutputText = Regex.Replace(aRawText, @"\p{C}+", string.Empty);

            foreach (string _TextElementToReplace in _TextToReplace)
                _OutputText = _OutputText.Replace(_TextElementToReplace, "");

            return _OutputText;
        }

        /// <inheritdoc />
        public void CleanFilesInDirectory(string aDirectoryPath, string aSearchPattern, SearchOption aSearchOption)
        {
            string[] _FilesToProcess = Directory.EnumerateFiles(aDirectoryPath, aSearchPattern, aSearchOption).ToArray();

            foreach (string _FilePathToClean in _FilesToProcess)
            {
                if (!File.Exists(_FilePathToClean))
                    continue;

                string _FileContent = File.ReadAllText(_FilePathToClean);

                if (string.IsNullOrWhiteSpace(_FileContent))
                    continue;

                string _CleanedContent = Clean(_FileContent);

                // Replace file content with cleaned content
                File.WriteAllText(_FilePathToClean, _CleanedContent);
            }
        }
    }
}
