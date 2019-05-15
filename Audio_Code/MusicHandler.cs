namespace Landlord
{
    internal class MusicHandler
    {
        string currentTrack;

        public MusicHandler ()
        {
            currentTrack = "Main_Menu";
        }

        public void Update()
        {
            DecideTrack();
        }

        public void DecideTrack()
        {
            if ( Program.CurrentState is Play ) {
                if (Program.WorldMap.LocalTile.InDungeon == false)
                {
                    if (Program.TimeHandler.CurrentTime.Hour >= 8 && Program.TimeHandler.CurrentTime.Hour < 20)
                        SetTrack( "Day1" );
                    else
                        SetTrack( "Night1" );
                } else
                {
                    SetTrack( "Dungeon1" );
                }
            }
        }

        public void SetTrack(string track)
        {
            if ( currentTrack != track ) {
                if ( currentTrack == "Main_Menu" ) {
                    Program.AudioEngine.PlayMusic( "Transition" );
                    System.Threading.Thread.Sleep( 1465 );
                }
                Program.AudioEngine.PlayMusic( track );
                currentTrack = track;
            }
        }
    }
}