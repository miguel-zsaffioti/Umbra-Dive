using UnityEngine;
using System.Collections;

public class Spectre : Boss
{
    GameObject OreShard;

    public IEnumerator MoveToCoroutine(Vector2 location, float moveSpeed)
    {
        while (Mathf.Abs(rb.position.x - location.x) > 0.01f)
        {
            float newX = Mathf.MoveTowards(rb.position.x, location.x, moveSpeed * Time.deltaTime);
            rb.MovePosition(new Vector2(newX, rb.position.y));

            yield return new WaitForFixedUpdate();
        }

        while (Mathf.Abs(rb.position.y - location.y) > 0.01f)
        {
            float newY = Mathf.MoveTowards(rb.position.y, location.y, moveSpeed * Time.deltaTime);
            // CORREÇÃO: newY vai na posição Y do Vector2
            rb.MovePosition(new Vector2(rb.position.x, newY));

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(location);
        //Debug.Log("Movimento Concluído");
    }

    public IEnumerator TeleportTo(Vector2 location, float waitTime) {
        yield return new WaitForSeconds(waitTime);
        rb.position = location;
    }

    public void SpawnProjectile(Vector2 direction, Quaternion rotation)
    {
        Vector2 spawnLocation = rb.position + direction;

        Instantiate(OreShard, spawnLocation, rotation);
    }

    public void SpawnProjectileInCrossPattern()
    {

    }

    public void SpawnProjectileInXPattern()
    {

    }

}
