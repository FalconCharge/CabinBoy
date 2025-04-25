using System;
using System.Collections.Generic;
using UnityEngine;

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

        public float positionSpring;

        public float positionDamper;
    }
        
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

    public void ApplyAll()
    {
        if (jointConfigs == null) return;

        foreach (var cfg in jointConfigs)
        {
            if (cfg.joint == null) continue;

            var drive = cfg.joint.slerpDrive;
            drive.positionSpring = cfg.positionSpring;
            drive.positionDamper = cfg.positionDamper;
            cfg.joint.slerpDrive = drive;
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
                positionDamper = j.slerpDrive.positionDamper
            });
        }
    }
}
