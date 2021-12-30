using System;
using System.Collections;
using UnityEngine;

namespace Sample05
{
    using StateBase = StateMachine<Honey>.StateBase;
    
    /// <summary>
    /// Honeyクラス
    /// 魚を受け取って食べるだけの生活
    /// </summary>
    public class Honey : MonoBehaviour
    {
        /// <summary>
        /// ステージ管理クラス
        /// </summary>
        [SerializeField] public StageManager stageManager;

        private enum StateType
        {
            WaitHome, // 待機
            Walking,  // 散歩
            Eating,   // 食事
        }
        private StateMachine<Honey> _stateMachine;

        private void Start()
        {
            // ステートマシン定義
            _stateMachine = new StateMachine<Honey>(this);
            _stateMachine.Add<StateWaitHome>((int) StateType.WaitHome);
            _stateMachine.Add<StateWalking>((int) StateType.Walking);
            _stateMachine.Add<StateEating>((int) StateType.Eating);
            // ステート開始
            _stateMachine.OnStart((int) StateType.WaitHome);
        }

        private void Update()
        {
            // ステート更新
            _stateMachine.OnUpdate();
        }

        /// <summary>
        /// 家で待機中か？
        /// </summary>
        public bool IsWaitingHome()
        {
            return _stateMachine.IsCurrentState((int) StateType.WaitHome);
        }

        /// <summary>
        /// 魚を受け取る
        /// </summary>
        /// <param name="fish"></param>
        public void ReceiveFish(GameObject fish)
        {
            // 魚を受け取る
            Debug.Log("<color=yellow>**honey** うれしいわダーリン</color>");
            fish.transform.parent = transform;
            var position = transform.position;
            position.y = fish.transform.position.y;
            fish.transform.position = position;
            // 食事ステートに変更
            _stateMachine.ChangeState((int) StateType.Eating);
        }

        // 各ステート処理
        // ----- wait home -----
        private class StateWaitHome : StateBase
        {
            public override void OnStart()
            {
                Debug.Log("<color=red>**honey** 家で待ちます</color>");
                // 待機スタート
                _isFinishWaiting = false;
                MonoBehaviorHandler.StartStaticCoroutine(WaitCoroutine());
            }

            public override void OnUpdate()
            {
                // 待機が完了したら次のステートへ
                if (_isFinishWaiting)
                {
                    StateMachine.ChangeState((int) StateType.Walking);
                }
            }

            public override void OnEnd()
            {
            }
            
            // 待機が完了しているか？
            private bool _isFinishWaiting;
            
            // 待機コルーチン
            private IEnumerator WaitCoroutine()
            {
                // 数秒待機
                yield return new WaitForSeconds(5.0f);
            
                // 待機完了
                _isFinishWaiting = true;
            }
        }

        // ----- walking -----
        private class StateWalking : StateBase
        {
            public override void OnStart()
            {
                Debug.Log("<color=red>**honey** 散歩してきます</color>");
                // 帰宅中フラグをfalseに設定
                _isGoingHome = false;
            }

            public override void OnUpdate()
            {
                var enemyPosition = Owner.transform.position;
                
                // 目的地の設定
                var targetPosition = _isGoingHome
                    ? Owner.stageManager.homeHoneyTransform.position // 帰宅中の場合、家の位置
                    : Owner.stageManager.walkTransform.position;     // 帰宅中でない場合、散歩する位置
                
                // 海へ到着したら次のステートへ
                if (Vector3.Distance(enemyPosition, targetPosition) < 0.5f)
                {
                    // 帰宅中フラグをONにしてreturn
                    if (!_isGoingHome)
                    {
                        _isGoingHome = true;
                        return;
                    }
                    // 次のステートへ遷移する
                    StateMachine.ChangeState((int) StateType.WaitHome);
                    return;
                }
                // 目的地へ向かう
                Owner.transform.position = Vector3.MoveTowards(
                    enemyPosition,
                    targetPosition, 
                    3.5f * Time.deltaTime);
                Owner.transform.LookAt(targetPosition);
            }

            public override void OnEnd()
            {
            }

            private bool _isGoingHome; // 帰宅中か？
        }

        // ----- eating -----
        private class StateEating : StateBase
        {
            public override void OnStart()
            {
                Debug.Log("<color=red>**honey** 魚食べます</color>");
                // 食事開始
                _isFinishEating = false;
                MonoBehaviorHandler.StartStaticCoroutine(EatCoroutine());
            }

            public override void OnUpdate()
            {
                // くるくる周る
                Owner.transform.Rotate(Vector3.up * 500.0f * Time.deltaTime);
                
                // 食事が完了したら次のステートへ
                if (_isFinishEating)
                {
                    StateMachine.ChangeState((int) StateType.WaitHome);
                }
            }

            public override void OnEnd()
            {
            }
            
            // 食事が完了しているか？
            private bool _isFinishEating;
            
            // 食事コルーチン
            private IEnumerator EatCoroutine()
            {
                // 食事中、数秒待機
                yield return new WaitForSeconds(3.0f);
                
                // 子オブジェクト(魚)を破棄
                foreach (Transform child in Owner.transform)
                {
                    Destroy(child.gameObject);
                }
            
                // 食事完了
                _isFinishEating = true;
            }
        }
    }
}
