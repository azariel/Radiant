using Radiant.Common.Serialization;

namespace Radiant.Custom.Finance.Budget.RadiantBudgetBridge.Configuration;

public static class RadiantBudgetBridgeConfigurationManager
{
    // ********************************************************************
    //                            Constants
    // ********************************************************************
    private const string RADIANT_BUDGET_BRIDGE_CONFIG_FILE_NAME = "RadiantBudgetBridgeConfig.json";

    // ********************************************************************
    //                            Private
    // ********************************************************************
    private static RadiantBudgetBridgeConfiguration fRadiantBudgetBridgeConfiguration;

    // ********************************************************************
    //                            Public
    // ********************************************************************
    public static RadiantBudgetBridgeConfiguration GetConfigFromMemory()
    {
        return fRadiantBudgetBridgeConfiguration ?? ReloadConfig();
    }

    public static RadiantBudgetBridgeConfiguration ReloadConfig()
    {
        // Create default config if doesn't exists
        if (!File.Exists(RADIANT_BUDGET_BRIDGE_CONFIG_FILE_NAME))
            SaveConfigInMemoryToDisk();

        string _ConfigFileContent = File.ReadAllText(RADIANT_BUDGET_BRIDGE_CONFIG_FILE_NAME);
        fRadiantBudgetBridgeConfiguration = JsonCommonSerializer.DeserializeFromString<RadiantBudgetBridgeConfiguration>(_ConfigFileContent);
        return fRadiantBudgetBridgeConfiguration;
    }

    public static void SaveConfigInMemoryToDisk()
    {
        if (fRadiantBudgetBridgeConfiguration == null)
            fRadiantBudgetBridgeConfiguration = new RadiantBudgetBridgeConfiguration();

        string _SerializedConfig = JsonCommonSerializer.SerializeToString(fRadiantBudgetBridgeConfiguration);
        File.WriteAllText(RADIANT_BUDGET_BRIDGE_CONFIG_FILE_NAME, _SerializedConfig);
    }

    public static void SetConfigInMemory(RadiantBudgetBridgeConfiguration configuration)
    {
        fRadiantBudgetBridgeConfiguration = configuration;
    }
}