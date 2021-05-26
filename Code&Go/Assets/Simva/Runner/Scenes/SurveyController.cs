using UnityEngine;
using UniRx;
using UnityFx.Async.Promises;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityFx.Async;
using Simva.Model;
using uAdventure.Runner;

namespace uAdventure.Simva
{
    // Manager for "Simva.Survey"
    public class SurveyController : MonoBehaviour , IRunnerChapterTarget
    {
        public CanvasGroup backupPopup;
        public Scrollbar progressBar;
        public Text successText;
        public Text errorText;
        private bool surveyOpened;
        private bool ready;
        private GameObject surveyOpener;
        private bool progressCallbackAdded;

        public object Data { get { return null; } set { } }

        public bool IsReady { get { return true; } }

        public void OpenSurvey()
        {
            SimvaExtension.Instance.NotifyLoading(true);
            string activityId = SimvaExtension.Instance.CurrentActivityId;
            string username = SimvaExtension.Instance.API.AuthorizationInfo.Username;
            SimvaExtension.Instance.API.Api.GetActivityTarget(activityId)
                .Then(result =>
                {
                    SimvaExtension.Instance.NotifyLoading(false);
                    surveyOpened = true;
                    Application.OpenURL(result[username]);
                })
                .Catch(error =>
                {
                    SimvaExtension.Instance.NotifyManagers(error.Message);
                    SimvaExtension.Instance.NotifyLoading(false);
                });
        }

        protected void OnApplicationResume()
        {
            if (surveyOpened)
            {
                surveyOpened = false;
                CheckSurvey();
            }
        }

        public void CheckSurvey()
        {
            SimvaExtension.Instance.NotifyLoading(true);
            string activityId = SimvaExtension.Instance.CurrentActivityId;
            string username = SimvaExtension.Instance.API.AuthorizationInfo.Username;
            SimvaExtension.Instance.API.Api.GetCompletion(activityId, username)
                .Then(result =>
                {
                    if (result[username])
                    {
                        return SimvaExtension.Instance.UpdateSchedule();
                    }
                    else
                    {
                        SimvaExtension.Instance.NotifyManagers("Survey not completed");
                        SimvaExtension.Instance.NotifyLoading(false);
                        var nullresult = new AsyncCompletionSource<Schedule>();
                        nullresult.SetResult(null);
                        return nullresult;
                    }
                })
                .Then(schedule =>
                {
                    var result = new AsyncCompletionSource();
                    if (schedule != null)
                    {
                        StartCoroutine(SimvaExtension.Instance.AsyncCoroutine(SimvaExtension.Instance.LaunchActivity(schedule.Next), result));
                    }
                    else
                    {
                        result.SetException(new Exception("No schedule!"));
                    }
                    return result;
                })
                .Catch(error =>
                {
                    SimvaExtension.Instance.NotifyManagers(error.Message);
                    SimvaExtension.Instance.NotifyLoading(false);
                });
        }

        public void Update()
        {
            if (SimvaExtension.Instance.backupOperation != null)
            {
                backupPopup.alpha = 1;
                Debug.Log("Last progress: " + SimvaExtension.Instance.backupOperation.Progress);
                /*if (!progressCallbackAdded)
                {
                    ((AsyncCompletionSource)SimvaExtension.Instance.backupOperation).AddProgressCallback((p) =>
                    {
                        progressBar.size = p;
                    });
                    progressCallbackAdded = true;
                }*/
                progressBar.size = SimvaExtension.Instance.backupOperation.Progress;

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

        public void RenderScene() 
        {
            //var background = GameObject.Find("background").GetComponent<Image>();
            /*var backgroundPath = 
            var backgroundSprite = Game.Instance.ResourceManager.getSprite();
            background.sprite = Game.Instance.ResourceManager.getSprite()*/
            backupPopup.alpha = 0;
            ready = true;
        }

        public void Destroy(float time, Action onDestroy)
        {
            GameObject.DestroyImmediate(gameObject);
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

