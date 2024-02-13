using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace EyE.Unity.UI.Templates
{
    [RequireComponent(typeof(Selectable))]
    public class SelectTrigger : MonoBehaviour, ISelectHandler, ITriggerOnSelect
    {
        UnityEvent _onSelectEvent= new UnityEvent();
        public UnityEvent onSelectEvent => _onSelectEvent;

        public void OnSelect(BaseEventData eventData)
        {
            onSelectEvent.Invoke();
        }
    }
}