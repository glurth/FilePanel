using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace EyE.Unity.UI.Example
{

    /// <summary>
    /// Convenience class providing UnityEvents to handle common UI conditions. Attach this script to a UI element to enable response to various UI events via UnityEvent subscriptions.
    /// </summary>
    /// <remarks>
    /// This class get input from Unity by implementing IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IPointerClickHandler
    /// It allows the user to subscribe to it's events which will be invoked when triggered by unity.  
    /// Users may use GetComponent on any of the trigger interfaces it implements (ITriggerOnSelect, ITriggerOnHover, ITriggerOnClick) to find this component
    /// </remarks>
    [RequireComponent(typeof(Selectable))]
    public class UITriggers : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IPointerClickHandler, ITriggerOnSelect, ITriggerOnHover, ITriggerOnClick
    {
        /// <summary>
        /// Event triggered when the UI element is selected.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to perform actions when the UI element is selected.
        /// </remarks>
        public UnityEvent onSelectEvent { get; } = new UnityEvent();

        /// <summary>
        /// Event triggered when the pointer enters the UI element.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to perform actions when the pointer enters the UI element.
        /// </remarks>
        public UnityEvent onPointerEnterEvent { get; } = new UnityEvent();

        /// <summary>
        /// Event triggered when the pointer exits the UI element.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to perform actions when the pointer exits the UI element.
        /// </remarks>
        public UnityEvent onPointerExitEvent { get; } = new UnityEvent();

        /// <summary>
        /// Event triggered when the UI element is clicked.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to perform actions when the UI element is clicked.
        /// </remarks>
        public UnityEvent onClickEvent { get; } = new UnityEvent();

        /// <summary>
        /// Implementation of IPointerClickHandler. Called by Unity when the UI element is clicked.
        /// </summary>
        /// <param name="eventData">Event data associated with the pointer event.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            onClickEvent.Invoke();
        }

        /// <summary>
        /// Implementation of IPointerEnterHandler. Called by Unity when the pointer enters the UI element.
        /// </summary>
        /// <param name="eventData">Event data associated with the pointer event.</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnterEvent.Invoke();
        }

        /// <summary>
        /// Implementation of IPointerExitsHandler. Called by Unity when the pointer exits the UI element.
        /// </summary>
        /// <param name="eventData">Event data associated with the pointer event.</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExitEvent.Invoke();
        }

        /// <summary>
        /// Implementation of ISelectHandler. Called by Unity when the UI selectable component on the same GameObject is selected.
        /// </summary>
        /// <param name="eventData">Event data associated with the selection event.</param>
        public void OnSelect(BaseEventData eventData)
        {
            onSelectEvent.Invoke();
        }
    }


}