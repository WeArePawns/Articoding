using UnityEngine;
using UniRx;
using UnityFx.Async.Promises;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Simva;
using UnityFx.Async;
using AssetPackage;
using uAdventure.Runner;

namespace uAdventure.Simva
{
    // Manager for "Simva.End"
    public class BackupController : MonoBehaviour, IRunnerChapterTarget
    {
        public Scrollbar progressBar;
        public Text successText;
        public Text errorText;
        private bool progressCallbackAdded;

        public object Data { get { return null; } set { } }

        public bool IsReady { get { return true; } }

        protected void OnApplicationResume()
        {
        }

        public void Update()
        {
            if(SimvaExtension.Instance.backupOperation != null)
            {
                Debug.Log("Last progress: " + SimvaExtension.Instance.backupOperation.Progress);
                if (!progressCallbackAdded)
                {
                    ((AsyncCompletionSource)SimvaExtension.Instance.backupOperation).AddProgressCallback((p) =>
                    {
                        progressBar.size = p;
                    });
                    progressCallbackAdded = true;
                }

                if (SimvaExtension.Instance.backupOperation.IsCompletedSuccessfully)
                {
                    progressBar.transform.parent.gameObject.SetActive(false);
                    successText.gameObject.SetActive(true);
                    errorText.gameObject.SetActive(false);
                }
                else if (SimvaExtension.Instance.backupOperation.IsFaulted)
                {
                    progressBar.transform.parent.gameObject.SetActive(false);
                    successText.gameObject.SetActive(false);
                    errorText.gameObject.SetActive(true);
                    var errorMessage = errorText.transform.GetChild(0).GetComponent<Text>();
                    errorMessage.text = SimvaExtension.Instance.backupOperation.Exception.Message;
                }
                else
                {
                    progressBar.transform.parent.gameObject.SetActive(true);
                    successText.gameObject.SetActive(false);
                    errorText.gameObject.SetActive(false);
                }
            }
        }

        public void Retry()
        {
            progressCallbackAdded = false;

            //This only works in windows player
            Application.runInBackground = true;
            var se = SimvaExtension.Instance;
            string traces = se.SimvaBridge.Load(((TrackerAssetSettings)TrackerAsset.Instance.Settings).BackupFile);
            se.backupOperation = se.SaveActivity(se.backupActivity.Id, traces, true);
            se.backupOperation.Then(() =>
            {
                se.AfterBackup();
                //This only works in windows player
                Application.runInBackground = false;
            });
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void RenderScene()
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
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

