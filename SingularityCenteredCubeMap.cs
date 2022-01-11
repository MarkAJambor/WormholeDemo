using System;
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
    public class SingularityCenteredCubeMap : MonoBehaviour
    {
        Camera objectCamera;
        public RenderTexture singularityCubemap;
        public SingularityObject parentSingularity;
        public Material singularityMaterial;
        public Camera mainCamera;
        public SingularityCenteredCubeMap wormholeCounterpartMap;
        public Material wormholeCounterpartMaterial;
        public BlackHole thisBlackHole;

        bool cubeMapUpdated = false;
        int screenBufferProperty;
        int cubemapFaceToUpdate = 0;


        void Awake()
        {
            thisBlackHole = this.gameObject.GetComponent<BlackHole>();
            objectCamera = gameObject.AddComponent<Camera>();
            objectCamera.cullingMask = (1 << 0);
            objectCamera.renderingPath = mainCamera.renderingPath;
            objectCamera.depthTextureMode = DepthTextureMode.None;
            objectCamera.farClipPlane = mainCamera.farClipPlane;
            objectCamera.nearClipPlane = mainCamera.nearClipPlane;

            objectCamera.clearFlags = CameraClearFlags.Color;
            objectCamera.backgroundColor = Color.clear;
            objectCamera.enabled = false;

            objectCamera.transform.position = gameObject.transform.position;
            objectCamera.transform.parent = gameObject.transform;

            singularityCubemap = new RenderTexture(1024, 1024, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            singularityCubemap.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            singularityCubemap.autoGenerateMips = true;
            singularityCubemap.filterMode = FilterMode.Trilinear;
            singularityCubemap.Create();
            StartCoroutine(SetMaterialTexture());

            objectCamera.targetTexture = null;

            screenBufferProperty = Shader.PropertyToID("useScreenBuffer");
        }

        IEnumerator SetMaterialTexture()
        {
            yield return new WaitForFixedUpdate();
            singularityMaterial.SetTexture("objectCubeMap", singularityCubemap);
            if (wormholeCounterpartMap != null)
            {
                singularityMaterial.SetTexture("wormholeCubemap", wormholeCounterpartMap.singularityCubemap);
            }
        }

        public void Update()
        {
            cubeMapUpdated = false;
            //if (thisBlackHole.wormholeEnabled)
            //{
            //    wormholeCounterpartMaterial = wormholeCounterpartMap.thisBlackHole.blackHoleMaterial;
            //}
        }

        public void OnWillRenderObject()
        {

            if (true)
            {
                singularityMaterial.SetFloat(screenBufferProperty, 1f); //use screenBuffer only on scaledSpace camera
                UpdateCubeMap(); //update only when called by scaledCamera (or in future by wormhole), to avoid singularities calling it on each other and being disabled in each other's cubemaps
            }

            //works on first black hole but breaks second one,and black holes always hide each other when using screenBuffer mode which is a shame
            //add switch for black holes which can show other black holes? how do I handle this? Like for the case of murph and the wormhole? what to do then
            //			else
            //			{
            //				singularityMaterial.SetFloat(screenBufferProperty,0f);
            //			}
        }

        public void UpdateCubeMap()
        {
            //limit to 1 cubeMap update per frame
            if (!cubeMapUpdated)
            {
                cubemapFaceToUpdate = (cubemapFaceToUpdate + 1) % 6; //update one face per cubemap per frame, later change it to only 1 face of ONE cubemap per frame
                int updateMask = 1 << cubemapFaceToUpdate;
                //int updateMask = (TimeWarp.CurrentRate > 4) ? 63 : (1 << cubemapFaceToUpdate);						

                //ScaledCamera.Instance.galaxyCamera.RenderToCubemap (objectCubemap); // broken
                //parentSingularity.DisableForSceneOrCubemap();
                objectCamera.RenderToCubemap(singularityCubemap, updateMask);
                cubeMapUpdated = true;
                if (!wormholeCounterpartMap.cubeMapUpdated)
                {
                    wormholeCounterpartMap.UpdateCubeMap();
                }
                //parentSingularity.ReEnable();


                //parentSingularity.UpdateTargetWormhole();
            }
            //if (thisBlackHole.wormholeEnabled)
            //{
            //    //wormholeCounterpartMap.thisBlackHole.wormholeEnabled = true;
            //    if (!wormholeCounterpartMap.cubeMapUpdated)
            //    {
            //        wormholeCounterpartMap.UpdateCubeMap();
            //    }
            //    thisBlackHole.blackHoleMaterial.SetTexture("wormholeCubemap", this.wormholeCounterpartMap.singularityCubemap);
            //    thisBlackHole.blackHoleMaterial.DisableKeyword("WORMHOLE_OFF");
            //    thisBlackHole.blackHoleMaterial.EnableKeyword("WORMHOLE_ON");
            //    wormholeCounterpartMap.thisBlackHole.blackHoleMaterial.SetTexture("wormholeCubemap", this.singularityCubemap);
            //}
            //else
            //{

            //}
        }

        public void OnDestroy()
        {
            Utils.LogInfo("Singularity cubemap ondestroy");
            if (!ReferenceEquals(singularityCubemap, null))
            {
                singularityCubemap.Release();
            }
        }
    }
}