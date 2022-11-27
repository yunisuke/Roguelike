using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MovingObject
{
    public float restartLevelDelay = 1f; // レベルを再始動するまでの秒単位の遅延時間
    public int pointsPerFood = 10; // フードオブジェクトを拾った時の獲得ポイント
    public int wallDamage = 1; // プレイヤーが壁を割ったときに与えるダメージ
    public Text foodText;

    private Animator animator;
    private int food;

    protected override void Start()
    {
        // プレイヤーのアニメーターコンポーネントへの参照を取得する
        animator = GetComponent<Animator>();

        // レベル間のgameManger.instanceに保存されている現在のフードポイントの合計を取得
        //food = GameManager.instance.playerFoodPoints;

        base.Start();
    }

    // 障害物にぶつかった際に呼び出す
    protected override void OnCanMove<GameObject>(GameObject component)
    {
        
    }
}
