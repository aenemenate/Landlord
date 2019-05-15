using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Landlord
{
    public enum Material
    {
        Wood,
        Dirt,
        Stone,
        Tin,
        Zinc,
        Copper,
        Bronze,
        Brass,
        Iron,
        Steel,
        Platinum,
        Glass,
        Leather,
        Cloth,
        Silk,
        Bone,
        Water,
        Coal,
        Null
    }

    public static class Physics
    {
        // calculated as pound per cubic inch
        private static Dictionary<Material, double> densities = new Dictionary<Material, double>();
        private static Dictionary<Material, string> materialNames = new Dictionary<Material, string>();
        private static Dictionary<Material, Color> materialColors = new Dictionary<Material, Color>();

        private static Dictionary<Material, double> impactYields = new Dictionary<Material, double>();
        private static Dictionary<Material, double> impactFractures = new Dictionary<Material, double>();

        private static Dictionary<Material, double> shearYields = new Dictionary<Material, double>();
        private static Dictionary<Material, double> shearFractures = new Dictionary<Material, double>();

        private static Dictionary<Rarity, double> rarityMultipliers = new Dictionary<Rarity, double>();


        // INIT FUNCTIONS

        public static void InitializePhysics()
        {
            InitializeDensities();
            InitializeImpactYields();
            InitializeImpactFractures();
            InitializeShearFractures();
            InitializeShearYields();
            InitializeRarityMultipliers();
            InitializeMaterialNames();
            InitializeMaterialColors();
        }

        public static void InitializeRarityMultipliers()
        {
            rarityMultipliers.Add(Rarity.Common, 0.7);
            rarityMultipliers.Add(Rarity.Uncommon, 1);
            rarityMultipliers.Add(Rarity.Rare, 1.4);
            rarityMultipliers.Add(Rarity.Legendary, 1.8);
        }

        private static void InitializeDensities()
        {
            densities.Add(Material.Wood, 0.026);
            densities.Add(Material.Tin, 0.26);
            densities.Add(Material.Zinc, 0.258);
            densities.Add( Material.Coal, 0.05 );
            densities.Add( Material.Stone, 0.09 );
            densities.Add(Material.Copper, 0.323);
            densities.Add(Material.Bronze, 0.34);
            densities.Add(Material.Brass, 0.31);
            densities.Add(Material.Iron, 0.28);
            densities.Add(Material.Steel, 0.28);
            densities.Add(Material.Platinum, 0.77);
            densities.Add(Material.Glass, 0.086);
            densities.Add(Material.Leather, 0.031);
            densities.Add(Material.Cloth, 0.014);
            densities.Add(Material.Silk, 0.009);
            densities.Add(Material.Bone, 0.052);
            densities.Add(Material.Water, 0.036);
        }

        private static void InitializeImpactYields()
        {
            impactYields.Add(Material.Cloth, 1);

            impactYields.Add(Material.Glass, 1);
            impactYields.Add(Material.Leather, 2);
            impactYields.Add(Material.Bone, 8);
            impactYields.Add( Material.Coal, 8 );
            impactYields.Add( Material.Stone, 13 );

            impactYields.Add(Material.Copper, 20);
            impactYields.Add(Material.Brass, 30);
            impactYields.Add(Material.Bronze, 40);
            impactYields.Add(Material.Iron, 50);
            impactYields.Add(Material.Steel, 60);
            impactYields.Add(Material.Platinum, 80);
        }

        private static void InitializeImpactFractures()
        {
            impactFractures.Add(Material.Cloth, 250);

            impactFractures.Add(Material.Glass, 12);
            impactFractures.Add(Material.Bone, 50);
            impactFractures.Add(Material.Leather, 125);
            impactFractures.Add( Material.Stone, 200 );
            impactFractures.Add( Material.Coal, 200 );

            impactFractures.Add(Material.Copper, 250);
            impactFractures.Add(Material.Brass, 300);
            impactFractures.Add(Material.Bronze, 350);
            impactFractures.Add(Material.Iron, 400);
            impactFractures.Add(Material.Steel, 500);
            impactFractures.Add(Material.Platinum, 600);
        }

        private static void InitializeShearYields()
        {
            shearYields.Add(Material.Cloth, 1);

            shearYields.Add(Material.Leather, 5);
            shearYields.Add(Material.Bone, 10);
            shearYields.Add( Material.Coal, 13 );
            shearYields.Add(Material.Stone, 16);
            shearYields.Add(Material.Glass, 20);

            shearYields.Add(Material.Copper, 21);
            shearYields.Add(Material.Brass, 26);
            shearYields.Add(Material.Bronze, 31);
            shearYields.Add(Material.Iron, 37);
            shearYields.Add(Material.Steel, 43);
            shearYields.Add(Material.Platinum, 51);
        }

        private static void InitializeShearFractures()
        {
            shearFractures.Add(Material.Cloth, 1);

            shearFractures.Add(Material.Leather, 25);
            shearFractures.Add(Material.Bone, 75);
            shearFractures.Add( Material.Coal, 100 );
            shearFractures.Add( Material.Stone, 100 );
            shearFractures.Add(Material.Glass, 500);

            shearFractures.Add(Material.Copper, 100);
            shearFractures.Add(Material.Brass, 150);
            shearFractures.Add(Material.Bronze, 200);
            shearFractures.Add(Material.Iron, 300);
            shearFractures.Add(Material.Steel, 400);
            shearFractures.Add(Material.Platinum, 500);
        }

        private static void InitializeMaterialNames()
        {
            materialNames.Add( Material.Wood, "wood" );
            materialNames.Add( Material.Dirt, "dirt" );
            materialNames.Add( Material.Coal, "coal" );
            materialNames.Add( Material.Stone, "stone" );
            materialNames.Add( Material.Tin, "tin" );
            materialNames.Add( Material.Zinc, "zinc" );
            materialNames.Add( Material.Copper, "copper" );
            materialNames.Add( Material.Bronze, "bronze" );
            materialNames.Add( Material.Brass, "brass" );
            materialNames.Add( Material.Iron, "iron" );
            materialNames.Add( Material.Steel, "steel" );
            materialNames.Add( Material.Platinum, "platinum" );
            materialNames.Add( Material.Glass, "glass" );
            materialNames.Add( Material.Leather, "leather" );
            materialNames.Add( Material.Cloth, "cloth" );
            materialNames.Add( Material.Silk, "silk" );
            materialNames.Add( Material.Bone, "bone" );
            materialNames.Add( Material.Water, "water" );
        }

        private static void InitializeMaterialColors()
        {
            materialColors.Add( Material.Wood, new Color( 130, 82, 1 ) );
            materialColors.Add( Material.Dirt, new Color( 155, 118, 83 ) );
            materialColors.Add( Material.Stone, Color.Gray );
            materialColors.Add( Material.Coal, Color.DarkSlateGray );
            materialColors.Add( Material.Tin, new Color( 211, 212, 213 ) );
            materialColors.Add( Material.Zinc, new Color( 186, 196, 200 ) );
            materialColors.Add( Material.Copper, new Color( 184, 115, 51 ) );
            materialColors.Add( Material.Bronze, new Color( 205, 127, 50 ) );
            materialColors.Add( Material.Brass, new Color( 181, 166, 66 ) );
            materialColors.Add( Material.Iron, new Color( 203, 205, 205 ) );
            materialColors.Add( Material.Steel, new Color( 224, 223, 219 ) );
            materialColors.Add( Material.Platinum, new Color( 229, 228, 226 ) );
            materialColors.Add( Material.Glass, new Color( 201, 219, 220 ) );
            materialColors.Add( Material.Leather, new Color( 122, 92, 57 ) );
            materialColors.Add( Material.Cloth, new Color( 253, 243, 234 ) );
            materialColors.Add( Material.Silk, new Color( 255, 248, 220 ) );
            materialColors.Add( Material.Bone, new Color( 227, 218, 201 ) );
            materialColors.Add( Material.Water, new Color( 64, 164, 223 ) );
        }

        
        public static List<Material> GetArmorSkillMaterials (Skill skill)
        {
            switch (skill)
            {
                case Skill.HeavyArmor:
                    return new List<Material>() { Material.Stone, Material.Copper, Material.Bronze, Material.Brass, Material.Iron, Material.Steel, Material.Platinum };
                case Skill.LightArmor:
                    return new List<Material>() { Material.Leather, Material.Bone, Material.Wood, Material.Glass, Material.Silk };
                default:
                    return new List<Material>();
            }
        }
        

        // PARAMETERS

        public static Dictionary<Material, double> Densities
        {
            get { return densities; }
        }

        public static Dictionary<Material, double> ImpactYields
        {
            get { return impactYields; }
        }

        public static Dictionary<Material, double> ImpactFractures
        {
            get { return impactFractures; }
        }

        public static Dictionary<Material, double> ShearYields
        {
            get { return shearYields; }
        }

        public static Dictionary<Material, double> ShearFractures
        {
            get { return shearFractures; }
        }

        public static Dictionary<Rarity, double> RarityMultipliers
        {
            get { return rarityMultipliers; }
        }

        public static Dictionary<Material, string> MaterialNames
        {
            get { return materialNames; }
        }

        public static Dictionary<Material, Color> MaterialColors
        {
            get { return materialColors; }
            set { materialColors = value; }
        }
    }
}
