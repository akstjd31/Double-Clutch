public interface IUIActionHandler
{
    void Handle(UIAction action);
}

public enum UIAction
{
    Main_Start,
    Main_Quit
}