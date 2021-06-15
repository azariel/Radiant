using Newtonsoft.Json;

namespace Radiant.Common.Serialization
{
    public static class JsonCommonSerializer
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static T DeserializeFromString<T>(string aSerializedObjectToDeserialize) // where T : ISerializable
        {
            return JsonConvert.DeserializeObject<T>(aSerializedObjectToDeserialize, Settings);
        }

        public static string SerializeToString<T>(T aObjectToSerialize) // where T : ISerializable
        {
            return JsonConvert.SerializeObject(aObjectToSerialize, Settings);
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public static JsonSerializerSettings Settings => new()
        {
            TypeNameHandling = TypeNameHandling.Objects
        };
    }
}
