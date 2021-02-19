using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Vikingloader
{
    public class Loader
    {
        public static void Init()
        {
            Loader.Load = new GameObject();
            Loader.Load.AddComponent<Hack>();
            UnityEngine.Object.DontDestroyOnLoad(Loader.Load);
        }
        public static void Unload()
        {
            _Unload();
        }
        private static void _Unload()
        {
            GameObject.Destroy(Loader.Load);
        }

        private static GameObject Load;
    }
}
