using System;
using UnityEngine;

public class FallerCollisionHandler : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        FallerController collidedFaller = collision.gameObject.GetComponent<FallerController>();
        if (collision.gameObject.name == "Player")
        {
            return;
        }else if (collision.gameObject.name.Contains("Floor"))
        {
            return;
        }
        if (collidedFaller == null)
        {
            return;
        }
        if(gameObject.GetComponent<FallerController>() == null)
        {
            return; 
        }
        Debug.Log("Collision detected between " + gameObject.name + " and " + collision.gameObject.name);
        Debug.Log("is " + gameObject.name + " frozen: " + gameObject.GetComponent<FallerController>().amIFrozen() + "is " + collidedFaller.name + " frozen: " + collidedFaller.amIFrozen());
        if (gameObject.GetComponent<FallerController>().amIFrozen() && collidedFaller.amIFrozen())
        {
            //Both frozen, do nothing
            Debug.Log("Both frozen, do nothing");
            return;
        }else if(collidedFaller.amIFrozen() && !gameObject.GetComponent<FallerController>().amIFrozen())
        {
            Debug.Log("Other faller frozen, this is not, freeze this");
            //other faller frozen, this is not, freeze this
            gameObject.GetComponent<FallerController>().FloorPause();
            return;
        }else if(!collidedFaller.amIFrozen() && gameObject.GetComponent<FallerController>().amIFrozen())
        {
            Debug.Log("Other faller not frozen, this is frozen, freeze other");
            //other faller not frozen, this is frozen, freeze other
            collidedFaller.FloorPause();
            return;
        }else if (!collidedFaller.amIFrozen() && !gameObject.GetComponent<FallerController>().amIFrozen())
        {
            Debug.Log("Neither frozen, delete other");
            //neither frozen, delete other? but if this same function gets called on the other faller,
            //it will delete this one, so both get deleted, which may be what we want im not sure
            collidedFaller.DeleteMe();
            return;
        }
        else if (collidedFaller != null)
        {
            collidedFaller.FloorPause();
        }

    }
}
