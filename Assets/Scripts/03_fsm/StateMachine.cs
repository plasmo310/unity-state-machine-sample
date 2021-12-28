using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sample03
{
    /// <summary>
    /// ステートマシンクラス
    /// 有限オートマトンの遷移も定義する
    /// </summary>
    public class StateMachine<TOwner>
    {
        /// <summary>
        /// ステート基底クラス
        /// 各ステートクラスはこのクラスを継承する
        /// </summary>
        public abstract class State
        {
            public StateMachine<TOwner> StateMachine;
            public readonly Dictionary<int, State> Transitions = new Dictionary<int, State>();
            protected TOwner Owner => StateMachine.Owner;
            
            public virtual void OnStart() { }
            public virtual void OnUpdate() { }
            public virtual void OnEnd() { }
        }
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
        private T Add<T>() where T : State, new()
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
        private T GetOrAdd<T>() where T : State, new()
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
        /// 遷移トランジション追加
        /// </summary>
        /// <param name="stateId">ステートID</param>
        /// <typeparam name="TFrom">遷移元ステート</typeparam>
        /// <typeparam name="TTo">遷移先ステート</typeparam>
        public void AddTransition<TFrom, TTo>(int stateId)
            where TFrom : State, new()
            where TTo : State, new()
        {
            var from = GetOrAdd<TFrom>();
            var to = GetOrAdd<TTo>();
            // 既にイベントIDが登録済ならエラー
            if (from.Transitions.ContainsKey(stateId))
            {
                Debug.LogError("already register transition.");
                return;
            }
            // 指定のイベントIDで追加する
            from.Transitions.Add(stateId, to);
        }

        /// <summary>
        /// AnyStateからの遷移トランジション追加
        /// </summary>
        private sealed class AnyState : State { }
        public void AddAnyTransition<TTo>(int eventId) where TTo : State, new()
        {
            AddTransition<AnyState, TTo>(eventId);
        }

        /// <summary>
        /// ステート開始処理
        /// </summary>
        /// <typeparam name="T">開始するステート</typeparam>
        public void Start<T>() where T : State, new()
        {
            CurrentState = GetOrAdd<T>();
            CurrentState.OnStart();
        }

        /// <summary>
        /// ステート更新処理
        /// </summary>
        public void Update()
        {
            CurrentState.OnUpdate();
        }

        /// <summary>
        /// 指定されたIDのステートに切り替える
        /// </summary>
        /// <param name="stateId">ステートID</param>
        public void ChangeState(int stateId)
        {
            // イベントIDからステート取得
            if (!CurrentState.Transitions.TryGetValue(stateId, out var nextState) 
                && !GetOrAdd<AnyState>().Transitions.TryGetValue(stateId, out nextState))
            {
                Debug.Log("not found eventId.");
                return;
            }
            // ステートを切り替える
            CurrentState.OnEnd();
            nextState.OnStart();
            CurrentState = nextState;
        }
    }
}