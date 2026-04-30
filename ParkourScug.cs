using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ParkourScugPlugin.ParkourScugPlugin;

namespace ParkourScugPlugin
{
    public static class ParkourScugFunctionality
    {
        public class ParkourScug
        {
            // Unused
        }

        public static void ParkourScugTick(Player player)
        {
            player.initSlideCounter++;
            if (player.animation == Player.AnimationIndex.BellySlide)
            {
                int rollCounter = player.rollCounter; // For some reason, the roll counter is used for the belly slide. the slideCounter var is actually the turn around.

                if (rollCounter < 3)
                {
                    player.bodyChunks[0].vel.x += 24.0f * player.rollDirection;
                    player.bodyChunks[0].vel.y -= 5.5f;
                }
                if (rollCounter > 3)
                {
                    player.rollCounter = 14;
                    player.bodyChunks[0].vel.x += 4.0f * player.rollDirection;
                }
            }
        }
    }
}
