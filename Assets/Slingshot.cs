using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public LineRenderer frontRope;
    public LineRenderer backRope;

    public Transform currentBird;
    public Transform slingshotTarget;
    public Transform center;

    public GameObject birdPrefab;

    public float throwForce;
    public float maxRopeSize;

    private Rigidbody2D birdRigidbody;
    private Vector2 targetLastPos;

    void Start()
    {
        //Setup Slingshot
        frontRope.positionCount = 2;
        backRope.positionCount = 2;
        frontRope.SetPosition(0, frontRope.transform.position);
        backRope.SetPosition(0, backRope.transform.position);
    }

    void Update()
    {
        //Yayýn iplerinin pozisyonunu günceller
        frontRope.SetPosition(1, slingshotTarget.position);
        backRope.SetPosition(1, slingshotTarget.position);

        if (Input.GetMouseButton(0))
        {
            UpdateSlingshot();
        }

        if (Input.GetMouseButtonUp(0))
        {
            ThrowBird();
        }
    }

    private void ThrowBird()
    {
        birdRigidbody = currentBird.GetComponent<Rigidbody2D>();
        birdRigidbody.isKinematic = false;
        birdRigidbody.AddForce((slingshotTarget.position - center.position) * throwForce * -10);

        targetLastPos = slingshotTarget.localPosition;
        StartCoroutine(ResetSlingshot(true));
    }

    private void SpawnNewBird()
    {
        GameObject newBird = Instantiate(birdPrefab, slingshotTarget.position, birdPrefab.transform.rotation);
        currentBird = newBird.transform;
    }

    private IEnumerator ResetSlingshot(bool spawnBirdAfterReset)
    {
        float resetTime = 0.1f;
        float elapsedTime = 0;

        while (elapsedTime < resetTime)
        {
            elapsedTime += Time.deltaTime;
            slingshotTarget.localPosition = Vector3.Lerp(targetLastPos, Vector3.zero, elapsedTime / resetTime);
            yield return null;
        }

        if(spawnBirdAfterReset)
        {
            yield return new WaitForSeconds(1);
            SpawnNewBird();
        }
    }

    private void UpdateSlingshot()
    {
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition = Vector3.ClampMagnitude(targetPosition - center.position, maxRopeSize);
        slingshotTarget.localPosition = targetPosition;

        //Update Bird Transform
        Vector2 direction = slingshotTarget.position - center.position;
        float angle = Mathf.Atan2(direction.y, direction.x);
        currentBird.position = slingshotTarget.position - (new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) / 4);
        currentBird.rotation = Quaternion.AngleAxis((angle * Mathf.Rad2Deg) - 180f, Vector3.forward);
    }
}
