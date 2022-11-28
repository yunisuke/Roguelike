using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;
    private bool isMoving;

    protected virtual void Start()
    {
        // このオブジェクトのBoxCollider2Dへのコンポーネント参照を取得します
        boxCollider = GetComponent<BoxCollider2D>();

        // このオブジェクトのRigidbody2Dへのコンポーネント参照を取得します
        rb2D = GetComponent<Rigidbody2D>();

        // 移動時間の逆数を保存することで、除算ではなく乗算で使用できるようになる
        inverseMoveTime = 1f / moveTime;
    }

    // 移動できる場合はtrueを、移動できない場合はfalseを返す
    // Moveはx方向、y方向、およびRaycastHit2Dのパラメータを取り衝突をチェック
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        // オブジェクトの開始位置を保存(現在位置)
        Vector2 start = transform.position;

        // Moveを呼び出すときに渡される方向パラメータに基づいて終了位置を計算
        Vector2 end = start + new Vector2(xDir, yDir);

        // boxColliderを無効にして、ラインキャストがこのオブジェクト自身のコライダーに当たらないようにする
        boxCollider.enabled = false;

        // 始点から終点までラインをキャストして、blockingLayerの衝突をチェックする
        hit = Physics2D.Linecast(start, end, blockingLayer);

        // ラインキャスト後にboxColliderを再度有効にする
        boxCollider.enabled = true;

        // 何かがヒットしたかどうかを確認
        if (hit.transform == null && !isMoving)
        {
            // 何もヒットしなかった場合は、Vector2エンドを宛先として渡してSmoothMovementコルーチンを開始する
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        // 何かがヒットした場合は、falseを返し、行動はできない
        return false;
    }

    // ユニットを今のスペースから次のスペースに移動するためのコルーチン。endを使用して移動先を指定する
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        isMoving = true;

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        // 残りの移動距離がイプシロン(ほぼゼロ)より大きい間
        while(sqrRemainingDistance > float.Epsilon)
        {
            // newPositionに移動途中の位置を設定
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime);

            // アタッチされたRigidbody2DでMovePositionを呼び出し、それを計算された位置に移動
            rb2D.MovePosition(newPosition);

            // 移動後の残り距離を再計算
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            // ループを終了するためにsqrRemainingDistanceがゼロに近づくまで戻り、ループする
            yield return null;
        }

        // 移動距離がイプシロンより小さくなったとき、終了地点まで移動する
        rb2D.MovePosition(end);

        // 移動判定をfalseに変更する
        isMoving = false;
    }

    // AttemptMoveはジェネリックパラメータTを取り、ブロックされた場合にユニットが操作するコンポーネントのタイプを指定
    protected virtual void AttemptMove<T>(int xDir, int yDir) 
        where T : Component
    {
        RaycastHit2D hit;

        // 移動が成功したか確認
        bool canMove = Move(xDir, yDir, out hit);

        // ラインキャストの影響を受けていないか確認する
        if (hit.transform == null)
        {
            // 何もヒットしなかった場合は終わり
            return;
        }

        // 障害となったコンポーネントを取得
        T hitComponent = hit.transform.GetComponent<T>();
        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
    }

    // 障害物にぶつかった際に呼び出す
    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
