using cfg;
using cfg.hero;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LubanMgr :AutoSingleton<LubanMgr>
{
    Tables mainTables;
    protected override void Awake()
    {
        base.Awake();
        mainTables = new Tables(Load);
    }

    public static Hero GetHeroData(int heroID)
    {
        return Instance.mainTables.TbHero.GetOrDefault(heroID);
    }

    private JSONNode Load(string filename)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(Application.dataPath);
        stringBuilder.Append("/../GenerateDatas/json/");
        stringBuilder.Append(filename);
        stringBuilder.Append(".json");
        return JSON.Parse(File.ReadAllText(stringBuilder.ToString()));
    }

}
