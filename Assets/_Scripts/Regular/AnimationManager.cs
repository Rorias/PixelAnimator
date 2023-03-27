using System.IO;
using System.Xml.Linq;


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

    private GameManager gameManager = GameManager.Instance;

    private Animation animation = null;

    private XDocument model;
    private XElement XModel;

    public void SaveFile(Animation _anim)
    {
        animation = _anim;
        SaveAnimationXMLFile(gameManager.animationsPath + _anim.animationName);
    }

    private void SaveAnimationXMLFile(string _path)
    {
        XModel = animation.Save();
        model = XDocument.Parse(XModel.ToString());
        model.Save(_path + ".xml");
    }

    public bool LoadAnimation(Animation _a)
    {
        animation = _a;

        if (File.Exists(gameManager.animationsPath + _a.animationName + ".xml"))
        {
            model = XDocument.Load(gameManager.animationsPath + _a.animationName + ".xml");
            XModel = model.Root;
            if (!_a.Load(XModel))
            {
                return false;
            }
        }

        return true;
    }
}