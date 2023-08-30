using System.Collections;
using UnityEngine;

public class GarbageItem : MonoBehaviour
{
    public enum GarbageType
    {
        Banana,
        PlasticBottle,
        CrumpledPaper
    }
    
    public GarbageType garbageType;
    private Rigidbody2D _rigidbody2D;
    private float _itemSpeed;
    private PolygonCollider2D _collider2D;
    public GameObject xMarker;
    
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _itemSpeed = GameManager.Instance.CurrentSpeed;
        _rigidbody2D.isKinematic = true;
        _rigidbody2D.velocityX = _itemSpeed;
        _collider2D = GetComponent<PolygonCollider2D>();
        GameManager.OnItemSpeedChanged += ChangeSpeed;
        StartCoroutine(DestroyAfterSeconds());
    }

    public void Drop()
    {
        GameManager.OnItemSpeedChanged -= ChangeSpeed;
        _rigidbody2D.isKinematic = false;
        _rigidbody2D.velocityX = 0;
    }
    
    private void ChangeSpeed(float newSpeed)
    {
        _rigidbody2D.velocityX = newSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        {
           //Debug.Log(other.tag);
            switch (garbageType)
            {
                case GarbageType.Banana:
                    if (other.gameObject.CompareTag("TrashBin"))
                    {
                        GameManager.Instance.UpdateScore();
                    }
                    else
                    {
                        GameManager.Instance.Failure();
                        Instantiate(xMarker, transform.position, Quaternion.identity);
                    }
                    Destroy(gameObject);
                    break;
                case GarbageType.CrumpledPaper:
                    if (other.gameObject.CompareTag("FiberBin"))
                    {
                        GameManager.Instance.UpdateScore();
                    }
                    else
                    {
                        GameManager.Instance.Failure();
                        Instantiate(xMarker, transform.position, Quaternion.identity);
                    }
                    Destroy(gameObject);
                    break;
                case GarbageType.PlasticBottle:
                    if (other.gameObject.CompareTag("PlasticBin"))
                    {
                        GameManager.Instance.UpdateScore();
                    }
                    else
                    {
                        GameManager.Instance.Failure();
                        Instantiate(xMarker, transform.position, Quaternion.identity);
                    }
                    Destroy(gameObject);
                    break;
                default:
                    GameManager.Instance.Failure();
                    Instantiate(xMarker, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        GameManager.OnItemSpeedChanged -= ChangeSpeed;
    }
    
    IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(25);
        Destroy(gameObject);
    }
}
