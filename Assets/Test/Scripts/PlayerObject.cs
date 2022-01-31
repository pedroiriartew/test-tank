using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject
{
    public BoltEntity character;//field will contain the instantiated object which represents the player's character in the world.
    public BoltConnection connection;//field will contain the connection to this player if one exists, this will be null on the server for the servers player object.

    public bool IsServer
    {
        get { return connection == null; }
    }

    public bool IsClient
    {
        get { return connection != null; }
    }

    public void Spawn()
    {
        if (!character)
        {
            character = BoltNetwork.Instantiate(BoltPrefabs.Cube, RandomPosition(), Quaternion.identity);

            if (IsServer)
            {
                character.TakeControl();
            }
            else
            {
                character.AssignControl(connection);
            }
        }

        // teleport entity to a random spawn position
        character.transform.position = RandomPosition();
    }

    private Vector3 RandomPosition()
    {
        float x = Random.Range(-32f, +32f);
        float z = Random.Range(-32f, +32f);
        return new Vector3(x, 0f, z);
    }
}
