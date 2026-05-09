using BepInEx; //wawawawa
using BepInEx.Logging;
using IL.MoreSlugcats;
using On.Watcher;
using RWCustom;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ParkourScugPlugin
{
    [BepInPlugin("com.flyingfishbone.ParkourScug", "Parkour Scug", "0.1")]
    [BepInDependency("slime-cubed.slugbase")]
    public class ParkourScugPlugin : BaseUnityPlugin
    {
        public static ManualLogSource logger;

        private bool IsParkourScug(Player player) { return player.slugcatStats.name == SlugcatStats.Name.White; }

        public void OnEnable()
        {
            logger = BepInEx.Logging.Logger.CreateLogSource("   ParkourScugPlugin");

            On.Player.Update += PlayerUpdateTick;
            On.SlugcatHand.EngageInMovement += SlugcatHandEngageInMovement;

            logger.LogInfo("Parkour Scug plugin loaded!");
        }

        private static readonly ConditionalWeakTable<Player, ParkourScugData> PlayerExtensionData = new ConditionalWeakTable<Player, ParkourScugData>();
        public static ParkourScugData GetParkourScugData(Player player) => PlayerExtensionData.GetValue(player, k => new ParkourScugData(player));
        private void PlayerUpdateTick(On.Player.orig_Update orig, Player player, bool eu)
        {
            if (!IsParkourScug(player))
            {
                orig(player, eu);
                return;
            }

            GetParkourScugData(player).ParkourScugTick(orig, eu);
        }
        private bool SlugcatHandEngageInMovement(On.SlugcatHand.orig_EngageInMovement orig, SlugcatHand hand)
        {
            Player player = hand.connection.owner as Player;
            if (!IsParkourScug(player))
            {
                return orig(hand);
            }

            return GetParkourScugData(player).animationData.OnHandEngageInMovement(orig, hand);
        }
    }
}
