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
    public class Singularity : MonoBehaviour
    {
        private static Singularity instance;
        public static Singularity Instance { get { return instance; } }

        private static Dictionary<string, Shader> LoadedShadersDictionary = new Dictionary<string, Shader>();
        public static Dictionary<string, Shader> LoadedShaders { get { return LoadedShadersDictionary; } }

        private string path, gameDataPath;
        public string GameDataPath { get { return gameDataPath; } }

        public Cubemap galaxyCubemap;
        public MaterialPropertyBlock galaxyCubeControlMPB;

        public List<SingularityObject> loadedObjects = new List<SingularityObject>();

        public RenderTexture screenBuffer;

        public ScaledSceneBufferRenderer scaledSceneBufferRenderer;

        double initialUniversalTime;

        public int galaxyCubemapResolution = 2048;
        public int objectCubemapResolution = 2048;

        public Singularity()
        {
            if (instance == null)
            {
                instance = this;
                Debug.Log("Instance created");
            }
            else
            {
                //destroy any duplicate instances that may be created by a duplicate install
                Debug.Log("Destroying duplicate instance, check your install for duplicate mod folders");
                UnityEngine.Object.Destroy(this);
            }
        }

        void Awake()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);

            path = Uri.UnescapeDataString(uri.Path);
            path = Path.GetDirectoryName(path);

            StartCoroutine(DelayedInit());
        }

        // Delay for the galaxy cubemap to be set correctly
        IEnumerator DelayedInit()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Init();
        }

        void Init()
        {

            SetupCubemap();

            screenBuffer = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, UnityEngine.RenderTextureReadWrite.Default);
            screenBuffer.filterMode = FilterMode.Bilinear;
            screenBuffer.Create();

            scaledSceneBufferRenderer = new ScaledSceneBufferRenderer();
            scaledSceneBufferRenderer.Init();

            initialUniversalTime = 0;
        }

        void SetupCubemap()
        {
            //try
            //{
            //    //setthisup
            //    galaxyCubeControlMPB = Skybox.GetField("mpb").GetValue(GalaxyCubeControl.Instance) as MaterialPropertyBlock;
            //    UnityEngine.Renderer[] cubeRenderers = typeof(GalaxyCubeControl).GetField("cubeRenderers").GetValue(GalaxyCubeControl.Instance) as UnityEngine.Renderer[];
            //    Component galaxyCubeControlComponent = (Component)GalaxyCubeControl.Instance;

            //    if (!ReferenceEquals(galaxyCubeControlMPB, null) && !ReferenceEquals(cubeRenderers, null))
            //    {
            //        // Disable cubemap dimming before we capture it
            //        galaxyCubeControlMPB.SetColor(PropertyIDs._Color, Color.white);
            //        for (int i = 0; i < cubeRenderers.Length; i++)
            //        {
            //            cubeRenderers[i].SetPropertyBlock(galaxyCubeControlMPB);
            //        }
            //        // De-rotate galaxy cubemap before we capture it, later adjust in shader for additional planetarium rotations
            //    }
            //}
            //catch (Exception E)
            //{
            //    Debug.Log("Couldn't setup galaxy cubeMap correctly, Exception thrown: ");
            //}

            galaxyCubemap = new Cubemap(galaxyCubemapResolution, TextureFormat.RGBA32, true);
            //ScaledCamera.Instance.galaxyCamera.RenderToCubemap(galaxyCubemap);
            Debug.Log("GalaxyCubemap initialized");
        }

        void OnDestroy()
        {
            foreach (SingularityObject singularityObject in loadedObjects)
            {
                singularityObject.OnDestroy();
                UnityEngine.Object.Destroy(singularityObject);
            }

            screenBuffer.Release();

            if (!ReferenceEquals(scaledSceneBufferRenderer, null))
            {
                scaledSceneBufferRenderer.Cleanup();
            }
        }

        public void DisableSingularitiesForSceneBuffer()
        {
            foreach (SingularityObject singularityObject in loadedObjects)
            {
                singularityObject.DisableForSceneOrCubemap();
            }
        }

        public void ReEnableSingularities()
        {
            foreach (SingularityObject singularityObject in loadedObjects)
            {
                singularityObject.ReEnable();
            }
        }

        // universal time is a large double, if we pass it to the shader directly as a float we lose enough precision that things look like they run at lower fps
        // pass the offset from an initial value as float
        public float getTime()
        {
            return (float)(Time.time);
        }
    }
}

