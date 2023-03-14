using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PokemonMod.Items.ThePokeball
{
    public class ThePokeball : ModItem
    {
        private int chargeTime = 1;
        private int maxChargeTime = 60; // Maximum charge time in ticks
        private int chargeLevel = 0; // Current charge level

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Pokeball");
            Tooltip.SetDefault("Hold down the mouse button to adjust the throw distance and trajectory.");
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 22;
            Item.height = 30;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = 4;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Pokeball.Pokeball>();
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

        public override bool? UseItem(Player player)
        {
            // Handle charging the Pokeball
            chargeTime++;
            chargeLevel = Math.Min(chargeTime / 20, 3);

            // Update the item animation and charge indicator
            player.itemAnimation = maxChargeTime - chargeTime;
            player.itemTime = player.itemAnimation;

            // Check if the throw button has been released
            if (!player.controlUseItem)
            {
                // Throw the Pokeball with the current charge level
                // Calculate the velocity of the Pokeball based on the player's position and the mouse's position
                Vector2 velocity = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * Item.shootSpeed;
                int type = ModContent.ProjectileType<Projectiles.Pokeball.Pokeball>();
                int damage = Item.damage;
                float knockBack = Item.knockBack;
                int owner = player.whoAmI;
                Terraria.DataStructures.IEntitySource source = player.GetSource_ItemUse(Item);
                //Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockBack, owner, aiStyle);


                Projectile.NewProjectile(source, player.Center, velocity, type, Item.damage, (int)Item.knockBack, player.whoAmI, 0f, chargeLevel);

                chargeTime = 1; // Reset the charge time
                chargeLevel = 0; // Reset the charge level

                player.itemAnimation = maxChargeTime;
                player.itemTime = maxChargeTime;
                //Main.PlaySound(SoundID.Item1, player.position);
            }

            return true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.itemAnimation > 0)
            {
                chargeTime++;
                if (chargeTime > maxChargeTime)
                {
                    chargeTime = maxChargeTime;
                }
            }
            else if (chargeTime > 0)
            {
                float chargeLevel = (float)chargeTime / maxChargeTime;
                int chargePercentage = (int)(chargeLevel * 100);
                Main.NewText($"Charge Level: {chargePercentage}%", Color.White);
                chargeTime = 0;
            }
        }
    }
}
