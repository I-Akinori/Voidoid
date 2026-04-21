using System.Collections.Generic;
using UnityEngine;

namespace Voidoid.Note
{
    [CreateAssetMenu(menuName = "Score/ScoreData")]
    public class ScoreData : ScriptableObject
    {
        public List<Note> score = new ();
    }
}