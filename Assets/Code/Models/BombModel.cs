using UnityEngine;
using Assets.Code.Models;

public class BombModel : IGameDataModel
    {

    public string Name { get; set; }
    public string ParticlePrefabName { get; set; }
    public int damage { get; set; }
    public Vector3 startPos { get; set; }
    public Vector3 endPos { get; set; }
    public Color color { get; set; }

}
