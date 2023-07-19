using BepInEx;
using BepInEx.Configuration;
using System.Reflection;
using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace EarlySpawnBoost
{
  [BepInPlugin("com.Nuxlar.EarlySpawnBoost", "EarlySpawnBoost", "1.0.5")]

  public class EarlySpawnBoost : BaseUnityPlugin
  {
    public static ConfigEntry<float> percentage;
    public static ConfigEntry<bool> shouldAffectTP;
    private static ConfigFile ESBConfig { get; set; }

    public void Awake()
    {
      ESBConfig = new ConfigFile(Paths.ConfigPath + "\\com.Nuxlar.EarlySpawnBoost.cfg", true);
      percentage = ESBConfig.Bind<float>("General", "% increase", 0.25f, "Default 25%");
      shouldAffectTP = ESBConfig.Bind<bool>("General", "Affect TP Spawns", false, "Should the boost affect teleporter spawn rate?");
      WipeConfig(ESBConfig);

      On.RoR2.CombatDirector.Awake += CombatDirector_Awake;
    }

    private void CombatDirector_Awake(On.RoR2.CombatDirector.orig_Awake orig, CombatDirector self)
    {
      if (Run.instance && Run.instance.stageClearCount < 2 && (Run.instance.name == "ClassicRun(Clone)" || Run.instance.name == "EclipseRun(Clone)"))
      {
        if (self.name == "Director" || (shouldAffectTP.Value && self.name == "Teleporter1(Clone)"))
        {
          self.minRerollSpawnInterval -= self.minRerollSpawnInterval * percentage.Value;
          self.maxRerollSpawnInterval -= self.maxRerollSpawnInterval * percentage.Value;
        }
      }
      orig(self);
    }

    private void WipeConfig(ConfigFile configFile)
    {
      PropertyInfo orphanedEntriesProp = typeof(ConfigFile).GetProperty("OrphanedEntries", BindingFlags.Instance | BindingFlags.NonPublic);
      Dictionary<ConfigDefinition, string> orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(configFile);
      orphanedEntries.Clear();

      configFile.Save();
    }
  }
}