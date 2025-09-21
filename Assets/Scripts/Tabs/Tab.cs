using System;
using UnityEngine;

namespace Tabs
{
    public abstract class Tab : MonoBehaviour
    {
        public event Action OnTabOpened;
        public event Action OnTabClosed;

        public virtual void OpenTab()
        {
            OnTabOpened?.Invoke();
        }
        
        public virtual void CloseTab()
        {
            OnTabClosed?.Invoke();
        }
    }
}
