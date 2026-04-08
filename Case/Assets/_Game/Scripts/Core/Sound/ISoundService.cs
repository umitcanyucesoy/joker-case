using Core.Enums;
using Service;

namespace Core.Sound
{
    public interface ISoundService : IService
    {
        void Play(SoundType type);
    }
}

