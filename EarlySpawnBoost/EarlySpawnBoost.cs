using BepInEx;
using BepInEx.Configuration;
using System.Reflection;
using System.Collections.Generic;
using RoR2;

namespace EarlySpawnBoost
{
  [BepInPlugin("com.Nuxlar.EarlySpawnBoost", "EarlySpawnBoost", "1.1.1")]

  public class EarlySpawnBoost : BaseUnityPlugin
  {
    public static ConfigEntry<float> percentIncrease;
    public static ConfigEntry<bool> shouldAffectTP;
    private static ConfigFile ESBConfig { get; set; }

    public void Awake()
    {
      ESBConfig = new ConfigFile(Paths.ConfigPath + "\\com.Nuxlar.EarlySpawnBoost.cfg", true);
      percentIncrease = ESBConfig.Bind<float>("General", "Percent Increase", 0.15f, "How many extra credits should the director get in percent?");
      shouldAffectTP = ESBConfig.Bind<bool>("General", "Affect TP Spawns", false, "Should the boost affect teleporter spawn rate?");
      WipeConfig(ESBConfig);
      On.RoR2.CombatDirector.Awake += CombatDirector_Awake;
    }

    private void CombatDirector_Awake(On.RoR2.CombatDirector.orig_Awake orig, CombatDirector self)
    {
      if (Run.instance && (Run.instance.name == "ClassicRun(Clone)" || Run.instance.name == "EclipseRun(Clone)"))
      {
        if ((self.name == "Director" || (shouldAffectTP.Value && self.name == "Teleporter1(Clone)")) && Run.instance.stageClearCount < 2)
          self.creditMultiplier *= 1 + percentIncrease.Value;
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