namespace PinGod.Base
{
    /// <summary>
    /// sync with external simulator. Used by memory map ReadStates
    /// </summary>
    public enum GameSyncState
    {
        /// <summary>
        /// No state
        /// </summary>
        None,
        /// <summary>
        /// Game started let . GameRunning
        /// </summary>
        started,
        /// <summary>
        /// quit godot
        /// </summary>
        quit,
        /// <summary>
        /// pause godot
        /// </summary>
        pause,
        /// <summary>
        /// resume godot
        /// </summary>
        resume,
        /// <summary>
        /// resume godot
        /// </summary>
        reset
    }
}