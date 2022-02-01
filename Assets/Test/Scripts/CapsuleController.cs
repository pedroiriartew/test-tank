using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class CapsuleController : EntityBehaviour<ICapsuleState>
{
    const float MOUSE_SENSITIVITY = 2f;

    bool _forward;
    bool _backward;
    bool _left;
    bool _right;
    bool _jump;

    float _yaw;
    float _pitch;

    CapsuleMotor _motor;

    void Awake()
    {
        _motor = GetComponent<CapsuleMotor>();
    }

    public override void Attached()
    {
        // This couples the Transform property of the State with the GameObject Transform
        state.SetTransforms(state.Transform, transform);
    }

    void PollKeys(bool mouse)
    {
        _forward = Input.GetKey(KeyCode.W);
        _backward = Input.GetKey(KeyCode.S);
        _left = Input.GetKey(KeyCode.A);
        _right = Input.GetKey(KeyCode.D);
        _jump = Input.GetKeyDown(KeyCode.Space);

        if (mouse)
        {
            _yaw += (Input.GetAxisRaw("Mouse X") * MOUSE_SENSITIVITY);
            _yaw %= 360f;

            _pitch += (-Input.GetAxisRaw("Mouse Y") * MOUSE_SENSITIVITY);
            _pitch = Mathf.Clamp(_pitch, -85f, +85f);
        }
    }

    void Update()
    {
        PollKeys(true);
    }

    public override void SimulateController()
    {
        PollKeys(false);

        ICapsuleCommandInput input = CapsuleCommand.Create();

        input.Forward = _forward;
        input.Backward = _backward;
        input.Left = _left;
        input.Right = _right;
        input.Jump = _jump;
        input.Yaw = _yaw;
        input.Pitch = _pitch;

        entity.QueueInput(input);
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        CapsuleCommand cmd = (CapsuleCommand)command;

        if (resetState)
        {
            // we got a correction from the server, reset (this only runs on the client)
            _motor.SetState(cmd.Result.Position, cmd.Result.Velocity, cmd.Result.IsGrounded, cmd.Result.JumpFrames);
        }
        else
        {
            // apply movement (this runs on both server and client)
            CapsuleMotor.State motorState = _motor.Move(cmd.Input.Forward, cmd.Input.Backward, cmd.Input.Left, cmd.Input.Right, cmd.Input.Jump, cmd.Input.Yaw);

            // copy the motor state to the commands result (this gets sent back to the client)
            cmd.Result.Position = motorState.position;
            cmd.Result.Velocity = motorState.velocity;
            cmd.Result.IsGrounded = motorState.isGrounded;
            cmd.Result.JumpFrames = motorState.jumpFrames;
        }
    }
}
