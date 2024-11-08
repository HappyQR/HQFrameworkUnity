namespace HQFramework.Resource
{
    public enum HQHotfixMode : byte
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
