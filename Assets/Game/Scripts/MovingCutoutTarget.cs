
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HingeJoint))]
[RequireComponent(typeof(Collider))]
public class MovingCutoutTarget : MonoBehaviour
{
    [SerializeField] private float flapTorque = 8f;

    private Rigidbody body;
    private HingeJoint hinge;
    private Collider targetCollider;
    private MovingCutoutManager manager;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isHit;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        hinge = GetComponent<HingeJoint>();
        targetCollider = GetComponent<Collider>();
        manager = GetComponentInParent<MovingCutoutManager>();

        startPosition = body.position;
        startRotation = body.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isHit ||
            collision.gameObject.GetComponent<ProjectileImpact>() == null)
        {
            return;
        }

        isHit = true;
        
        hinge.useSpring = false;
        targetCollider.enabled = false;

        body.AddTorque(transform.right * flapTorque, ForceMode.Impulse);
        
        manager.RegisterHit(this);
    }

    public void PrepareForReset()
    {
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.isKinematic = true;

        body.position = startPosition;
        body.rotation = startRotation;

        hinge.useSpring = true;

        JointSpring spring = hinge.spring;
        spring.targetPosition = 0f;
        hinge.spring = spring;

        targetCollider.enabled = true;
        isHit = false;
    }

    public void FinishReset()
    {
        body.isKinematic = false;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }
}
