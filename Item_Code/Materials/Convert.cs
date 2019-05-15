using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{

    static class Convert
    {
        public static double GetWeightOfItem(Item item)
        {
            if (item is Potion)
                return Physics.Densities[item.Material] * (FromCubicFeetToCubicInches(item.Volume) - FromCubicFeetToCubicInches(item.Volume - item.Volume / 8))
                    + Physics.Densities[Material.Water] * FromCubicFeetToCubicInches(item.Volume - item.Volume / 9);
            else if (item is EmptyBottle)
                return Physics.Densities[item.Material] * (FromCubicFeetToCubicInches(item.Volume) - FromCubicFeetToCubicInches(item.Volume - item.Volume / 3));
            if (!item.Hollow)
                return Physics.Densities[item.Material] * FromCubicFeetToCubicInches(item.Volume);
            else
                return Physics.Densities[item.Material] * (FromCubicFeetToCubicInches(item.Volume) - FromCubicFeetToCubicInches(item.Volume - item.Volume / 8));
        }
        
        public static double FromCubicFeetToCubicInches(double cubicFeet)
        {
            return cubicFeet * 1728;
        }

        public static double GetLengthOfCube(double volume)
        {
            return FromCubicFeetToCubicInches(volume) / 3;
        }

        public static double GetImpactSurfaceAreaOfCube(double volume)
        {
            double length = GetLengthOfCube(volume);
            return 2 * (length * length + length * length + length * length) / (6);
        }

        public static double GetImpactSurfaceAreaOfSphere(double volume)
        {
            double length = GetLengthOfCube(volume);
            return 2 * (length * length + length * length + length * length) / (6 * 3.14);
        }
    }
}
