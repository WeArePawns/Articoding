using UnityEngine;

namespace uAdventure.Runner
{
    public interface IRunnerChapterTarget
    {
        object Data { get; set; }
        void RenderScene();
        bool IsReady { get; }
        void Destroy(float time, System.Action onDestroy);
        GameObject gameObject { get; }
    }

}

