using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class JointSlerpDriveController : MonoBehaviour
{
    [Serializable]
    public class JointConfig
    {
        [HideInInspector]
        public string jointName;

        [HideInInspector]
        public ConfigurableJoint joint;

        [Tooltip("Spring strength for the slerp drive")]
        public float positionSpring = 0f;

        [Tooltip("Damping value for the slerp drive")]
        public float positionDamper = 0f;

        [Tooltip("Maximum force the slerp drive can apply")]
        public float maximumForce = Mathf.Infinity;
    }

    [Tooltip("List of joints to configure")]
    public List<JointConfig> jointConfigs = new List<JointConfig>();

    void Reset()
    {
        AutoPopulate();
    }

    void Awake()
    {
        if (jointConfigs == null || jointConfigs.Count == 0)
            AutoPopulate();
        ApplyAll();
    }

    void OnValidate()
    {
        ApplyAll();
    }

    [ContextMenu("Apply Slerp Drives")]
    public void ApplyAll()
    {
        if (jointConfigs == null)
            return;

        foreach (var cfg in jointConfigs)
        {
            if (cfg.joint == null)
                continue;

            var drive = cfg.joint.slerpDrive;
            drive.positionSpring = cfg.positionSpring;
            drive.positionDamper = cfg.positionDamper;
            drive.maximumForce = cfg.maximumForce;
            cfg.joint.slerpDrive = drive;

#if UNITY_EDITOR
            EditorUtility.SetDirty(cfg.joint);
#endif
        }
    }

    public void AutoPopulate()
    {
        jointConfigs.Clear();
        var allJoints = GetComponentsInChildren<ConfigurableJoint>(true);
        foreach (var j in allJoints)
        {
            jointConfigs.Add(new JointConfig
            {
                jointName = j.gameObject.name,
                joint = j,
                positionSpring = j.slerpDrive.positionSpring,
                positionDamper = j.slerpDrive.positionDamper,
                maximumForce = j.slerpDrive.maximumForce,
            });
        }
    }
}
