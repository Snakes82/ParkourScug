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
            if (player.room.world.game.IsArenaSession)
            {
                if (player.slugcatStats.name == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Saint)
                {
                    bool flag1 = player.input[0].spec && !player.input[1].spec;
                    bool flag2 = (player.input[0].jmp && player.input[0].pckp) && !(player.input[1].jmp && player.input[1].pckp);
                    if (flag1 || flag2)
                    {
                        if (player.godDeactiveTimer > 0)
                        {
                            player.ActivateAscension();
                        }
                        else
                        {
                            player.DeactivateAscension();
                        }
                    }
                }
            }
            if (!IsParkourScug(player))
            {
                orig(player, eu);
                return;
            }

            GetParkourScugData(player).ParkourScugTick(orig, eu);
        }
        private bool SlugcatHandEngageInMovement(On.SlugcatHand.orig_EngageInMovement orig, SlugcatHand hand)
        {
            return orig(hand); // Unimplemented
            Player player = null;
            if (!IsParkourScug(player))
            {
                return orig(hand);
            }

            GetParkourScugData(player);
            return orig(hand);
        }
    }
}
