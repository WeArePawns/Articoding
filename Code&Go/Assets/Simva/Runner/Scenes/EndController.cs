using UnityEngine;
using UniRx;
using UnityFx.Async.Promises;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using uAdventure.Runner;

namespace uAdventure.Simva
{
    // Manager for "Simva.End"
    public class EndController : MonoBehaviour, IRunnerChapterTarget
    {
        private bool ready;

        public object Data { get { return null; } set { } }

        public bool IsReady { get { return ready; } }

        protected void OnApplicationResume()
        {
        }

        public void Quit()
        {
            Application.Quit();
        }


        public void RenderScene()
        {
            ready = true;
        }

        public void Destroy(float time, Action onDestroy)
        {
            GameObject.DestroyImmediate(this.gameObject);
            onDestroy();
        }

        public bool canBeInteracted()
        {
            return false;
        }

        public void setInteractuable(bool state)
        {
        }
    }
}

