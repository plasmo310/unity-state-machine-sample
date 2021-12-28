using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sample03
{
    /// <summary>
    /// ステートマシンクラス
    /// 有限状態機械(FSM)
    /// </summary>
    public class StateMachine<TOwner>
    {
        /// <summary>
        /// ステート基底クラス
        /// </summary>
        public abstract class State
        {
            public StateMachine<TOwner> StateMachine;
            public readonly Dictionary<int, State> Transitions = new Dictionary<int, State>();
            protected TOwner Owner => StateMachine.Owner;

            internal void Start(State prevState)
            {
                OnStart(prevState);
            }
            protected virtual void OnStart(State prevState) { }

            internal void Update()
            {
                OnUpdate();
            }
            protected virtual void OnUpdate() { }

            internal void End(State nextState)
            {
                OnEnd(nextState);
            }
            protected virtual void OnEnd(State nextState) { }
        }
        private sealed class AnyState : State { }
        private TOwner Owner { get; } // StateMachineを持つOwner
        private State CurrentState { get; set; } // 現在のステート
        private readonly LinkedList<State> _states = new LinkedList<State>(); // 全てのステート定義

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StateMachine(TOwner owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// ステート追加
        /// </summary>
        public T Add<T>() where T : State, new()
        {
            // ステートを追加
            var newState = new T
            {
                StateMachine = this
            };
            _states.AddLast(newState);
            return newState;
        }

        /// <summary>
        /// ステート取得、無ければ追加
        /// </summary>
        public T GetOrAdd<T>() where T : State, new()
        {
            // 追加されていれば返却
            foreach (var state in _states)
            {
                if (state is T result)
                {
                    return result;
                }
            }
            // 無ければ追加
            return Add<T>();
        }

        /// <summary>
        /// トランジション追加
        /// </summary>
        /// <param name="eventId">イベントID</param>
        /// <typeparam name="TFrom">遷移元ステート</typeparam>
        /// <typeparam name="TTo">遷移先ステート</typeparam>
        public void AddTransition<TFrom, TTo>(int eventId)
            where TFrom : State, new()
            where TTo : State, new()
        {
            var from = GetOrAdd<TFrom>();
            var to = GetOrAdd<TTo>();
            // 既にイベントIDが登録済ならエラー
            if (from.Transitions.ContainsKey(eventId))
            {
                Debug.LogError("already register transition.");
                return;
            }
            // 指定のイベントIDで追加する
            from.Transitions.Add(eventId, to);
        }

        /// <summary>
        /// AnyStateからのトランジション追加
        /// </summary>
        /// <param name="eventId">イベントID</param>
        /// <typeparam name="TTo">遷移先ステート</typeparam>
        public void AddAnyTransition<TTo>(int eventId) where TTo : State, new()
        {
            AddTransition<AnyState, TTo>(eventId);
        }

        /// <summary>
        /// ステート開始
        /// </summary>
        /// <typeparam name="T">開始するステート</typeparam>
        public void Start<T>() where T : State, new()
        {
            CurrentState = GetOrAdd<T>();
            CurrentState.Start(null);
        }

        /// <summary>
        /// ステート更新処理
        /// </summary>
        public void Update()
        {
            CurrentState.Update();
        }

        /// <summary>
        /// イベント発行
        /// </summary>
        /// <param name="eventId">イベントID</param>
        public void Dispatch(int eventId)
        {
            if (!CurrentState.Transitions.TryGetValue(eventId, out var to) 
                && !GetOrAdd<AnyState>().Transitions.TryGetValue(eventId, out to))
            {
                Debug.Log("not found eventId.");
                return;
            }
            // ステート変更
            ChangeState(to);
        }

        /// <summary>
        /// ステート変更処理
        /// </summary>
        /// <param name="nextState">遷移先のステート</param>
        private void ChangeState(State nextState)
        {
            CurrentState.End(nextState);
            nextState.Start(CurrentState);
            CurrentState = nextState;
        }
    }
}