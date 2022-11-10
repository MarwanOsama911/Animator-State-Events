using System;
using UnityEngine;
using UnityEngine.Events;

namespace Components.FCMEvents
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorEventsListener : MonoBehaviour
    {
        [SerializeField] private bool debug;
        [SerializeField] private UnityEvent<string> events = new UnityEvent<string>();
        public UnityEvent<string> GetEvents => events;

        private void Awake()
        {
            events.AddListener(OnEventCalled);
        }

        private void OnEventCalled(string eventName)
        {
            if (debug)
                print(eventName);
        }
    }
}