namespace HQFramework.Resource
{
    public enum AssetHotfixMode : byte
    {
        /// <summary>
        /// disable hotfix, just stand-alone
        /// </summary>
        NoHotfix,

        /// <summary>
        /// hotfix all assets before entering game
        /// </summary>
        PreHotfix,

        /// <summary>
        /// hotfix or download modules at gaming time separately
        /// </summary>
        SeparateHotfix
    }
}
