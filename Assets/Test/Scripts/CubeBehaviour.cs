using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
public class CubeBehaviour : EntityEventListener<ICubeState>
{
    private float _resetColorTime;
    private Renderer _renderer;

    private CharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    public override void Attached()
    {
        _renderer = GetComponent<Renderer>();


        if (entity.IsOwner)
        {
            state.CubeColor = new Color(Random.value, Random.value, Random.value);
        }

        state.AddCallback("CubeColor", ColorChanged);//Property callback. Like a delegate
    }

    public Vector3 Move(bool forward, bool backward, bool left, bool right)//Similar to Update() but from Bolt
    {
        float speed = 4f;
        Vector3 movement = Vector3.zero;

        if (forward) { movement.z += 1; }
        if (backward) { movement.z -= 1; }
        if (left) { movement.x -= 1; }
        if (right) { movement.x += 1; }

        if (movement != Vector3.zero)
        {
            _cc.Move(transform.position + (movement.normalized * speed * BoltNetwork.FrameDeltaTime));
        }

        return transform.position;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;

        _cc.Move(position - transform.localPosition);
    }
    public override void OnEvent(FlashColorEvent evnt)
    {
        _resetColorTime = Time.time + 0.2f;
        _renderer.material.color = evnt.FlashColor;
    }
    void Update() //I believe we are using the Update method because we are keeping track of Time.time. I have to check it out later.
    {
        if (_resetColorTime < Time.time)
        {
            _renderer.material.color = state.CubeColor;
        }
    }

    private void OnGUI()
    {
        if (entity.IsOwner)
        {
            GUI.color = state.CubeColor;
            GUILayout.Label("@@@");
            GUI.color = Color.white;
        }
    }

    private void ColorChanged()//PropertyCallback-->it is invoked when the property changes value
    {
        _renderer.material.color = state.CubeColor;
    }
}

//// NEW: On the owner, we want to setup the weapons, the Id is set just as the index
//// and the Ammo is randomized between 50 to 100
//for (int i = 0; i < state.WeaponsArray.Length; i++)
//{
//    state.WeaponsArray[i].WeaponID = i;
//    state.WeaponsArray[i].WeaponAmmo = Random.Range(50,100);
//}

////NEW: by default we don't have any weapon up, so set index to -1
//state.WeaponActiveIndex = -1;

// NEW: we also setup a callback for whenever the index changes
//state.AddCallback("WeaponActiveIndex", WeaponActiveIndexChanged);
