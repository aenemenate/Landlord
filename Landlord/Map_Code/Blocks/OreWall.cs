
namespace Landlord
{
    class OreWall : Wall
    {
        public OreWall(Material material, byte graphic = 178, BlockType type = BlockType.OreWall, string name = "ore", bool solid = true, bool opaque = true, bool interactive = true, bool enterable = false)
            : base(material, graphic, type, name, solid, opaque, interactive, enterable)
        {
            HP = (int)Physics.ImpactFractures[Material.Coal];
        }

        public OreWall() : base()
        {
        }
        public override void Activate(Creature user)
        {
            user.PickWall(this);
        }
    }
}
