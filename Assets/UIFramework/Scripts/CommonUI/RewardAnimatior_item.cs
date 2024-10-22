using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FastDev.UiFramework
{
    public class RewardAnimatior_item : MonoBehaviour
{
    public Image img;
    public void TryRefreshIcon(Sprite sp)
    {
        img.sprite = sp;
    }
    public void Refresh(RectTransform targetTrans )
    {
        gameObject.SetActive(true);
        //初始化到原点
        transform.localPosition = Vector3.zero;
        //随机发散到50像素方向
        path = new Queue<Vector3>();
        float randLenght = 150f;
        float randLeghtMin = 100f;
        float randX = Random.Range(-randLenght, randLenght);randX = Mathf.Abs(randX) < randLeghtMin ? randX < 0 ? -randLeghtMin : randLenght:randX;
        float randY = Random.Range(-randLenght, randLenght); randY = Mathf.Abs(randY) < randLeghtMin ? randY < 0 ? -randLeghtMin : randLenght : randY;

        Vector3 rand =transform.position+ new Vector3(randX , randY, 0);

        path.Enqueue(rand);
        path.Enqueue(targetTrans.position);
        //再次移动到目标
        worldTargetPos = path.Dequeue();
        isMove = true;
    }

    private bool isMove;
    private Vector3 worldTargetPos;
    private Queue<Vector3> path;
    private float speed = 1.2f;
    void Update()
    {
        if (isMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, worldTargetPos, speed);
            if (Vector3.Distance(transform.position, worldTargetPos) <= 0.1f)
            {
                if (path.Count > 0)
                {
                    worldTargetPos = path.Dequeue();
                    speed = 5;
                }
                else
                {
                    isMove = false;
                    Destroy(gameObject);
                }
            }

        }
    }
}
}