using HitMe.Characters;
using UnityEngine;
namespace HitMe.UI
{
    // Compatibility adapter for Sprint 2 callers/tests. CharacterVisual owns all rendering.
    [DisallowMultipleComponent]
    public sealed class CharacterPresentation : MonoBehaviour
    {
        public CharacterPose Pose { get; private set; }
        public CharacterVisual Visual { get; private set; }
        public void Initialize(CharacterManifest source,string id,HitMe.Characters.CharacterDefinition definition=null)
        { Visual=GetComponent<CharacterVisual>();if(Visual==null)Visual=gameObject.AddComponent<CharacterVisual>();Visual.Initialize(definition,source,id); }
        public void Present(CharacterPose pose,Color color)
        { Pose=pose;Visual.Present(CharacterVisual.FromLegacy(pose),color,Time.unscaledTime); }
    }
}
