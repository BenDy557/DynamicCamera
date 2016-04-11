using UnityEngine;
using System.Collections;

public class ShipMovementSpaceShooter : MonoBehaviour {


    private Rigidbody rigidBody;
    private SpriteRenderer spriteRenderer;


    public float impulseAttackPower;
    public float kickBackRatio;

    public float impulseAttackCooldown;
    public float impulseBombCooldown;
    
    private float currentImpulseAttackCooldown;
    private float currentImpulseBombCooldown;

    private bool readyToFireImpulseAttack;
    private bool readyToFireImpulseBomb;
    private Vector3 aimDirection;
    


    //New stuff

    //Input
    private Vector2 leftStick;
    private Vector2 previousLeftStick;
    private bool m_LeftBumper;
    private Vector2 mCameraDirection;

    //Speed
    public float acceleration;//speed/sec
    public float decceleration;//-speed/sec
    public float m_MaxSpeed;
    private float m_Speed;

    //Direction
    public float m_TurnRateAcceleration;
    public float m_MaxTurnRate;//angles/sec
    public float m_MinTurnRate;//angles/sec
    private float m_TurnRate;//angles/sec
    private float m_Heading;
    private float m_Bearing;

    //Dashing
    public float m_DashSpeed;
    private Vector3 m_DashDirection;
    public bool m_IsDashing;
    public float m_DashTimerAmount;
    private float m_DashTimer;

    //Playerinfo
    public int playerNumber;
    private enum ControllerType { Xbox, Playstation };
    private int controllerType;
    private string playerNumberString;
    private string controllerTypeString;



    //Collision info
    public Vector3 collisionVelocity;



	// Use this for initialization
	void Start () {

        rigidBody = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //new stuff
        m_Speed = 0;
        m_TurnRate = 0;
        m_Heading = 0;
        m_Bearing = 0;

        controllerType = (int)ControllerType.Xbox;
        switch (controllerType)
        {
            case (int)ControllerType.Xbox :
                controllerTypeString = "Xbox";
                break;


            case (int)ControllerType.Playstation:
                controllerTypeString = "Playstation";
                break;
        }
        
        switch (playerNumber)
        {
            case 0:
                playerNumberString = "Player1";
                break;

            case 1:
                playerNumberString = "Player2";
                break;

            case 2:
                playerNumberString = "Player3";
                break;

            case 3:
                playerNumberString = "Player4";
                break;
        }

	}
	


    // Update is called once per frame
	void Update () 
    {
        //INPUT/////////////////
        ////////////////////////
        //Analog
        leftStick = new Vector2(Input.GetAxis(controllerTypeString+playerNumberString+"LeftStickAxisX"), Input.GetAxis(controllerTypeString+playerNumberString+"LeftStickAxisY"));
        leftStick.Normalize();
        //Buttons
        m_LeftBumper = Input.GetButton(controllerTypeString + playerNumberString + "LeftBumper");

       
        //.transform.parent
        
        /*
        Debug.Log(playerNumberString);
        Debug.Log(Input.GetAxis(controllerTypeString + playerNumberString + "LeftStickAxisX"));
        Debug.Log(Input.GetAxis(controllerTypeString + playerNumberString + "LeftStickAxisY"));
        */
        //MOVEMENT//////////////
        ////////////////////////
        
        //Speed
        if (leftStick.magnitude > 0.0f)
        {
            previousLeftStick = leftStick;
            m_Speed += leftStick.magnitude * acceleration * Time.deltaTime;
        }
        else
        {
            m_Speed -= decceleration * Time.deltaTime;
        }

        m_Speed = Mathf.Clamp(m_Speed, 0.0f, m_MaxSpeed);
        


        //Direction

        if (leftStick.sqrMagnitude > 0 && !m_IsDashing)
        {
            mCameraDirection.x = Camera.main.transform.forward.x;
            mCameraDirection.y = Camera.main.transform.forward.z;

            float tempCameraBearing = Vector2.Angle(Vector2.up, mCameraDirection);

            if (mCameraDirection.x < 0.0f)
            {
                tempCameraBearing *= -1.0f;
            }

            float tempInputHeading = Vector2.Angle(Vector2.up, leftStick);

            if (leftStick.x < 0.0f)
            {
                tempInputHeading *= -1.0f;   
            }



            m_Heading = tempInputHeading + tempCameraBearing;
        }
        else
        {
            m_Heading = m_Bearing;
        }

        float amountToTurn = m_Heading - m_Bearing;

        while (amountToTurn > 180)
        {
            amountToTurn -= 360;
        }
        while (amountToTurn < -180)
        {
            amountToTurn += 360;
        }


        if (amountToTurn == 0.0f)
        {
            m_TurnRate = 0.0f;
        }
		else if(amountToTurn > 0.0f)
        {
            
            m_TurnRate = amountToTurn / 10;

            if (m_TurnRate > m_MaxTurnRate * Time.deltaTime)
            {
                m_TurnRate = m_MaxTurnRate * Time.deltaTime;
            }
            if (m_TurnRate < m_MinTurnRate * Time.deltaTime)
            {
                m_TurnRate = m_MinTurnRate * Time.deltaTime;
            }
            if (amountToTurn < m_MinTurnRate * Time.deltaTime)
            {
                m_TurnRate = amountToTurn * Time.deltaTime;
            }
            
             
            //m_TurnRate
        }
		else if(amountToTurn < 0.0f)
		{
			m_TurnRate = amountToTurn / 10;
			
			if (m_TurnRate < -m_MaxTurnRate * Time.deltaTime)
			{
				m_TurnRate = -m_MaxTurnRate * Time.deltaTime;
			}
			if (m_TurnRate > -m_MinTurnRate * Time.deltaTime)
			{
				m_TurnRate = -m_MinTurnRate * Time.deltaTime;
			}
			if (amountToTurn > -m_MinTurnRate * Time.deltaTime)
			{
				m_TurnRate = amountToTurn * Time.deltaTime;
			}
		}


        //m_TurnRate = Mathf.Clamp(m_TurnRate, -m_MaxTurnRate, m_MaxTurnRate);
        m_Bearing += m_TurnRate;

        
        gameObject.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        gameObject.transform.Rotate(0.0f, m_Bearing, 0.0f);

        
        //Movement
        //speed bearing
        rigidBody.velocity = new Vector3(gameObject.transform.forward.x * m_Speed, 0.0f, gameObject.transform.forward.z * m_Speed);

        //Dashing
        if (!m_IsDashing && m_LeftBumper)
        {
            m_DashDirection = gameObject.transform.forward;
            m_IsDashing = true;
            m_DashTimer = m_DashTimerAmount;
        }

        if (m_IsDashing)
        {
            rigidBody.velocity += m_DashDirection * m_DashSpeed;
            m_DashTimer -= Time.deltaTime;
            if (m_DashTimer < 0.0f)
            {
                m_IsDashing = false;
            }
        }



        rigidBody.velocity += collisionVelocity;

        if (collisionVelocity.x > 0.0f)
        {
            collisionVelocity.x *= 0.9f;
        }
        else if (collisionVelocity.x < 0.0f)
        {
            collisionVelocity.x *= 0.9f;
        }

        if (collisionVelocity.y > 0.0f)
        {
            collisionVelocity.y *= 0.9f;
        }
        else if (collisionVelocity.y < 0.0f)
        {
            collisionVelocity.y *= 0.9f;
        }
	}
}

