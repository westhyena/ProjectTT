using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{

    Collider2D coll;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        Vector3 playerPos = GameManager.Instance.Player.transform.position;
        Vector3 myPos = transform.position;
        float diffX = Mathf.Abs(playerPos.x - myPos.x);
        float diffY = Mathf.Abs(playerPos.y - myPos.y);

        Vector3 playerDir = GameManager.Instance.Player.inputVec;
        
        //노멀라이즈 되어있을 경우 방향 파악
        float dirX = playerDir.x < 0 ? -1 : 1;
        float dirY = playerDir.y < 0 ? -1 : 1;

        switch(transform.tag)
        {
            case "Ground":
                if(diffX > diffY)
                {
                    //48은 타일맵의 크기 * 2 
                    transform.Translate(Vector3.right * dirX * 48);
                }
                else
                {
                    //48은 타일맵의 크기 * 2 
                    transform.Translate(Vector3.up * dirY * 48);
                }
                break;


            case "Enemy":
                if(coll.enabled)
                {
                    transform.Translate(playerDir * 24 + new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f), Random.Range(-4f, 4f)));
                }
                break;
        }

    }
}
