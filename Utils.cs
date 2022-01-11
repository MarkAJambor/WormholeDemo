using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Reflection;

namespace Singularity
{
    public class Utils
    {
        public static BindingFlags reflectionFlags = BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public |
            BindingFlags.Instance | BindingFlags.Static;

        public static void LogDebug(string log)
        {
            Debug.Log("[Singularity][Debug] " + log);
        }

        public static void LogInfo(string log)
        {
            Debug.Log("[Singularity][Info] " + log);
        }

        public static void LogError(string log)
        {
            Debug.LogError("[Singularity][Error] " + log);
        }

        public static Dictionary<string, Shader> LoadAssetBundle(string path)
        {
            string shaderspath;
            Dictionary<string, Shader> LoadedShaders = new Dictionary<string, Shader>();

            shaderspath = path + "/shaders";

            LoadedShaders.Clear();

            using (WWW www = new WWW("file://" + shaderspath))
            {
                AssetBundle bundle = www.assetBundle;
                Shader[] shaders = bundle.LoadAllAssets<Shader>();

                foreach (Shader shader in shaders)
                {
                    LogDebug(shader.name + " loaded. Supported?" + shader.isSupported.ToString());
                    LoadedShaders.Add(shader.name, shader);
                }

                bundle.Unload(false); // unload the raw asset bundle
                www.Dispose();
            }

            return LoadedShaders;
        }

        public static char[] delimiters = new char[4]
        {
            ' ',',',';','\t'
        };
    }
}