using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Keybinds", menuName = "Keybinds")]
public class PlayerInputs : ScriptableObject
{
    public KeyCode attack;
    public KeyCode dash;
}
