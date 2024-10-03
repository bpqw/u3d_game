using UnityEngine;

public class Food : MonoBehaviour
{

    [System.Obsolete]
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            LevelManager.Instance.CollectOne();
            gameObject.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 180 * Time.deltaTime, 0);
    }
}
