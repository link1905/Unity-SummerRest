using System.Collections.Generic;
using SummerRest.Runtime.DataStructures;

namespace SummerRest.Runtime.Parsers
{
    /// <summary>
    /// Used to build a url composing of a original url and a collection of parameters <br/>
    /// Default support is <see cref="DefaultUrlBuilder"/> <br/>
    /// You can adapt <see cref="IDefaultSupport{TInterface,TDefault}.Current"/> to your preference in runtime
    /// </summary>
    public interface IUrlBuilder : IDefaultSupport<IUrlBuilder, DefaultUrlBuilder>
    {
        string BuildUrl(string url, IEnumerable<KeyValuePair<string, ICollection<string>>> parameters);
    }
}