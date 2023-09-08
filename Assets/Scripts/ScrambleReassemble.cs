using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScrambleReassemble : MonoBehaviour
{
    [SerializeField] private float rangeMin = 0f;
    [SerializeField] private float rangeMax = 10f;
    [SerializeField] private float animationTime = 1f;
    [SerializeField] private InputAction scrambleButton;
    [SerializeField] private InputAction assembleButton;

    private List<Transform> childTransforms = new();

    private List<Vector3> defaultPositions = new();
    private List<Quaternion> defaultRotations = new();

    private List<Vector3> startPositions = new();
    private List<Quaternion> startRotations = new();

    private List<Vector3> endPositions = new();
    private List<Quaternion> endRotations = new();

    private bool shouldMove = false;
    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        LoadTransforms(transform);
        scrambleButton.Enable();
        assembleButton.Enable();
    }

    // Update is called once per frameframe
    void Update()
    {
        if (scrambleButton.WasPressedThisFrame())
        {
            Scramble();
        }
        if (assembleButton.WasPressedThisFrame())
        {
            Reassemble();
        }

        if (shouldMove)
        {
            timer += Time.deltaTime;
            Move();
        }
    }

    void LoadTransforms(Transform parentTransform)
    {
        foreach (Transform childTransform in parentTransform)
        {
            if (childTransform.GetComponent<MeshRenderer>() != null)
            {
                childTransforms.Add(childTransform);
                childTransform.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);

                defaultPositions.Add(position);
                defaultRotations.Add(rotation);

                startPositions.Add(position);
                startRotations.Add(rotation);

                endPositions.Add(position);
                endRotations.Add(rotation);
            }

            if (childTransform.childCount > 0)
            {
                LoadTransforms(childTransform);
            }
        }
    }

    void Scramble()
    {
        timer = 0f;

        for (int i = 0; i < childTransforms.Count; i++)
        {
            startPositions[i] = childTransforms[i].position;
            startRotations[i] = childTransforms[i].rotation;

            endPositions[i] = new Vector3(
                Random.Range(rangeMin, rangeMax),
                Random.Range(rangeMin, rangeMax),
                Random.Range(rangeMin, rangeMax)
            );

            endRotations[i] = Quaternion.Euler(new Vector3(
                Random.Range(0, 360),
                Random.Range(0, 360),
                Random.Range(0, 360)
            ));
        }

        shouldMove = true;
    }

    void Reassemble()
    {
        timer = 0f;

        for (int i = 0; i < childTransforms.Count; i++)
        {
            startPositions[i] = childTransforms[i].position;
            startRotations[i] = childTransforms[i].rotation;
        }

        endPositions.Clear();
        endRotations.Clear();

        endPositions.AddRange(defaultPositions);
        endRotations.AddRange(defaultRotations);

        shouldMove = true;
    }

    void Move()
    {
        float percent = timer / animationTime;

        for (int i = 0; i < childTransforms.Count; i++)
        {
            Transform childTransform = childTransforms[i];

            Vector3 childPos = childTransform.position;

            Vector3 endPos = endPositions[i];
            Quaternion endRot = endRotations[i];

            if (childPos == endPos)
            {
                shouldMove = false;
                break;
            }
            else
            {
                childTransform.SetPositionAndRotation(
                    Vector3.Lerp(startPositions[i], endPos, percent),
                    Quaternion.Lerp(startRotations[i], endRot, percent)
                );
            }

        }
    }
}
