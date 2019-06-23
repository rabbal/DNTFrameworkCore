namespace DNTFrameworkCore.EFCore.Context.Hooks
{
    /// <summary>
    /// A hook that is executed before an action.
    /// </summary>
    public interface IPreActionHook : IHook
    {
        bool RequiresValidation { get; }
    }
}