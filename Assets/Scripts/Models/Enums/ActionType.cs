public enum ActionType
{
	NOTHING,    // Nothing
    ATTACK,     // 0 vs defend     = -1 vs magic         = +3 vs attack
    DEFEND,     // 0 vs attack     = +1 vs magic         = 0 vs heal
    MAGIC,      // +1 vs attack    = -1 vs defend        = +1 vs heal
    HEAL        // -3 vs attack    = heal 3 vs defend    = -1 vs magic
}
