using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

[BoltGlobalBehaviour]//Only create an instance of this class
public class ServerCallbacks : GlobalEventListener
{
    private void Awake()
    {
        PlayerObjectRegistry.CreateServerPlayer();
    }

    public override void Connected(BoltConnection connection)
    {
        PlayerObjectRegistry.CreateClientPlayer(connection);
    }

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        PlayerObjectRegistry.ServerPlayer.Spawn();
    }

    public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
    {
        PlayerObjectRegistry.GetTutorialPlayer(connection).Spawn();
    }

}
