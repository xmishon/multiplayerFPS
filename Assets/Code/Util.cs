﻿using UnityEngine;

namespace Utils
{
    public class Util
    {
        #region publicFields

        public static void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (obj == null)
                return;
            obj.layer = newLayer;
            foreach(Transform child in obj.transform)
            {
                if(child == null)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

        #endregion
    }
}
