using EveChatMonitorer.Models;

namespace EveChatMonitorer.Configuration
{
    public class EveChatMonitorerConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string LogsDirectoryPath { get; set; }
        public string ForcePlayerNameInLogs { get; set; }
        public int RefreshChatsEveryXMs { get; set; } = 3000;// Will check logs every X ms
        public string KeyLogFileName {get; set; }
        public List<KeywordTriggerNotification> KeywordTriggerNotificationCollection { get; set; } = new List<KeywordTriggerNotification>();
    }
}
