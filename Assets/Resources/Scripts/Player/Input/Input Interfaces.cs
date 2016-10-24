using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts;

namespace Assets.Resources.Scripts.Player.Input
{
    public interface IPlayerInputManager
    {
        void SetPlaying();
        IObserveSubject SetNonPlaying();    
    }

    public interface IPlayerInputController
    {
        void ReadInput();
        void ApplyInput();
    }
}
