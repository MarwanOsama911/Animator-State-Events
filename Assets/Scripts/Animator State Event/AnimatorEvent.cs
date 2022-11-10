using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components.FCMEvents
{
    public enum FcmTiming
    {
        OnEnter,
        OnExit,
        OnUpdate,
        OnEnd
    }

    public class AnimatorEvent : StateMachineBehaviour
    {
        [Serializable]
        public class FcmEventDetails
        {
            public bool fired;
            public string eventName;
            public FcmTiming timing;
            public float onUpdateFrame = 1;
        }

        [SerializeField] private int totalFrames;
        [SerializeField] private int currentFrames;
        [SerializeField] private float normalizedTime;
        [SerializeField] private float normalizedTimeUncapped;
        [SerializeField] private string motionTime = "";

        public List<FcmEventDetails> events = new List<FcmEventDetails>();
        private bool _hasParam;
        private AnimatorEventsListener _eventsListener;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _hasParam = HasParameter(animator, motionTime);
            _eventsListener = animator.GetComponent<AnimatorEventsListener>();
            totalFrames = GetTotalFrames(animator, layerIndex);

            normalizedTimeUncapped = stateInfo.normalizedTime;
            normalizedTime = _hasParam ? animator.GetFloat(motionTime) : GetNormalizedTime(stateInfo);
            currentFrames = GetCurrentFrame(totalFrames, normalizedTime);

            if (_eventsListener != null)
            {
                foreach (FcmEventDetails fcmEvent in events)
                {
                    fcmEvent.fired = false;
                    if (fcmEvent.timing == FcmTiming.OnEnter)
                    {
                        fcmEvent.fired = true;
                        _eventsListener.GetEvents.Invoke(fcmEvent.eventName);
                    }
                }
            }
        }


        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            normalizedTimeUncapped = stateInfo.normalizedTime;
            normalizedTime = _hasParam ? animator.GetFloat(motionTime) : GetNormalizedTime(stateInfo);
            currentFrames = GetCurrentFrame(totalFrames, normalizedTime);

            if (_eventsListener != null)
            {
                foreach (FcmEventDetails fcmEvent in events)
                {
                    if (!fcmEvent.fired)
                    {
                        if (fcmEvent.timing == FcmTiming.OnUpdate)
                        {
                            if (currentFrames >= fcmEvent.onUpdateFrame)
                            {
                                fcmEvent.fired = true;
                                _eventsListener.GetEvents.Invoke(fcmEvent.eventName);
                            }
                        }
                        else if (fcmEvent.timing == FcmTiming.OnEnd)
                        {
                            if (currentFrames >= totalFrames)
                            {
                                fcmEvent.fired = true;
                                _eventsListener.GetEvents.Invoke(fcmEvent.eventName);
                            }
                        }
                    }
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_eventsListener != null)
            {
                foreach (FcmEventDetails fcmEvent in events)
                {
                    fcmEvent.fired = false;
                    if (fcmEvent.timing == FcmTiming.OnExit)
                    {
                        fcmEvent.fired = true;
                        _eventsListener.GetEvents.Invoke(fcmEvent.eventName);
                    }
                }
            }
        }

        private bool HasParameter(Animator animator, string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName) || string.IsNullOrWhiteSpace(parameterName))
            {
                return false;
            }

            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.name == parameterName)
                {
                    return true;
                }
            }

            return false;
        }

        private int GetTotalFrames(Animator animator, int layerIndex)
        {
            AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(layerIndex);
            if (clipInfos.Length == 0)
            {
                clipInfos = animator.GetCurrentAnimatorClipInfo(layerIndex);
            }

            AnimationClip clip = clipInfos[0].clip;
            return Mathf.RoundToInt(clip.length * clip.frameRate);
        }

        private float GetNormalizedTime(AnimatorStateInfo stateInfo)
        {
            return stateInfo.normalizedTime > 1 ? 1 : stateInfo.normalizedTime;
        }


        private int GetCurrentFrame(int totalFramesParam, float normalizedTimeParam)
        {
            return Mathf.RoundToInt(totalFramesParam * normalizedTimeParam);
        }
    }
}