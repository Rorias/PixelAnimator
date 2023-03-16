using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

using UnityEngine;


public sealed class AnimationManager
{
    #region Singleton
    private static AnimationManager _instance;
    private static object _lock = new object();

    private AnimationManager() { }

    public static AnimationManager Instance
    {
        get
        {
            if (null == _instance)
            {
                lock (_lock)
                {
                    if (null == _instance)
                    {
                        _instance = new AnimationManager();
                    }
                }
            }
            return _instance;
        }
    }
    #endregion

    private Animation animation = null;

    private XDocument model;
    private XElement XModel;

    public bool LoadAnimation(Animation _a)
    {
        animation = _a;

        if (File.Exists(Application.dataPath + "/StreamingAssets/" + _a.animationName + ".xml"))
        {
            model = XDocument.Load(Application.dataPath + "/StreamingAssets/" + _a.animationName + ".xml");
            XModel = model.Root;
            if (!_a.Load(XModel))
            {
                return false;
            }
        }

        return true;
    }
}