using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class CubeController : EntityBehaviour<ICubeState>
{
    private bool _forward;
    private bool _backward;
    private bool _left;
    private bool _right;
    private bool _jump;

    private CubeBehaviour _cb;

    private void Awake()
    {
        _cb = GetComponent<CubeBehaviour>();
    }

    public override void Attached()
    {
        // This couples the Transform property of the State with the GameObject Transform
        state.SetTransforms(state.Transform, transform);//Recommended approach when you want to sync the Entity transform.
    }

    public void Pollkeys()
    {
        _forward = Input.GetKey(KeyCode.W);
        _backward = Input.GetKey(KeyCode.S);
        _left = Input.GetKey(KeyCode.A);
        _right = Input.GetKey(KeyCode.D);
        _jump = Input.GetKey(KeyCode.Space);
    }

    private void Update()
    {
        Pollkeys();
    }

    public override void SimulateController()
    {
        Pollkeys();

        ICubeCommandInput input = CubeCommand.Create();


        input.Forward = _forward;
        input.Backward = _backward;
        input.Left = _left;
        input.Right = _right;

        entity.QueueInput(input);
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        CubeCommand cmd = (CubeCommand)command;

        if (resetState)
        {
            _cb.SetPosition(cmd.Result.Position);
        }
        else
        {
            // apply movement (this runs on both server and client)
            Vector3 position = _cb.Move(cmd.Input.Forward, cmd.Input.Backward, cmd.Input.Left, cmd.Input.Right);

            //this gets sent back to the client
            cmd.Result.Position = position;
        }
    }
}
