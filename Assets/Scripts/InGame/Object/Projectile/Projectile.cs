using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Character source;
    Character target;
    Vector2 targetPosition2D;

    public float speed = 30.0f;
    public float positionYOffset = 4.0f;
    public float positionZOffset = 0.1f;
    public float angleOffset = 90.0f;

    public void Initialize(Character source, Character target)
    {
        this.source = source;
        this.target = target;
        this.transform.position = source.transform.position + new Vector3(0.0f, positionYOffset, positionZOffset);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = target.Position2D + new Vector2(0.0f, positionYOffset) - (Vector2)transform.position;
        float sqrDistance = direction.sqrMagnitude;
        if (sqrDistance < 0.1f * 0.1f)
        {
            target.Damage(source.AttackDamage);
            Destroy(gameObject);
            return;
        }

        

        direction.Normalize();
        transform.position +=  speed * Time.deltaTime * (Vector3)direction;

        transform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset);
    }
}
