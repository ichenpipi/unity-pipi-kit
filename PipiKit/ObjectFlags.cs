#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace PipiKit
{

    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class ObjectFlags : MonoBehaviour
    {

        [Header("Self Components (Excluding 'ObjectFlags' Component)")]
        [SerializeField]
        public bool applySelf = false;

        [SerializeField]
        public bool notEditableSelf = false;

        [SerializeField]
        public bool hideInInspectorSelf = false;

        [Header("Children GameObject")]
        [SerializeField]
        public bool applyChildren = false;

        [SerializeField]
        public bool recursive = false;

        [SerializeField]
        public bool notEditable = false;

        [SerializeField]
        public bool hideInHierarchy = false;

        [SerializeField]
        public bool hideInInspector = false;

        [Header("Child Filtering")]
        [SerializeField]
        public bool enableChildNameFilter = false;

        [SerializeField]
        public bool skipFilteredRecursive = false;

        [SerializeField]
        public List<string> excludeChildNameList = new List<string>();

        private void OnEnable()
        {
            UpdateSelfFlags();
            UpdateChildrenFlags();
        }

        private void OnDisable()
        {
            ResetSelfFlags();
            ResetChildrenFlags();
        }

        public void OnValidate()
        {
            UpdateSelfFlags();
            UpdateChildrenFlags();
        }

        private void UpdateSelfFlags()
        {
            HideFlags flags = HideFlags.None;
            if (applySelf && notEditableSelf) flags |= HideFlags.NotEditable;
            if (applySelf && hideInInspectorSelf) flags |= HideFlags.HideInInspector;

            foreach (Component c in GetComponents<Component>())
            {
                if (c == this) continue;
                c.hideFlags = flags;
            }
        }

        private void ResetSelfFlags()
        {
            foreach (Component c in GetComponents<Component>())
            {
                if (c == this) continue;
                c.hideFlags = HideFlags.None;
            }
        }

        public void UpdateChildrenFlags()
        {
            HideFlags flags = HideFlags.None;
            if (applyChildren && notEditable) flags |= HideFlags.NotEditable;
            if (applyChildren && hideInHierarchy) flags |= HideFlags.HideInHierarchy;
            if (applyChildren && hideInInspector) flags |= HideFlags.HideInInspector;

            ResetChildrenFlags();

            if (flags == HideFlags.None) return;

            WalkTransform(transform,
                (t =>
                {
                    // ?????? ObjectFlags ?????????????????????
                    if (t.GetComponent<ObjectFlags>() != null)
                    {
                        return false;
                    }
                    // ??????????????????????????????????????????????????????????????????
                    if (enableChildNameFilter && excludeChildNameList.Contains(t.name) && skipFilteredRecursive)
                    {
                        return false;
                    }
                    // ????????????
                    if (!enableChildNameFilter || !excludeChildNameList.Contains(t.name))
                    {
                        // ?????? Flags
                        t.gameObject.hideFlags = flags;
                    }
                    // ???????????????
                    return recursive;
                })
            );
        }

        private void ResetChildrenFlags()
        {
            WalkTransform(transform,
                (t =>
                {
                    // ?????? ObjectFlags ?????????????????????
                    if (t.GetComponent<ObjectFlags>() != null)
                    {
                        return false;
                    }
                    // ??????????????????????????????????????????????????????????????????
                    if (enableChildNameFilter && excludeChildNameList.Contains(t.name) && skipFilteredRecursive)
                    {
                        return false;
                    }
                    // ????????????
                    if (!enableChildNameFilter || !excludeChildNameList.Contains(t.name))
                    {
                        // ?????? Flags
                        t.gameObject.hideFlags = HideFlags.None;
                    }
                    // ???????????????
                    return true;
                })
            );
        }

        private static void WalkTransform(Transform transform, Func<Transform, bool> executor)
        {
            foreach (Transform t in transform)
            {
                if (executor(t))
                {
                    WalkTransform(t, executor);
                }
            }
        }

    }

}

#endif
