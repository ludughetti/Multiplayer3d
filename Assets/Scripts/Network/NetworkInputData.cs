using Fusion;
using UnityEngine;

namespace Network
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector2 Move;
        public float YawDelta;
    }
}
