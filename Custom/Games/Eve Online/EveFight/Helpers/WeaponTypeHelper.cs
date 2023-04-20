using EveFight.Configuration;
using EveFight.Models;
using Radiant.Common.Diagnostics;
using System;
using System.Linq;

namespace EveFight.Helpers
{
    public static class WeaponTypeHelper
    {
        public static WeaponDefinition GetWeaponDefinitionFromRawStr(string aRawWeaponType)
        {
            EveFightConfiguration _Config = EveFightConfigurationManager.GetConfigFromMemory();

            // Note: when we'll handle amminition types, this will change
            WeaponDefinition _WeaponDefinition = _Config.WeaponDefinitions.FirstOrDefault(w => aRawWeaponType.Contains(w.WeaponType, StringComparison.InvariantCultureIgnoreCase));

            if (_WeaponDefinition == null)
            {
                LoggingManager.LogToFile("65A8A3BC-72BE-4CB3-BE6D-6EA94AEDFE37", $"Weapon type [{aRawWeaponType}] wasn't found in configuration.");
                return null;
            }

            return _WeaponDefinition;
        }
    }
}
