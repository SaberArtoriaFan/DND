//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using SimpleJSON;


namespace cfg
{ 
   
public sealed partial class Tables
{
    public item.TbItem TbItem {get; }
    public hero.TbHero TbHero {get; }

    public Tables(System.Func<string, JSONNode> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        TbItem = new item.TbItem(loader("item_tbitem")); 
        tables.Add("item.TbItem", TbItem);
        TbHero = new hero.TbHero(loader("hero_tbhero")); 
        tables.Add("hero.TbHero", TbHero);
        PostInit();

        TbItem.Resolve(tables); 
        TbHero.Resolve(tables); 
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        TbItem.TranslateText(translator); 
        TbHero.TranslateText(translator); 
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}