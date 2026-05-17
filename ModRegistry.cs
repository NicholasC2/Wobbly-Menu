using Rewired;
using UnityEngine;

public static class ModRegistry
{
    public static void RegisterAll()
    {
        RegisterVariables();
    }

    public static void RegisterVariables()
    {
        // var players = UnitySingleton<GameInstance>.Instance.GetLocalPlayerControllers();

        // foreach(PlayerController player in players)
        // {
        //     if (!player) continue;

        //     Transform t = player.GetPlayerTransform();
            
        //     VariableRegistry.Register(new Variable("pos-x", t, "position"));
        // }
    }
}