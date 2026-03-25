public interface IUIActionHandler
{
    void Handle(UIAction action);
}

public enum UIAction
{
    Main_Start,
    Tutorial,
    Main_Quit
}