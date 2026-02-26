using System;

namespace Extensions.FiniteStateMachine
{
    /// <summary>
    /// Абстракция состояния ожидания
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class WaitHandleState<TContext> : BaseState<TContext>
    {
        protected IWaitHandle waitHandle;
        protected readonly string nextStateId;

        protected WaitHandleState(string nextStateId)
        {
            if (string.IsNullOrEmpty(nextStateId)) throw new ArgumentException("nextStateId cannot be null or empty");
            this.nextStateId = nextStateId;
        }

        public override void Enter(TContext context)
        {
            if (waitHandle != null)
            {
                waitHandle.Cancel();
                waitHandle = null;
            }

            waitHandle = CreateWaitHandle(context);
        }

        public override StateResult Tick(TContext context)
        {
            if (waitHandle == null)
            {
                return StateResult.Switch(nextStateId);
            }

            if (!waitHandle.IsDone)
            {
                return StateResult.StayInState();
            }

            return StateResult.Switch(nextStateId);
        }

        public override void Exit(TContext context)
        {
            if (waitHandle != null)
            {
                waitHandle.Cancel();
            }

            waitHandle = null;
        }

        protected abstract IWaitHandle CreateWaitHandle(TContext context);
    }
}