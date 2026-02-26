using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Blocks", menuName = "Scriptable Objects/Blocks")]
public class Blocks : ScriptableObject
{
    public MonoScript blockLogic;
    public Color blockColor;
}
