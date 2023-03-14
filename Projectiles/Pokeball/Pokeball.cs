using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PokemonMod.Projectiles.Pokeball
{
    public class Pokeball : ModProjectile
    {
        private bool hasHit = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pokeball");
            Main.projFrames[Projectile.type] = 3;
            // The first frame is the closed pokeball
            // The second frame is the half open pokeball
            // The third frame is the open pokeball
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 30;

            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f; // Use a timer to wait 15 ticks before applying gravity.
            if (hasHit)
            {
                Projectile.velocity.X *= 0.92f;
                Projectile.velocity.Y *= 0.92f;
                if (Math.Abs(Projectile.velocity.X) < 0.1f)
                {
                    Projectile.velocity.X = 0f;
                }
                // Pokeball has hit an NPC or a tile, wait 15 frames on the second frame before changing to the third frame
                Projectile.ai[0]++;
                if (Projectile.ai[0] >= 15f)
                {
                    Projectile.ai[0] = 15f;
                    Projectile.frame = 2;

                    int width = 50;
                    int height = 50;
                    // Make a splash effect when the pokeball hits something
                    for (int i = 0; i < 10; i++)
                    {
                        int dust = Dust.NewDust(Projectile.position, width, height, 6, 0f, 0f, 100, Color.White, 1.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity = Projectile.velocity * 0.5f;
                    }
                }
            }
            else
            {
                Projectile.ai[0] += 1f; // Use a timer to wait 15 ticks before applying gravity.
                if (Projectile.ai[0] >= 15f)
                {
                    Projectile.ai[0] = 15f;
                    Projectile.velocity.Y += 0.1f;
                }
                if (Projectile.velocity.Y > 16f)
                {
                    Projectile.velocity.Y = 16f;
                }
            }

            if (!hasHit)
            {
                Projectile.rotation += 0.4f * (float)Projectile.direction;
            }
            else // Rotate the pokeball so the rotation is reset to the default state
            {
                Projectile.rotation = 0f;
            }
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!hasHit)
            {
                hasHit = true;
                Projectile.damage = 0;

                Projectile.velocity.Y = -6f;
                //Projectile.velocity.X *= 0.75f;
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;

                if (Projectile.frame == 0)
                {

                }

                Projectile.frame = 1;
                Projectile.timeLeft = 60;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            OnHitNPC(null, 0, 0f, false);
            return false;
        }
    }
}