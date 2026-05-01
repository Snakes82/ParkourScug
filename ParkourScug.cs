using System; // wa
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ParkourScugPlugin.ParkourScugPlugin;
using RWCustom;

namespace ParkourScugPlugin
{
    public class ParkourScugData
    {
        public Player player;
        private readonly Room room;
        private Player.InputPackage PlayerInput => player.input[0];


        public int bellySlideExitCounter = 0;

        public bool betterPoleSlide = false;

        private bool oddTick = false;
        private Player.AnimationIndex previousAnimation = Player.AnimationIndex.None;
        private Vector2 previousVelocity = Vector2.zero;


        public ParkourScugData(Player player)
        {
            this.player = player;
            room = player.room;
        }
        public virtual void ParkourScugTick(On.Player.orig_Update orig, bool eu)
        {
            MovementTick();
            orig(player, eu);

            ///Custom.LogImportant("Var Tracker: " + player.slideUpPole);
        }
        private void MovementTick()
        {
            /* ------------------------------ Smaller Modifiers ------------------------------ */

            player.initSlideCounter++;


            /* ------------------------------ Momentum Mechanics --------------------------- */

            // Slide into beam momentum
            if (player.animation == Player.AnimationIndex.ClimbOnBeam && previousAnimation != Player.AnimationIndex.ClimbOnBeam)
            {
                player.slideUpPole = (int)(-previousVelocity.y < Math.Abs(previousVelocity.x) ? Math.Sqrt(previousVelocity.magnitude) + 6 : 0) * 2; betterPoleSlide = true;
                player.firstChunk.vel.x = 0.0f;
            }

            // Better pole climb
            if (betterPoleSlide == true)
            {
                if (player.slideUpPole < 10)
                {
                    player.slideUpPole = 0;
                    betterPoleSlide = false;
                }
                else
                {
                    if (oddTick) player.slideUpPole += 1;
                }
            }

            /* ------------------------------ Belly Slide Mechanics ------------------------------ */

            if (player.animation == Player.AnimationIndex.BellySlide)
            {
                bellySlideExitCounter = 0;

                int rollCounter = player.rollCounter; /// For some reason, rollCounter is used for the belly slide. the slideCounter var is actually the turn around.

                if (rollCounter >= 0 && rollCounter < 3)
                {
                    player.firstChunk.vel.x += 24.0f * player.rollDirection * (PlayerInput.jmp ? 1.0f : 0.5f);
                    player.firstChunk.vel.y -= 5.5f;
                    for (int i = 0; i < 3; i++) room.AddObject(new WaterDrip(player.firstChunk.pos, Custom.DegToVec((-3.0f * player.slideDirection) * Mathf.Lerp(30f, 70f, UnityEngine.Random.value)) * Mathf.Lerp(6f, 11f, UnityEngine.Random.value), false));

                }
                if (rollCounter >= 3 && rollCounter < 20)
                {
                    player.rollCounter = 14;
                    player.firstChunk.vel.x += 4.0f * player.rollDirection;
                    room.AddObject(new WaterDrip(player.firstChunk.pos, Custom.DegToVec((-1.0f * player.slideDirection) * Mathf.Lerp(30f, 70f, UnityEngine.Random.value)) * Mathf.Lerp(6f, 11f, UnityEngine.Random.value), false));
                }

                if (PlayerInput.jmp)
                {
                    player.longBellySlide = true;
                    bellySlideExitCounter = -10;
                }
                else
                {
                    player.longBellySlide = false;
                }
            }
            else if (bellySlideExitCounter < 20)
            {
                // Larger Coyote Frames
                if (bellySlideExitCounter < 10)
                {
                    player.canJump += 1;
                }
            }


            /* ------------------------------ Final Variable Updates ------------------------------ */

            oddTick = !oddTick;
            previousAnimation = player.animation;
            previousVelocity = player.firstChunk.vel;
        }
    }
}
