using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sample01
{
    /// <summary>
    /// Enemyクラス
    /// enumを使用したステート遷移
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        /// <summary>
        /// ステート
        /// </summary>
        private enum State
        {
            Wait, // 待機
            Move, // 動く
            Sleep, // 寝る
        }
        private State _state = State.Wait; // 現在のステート
        private State _nextState = State.Wait; // 次のステート

        private void Awake()
        {
            Debug.Log("start sample01!!");
        }

        private void Start()
        {
            WaitStart();
        }

        private void Update()
        {
            // 現在のステートのUpdateを呼び出す
            switch (_state)
            {
                case State.Wait:
                    WaitUpdate();
                    break;
                case State.Move:
                    MoveUpdate();
                    break;
                case State.Sleep:
                    SleepUpdate();
                    break;
            }

            // ステートが切り替わったら
            if (_state != _nextState)
            {
                // 終了処理を呼び出して
                switch (_state)
                {
                    case State.Wait:
                        WaitEnd();
                        break;
                    case State.Move:
                        MoveEnd();
                        break;
                    case State.Sleep:
                        SleepEnd();
                        break;
                }

                // 次のステートに遷移する
                _state = _nextState;
                switch (_state)
                {
                    case State.Wait:
                        WaitStart();
                        break;
                    case State.Move:
                        MoveStart();
                        break;
                    case State.Sleep:
                        SleepStart();
                        break;
                }
            }
        }

        /// <summary>
        /// 遷移先のステート設定
        /// </summary>
        /// <param name="nextState">次のステート</param>
        private void ChangeState(State nextState)
        {
            _nextState = nextState;
        }

        private const float ChangeStateTime = 3.0f; // ステート切替時間
        private float _countTime = 0.0f; // 時間カウント用

        // 各ステート処理
        // ----- wait -----
        private void WaitStart()
        {
            Debug.Log("start wait");
        }

        private void WaitUpdate()
        {
            // Wait -> Move
            _countTime += Time.deltaTime;
            if (_countTime > ChangeStateTime)
            {
                ChangeState(State.Move);
                _countTime = 0.0f;
            }
        }

        private void WaitEnd()
        {
            Debug.Log("end wait");
        }

        // ----- move -----
        private void MoveStart()
        {
            Debug.Log("start move");
        }

        private void MoveUpdate()
        {
            // Move -> Sleep
            _countTime += Time.deltaTime;
            if (_countTime > ChangeStateTime)
            {
                ChangeState(State.Sleep);
                _countTime = 0.0f;
            }
        }

        private void MoveEnd()
        {
            Debug.Log("end move");
        }

        // ----- sleep -----
        private void SleepStart()
        {
            Debug.Log("start sleep");
        }

        private void SleepUpdate()
        {
            // Sleep -> Wait
            _countTime += Time.deltaTime;
            if (_countTime > ChangeStateTime)
            {
                ChangeState(State.Wait);
                _countTime = 0.0f;
            }
        }

        private void SleepEnd()
        {
            Debug.Log("end sleep");
        }
    }
}