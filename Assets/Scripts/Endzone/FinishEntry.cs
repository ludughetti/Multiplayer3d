using Player;

namespace Endzone
{
    public struct FinishEntry
    {
        public PlayerController Player;
        public float FinishTime;

        public FinishEntry(PlayerController player, float finishTime)
        {
            Player = player;
            FinishTime = finishTime;
        }
    }
}
