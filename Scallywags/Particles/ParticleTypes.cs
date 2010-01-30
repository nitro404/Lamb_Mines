using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Particles
{
    class ParticleTypes
    {

        public ParticleTypes()
        {

        }

        public ParticleSettings Wake
        {
            get
            {
                ParticleSettings settings = new ParticleSettings();

                settings.TextureName = "wake";

                settings.State = ParticleState.Wake;
                
                settings.MaxParticles = 500;

                settings.Duration = TimeSpan.FromSeconds(2);

                settings.DurationRandomness = 1;

                settings.MinHorizontalVelocity = -4;
                settings.MaxHorizontalVelocity = 4;

                settings.MinVerticalVelocity = 0;
                settings.MaxVerticalVelocity = 0;

                // Set gravity upside down, so the flames will 'fall' upward.
                settings.Gravity = new Vector3(0, 0, 0);

                settings.MinColor = new Color(255, 255, 255, 10);
                settings.MaxColor = new Color(255, 255, 255, 40);

                settings.MinStartSize = 2;
                settings.MaxStartSize = 5;

                settings.MinEndSize = 10;
                settings.MaxEndSize = 25;

                // Use additive blending.
                settings.SourceBlend = Blend.SourceAlpha;
                settings.DestinationBlend = Blend.One;

                return settings;                
            }
        }

        public ParticleSettings Fire
        {
            get
            {
                ParticleSettings settings = new ParticleSettings();

                settings.TextureName = "fire";

                settings.State = ParticleState.Fire;

                settings.MaxParticles = 500;

                settings.Duration = TimeSpan.FromSeconds(3);

                settings.DurationRandomness = 2;

                settings.MinHorizontalVelocity = 0;
                settings.MaxHorizontalVelocity = 3;

                settings.MinVerticalVelocity = -1;
                settings.MaxVerticalVelocity = 3;

                // Set gravity upside down, so the flames will 'fall' upward.
                settings.Gravity = new Vector3(0, 3, 0);

                settings.MinColor = new Color(255, 255, 255, 10);
                settings.MaxColor = new Color(255, 255, 255, 40);

                settings.MinStartSize = 4;
                settings.MaxStartSize = 10;

                settings.MinEndSize = 20;
                settings.MaxEndSize = 50;

                // Use additive blending.
                settings.SourceBlend = Blend.SourceAlpha;
                settings.DestinationBlend = Blend.One;

                return settings;
            }
        }

        public ParticleSettings Smoke
        {
            get
            {
                ParticleSettings settings = new ParticleSettings();

                settings.TextureName = "smoke";

                settings.State = ParticleState.Smoke;

                settings.MaxParticles = 400;

                settings.Duration = TimeSpan.FromSeconds(8);

                settings.MinHorizontalVelocity = 0;
                settings.MaxHorizontalVelocity = 5;

                settings.MinVerticalVelocity = 10;
                settings.MaxVerticalVelocity = 25;

                // Create a wind effect by tilting the gravity vector sideways.
                settings.Gravity = new Vector3(-20, -5, 0);

                settings.EndVelocity = 0.75f;

                settings.MinRotateSpeed = -1;
                settings.MaxRotateSpeed = 1;

                settings.MinStartSize = 5;
                settings.MaxStartSize = 10;

                settings.MinEndSize = 25;
                settings.MaxEndSize = 100;

                return settings;
            }
        }

        public ParticleSettings Splash
        {
            get
            {
                ParticleSettings settings = new ParticleSettings();

                settings.TextureName = "splashWater";

                settings.State = ParticleState.Splash;

                settings.MaxParticles = 400;

                settings.Duration = TimeSpan.FromSeconds(5);
                //settings.DurationRandomness = 0;

                settings.EmitterVelocitySensitivity = 1;

                settings.MinHorizontalVelocity = -5;
                settings.MaxHorizontalVelocity = 5;

                settings.MinVerticalVelocity = 15;
                settings.MaxVerticalVelocity = 25;

                // Create a wind effect by tilting the gravity vector sideways & pulling downwards gravity as well
                settings.Gravity = new Vector3(0, -100, 0);

                settings.MinColor = new Color(255, 255, 255, 75);
                settings.MaxColor = new Color(255, 255, 255, 90);

                settings.EndVelocity = 0.75f;

                settings.MinRotateSpeed = 0;
                settings.MaxRotateSpeed = 0;

                settings.MinStartSize = 6;
                settings.MaxStartSize = 10;

                settings.MinEndSize = 2;
                settings.MaxEndSize = 6;

                // Use additive blending.
                settings.SourceBlend = Blend.SourceAlpha;
                settings.DestinationBlend = Blend.One;

                return settings;
            }
        }
        public ParticleSettings Dirt
        {
            get
            {
                ParticleSettings settings = new ParticleSettings();

                settings.TextureName = "Dirt";

                settings.State = ParticleState.Dirt;

                settings.MaxParticles = 400;

                settings.Duration = TimeSpan.FromSeconds(5);
                //settings.DurationRandomness = 0;

                settings.EmitterVelocitySensitivity = 1;

                settings.MinHorizontalVelocity = -5;
                settings.MaxHorizontalVelocity = 5;

                settings.MinVerticalVelocity = 15;
                settings.MaxVerticalVelocity = 25;

                // Create a wind effect by tilting the gravity vector sideways & pulling downwards gravity as well
                settings.Gravity = new Vector3(0, -100, 0);

                settings.MinColor = new Color(200, 200, 200, 100);
                settings.MaxColor = new Color(200, 200, 200, 100);

                settings.EndVelocity = 0.75f;

                settings.MinRotateSpeed = 0;
                settings.MaxRotateSpeed = 0;

                settings.MinStartSize = 2;
                settings.MaxStartSize = 4;

                settings.MinEndSize = 6;
                settings.MaxEndSize = 10;

                // Use additive blending.
                //settings.SourceBlend = Blend.SourceAlpha;
                //settings.DestinationBlend = Blend.One;

                return settings;
            }
        }
        public ParticleSettings Sand
        {
            get
            {
                ParticleSettings settings = new ParticleSettings();

                settings.TextureName = "Sand";

                settings.State = ParticleState.Sand;

                settings.MaxParticles = 1000;

                settings.Duration = TimeSpan.FromSeconds(5);
                //settings.DurationRandomness = 0;

                settings.EmitterVelocitySensitivity = 1;

                settings.MinHorizontalVelocity = -5;
                settings.MaxHorizontalVelocity = 5;

                settings.MinVerticalVelocity = 15;
                settings.MaxVerticalVelocity = 25;

                // Create a wind effect by tilting the gravity vector sideways & pulling downwards gravity as well
                settings.Gravity = new Vector3(0, -100, 0);

                settings.MinColor = new Color(225, 225, 225, 80);
                settings.MaxColor = new Color(225, 225, 225, 80);

                settings.EndVelocity = 0.75f;

                settings.MinRotateSpeed = 0;
                settings.MaxRotateSpeed = 0;

                settings.MinStartSize = 1;
                settings.MaxStartSize = 2;

                settings.MinEndSize = 3;
                settings.MaxEndSize = 4;

                // Use additive blending.
                //settings.SourceBlend = Blend.SourceAlpha;
                //settings.DestinationBlend = Blend.One;

                return settings;
            }
        }
        public ParticleSettings LandBoomSmoke
        {
            get
            {
                ParticleSettings settings = new ParticleSettings();

                settings.TextureName = "smoke";

                settings.State = ParticleState.LandBoomSmoke;

                settings.MaxParticles = 400;

                settings.Duration = TimeSpan.FromSeconds(4);

                settings.MinHorizontalVelocity = 0;
                settings.MaxHorizontalVelocity = 5;

                settings.MinVerticalVelocity = 10;
                settings.MaxVerticalVelocity = 25;

                // Create a wind effect by tilting the gravity vector sideways.
                settings.Gravity = new Vector3(0, 0, 0);

                settings.MinColor = new Color(255, 255, 255, 40);
                settings.MaxColor = new Color(255, 255, 255, 65);

                settings.EndVelocity = 0.2f;

                settings.MinRotateSpeed = -1;
                settings.MaxRotateSpeed = 1;

                settings.MinStartSize = 2;
                settings.MaxStartSize = 6;

                settings.MinEndSize = 10;
                settings.MaxEndSize = 26;

                return settings;
            }
        }
        public ParticleSettings StructureBoomSmoke1
        {
            get
            {
                ParticleSettings settings = new ParticleSettings();

                settings.TextureName = "smoke";

                settings.State = ParticleState.StructureBoomSmoke1;

                settings.MaxParticles = 400;

                settings.Duration = TimeSpan.FromSeconds(3);

                settings.MinHorizontalVelocity = 0;
                settings.MaxHorizontalVelocity = 5;

                settings.MinVerticalVelocity = 10;
                settings.MaxVerticalVelocity = 25;

                // Create a wind effect by tilting the gravity vector sideways.
                settings.Gravity = new Vector3(0, 0, 0);

                settings.MinColor = new Color(255, 255, 255, 60);
                settings.MaxColor = new Color(255, 255, 255, 85);

                settings.EndVelocity = 0.2f;

                settings.MinRotateSpeed = -1;
                settings.MaxRotateSpeed = 1;

                settings.MinStartSize = 2;
                settings.MaxStartSize = 6;

                settings.MinEndSize = 14;
                settings.MaxEndSize = 36;

                return settings;
            }
        }
        public ParticleSettings StructureBoomSmoke2
        {
            get
            {
                ParticleSettings settings = new ParticleSettings();

                settings.TextureName = "smoke";

                settings.State = ParticleState.StructureBoomSmoke2;

                settings.MaxParticles = 400;

                settings.Duration = TimeSpan.FromSeconds(3);

                settings.MinHorizontalVelocity = 0;
                settings.MaxHorizontalVelocity = 5;

                settings.MinVerticalVelocity = 10;
                settings.MaxVerticalVelocity = 25;

                // Create a wind effect by tilting the gravity vector sideways.
                settings.Gravity = new Vector3(0, -100, 0);

                settings.MinColor = new Color(255, 255, 255, 80);
                settings.MaxColor = new Color(255, 255, 255, 90);

                settings.EndVelocity = 0.2f;

                settings.MinRotateSpeed = -1;
                settings.MaxRotateSpeed = 1;

                settings.MinStartSize = 2;
                settings.MaxStartSize = 6;

                settings.MinEndSize = 10;
                settings.MaxEndSize = 26;

                return settings;
            }
        }
        public ParticleSettings Stone
        {
            get
            {
                ParticleSettings settings = new ParticleSettings();

                settings.TextureName = "Stone";

                settings.State = ParticleState.Stone;

                settings.MaxParticles = 400;

                settings.Duration = TimeSpan.FromSeconds(4);

                settings.MinHorizontalVelocity = 0;
                settings.MaxHorizontalVelocity = 5;

                settings.MinVerticalVelocity = 10;
                settings.MaxVerticalVelocity = 25;

                // Create a wind effect by tilting the gravity vector sideways.
                settings.Gravity = new Vector3(0, -100, 0);

                settings.MinColor = new Color(255, 255, 255, 100);
                settings.MaxColor = new Color(255, 255, 255, 100);

                settings.EndVelocity = 0.75f;

                settings.MinRotateSpeed = -10;
                settings.MaxRotateSpeed = 10;

                settings.MinStartSize = 2;
                settings.MaxStartSize = 2;

                settings.MinEndSize = 6;
                settings.MaxEndSize = 8;

                return settings;
            }
        }
        public ParticleSettings FireBoom
        {
            get
            {
                ParticleSettings settings = new ParticleSettings();

                settings.TextureName = "fire";

                settings.State = ParticleState.FireBoom;

                settings.MaxParticles = 500;

                settings.Duration = TimeSpan.FromSeconds(1.25);

                settings.DurationRandomness = 2;

                settings.MinHorizontalVelocity = 0;
                settings.MaxHorizontalVelocity = 3;

                settings.MinVerticalVelocity = -1;
                settings.MaxVerticalVelocity = 3;

                // Set gravity upside down, so the flames will 'fall' upward.
                settings.Gravity = new Vector3(0, 1, 0);

                settings.MinColor = new Color(255, 255, 255, 10);
                settings.MaxColor = new Color(255, 255, 255, 40);

                settings.MinStartSize = 4;
                settings.MaxStartSize = 10;

                settings.MinEndSize = 50;
                settings.MaxEndSize = 150;

                // Use additive blending.
                settings.SourceBlend = Blend.SourceAlpha;
                settings.DestinationBlend = Blend.One;

                return settings;
            }
        }
        public ParticleSettings Cloud
        {
            get
            {
                //Haha! In your code! Commenting your particles!
                ParticleSettings settings = new ParticleSettings();

                settings.TextureName = "Cloud";

                settings.State = ParticleState.Cloud;

                settings.MaxParticles = 50;

                settings.Duration = TimeSpan.FromSeconds(10);

                settings.DurationRandomness = 2;

                settings.MinHorizontalVelocity = 0;
                settings.MaxHorizontalVelocity = 3;

                settings.MinVerticalVelocity = -1;
                settings.MaxVerticalVelocity = 3;

                // Set gravity upside down, so the flames will 'fall' upward.
                settings.Gravity = new Vector3(0, 1, 0);

                settings.MinColor = new Color(255, 255, 255, 40);
                settings.MaxColor = new Color(255, 255, 255, 80);

                settings.MinStartSize = 560; //560
                settings.MaxStartSize = 620; //620

                settings.MinEndSize = 900; //900
                settings.MaxEndSize = 950; //950

                // Use additive blending.
             //   settings.SourceBlend = Blend.SourceAlpha;
             //   settings.DestinationBlend = Blend.One;

                return settings;
            }
        }
        public ParticleSettings FireworkStars
        {
            get
            {
                ParticleSettings settings = new ParticleSettings();

                settings.TextureName = "Star";

                settings.State = ParticleState.FireworkStars;

                settings.MaxParticles = 200;

                settings.Duration = TimeSpan.FromSeconds(1.25);
                settings.MaxHorizontalVelocity = 10;

                settings.MinVerticalVelocity = 20;
                settings.MaxVerticalVelocity = 45;

                // Create a wind effect by tilting the gravity vector sideways.
                settings.Gravity = new Vector3(0, -75, 0);

                settings.MinColor = new Color(255, 255, 255, 100);
                settings.MaxColor = Color.Red;//new Color(255, 0, 0, 100);
                

                settings.EndVelocity = 0.75f;

                //settings.MinRotateSpeed = ;
                //settings.MaxRotateSpeed = 1;

                settings.MinStartSize = 2;
                settings.MaxStartSize = 2;

                settings.MinEndSize = 2;
                settings.MaxEndSize = 2;

                return settings;
            }
        }

    }
}
