using SummerRest.DataStructures.Containers;
using UnityEngine;

namespace SummerRest.Models
{
    public partial class Domain : EndpointContainer
    {
        [SerializeField] private OptionsArray<string> versions;
        public string ActiveVersion
        {
            get
            {
                var activeVersion = versions.Value;
                if (string.IsNullOrEmpty(activeVersion))
                    return "(No origin)";
                return activeVersion;
            }
        }

        public override string TypeName => nameof(SummerRest.Models.Domain);

    }
#if UNITY_EDITOR
    public partial class Domain
    {
    }
#endif
}