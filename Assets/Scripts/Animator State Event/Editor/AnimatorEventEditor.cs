using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Components.FCMEvents
{
    [CustomEditor(typeof(AnimatorEvent))]
    public class AnimatorEventEditor : Editor
    {
        private SerializedProperty _totalFrames;
        private SerializedProperty _currentFrames;
        private SerializedProperty _normalizedTime;
        private SerializedProperty _normalizedTimeUncapped;
        private SerializedProperty _motionTime;
        private SerializedProperty _events;

        private ReorderableList _eventsList;

        private void OnEnable()
        {
            _totalFrames = serializedObject.FindProperty("totalFrames");
            _currentFrames = serializedObject.FindProperty("currentFrames");
            _normalizedTime = serializedObject.FindProperty("normalizedTime");
            _normalizedTimeUncapped = serializedObject.FindProperty("normalizedTimeUncapped");
            _motionTime = serializedObject.FindProperty("motionTime");
            _events = serializedObject.FindProperty("events");
            _eventsList = new ReorderableList(serializedObject, _events, true, true, true, true)
            {
                drawHeaderCallback = DrawHeaderCallback,
                drawElementCallback = DrawElementCallback,
                elementHeightCallback = ElementHeightCallback
            };
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.IndentLevelScope(1))
            {
                using (new EditorGUI.DisabledGroupScope(true))
                {
                    EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((AnimatorEvent)target),
                        typeof(AnimatorEvent), false);
                    EditorGUILayout.PropertyField(_totalFrames);
                    EditorGUILayout.PropertyField(_currentFrames);
                    EditorGUILayout.PropertyField(_normalizedTime);
                    EditorGUILayout.PropertyField(_normalizedTimeUncapped);
                }

                EditorGUILayout.PropertyField(_motionTime);
            }

            _eventsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        #region Events List Drawer

        private void DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Events");
        }


        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _eventsList.serializedProperty.GetArrayElementAtIndex(index);
            var eventName = element.FindPropertyRelative("eventName");
            var timing = element.FindPropertyRelative("timing");


            var timingIndex = timing.enumValueIndex;

            var elementTitle = string.IsNullOrEmpty(eventName.stringValue)
                ? $"Event *Name* ({timing.enumDisplayNames[index]})"
                : $"Event {eventName.stringValue} ({timing.enumDisplayNames[timingIndex]})";

            EditorGUI.PropertyField(rect, element, new GUIContent(elementTitle), true);
        }

        private float ElementHeightCallback(int index)
        {
            var element = _eventsList.serializedProperty.GetArrayElementAtIndex(index);
            var propertyHeight = EditorGUI.GetPropertyHeight(element, true);
            return propertyHeight;
        }

        #endregion
    }
}