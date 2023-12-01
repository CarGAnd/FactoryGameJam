using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace SOS
{
[CreateAssetMenu(fileName = "SOSOverview", menuName = "SOS/Overview", order = 2)]
[InlineEditor]
    public class SOSOverview : ScriptableObject
    {
        [SerializeField]
        [FoldoutGroup("DefaultRefs", expanded: true)]
        [HorizontalGroup("DefaultRefs/Row1", LabelWidth = 200)]
        [VerticalGroup("DefaultRefs/Row1/Left")]
        [OnInspectorInit("GetAllFloats")]
        [ReadOnly]
        [ListDrawerSettings(OnBeginListElementGUI = "BeginDrawListElementFloat", OnEndListElementGUI = "EndDrawListElement", HideRemoveButton = true, DraggableItems = true, DefaultExpandedState = true, HideAddButton = true)]
        List<FloatRef> overViewFloats;

        private void GetAllFloats() {
            List<ScriptableFloat> scriptableFloats = AssetUtilities.GetAllAssetsOfType<ScriptableFloat>().ToList();
            List<FloatRef> floatRefs = new List<FloatRef>();

            foreach (ScriptableFloat scriptableObject in scriptableFloats) {
                if (scriptableObject == null)
                    return;

                FloatRef floatRef = new FloatRef();
                floatRef.variable = scriptableObject;
                floatRefs.Add(floatRef);
            }
            overViewFloats = floatRefs;
        }

        private void BeginDrawListElementFloat(int index)
        {
            SirenixEditorGUI.BeginBox(overViewFloats[index].variable.name);
        }

        [SerializeField]
        [VerticalGroup("DefaultRefs/Row1/Right")]
        [OnInspectorInit("GetAllBools")]
        [ReadOnly]
        [ListDrawerSettings(OnBeginListElementGUI = "BeginDrawListElementBool", OnEndListElementGUI = "EndDrawListElement", HideRemoveButton = true, DraggableItems = true, DefaultExpandedState = true, HideAddButton = true)]
        List<BoolRef> overViewBools;

        private void GetAllBools() {
            List<ScriptableBool> scriptableObjects = AssetUtilities.GetAllAssetsOfType<ScriptableBool>().ToList();
            List<BoolRef> objectRefs = new List<BoolRef>();

            foreach (ScriptableBool scriptableObject in scriptableObjects) {
                if (scriptableObject == null)
                    return;

                BoolRef objectRef = new BoolRef();
                objectRef.variable = scriptableObject;
                objectRefs.Add(objectRef);
            }
            overViewBools = objectRefs;
        }

        private void BeginDrawListElementBool(int index)
        {
            SirenixEditorGUI.BeginBox(overViewBools[index].variable.name);
        }

        [SerializeField]
        [VerticalGroup("DefaultRefs/Row1/Right")]
        [OnInspectorInit("GetAllInts")]
        [ReadOnly]
        [ListDrawerSettings(OnBeginListElementGUI = "BeginDrawListElementInt", OnEndListElementGUI = "EndDrawListElement", HideRemoveButton = true, DraggableItems = true, DefaultExpandedState = true, HideAddButton = true)]
        List<IntRef> overViewInts;

        private void GetAllInts() {
            List<ScriptableInt> scriptableObjects = AssetUtilities.GetAllAssetsOfType<ScriptableInt>().ToList();
            List<IntRef> objectRefs = new List<IntRef>();

            foreach (ScriptableInt scriptableObject in scriptableObjects) {
                if (scriptableObject == null)
                    return;

                IntRef objectRef = new IntRef();
                objectRef.variable = scriptableObject;
                objectRefs.Add(objectRef);
            }
            overViewInts = objectRefs;
        }

        private void BeginDrawListElementInt(int index)
        {
            SirenixEditorGUI.BeginBox(overViewInts[index].variable.name);
        }

        [SerializeField]
        [VerticalGroup("DefaultRefs/Row1/Right")]
        [OnInspectorInit("GetAllQuaternions")]
        [ReadOnly]
        [ListDrawerSettings(OnBeginListElementGUI = "BeginDrawListElementQuaternion", OnEndListElementGUI = "EndDrawListElement", HideRemoveButton = true, DraggableItems = true, DefaultExpandedState = true, HideAddButton = true)]
        List<QuaternionRef> overViewQuaternions;

        private void GetAllQuaternions() {
            List<ScriptableQuaternion> scriptableObjects = AssetUtilities.GetAllAssetsOfType<ScriptableQuaternion>().ToList();
            List<QuaternionRef> objectRefs = new List<QuaternionRef>();

            foreach (ScriptableQuaternion scriptableObject in scriptableObjects) {
                if (scriptableObject == null)
                    return;

                QuaternionRef objectRef = new QuaternionRef();
                objectRef.variable = scriptableObject;
                objectRefs.Add(objectRef);
            }
            overViewQuaternions = objectRefs;
        }

        private void BeginDrawListElementQuaternion(int index)
        {
            SirenixEditorGUI.BeginBox(overViewQuaternions[index].variable.name);
        }

        [SerializeField]
        [VerticalGroup("DefaultRefs/Row1/Left")]
        [OnInspectorInit("GetAllVector2s")]
        [ReadOnly]
        [ListDrawerSettings(OnBeginListElementGUI = "BeginDrawListElementVector2", OnEndListElementGUI = "EndDrawListElement", HideRemoveButton = true, DraggableItems = true, DefaultExpandedState = true, HideAddButton = true)]
        List<Vector2Ref> overViewVector2s;

        private void GetAllVector2s() {
            List<ScriptableVector2> scriptableObjects = AssetUtilities.GetAllAssetsOfType<ScriptableVector2>().ToList();
            List<Vector2Ref> objectRefs = new List<Vector2Ref>();

            foreach (ScriptableVector2 scriptableObject in scriptableObjects) {
                if (scriptableObject == null)
                    return;

                Vector2Ref objectRef = new Vector2Ref();
                objectRef.variable = scriptableObject;
                objectRefs.Add(objectRef);
            }
            overViewVector2s = objectRefs;
        }

        private void BeginDrawListElementVector2(int index)
        {
            SirenixEditorGUI.BeginBox(overViewVector2s[index].variable.name);
        }

        [SerializeField]
        [VerticalGroup("DefaultRefs/Row1/Left")]
        [OnInspectorInit("GetAllVector3s")]
        [ReadOnly]
        [ListDrawerSettings(OnBeginListElementGUI = "BeginDrawListElementVector3", OnEndListElementGUI = "EndDrawListElement", HideRemoveButton = true, DraggableItems = true, DefaultExpandedState = true, HideAddButton = true)]
        List<Vector3Ref> overViewVector3s;

        private void GetAllVector3s() {
            List<ScriptableVector3> scriptableObjects = AssetUtilities.GetAllAssetsOfType<ScriptableVector3>().ToList();
            List<Vector3Ref> objectRefs = new List<Vector3Ref>();

            foreach (ScriptableVector3 scriptableObject in scriptableObjects) {
                if (scriptableObject == null)
                    return;

                Vector3Ref objectRef = new Vector3Ref();
                objectRef.variable = scriptableObject;
                objectRefs.Add(objectRef);
            }
            overViewVector3s = objectRefs;
        }

        private void BeginDrawListElementVector3(int index)
        {
            SirenixEditorGUI.BeginBox(overViewVector3s[index].variable.name);
        }

        [SerializeField]
        [VerticalGroup("DefaultRefs/Row1/Right")]
        [OnInspectorInit("GetAllGameEvents")]
        [ReadOnly]
        [ListDrawerSettings(OnBeginListElementGUI = "BeginDrawListElementGameEvent", OnEndListElementGUI = "EndDrawListElement", HideRemoveButton = true, DraggableItems = true, DefaultExpandedState = true, HideAddButton = true)]
        List<GameEvent> gameEvents;

        private void GetAllGameEvents() {
            List<ScriptableGameEvent> scriptableObjects = AssetUtilities.GetAllAssetsOfType<ScriptableGameEvent>().ToList();
            List<GameEvent> objectRefs = new List<GameEvent>();

            foreach (ScriptableGameEvent scriptableObject in scriptableObjects) {
                if (scriptableObject == null)
                    return;

                GameEvent objectRef = new GameEvent();
                objectRef.variable = scriptableObject;
                objectRefs.Add(objectRef);
            }
            gameEvents = objectRefs;
        }

        private void BeginDrawListElementGameEvent(int index)
        {
            SirenixEditorGUI.BeginBox(gameEvents[index].variable.name);
        }

        private void EndDrawListElement(int index)
        {
            SirenixEditorGUI.EndBox();
        }
    }
}