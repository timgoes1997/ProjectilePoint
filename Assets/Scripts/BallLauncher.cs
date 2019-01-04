using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLauncher : MonoBehaviour
{

	public Rigidbody ball;
	public Transform target;

	public int resolution = 50;

	public float height = 25;
	public float gravity = -18;

	[Range(0.0f, 2.5f)]
	public float defaultDisplacement = 0.25f;

	public bool debugPath = true;

	// Use this for initialization
	void Start()
	{
		ball.useGravity = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Launch();
		}

		if (debugPath)
		{
			DrawPath();
		}
	}

	void Launch()
	{
		Physics.gravity = Vector3.up * gravity;
		ball.useGravity = true;
		LaunchStruct launchStruct = CalculateLaunchVelocity();
		ball.velocity = launchStruct.velocity;
		Debug.Log(launchStruct.velocity);
	}

	LaunchStruct CalculateLaunchVelocity()
	{
		float displacementY = target.position.y - ball.position.y;
		Vector3 displacementXZ = new Vector3(target.position.x - ball.position.x, 0, target.position.z - ball.position.z);

		float customHeight = (target.position.y > ball.position.y)
			? displacementY + (defaultDisplacement) + (0.1f * Mathf.Sqrt(displacementXZ.magnitude))
			: defaultDisplacement + (0.1f * Mathf.Sqrt(displacementXZ.magnitude));

		float time = (Mathf.Sqrt(-2 * customHeight / gravity) + Mathf.Sqrt(2 * (displacementY - customHeight) / gravity));
		Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * customHeight);
		Vector3 velocityXZ = displacementXZ / time;

		return new LaunchStruct(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
	}

	void DrawPath()
	{
		LaunchStruct ls = CalculateLaunchVelocity();

		Vector3 previousDrawPoint = ball.position;

		for (int i = 0; i <= resolution; i++)
		{
			float simulationTime = i /(float)resolution * ls.time;
			Vector3 displacement = (ls.velocity * simulationTime) + (Vector3.up * gravity * Mathf.Pow(simulationTime, 2)) / 2f;
			Vector3 drawPoint = ball.position + displacement;
			Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
			previousDrawPoint = drawPoint;
		}
	}

	public struct LaunchStruct
	{
		public readonly Vector3 velocity;
		public readonly float time;

		public LaunchStruct(Vector3 velocity, float time)
		{
			this.velocity = velocity;
			this.time = time;
		}
	}
}
