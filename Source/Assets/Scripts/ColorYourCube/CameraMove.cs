using UnityEngine;

public class CameraMove : MonoBehaviour
{
    GameObject viewCamera;
    public float moveSpeed = 10f;
    Vector3 pivot;
    public static Vector3 rotateVector, rotateVectorPerpendicular;
    float xSpeed, ySpeed = 0f;

    float momentum = 1f;

    bool usingJoystick = false;

    void Start()
    {
        viewCamera = FindObjectOfType<Camera>().gameObject;
        pivot = GameObject.Find("Pivot").transform.position;
    }

    void FixedUpdate()
    {
        rotateVector = Quaternion.AngleAxis(viewCamera.transform.eulerAngles.z, viewCamera.transform.forward) * new Vector3(viewCamera.transform.forward.z, 0f, -viewCamera.transform.forward.x);
        //Debug.DrawLine(viewCamera.transform.position, viewCamera.transform.position + rotateVector * 5f, Color.red);
        rotateVectorPerpendicular = Quaternion.AngleAxis(viewCamera.transform.eulerAngles.z + 90f, viewCamera.transform.forward) * rotateVector;
        //Debug.DrawLine(viewCamera.transform.position, viewCamera.transform.position + rotateVectorPerpendicular * 5f, Color.blue);

        viewCamera.transform.RotateAround(pivot, Vector3.up, moveSpeed * Time.deltaTime * xSpeed * momentum);
        viewCamera.transform.RotateAround(pivot, rotateVector, moveSpeed * Time.deltaTime * ySpeed * momentum);

        momentum = (xSpeed == 0f) && (ySpeed == 0f) ? 1f : momentum + 0.04f;
    }

    public void UpdateMode(string joystickInput)
    {
        float speedValue = float.Parse(ParseString(joystickInput)) - 3f;

        if (joystickInput.Contains("X"))
        {
            usingJoystick = true;
            xSpeed = speedValue;
        }
        else if (joystickInput.Contains("Y"))
        {
            usingJoystick = true;
            ySpeed = speedValue;
        }
    }

    public void UpdateMode(float inputValue, char axis)
    {
        if (!usingJoystick)
        {
            switch (axis)
            {
                case 'X':
                    xSpeed = inputValue;
                    break;
                case 'Y':
                    ySpeed = inputValue;
                    break;
            }
        }
    }

    string ParseString(string input)
    {
        string result = string.Empty;

        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsDigit(input[i]))
                result += input[i];
        }

        return result;
    }
}
