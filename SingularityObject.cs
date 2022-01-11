﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime;
using UnityEngine;
using UnityEngine.Rendering;

namespace Singularity
{
    public class SingularityObject : MonoBehaviour
    {
        public string name;

        public float gravity = 1f;

        public bool hideCelestialBody = true;

        public bool useAccretionDisk = false;
        public bool useRadialTextureMapping = false;

        public Vector3 accretionDiskNormal = Vector3.up;
        public float accretionDiskInnerRadius = 1f;
        public float accretionDiskOuterRadius = 5f;

        public float accretionDiskRotationSpeed = 60f;

        public string wormholeTarget = "";
        public string accretionDiskTexturePath = "";

        public float scaleEnclosingMesh = 1f;

        float scaledRadius = 1f;
        float enclosingMeshRadius = 1f;

        Material singularityMaterial;
        Texture2D AccretionDiskTexture;

        MeshRenderer scaledPlanetMeshRenderer;

        GameObject singularityGO;
        public SingularityCenteredCubeMap singularityCubeMap;

        bool hasWormhole = false;
        SingularityCenteredCubeMap wormholeCubeMap;

        public SingularityObject()
        {

        }

        public void Awake()
        {
            //setthisup
            singularityMaterial = FindObjectOfType<Helper>().singularityMaterial;

            scaledRadius = Mathf.Sqrt(Mathf.Max(gravity, 0f)) * 5f;         // The apparent radius (in scaled Space) of the black hole (or event horizon), not physically correct
            singularityMaterial.SetFloat("blackHoleRadius", scaledRadius);

            enclosingMeshRadius = scaleEnclosingMesh * Mathf.Sqrt(Mathf.Abs(gravity)) * 120f;   // The radius (in scaled Space) at which the gravity no longer warps the image
                                                                                                // Serves as the radius of our enclosing mesh, value finetuned manually

            singularityMaterial.SetFloat("gravity", gravity);
            singularityMaterial.renderQueue = 2999; //same renderqueue as scatterer sky, so it can render below or on top of it, depending on which is in front, EVE clouds are handled by depth-testing 

            singularityMaterial.EnableKeyword("GALAXYCUBEMAPONLY_OFF");
            singularityMaterial.DisableKeyword("GALAXYCUBEMAPONLY_ON");

            ConfigureAccretionDisk();

            scaledPlanetMeshRenderer = gameObject.GetComponent<MeshRenderer>();


            //			// When not hiding the celestialBody, objects write to depth buffer which messes up the lensing, try to disable it through renderType Tags
            //			// But it's not just the depth, we also need to disable the actual object when pre-rendering the screen
            //			// Didn't work, but since we shouldn't have planets that close to stars, whatever
            //			if (!ReferenceEquals (scaledPlanetMeshRenderer, null) && !ReferenceEquals (scaledPlanetMeshRenderer.material, null))
            //			{
            //				scaledPlanetMeshRenderer.material.SetOverrideTag ("RenderType", "Transparent")
            //			}

            if (hideCelestialBody)
            {
                HideCelestialBody();
            }

            SetupGameObject();

            singularityMaterial.SetTexture("CubeMap", Singularity.Instance.galaxyCubemap);
            singularityMaterial.SetTexture("screenBuffer", Singularity.Instance.screenBuffer);

            StartCoroutine(SetupWormhole());
        }

        void ConfigureAccretionDisk()
        {
            singularityMaterial.DisableKeyword("ACCRETION_DISK_ON");
            singularityMaterial.EnableKeyword("ACCRETION_DISK_OFF");

            if (useAccretionDisk)
            {
                if (String.IsNullOrEmpty(accretionDiskTexturePath))
                {
                    Debug.Log("Accretion disk enabled but no accretion disk texture, disabling accretion disk");
                    useAccretionDisk = false;
                    return;
                }

                if (!System.IO.File.Exists(Singularity.Instance.GameDataPath + accretionDiskTexturePath))
                {
                    Debug.Log("Accretion disk enabled but texture can't be located at: " + accretionDiskTexturePath + ", disabling accretion disk");
                    useAccretionDisk = false;
                    return;
                }

                AccretionDiskTexture = new Texture2D(1, 1);
                AccretionDiskTexture.LoadImage(System.IO.File.ReadAllBytes(Singularity.Instance.GameDataPath + accretionDiskTexturePath));
                AccretionDiskTexture.wrapMode = useRadialTextureMapping ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
                singularityMaterial.SetTexture("AccretionDisk", AccretionDiskTexture);

                if (useRadialTextureMapping)
                {
                    singularityMaterial.DisableKeyword("RADIAL_DISK_MAPPING_OFF");
                    singularityMaterial.EnableKeyword("RADIAL_DISK_MAPPING_ON");
                }
                else
                {
                    singularityMaterial.DisableKeyword("RADIAL_DISK_MAPPING_ON");
                    singularityMaterial.EnableKeyword("RADIAL_DISK_MAPPING_OFF");
                }

                singularityMaterial.SetVector("diskNormal", accretionDiskNormal);
                singularityMaterial.SetFloat("diskInnerRadius", accretionDiskInnerRadius / 6000f); //change to scaledSpace scale
                singularityMaterial.SetFloat("diskOuterRadius", accretionDiskOuterRadius / 6000f);

                //convert from RPM to rad/s
                singularityMaterial.SetFloat("rotationSpeed", accretionDiskRotationSpeed * (Mathf.PI * 2) / 60);

                singularityMaterial.DisableKeyword("ACCRETION_DISK_OFF");
                singularityMaterial.EnableKeyword("ACCRETION_DISK_ON");
            }
        }

