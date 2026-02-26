using System;
using System.Collections.Generic;
using Extensions.Log;

namespace Extensions.FiniteStateMachine
{
    /// <summary>
    /// Базовая стейт-машина
    /// </summary>
    /// <remarks>
    /// Особенности стейтов:
    /// - Pause() — временная остановка без полного “dispose”
    /// - Resume() — возврат после Pause()
    /// - Exit() — гарантированный финальный cleanup, может вызываться из Stop() даже если стейт был paused
    /// </remarks>
    /// <typeparam name="TContext">Входные данные стейт-машины</typeparam>
    public class StateMachine<TContext>
    {
        /// <summary>
        /// Событие изменения активного состояния машины
        /// </summary>
        /// <typeparam name="string">Идентификатор предыдущего состояния</typeparam>
        /// <typeparam name="string">Идентификатор нового состояния</typeparam>
        public event Action<string, string> onStateChanged;
        
        /// <summary>
        /// Состояние исполнения машины
        /// </summary>
        public bool IsRunning => currentState != null;
        /// <summary>
        /// Идентификатор текущего исполняемого состояния
        /// </summary>
        public string CurrentStateId => currentStateId;

        protected readonly IStateRegistry<TContext> registry;

        protected IState<TContext> currentState;
        protected string currentStateId;
        
        protected struct StackEntry
        {
            public string StateId;
            public IState<TContext> State;
        }
        protected readonly Stack<StackEntry> stack = new Stack<StackEntry>();

        /// <summary>
        /// Новая стейт-машина
        /// </summary>
        /// <param name="registry">Реестр состояний</param>
        /// <exception cref="ArgumentNullException">Ошибка реестра</exception>
        public StateMachine(IStateRegistry<TContext> registry)
        {
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }
        
        #region Манипуляция машиной

        /// <summary>
        /// Запуск машины с конкретного состояния
        /// </summary>
        /// <param name="initialStateId">Входное состояние</param>
        /// <param name="context"></param>
        /// <exception cref="ArgumentException">Ошибка входного состояния</exception>
        /// <exception cref="InvalidOperationException">Ошибка запуска машины</exception>
        public void Start(string initialStateId, TContext context)
        {
            if (string.IsNullOrEmpty(initialStateId)) throw new ArgumentException("Initial state id is null or empty.", nameof(initialStateId));
            if (currentState != null) throw new InvalidOperationException("StateMachine is already started.");

            currentStateId = initialStateId;
            currentState = registry.Get(initialStateId);
            if (currentState == null) throw new InvalidOperationException($"Registry returned null state for id '{initialStateId}'.");

            currentState.Enter(context);
        }
        
        /// <summary>
        /// Перезапуск стейт-машины
        /// </summary>
        /// <param name="initialStateId">Входное состояние</param>
        /// <param name="context">Входные данные стейт-машины</param>
        public void Restart(string initialStateId, TContext context)
        {
            Stop(context);
            Start(initialStateId, context);
        }
        
        /// <summary>
        /// Тик активного состояния машины
        /// </summary>
        /// <param name="context">Входные данные стейт-машины</param>
        /// <exception cref="InvalidOperationException">Ошибка перехода состояния</exception>
        public void Tick(TContext context)
        {
            if (!IsRunning)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                ServiceDebug.LogWarning("Tick called but machine is not running.");
#endif
                return;
            }
            
            if (currentState == null)
            {
                return;
            }

            StateResult result = currentState.Tick(context);

            switch (result.Transition)
            {
                case StateTransition.Stay:
                    return;
                case StateTransition.Switch when string.IsNullOrEmpty(result.NextStateId):
                    throw new InvalidOperationException("Switch transition requires non-empty NextStateId.");
                case StateTransition.Switch:
                    Switch(result.NextStateId, context);
                    return;
                case StateTransition.Push when string.IsNullOrEmpty(result.NextStateId):
                    throw new InvalidOperationException("Push transition requires non-empty NextStateId.");
                case StateTransition.Push:
                    Push(result.NextStateId, context);
                    return;
                case StateTransition.Pop:
                    Pop(context);
                    return;
                default:
                    throw new InvalidOperationException($"Unknown StateTransition value: {result.Transition}");
            }
        }

        /// <summary>
        /// Остановка стейт-машины
        /// </summary>
        /// <param name="context">Входные данные стейт-машины</param>
        public void Stop(TContext context)
        {
            if (currentState != null)
            {
                currentState.Exit(context);
            }

            while (stack.Count > 0)
            {
                StackEntry entry = stack.Pop();
                if (entry.State == null) continue;
                if (ReferenceEquals(entry.State, currentState)) continue;

                IStackableState<TContext> stackable = entry.State as IStackableState<TContext>;
                if (stackable != null)
                    stackable.ForceExit(context); // знает что делать из Paused
                else
                    entry.State.Exit(context);  
            }

            stack.Clear();
            currentState = null;
            currentStateId = null;
        }
        
        #endregion

        #region Манипуляция состояниями
        
        protected void Switch(string nextStateId, TContext context)
        {
            string previous = currentStateId;

            while (stack.Count > 0)
            {
                StackEntry entry = stack.Pop();
                entry.State?.Exit(context);
            }
            
            currentState.Exit(context);

            currentStateId = nextStateId;
            currentState = registry.Get(nextStateId);
            if (currentState == null) throw new InvalidOperationException($"Registry returned null state for id '{nextStateId}'.");

            currentState.Enter(context);

            onStateChanged?.Invoke(previous, nextStateId);
        }

        protected void Push(string nextStateId, TContext context)
        {
            string previous = currentStateId;

            stack.Push(new StackEntry { StateId = currentStateId, State = currentState });

            IStackableState<TContext> stackable = currentState as IStackableState<TContext>;
            if (stackable != null)
            {
                stackable.Pause(context);
            }
            else
            {
                currentState.Exit(context);
            }

            currentStateId = nextStateId;
            currentState = registry.Get(nextStateId);
            if (currentState == null) throw new InvalidOperationException($"Registry returned null state for id '{nextStateId}'.");

            currentState.Enter(context);

            if (onStateChanged != null) onStateChanged(previous, nextStateId);
        }

        protected void Pop(TContext context)
        {
            if (stack.Count == 0)
            {
                throw new InvalidOperationException("Pop transition requested, but state stack is empty.");
            }

            string previous = currentStateId;

            currentState.Exit(context);

            StackEntry entry = stack.Pop();

            currentStateId = entry.StateId;
            currentState = entry.State;
            if (currentState == null) throw new InvalidOperationException($"Stack contains null state for id '{currentStateId}'.");

            IStackableState<TContext> stackable = currentState as IStackableState<TContext>;
            if (stackable != null)
            {
                stackable.Resume(context);
            }
            else
            {
                currentState.Enter(context);
            }

            if (onStateChanged != null) onStateChanged(previous, currentStateId);
        }
        
        #endregion
    }
}