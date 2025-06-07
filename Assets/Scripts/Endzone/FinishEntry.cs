using Player;

namespace Endzone
{
    public struct FinishEntry
    {
        public string PlayerName;
        public float FinishTime;

        public FinishEntry(string playerName, float finishTime)
        {
            PlayerName = playerName;
            FinishTime = finishTime;
        }
    }
}
