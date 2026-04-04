using Core.Data;
using UnityEngine;

namespace Core.Tokens
{
    public interface ITokenController
    {
        public void MoveActiveTokenBySteps(int steps);
    }
}