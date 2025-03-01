//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;



namespace cfg.hero
{ 

public sealed partial class TbHero
{
    private readonly Dictionary<int, hero.Hero> _dataMap;
    private readonly List<hero.Hero> _dataList;
    
    public TbHero(JSONNode _json)
    {
        _dataMap = new Dictionary<int, hero.Hero>();
        _dataList = new List<hero.Hero>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = hero.Hero.DeserializeHero(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.HeroId, _v);
        }
        PostInit();
    }

    public Dictionary<int, hero.Hero> DataMap => _dataMap;
    public List<hero.Hero> DataList => _dataList;

    public hero.Hero GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public hero.Hero Get(int key) => _dataMap[key];
    public hero.Hero this[int key] => _dataMap[key];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }
    
    
    partial void PostInit();
    partial void PostResolve();
}

}