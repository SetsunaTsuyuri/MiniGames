namespace GameOfLife
{
    /// <summary>
    /// 次世代での生死を決めるステート
    /// </summary>
    public enum NextStepCellState
    {
        None,
        Reproduction,
        Survival,
        Underpopulation,
        Overpopulation
    }
}
