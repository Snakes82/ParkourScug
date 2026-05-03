using System; // wa
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ParkourScugPlugin.ParkourScugPlugin;
using RWCustom;
using IL.Menu.Remix;
using IL.MoreSlugcats;

namespace ParkourScugPlugin
{
    public class ParkourScugData
    {
        #pragma warning disable IDE1006
        public Player player;
        private Room room => player.room;
        private Player.InputPackage input => player.input[0];
        private PlayerGraphics GetPlayerGraphics() => (player.graphicsModule as PlayerGraphics);
        #pragma warning restore IDE1006


        public int bellySlideExitCounter = 0;

        public bool betterPoleSlide = false;

        private bool oddTick = false;
        private Player.AnimationIndex previousAnimation = Player.AnimationIndex.None;
        private Vector2 previousVelocity = Vector2.zero;


        public ParkourScugData(Player player)
        {
            this.player = player;
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


            /* ------------------------------ New Mechanics --------------------------- */

            // Ceiling hang
            if (player.IsTileSolid(0, 0, 1) && !player.IsTileSolid(1, 0, -1) && player.animation == Player.AnimationIndex.None && input.y > 0)
            {
                player.animation = Player.AnimationIndex.HangFromBeam;
                for (int i = 0; i < GetPlayerGraphics().hands.Length; i++)
                {
                    SlugcatHand hand = GetPlayerGraphics().hands[i];
                    hand.absoluteHuntPos = new Vector2(player.bodyChunks[0].pos.x, player.room.MiddleOfTile(player.bodyChunks[0].pos).y);
                    hand.absoluteHuntPos.y -= 1f;
                    hand.absoluteHuntPos.x += ((hand.limbNumber == 0) ? (-1f) : 1f) * (10f + 3f * Mathf.Sin((float)Math.PI * 2f * (float)player.animationFrame / 20f));
                }
            }


            /* ------------------------------ Momentum Mechanics --------------------------- */

            // Instant roll pounce
            if (player.animation == Player.AnimationIndex.Roll && player.input[0].jmp && !player.input[1].jmp && player.rollCounter < 5)
            {
                BoostEffect();
                BoostDir(18.0f, 6.0f);
                player.rollCounter = 5;
            }

            // Slide into beam momentum
            if (player.animation == Player.AnimationIndex.ClimbOnBeam && previousAnimation != Player.AnimationIndex.ClimbOnBeam)
            {
                player.slideUpPole = (int)(-previousVelocity.y < Math.Abs(previousVelocity.x) ? Mathf.Sqrt(previousVelocity.magnitude) + 6 : 0) * 2; betterPoleSlide = true;
                player.firstChunk.vel.x = 0.0f;
            }

            // Better pole climb
            if (betterPoleSlide == true)
            {
                if (player.slideUpPole > 10)
                {
                    if (oddTick) player.slideUpPole += 1;
                    if (player.animation == Player.AnimationIndex.GetUpToBeamTip)
                    {
                        player.animation = Player.AnimationIndex.None;
                        player.firstChunk.vel.y += Mathf.Sqrt(player.slideUpPole - 10) + 5;
                    }
                }
                else
                {
                    player.slideUpPole = 0;
                    betterPoleSlide = false;
                }
            }

            /* ------------------------------ Belly Slide Mechanics ------------------------------ */

            if (player.animation == Player.AnimationIndex.Roll && previousAnimation != Player.AnimationIndex.Roll && input.jmp && bellySlideExitCounter > 10)
            {
                player.animation = Player.AnimationIndex.BellySlide;
                player.rollCounter = 0;
                player.allowRoll = -1;
            }

            if (player.animation == Player.AnimationIndex.BellySlide)
            {
                bellySlideExitCounter = 0;

                int rollCounter = player.rollCounter; /// For some reason, rollCounter is used for the belly slide. the slideCounter var is actually the turn around.
                Vector2 feetPos = player.bodyChunks[1].pos;

                if (rollCounter >= 0 && rollCounter < 3)
                {
                    BoostDir((input.jmp ? 24.0f : 12.0f), -5.5f);

                    if (rollCounter == 1)
                    {
                        BoostEffect();
                    }
                }
                if (rollCounter >= 3 && rollCounter < 20)
                {
                    player.rollCounter = 14;
                    BoostDir(4.0f, 0.0f);

                    room.AddObject(new Spark(
                        feetPos, 
                        Custom.DegToVec((-input.x) * Mathf.Lerp(30f, 70f, UnityEngine.Random.value)) * Mathf.Lerp(6f, 8f, UnityEngine.Random.value), 
                        new Color(1f, 0.8f, 0.5f), 
                        null, 6, 11
                        ));
                    room.PlaySound(SoundID.Cyan_Lizard_Prepare_Small_Jump, feetPos, 1.0f, 3.0f);
                }

                if (input.jmp)
                {
                    player.longBellySlide = true;
                }
                else
                {
                    player.longBellySlide = false;
                }
            }
            else if (bellySlideExitCounter < 20)
            {
                bellySlideExitCounter++;

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

        private void Boost(Vector2 vel) { player.firstChunk.vel += vel; }
        private void Boost(float x, float y) { Boost(new Vector2(x, y)); }
        private void BoostDir(float x, float y) { Boost(x * input.x, y); }

        public void BoostEffect()
        {
            Vector2 feetPos = player.bodyChunks[1].pos;
            Color color = new Color(1f, 0.8f, 0.5f);
            for (int i = 0; i < 9; i++) room.AddObject(new Spark(feetPos, 3.0f * Custom.DegToVec((-input.x) * Mathf.Lerp(30f, 70f, UnityEngine.Random.value)) * Mathf.Lerp(6f, 11f, UnityEngine.Random.value), color, null, 6, 11));
            room.AddObject(new Explosion.ExplosionLight(feetPos, 100.0f, 0.5f, 6, color));
            room.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Throw_FireSpear, feetPos, 2.0f, 1.3f);
        }
    }
}
