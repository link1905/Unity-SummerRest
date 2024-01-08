using SummerRest.Scripts.Utilities.DataStructures;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    public abstract class Domain : EndpointContainer
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

        public override string TypeName => nameof(Models.Domain);

    }
}