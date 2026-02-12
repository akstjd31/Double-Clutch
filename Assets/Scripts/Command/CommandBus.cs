using System;
using UnityEngine;

public sealed class CommandBus
{
    public event Action<ICommand> OnExecuted;
    public event Action<ICommand, string> OnFailed;

    public bool AutoSave { get; set; } = true;

    // 실행 (검증단계 포함)
    public bool Execute(GameContext ctx, ICommand cmd)
    {
        if (ctx == null)
        {
            Debug.LogError("ctx가 null임!");
            return false;
        }

        if (cmd == null)
        {
            Debug.LogError("cmd가 null임!");
            return false;
        }

        if (!cmd.CanExecute(ctx, out var reason))
        {
            OnFailed?.Invoke(cmd, reason ?? "수행 불가능!");
            return false;
        }

        if (!cmd.Execute(ctx, out var error))
        {
            OnFailed?.Invoke(cmd, error ?? "수행 실패!");
            return false;
        }

        if (AutoSave && cmd.RequiresSave)
            ctx.CommitSave();
        

        OnExecuted?.Invoke(cmd);
        return true;
    }
}