
namespace Landlord
{
    class ViewWorld : GameState
    {
        public ViewWorld(bool generateView = false) : base()
        {
            Program.AudioEngine.PlaySound(Program.AudioEngine.CachedSoundFX["openMap"]);
            if (generateView)
                Menus.WorldMap.GenerateWorldView();
        }

        public override void Update()
        {
            InventoryPanel.HandleInventory();
            StatusPanel.HandleStatus();
        }

        public override void Render(ref SadConsole.Console console, ref Window window)
        {
            Menus.WorldMap.View();
        }

        public override void ClientSizeChanged()
        {
            Menus.WorldMap.GenerateWorldView();
        }
    }
}
