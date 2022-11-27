using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    // Map上にランダム生成するアイテム最小値、最大値を決めるclass
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // Mapの縦横
    public int columns = 8;
    public int rows = 8;

    // 生成するアイテムの個数
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);

    // Mapの材料
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] outerWallTiles;
    public GameObject[] enemyTiles;
    public GameObject[] foodTiles;

    // 変換用
    private Transform boardHolder;

    // 6*6のマスでobjectがない場所を管理する
    private List<Vector3> gridPositions = new List<Vector3>();

    public void SetupScene(int level)
    {
        // Mapの生成
        BoardSetup();

        // gridPositionsのクリアと再取得
        InitialiseList();

        // ランダムに壁を生成
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);

        // 食べ物生成
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

        // 敵を生成
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        // 出口の設置
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0), Quaternion.identity);
    }

    // フィールド生成
    void BoardSetup()
    {
        // Boardをインスタンス化してboardHolderに設定
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                // 床を設置してインスタンス化の準備
                GameObject toInstantiate = floorTiles[Random.Range (0, floorTiles.Length)];;

                // 8*8マス外は外壁を設置してインスタンス化の準備
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range (0, outerWallTiles.Length)];;
                }

                // toInstantiateに設定されたものをインスタンス化
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0), Quaternion.identity) as GameObject;

                // インスタンス化された床 or 外壁の親要素をboardHolderに設定
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    // gridPosiotionsをクリアする
    void InitialiseList()
    {
        // リストをクリアする
        gridPositions.Clear();

        // 6*6のマスをリストに取得する
        for (int x = 1; x < columns -1; x++)
        {
            for (int y = 1; y < rows -1; y++)
            {
                // gridPositionsにx、yの値を入れる
                gridPositions.Add(new Vector3(x, y, 0));
            }
        }
    }

    // gridPositionsからランダムな位置を取得する
    Vector3 RandomPosition()
    {
        // randomIndexを宣言して、gridPositionsの数から数値をランダムで入れる
        int randomIndex = Random.Range(0, gridPositions.Count);

        // randomPositionを宣言して、gridPositionsのrandomIndexに設定する
        Vector3 randomPosition = gridPositions[randomIndex];

        // 使用したgridPositionsの要素を削除
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    // Mapにランダムで引数のものを配置する(敵、壁、アイテム)
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        // 生成するアイテムの個数を最小最大値からランダムに決め、objectCountに設定する
        int objectCount = Random.Range(minimum, maximum);

        // 配置するオブジェクトの数分ループを回す
        for (int i = 0; i < objectCount; i++)
        {
            // 現在オブジェクトが置かれていない、ランダムな位置を取得
            Vector3 randomPosition = RandomPosition();

            // 引数tileArrayからランダムで1つ選択
            GameObject tileChoise = tileArray[Random.Range(0, tileArray.Length)];

            // 生成
            Instantiate(tileChoise, randomPosition, Quaternion.identity);
        }
    }
}
