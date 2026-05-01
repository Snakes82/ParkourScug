using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ParkourScugPlugin.ParkourScugPlugin;
using RWCustom;

namespace ParkourScugPlugin
{
    public static class ParkourScugFunctionality
    {
        public class ParkourScug
        {
            // Unused
        }

        public static void ParkourScugTick(Player player, On.Player.orig_Update orig, bool eu)
        {
            Room room = player.room;

            // Movement -------------------------------------

            // Slide Mechanics ------------------------------
            player.initSlideCounter++;
            if (player.animation == Player.AnimationIndex.BellySlide)
            {
                int rollCounter = player.rollCounter; // For some reason, the roll counter is used for the belly slide. the slideCounter var is actually the turn around.

                if (rollCounter >= 0 && rollCounter < 3)
                {
                    player.bodyChunks[0].vel.x += 24.0f * player.rollDirection;
                    player.bodyChunks[0].vel.y -= 5.5f;
                    for (int i = 0; i < 3; i++) room.AddObject(new WaterDrip(player.bodyChunks[0].pos, Custom.DegToVec((-3.0f * player.slideDirection) * Mathf.Lerp(30f, 70f, UnityEngine.Random.value)) * Mathf.Lerp(6f, 11f, UnityEngine.Random.value), false));
                }
                if (rollCounter >= 3 && rollCounter < 20)
                {
                    player.rollCounter = 14;
                    player.bodyChunks[0].vel.x += 4.0f * player.rollDirection;
                    room.AddObject(new WaterDrip(player.bodyChunks[0].pos, Custom.DegToVec((-1.0f * player.slideDirection) * Mathf.Lerp(30f, 70f, UnityEngine.Random.value)) * Mathf.Lerp(6f, 11f, UnityEngine.Random.value), false));
                }
                
                if (player.input[0].jmp)
                {
                    player.longBellySlide = true;
                }
                else
                {
                    player.longBellySlide = false;
                }

                orig(player, eu);

                if (player.animation == Player.AnimationIndex.ClimbOnBeam) // Slide into beam momentum
                {
                    if (player.input[0].jmp) player.slideUpPole = 30;
                    else player.slideUpPole = 20;
                }

                return;
            }

            orig(player, eu);
        }
    }
}