        void HideCelestialBody()
        {
            if (!ReferenceEquals(scaledPlanetMeshRenderer, null))
            {
                scaledPlanetMeshRenderer.enabled = false;
            }
        }

        void UnHideCelestialBody()
        {
            if (!ReferenceEquals(scaledPlanetMeshRenderer, null))
            {
                scaledPlanetMeshRenderer.enabled = true;
            }
        }

        void SetupGameObject()
        {
            singularityGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            singularityGO.name = name + " singularity";

            singularityGO.layer = 10;

            singularityGO.transform.position = gameObject.transform.position;
            singularityGO.transform.parent = gameObject.transform;

            GameObject.Destroy(singularityGO.GetComponent<Collider>());

            singularityGO.transform.localScale = new Vector3(enclosingMeshRadius / gameObject.transform.localScale.x, enclosingMeshRadius / gameObject.transform.localScale.y, enclosingMeshRadius / gameObject.transform.localScale.z);

            MeshRenderer singularityMR = singularityGO.GetComponent<MeshRenderer>();
            singularityMR.material = singularityMaterial;
            singularityMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            singularityMR.receiveShadows = false;
            singularityMR.enabled = true;

            singularityCubeMap = singularityGO.AddComponent<SingularityCenteredCubeMap>();
            singularityCubeMap.singularityMaterial = singularityMaterial;
            singularityCubeMap.parentSingularity = this;
        }

        IEnumerator SetupWormhole()
        {
            yield return new WaitForFixedUpdate();

            singularityMaterial.DisableKeyword("WORMHOLE_ON");
            singularityMaterial.EnableKeyword("WORMHOLE_OFF");
            hasWormhole = false;

            if (!String.IsNullOrEmpty(wormholeTarget))
            {
                SingularityObject wormholeTargetSingularity = Singularity.Instance.loadedObjects.SingleOrDefault(_so => _so.name == wormholeTarget);

                if (ReferenceEquals(wormholeTargetSingularity, null))
                {
                    Debug.Log("Wormhole target not found, disabling");
                    yield break;
                }
                singularityMaterial.SetTexture("wormholeCubemap", wormholeTargetSingularity.singularityCubeMap.singularityCubemap);
                singularityMaterial.DisableKeyword("WORMHOLE_OFF");
                singularityMaterial.EnableKeyword("WORMHOLE_ON");
                wormholeCubeMap = wormholeTargetSingularity.singularityCubeMap;
                hasWormhole = true;
            }
        }

        public void Update()
        {
            // Is this needed every frame?
            if (hideCelestialBody)
                HideCelestialBody();

            singularityMaterial.SetColor("galaxyFadeColor", Color.white);
            singularityMaterial.SetMatrix("cubeMapRotation", Matrix4x4.Rotate(Quaternion.identity).inverse);

            if (useAccretionDisk)
                singularityMaterial.SetFloat("universalTime", Singularity.Instance.getTime());
        }

        // Disable rendering from our cubeMap (so no recursive rendering) or sceneBuffer
        // Called from onWillRender, disabling MR or GO here will break rendering, so use layer
        public void DisableForSceneOrCubemap()
        {
            singularityGO.layer = 0;
        }

        public void ReEnable()
        {
            singularityGO.layer = 10;
        }

        public void UpdateTargetWormhole()
        {
            if (hasWormhole)
            {
                wormholeCubeMap.UpdateCubeMap();
            }
        }

        public void ApplyFromUI()
        {
            Debug.Log("Applying config from UI:\r\n");

            scaledRadius = Mathf.Sqrt(Mathf.Max(gravity, 0f)) * 5f;
            singularityMaterial.SetFloat("blackHoleRadius", scaledRadius);

            singularityMaterial.SetFloat("gravity", gravity);

            ConfigureAccretionDisk();

            if (hideCelestialBody)
            {
                HideCelestialBody();
            }
            else
            {
                UnHideCelestialBody();
            }

            enclosingMeshRadius = scaleEnclosingMesh * Mathf.Sqrt(Mathf.Abs(gravity)) * 120f;
            singularityGO.transform.localScale = new Vector3(enclosingMeshRadius / gameObject.transform.localScale.x, enclosingMeshRadius / gameObject.transform.localScale.y, enclosingMeshRadius / gameObject.transform.localScale.z);

            StartCoroutine(SetupWormhole());
        }

        public void OnDestroy()
        {
            if (!ReferenceEquals(singularityGO, null))
            {
                UnityEngine.Object.Destroy(singularityGO);
            }

            if (!ReferenceEquals(scaledPlanetMeshRenderer, null))
            {
                scaledPlanetMeshRenderer.enabled = true;
            }
        }
    }
}