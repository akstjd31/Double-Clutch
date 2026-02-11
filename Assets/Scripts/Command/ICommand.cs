public interface ICommand
{
    bool CanExecute(GameContext ctx, out string reason);
    bool Execute(GameContext ctx, out string error);
    bool RequiresSave { get; }
}