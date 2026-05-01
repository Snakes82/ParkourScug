using BepInEx; //wawa
using BepInEx.Logging;
using IL.MoreSlugcats;
using On.Watcher;
using RWCustom;
using UnityEngine;

namespace ParkourScugPlugin
{
    [BepInPlugin("com.flyingfishbone.ParkourScug", "Parkour Scug", "0.1")]
    [BepInDependency("slime-cubed.slugbase")]
    public class ParkourScugPlugin : BaseUnityPlugin
    {
        public static ManualLogSource logger;

        private bool IsParkourScug(Player player) { return true; }

        public void OnEnable()
        {
            logger = BepInEx.Logging.Logger.CreateLogSource("   ParkourScugPlugin");

            On.Player.Update += PlayerUpdateTick;

            logger.LogInfo("Parkour Scug plugin loaded!");
        }

        private void PlayerUpdateTick(On.Player.orig_Update orig, Player player, bool eu)
        {
            if (!IsParkourScug(player))
            {
                orig(player, eu);
                return;
            }

            ParkourScugFunctionality.ParkourScugTick(player, orig, eu);
        }
    }
}
