using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PokemonMod.Items.ThePokeball
{
    public class ThePokeball : ModItem
    {
        private int chargeTime = 0;
        private int maxChargeTime = 60; // Maximum charge time in ticks

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Pokeball");
            Tooltip.SetDefault("Hold down the mouse button to adjust the throw distance and trajectory.");
        }

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 22;
            Item.height = 30;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = 4;
            Item.autoReuse = true;
            Item.shootSpeed = 6f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Ruby, 6);
            recipe.AddIngredient(ItemID.Diamond, 1);
            recipe.AddIngredient(ItemID.IronBar, 10);
            recipe.AddIngredient(ItemID.LeadBar, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }

        public override void HoldItem(Player player)
        {
            if (player.controlUseItem)
            {
                Item.damage = 0;
                //Main.NewText("Charge time: " + chargeTime + " / " + maxChargeTime);
                chargeTime++;
                if (chargeTime > maxChargeTime)
                {
                    chargeTime = maxChargeTime;
                }
                // Keep the animation at the beginning while charging
                player.itemAnimation = Item.useAnimation;
                player.itemTime = Item.useAnimation;
            }
            else if (chargeTime > 0 && !player.controlUseItem)
            {
                Item.damage = 100;
                int chargeLevel = Math.Min(chargeTime / 20, 3);
                Vector2 velocity = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * (Item.shootSpeed + 2f * chargeLevel);
                int type = ModContent.ProjectileType<Projectiles.Pokeball.Pokeball>();

                // Calculate the final damage based on the charge time
                float damageMultiplier = 1 + (float)chargeTime / maxChargeTime;
                int damage = (int)(Item.damage * damageMultiplier);

                float knockBack = Item.knockBack;
                int owner = player.whoAmI;
                Terraria.DataStructures.IEntitySource source = player.GetSource_ItemUse(Item);

                Projectile.NewProjectile(source, player.Center, velocity, type, damage, (int)Item.knockBack, player.whoAmI, 0f, chargeLevel);

                chargeTime = 0;
            }
        }

    }
}
