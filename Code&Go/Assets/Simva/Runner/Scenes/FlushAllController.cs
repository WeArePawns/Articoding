using UnityEngine;
using uAdventure.Runner;
using System;
using System.Collections;

namespace uAdventure.Simva
{
    // Manager for "Simva.End"
    public class FlushAllController : MonoBehaviour, IRunnerChapterTarget
    {
        private bool ready;

        public object Data { get { return null; } set { } }

        public bool IsReady { get { return ready; } }

        protected void OnApplicationResume()
        {
        }

        public void RenderScene()
        {
            SimvaExtension.Instance.NotifyLoading(true);
            this.transform.GetChild(0).gameObject.SetActive(true);
            StartCoroutine(FinishTracker());
            ready = true;
        }

        private IEnumerator FinishTracker()
        {
            yield return SimvaExtension.Instance.FinishTracker();
            SimvaExtension.Instance.AfterFlush();
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

