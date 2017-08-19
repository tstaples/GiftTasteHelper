using Newtonsoft.Json;

namespace GiftTasteHelper.Framework
{
    /// <summary>Temporary work-around for the serialization bug that is fixed in SMAPI 1.15.2. This should be removed after updating to that version.</summary>
    internal class SemanticVersion : StardewModdingAPI.SemanticVersion
    {
        [JsonConstructor]
        public SemanticVersion(int majorVersion, int minorVersion, int patchVersion, string build = null)
            : base(majorVersion, minorVersion, patchVersion, build)
        {
        }

        public SemanticVersion(string version)
            : base(version)
        {
        }
    }
}
