

using UnityEngine;

namespace RPG.Core
{
    
    public class ActionScheduler: MonoBehaviour
    {
        private MonoBehaviour _currentAction;
        public void StartAction(MonoBehaviour action)
        {
            if(action == _currentAction) {return;}
            if (_currentAction != null)
            {
                print($"Cancel: {_currentAction} and Start: {action}");
            }
            _currentAction = action;
        }
    }
}