using Fusion;
using UnityEngine;

namespace Network
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector2 Move;
        public float YawDelta;
        private byte _buttonsPressed;
        
        public void AddInput(NetworkButtonType inputType)
        {
            var flag = (byte)(1 << (int)inputType);
            _buttonsPressed |= flag;
        }
        
        public readonly bool IsInputDown(NetworkButtonType inputType)
        {
            var flag = (byte)(1 << (int)inputType);
            return (_buttonsPressed & flag) != 0;
        }
    }
}
