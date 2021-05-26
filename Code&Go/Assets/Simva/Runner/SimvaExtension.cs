using AssetPackage;
using SimpleJSON;
using System;
using System.IO;
using uAdventure.Runner;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityFx.Async;
using UnityFx.Async.Promises;
using SimvaPlugin;
using Simva.Api;
using Simva.Model;
using Simva;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using uAdventure.Analytics;

namespace uAdventure.Simva
{
    public class SimvaExtension : MonoBehaviour
    {
        private static SimvaExtension instance;

        public delegate void LoadingDelegate(bool loading);
        public delegate void ResponseDelegate(string message);

        private LoadingDelegate loadingListeners;
        private ResponseDelegate responseListeners;
        private string savedGameTarget;
        private bool wasAutoSave;
        private bool firstTimeDisabling = true;

        private AuthorizationInfo auth;
        private Schedule schedule;
        private SimvaApi<IStudentsApi> simvaController;

        private TrackerConfig trackerConfig;
        private float nextFlush = 0;
        private bool flushRequested = true;

        private IRunnerChapterTarget runner;

        protected void Awake()
        {
            instance = this;
        }

        public static SimvaExtension Instance
        {
            get
            {
                return instance;
            }
        }

        public Schedule Schedule
        {
            get
            {
                return schedule;
            }
        }

        public SimvaApi<IStudentsApi> API
        {
            get
            {
                return simvaController;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.simvaController != null && this.auth != null && Schedule != null;
            }
        }

        public string CurrentActivityId
        {
            get
            {
                if (schedule != null)
                {
                    return Schedule.Next;
                }
                return null;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(SimvaConf.Local.Study);
            }
        }

        public SimvaBridge SimvaBridge { get; private set; }

        private void Update()
        {
            CheckTrackerFlush();
        }

        public IEnumerator OnAfterGameLoad()
        {
            TrackerConfig defaultTrackerConfig = new TrackerConfig();
            defaultTrackerConfig.setStorageType(TrackerConfig.StorageType.LOCAL);
            defaultTrackerConfig.setTraceFormat(TrackerConfig.TraceFormat.XAPI);
            defaultTrackerConfig.setRawCopy(true);
            defaultTrackerConfig.setDebug(true);

            Debug.Log("[SIMVA] Starting...");
            if(SimvaConf.Local == null)
            {
                SimvaConf.Local = new SimvaConf();
                yield return StartCoroutine(SimvaConf.Local.LoadAsync());
                Debug.Log("[SIMVA] Conf Loaded...");
            }

            if (!IsEnabled)
            {
                Debug.Log("[SIMVA] Study is not set! Stopping...");
                yield return StartTracker(defaultTrackerConfig);
                yield break;
            }
            else if (IsActive)
            {
                Debug.Log("[SIMVA] Simva is already started...");
                // No need to restart
                yield break;
            }
            else
            {
                Debug.Log("[SIMVA] Setting current target to Simva.Login...");
                savedGameTarget = SceneManager.GetActiveScene().name;
                LoadManager.Instance.AutoStart = false;
                yield return RunTarget("Simva.Login");
                yield return OnGameReady();
            }
        }

        // TODO: llamar cuando se guarde
        public void OnBeforeGameSave()
        {
            if(auth != null)
            {
                PlayerPrefs.SetString("simva_auth", JsonConvert.SerializeObject(auth));
            }
        }

        public IAsyncOperation backupOperation;
        public Activity backupActivity;

        private bool afterFlush;
        public void AfterFlush()
        {
            afterFlush = true;
        }

        private bool afterBackup;
        public void AfterBackup()
        {
            afterBackup = true;
        }

        public IEnumerator OnGameFinished()
        {
            if (IsActive)
            {
                var readyToClose = false;
                yield return RunTarget("Simva.FlushAll");
                yield return new WaitUntil(() => afterFlush);
                Continue(CurrentActivityId, true)
                    .Then(() => readyToClose = true);

                yield return new WaitUntil(() => readyToClose);
            }
            else
            {
                yield return FinishTracker();
            }
        }

        public IEnumerator FinishTracker()
        {
            if (TrackerAsset.Instance.Active)
            {
                var flushed = false;
                TrackerAsset.Instance.ForceCompleteTraces();
                TrackerAsset.Instance.FlushAll(() => flushed = true);
                var time = Time.time;
                yield return new WaitUntil(() => flushed);
                TrackerAsset.Instance.Stop();
            }
        }

        public IEnumerator OnGameReady()
        {
            if (PlayerPrefs.HasKey("simva_auth"))
            {
                NotifyLoading(true);
                this.auth = JsonConvert.DeserializeObject<AuthorizationInfo>(PlayerPrefs.GetString("simva_auth"));
                this.auth.ClientId = "uadventure"; // TODO: poner id especifico
                SimvaApi<IStudentsApi>.Login(this.auth)
                    .Then(simvaController =>
                {
                    this.auth = simvaController.AuthorizationInfo;
                    this.simvaController = simvaController;
                    return UpdateSchedule();
                })
                .Then(schedule =>
                {
                    var result = new AsyncCompletionSource();
                    StartCoroutine(AsyncCoroutine(LaunchActivity(schedule.Next), result));
                    return result;
                })
                .Catch(error =>
                {
                    NotifyLoading(false);
                    NotifyManagers(error.Message);
                })
                .Finally(() =>
                {
                    OpenIdUtility.tokenLogin = false;
                });

            }
            else if (HasLoginInfo())
            {
                ContinueLoginAndSchedule();
            }
            yield return null;
        }


        public void InitUser()
        {
            LoginAndSchedule();
        }

        public void LoginAndSchedule()
        {
            NotifyLoading(true);
            OpenIdUtility.tokenLogin = true;
            SimvaApi<IStudentsApi>.Login()
                .Then(simvaController =>
                {
                    this.auth = simvaController.AuthorizationInfo;
                    this.simvaController = simvaController;
                    return UpdateSchedule();
                })
                .Then(schedule =>
                {
                    var result = new AsyncCompletionSource();
                    StartCoroutine(AsyncCoroutine(LaunchActivity(schedule.Next), result));
                    return result;
                })
                .Catch(error =>
                {
                    NotifyLoading(false);
                    NotifyManagers(error.Message);
                })
                .Finally(()=>
                {
                    OpenIdUtility.tokenLogin = false;
                });
        }

        public void LoginAndSchedule(string token)
        {
            NotifyLoading(true);
            SimvaApi<IStudentsApi>.LoginWithToken(token)
                .Then(simvaController =>
                {
                    this.auth = simvaController.AuthorizationInfo;
                    this.simvaController = simvaController;
                    PlayerPrefs.SetString("simva_auth", JsonConvert.SerializeObject(auth));
                    PlayerPrefs.Save();
                    return UpdateSchedule();
                })
                .Then(schedule =>
                {
                    var result = new AsyncCompletionSource();
                    StartCoroutine(AsyncCoroutine(LaunchActivity(schedule.Next), result));
                    return result;
                })
                .Catch(error =>
                {
                    NotifyLoading(false);
                    NotifyManagers(error.Message);
                });
        }

        public void ContinueLoginAndSchedule()
        {
            NotifyLoading(true);
            SimvaApi<IStudentsApi>.ContinueLogin()
                .Then(simvaController =>
                {
                    this.auth = simvaController.AuthorizationInfo;
                    this.simvaController = simvaController;
                    return UpdateSchedule();
                })
                .Then(schedule =>
                {
                    var result = new AsyncCompletionSource();
                    StartCoroutine(AsyncCoroutine(LaunchActivity(schedule.Next), result));
                    return result;
                })
                .Catch(error =>
                {
                    NotifyLoading(false);
                    NotifyManagers(error.Message);
                })
                .Finally(() =>
                {
                    OpenIdUtility.tokenLogin = false;
                });
        }

        public IAsyncOperation<Schedule> UpdateSchedule()
        {
            var result = new AsyncCompletionSource<Schedule>(); 
                
            simvaController.Api.GetSchedule(simvaController.SimvaConf.Study)
                .Then(schedule =>
                {
                    this.schedule = schedule;
                    foreach(var a in schedule.Activities)
                    {
                        schedule.Activities[a.Key].Id = a.Key;
                    }
                    Debug.Log("[SIMVA] Schedule: " + JsonConvert.SerializeObject(schedule));
                    result.SetResult(schedule);
                })
                .Catch(result.SetException);
            return result;
        }


        public IAsyncOperation SaveActivity(string activityId, string traces, bool completed)
        {
            NotifyLoading(true);

            var body = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(traces))
            {
                body.Add("tofile", true);
                body.Add("result", traces);
            }

            var result = new AsyncCompletionSource();

            var response = (AsyncCompletionSource) API.Api.SetResult(activityId, API.AuthorizationInfo.Username, body);
            response.AddProgressCallback((p) =>
             {
                 UnityEngine.Debug.Log("SaveActivityAndContinue progress: " + p);
                 if (!result.IsCompleted && !result.IsCanceled)
                 {
                     result.SetProgress(p);
                 }
             });

            response
                .Then(() =>
                {
                    NotifyLoading(false);
                    result.SetCompleted();
                })
                .Catch(e => {
                    result.SetException(e);
                });

            return result;
        }

        public IAsyncOperation Continue(string activityId, bool completed)
        {
            NotifyLoading(true);
            return API.Api.SetCompletion(activityId, API.AuthorizationInfo.Username, completed)
                .Then(() =>
                {
                    backupActivity = GetActivity(CurrentActivityId);
                    string activityType = backupActivity.Type;
                    if (activityType.Equals("gameplay", StringComparison.InvariantCultureIgnoreCase)
                    && backupActivity.Details != null && backupActivity.Details.ContainsKey("backup") && (bool)backupActivity.Details["backup"])
                    {
                        //This only works in windows player
                        Application.runInBackground = true;
                        string traces = SimvaBridge.Load(((TrackerAssetSettings)TrackerAsset.Instance.Settings).BackupFile);
                        backupOperation = SaveActivity(CurrentActivityId, traces, true);
                        backupOperation.Then(() =>
                        {
                            afterBackup = true;
                            //This only works in windows player
                            Application.runInBackground = false;
                        });
                    }

                    return UpdateSchedule();
                })
                .Then(schedule =>
                {
                    NotifyLoading(false);
                    var result = new AsyncCompletionSource();
                    StartCoroutine(AsyncCoroutine(LaunchActivity(schedule.Next), result));
                    return result;
                })
                .Catch(error =>
                {
                    NotifyLoading(false);
                    NotifyManagers(error.Message);
                });
        }

        public Activity GetActivity(string activityId)
        {
            if (schedule != null)
            {
                return Schedule.Activities[activityId];
            }
            return null;
        }

        public IEnumerator LaunchActivity(string activityId)
        {
            if (activityId == null)
            {
                RunTarget("Simva.End");
                schedule = null;
            }
            else
            {
                Activity activity = GetActivity(activityId);

                if (activity != null)
                {
                    Debug.Log("[SIMVA] Schedule: " + activity.Type + ". Name: " + activity.Name + " activityId " + activityId);
                    switch (activity.Type)
                    {
                        case "limesurvey":
                            Debug.Log("[SIMVA] Starting Survey...");
                            RunTarget("Simva.Survey");
                            break;
                        case "gameplay":
                        default:
                            var trackerConfig = new TrackerConfig();

                            trackerConfig.setStorageType(TrackerConfig.StorageType.LOCAL);
                            trackerConfig.setTraceFormat(TrackerConfig.TraceFormat.XAPI);
                            trackerConfig.setRawCopy(true);
                            trackerConfig.setDebug(true);

                            if (ActivityHasDetails(activity, "realtime", "trace_storage"))
                            {
                                // Realtime
                                trackerConfig.setStorageType(TrackerConfig.StorageType.NET);
                                trackerConfig.setHost(simvaController.SimvaConf.URL);
                                trackerConfig.setBasePath("");
                                trackerConfig.setLoginEndpoint("/users/login");
                                trackerConfig.setStartEndpoint("/activities/{0}/result");
                                trackerConfig.setTrackEndpoint("/activities/{0}/result");
                                trackerConfig.setTrackingCode(activityId);
                                trackerConfig.setUseBearerOnTrackEndpoint(true);
                                Debug.Log("TrackingCode: " + activity.Id + " settings " + trackerConfig.getTrackingCode());
                            }

                            if (ActivityHasDetails(activity, "backup"))
                            {
                                // Local
                                trackerConfig.setRawCopy(true);
                            }

                            if (ActivityHasDetails(activity, "realtime", "trace_storage", "backup"))
                            {
                                SimvaBridge = new SimvaBridge(API.ApiClient);
                                Debug.Log("[SIMVA] Starting tracker...");
                                yield return StartTracker(trackerConfig, SimvaBridge);
                            }

                            DestroyImmediate(runner.gameObject);
                            yield return SceneManager.UnloadSceneAsync("Simva");

                            Debug.Log("[SIMVA] Starting Gameplay...");
                            LoadManager.Instance.LoadScene("MenuScene");

                            break;
                    }
                }
                else if(backupOperation != null && !backupOperation.IsCompleted)
                {
                    yield return RunTarget("Simva.Backup");
                }
            }
        }

        public IEnumerator StartTracker(TrackerConfig config, IBridge bridge = null)
        {
            trackerConfig = config;
            string domain = "";
            int port = 80;
            bool secure = false;

            Debug.Log("[ANALYTICS] Setting up tracker...");
            try
            {
                if (config.getHost() != "")
                {
                    string[] splitted = config.getHost().Split('/');

                    if (splitted.Length > 1)
                    {
                        string[] host_splitted = splitted[2].Split(':');
                        if (host_splitted.Length > 0)
                        {
                            domain = host_splitted[0];
                            port = (host_splitted.Length > 1) ? int.Parse(host_splitted[1]) : (splitted[0] == "https:" ? 443 : 80);
                            secure = splitted[0] == "https:";
                        }
                    }
                }
                else
                {
                    config.setHost("localhost");
                }
                Debug.Log("[ANALYTICS] Config: " + JsonConvert.SerializeObject(config));
            }
            catch (System.Exception e)
            {
                Debug.Log("Tracker error: Host bad format");
            }

            TrackerAsset.TraceFormats format;
            switch (config.getTraceFormat())
            {
                case TrackerConfig.TraceFormat.XAPI:
                    format = TrackerAsset.TraceFormats.xapi;
                    break;
                default:
                    format = TrackerAsset.TraceFormats.csv;
                    break;
            }
            Debug.Log("[ANALYTICS] Format: " + format);

            TrackerAsset.StorageTypes storage;
            switch (config.getStorageType())
            {
                case TrackerConfig.StorageType.NET:
                    storage = TrackerAsset.StorageTypes.net;
                    break;
                default:
                    storage = TrackerAsset.StorageTypes.local;
                    break;
            }
            Debug.Log("[ANALYTICS] Storage: " + storage);

            TrackerAssetSettings tracker_settings = new TrackerAssetSettings()
            {
                Host = domain,
                TrackingCode = config.getTrackingCode(),
                BasePath = trackerConfig.getBasePath() ?? "/api",
                LoginEndpoint = trackerConfig.getLoginEndpoint() ?? "login",
                StartEndpoint = trackerConfig.getStartEndpoint() ?? "proxy/gleaner/collector/start/{0}",
                TrackEndpoint = trackerConfig.getStartEndpoint() ?? "proxy/gleaner/collector/track",
                Port = port,
                Secure = secure,
                StorageType = storage,
                TraceFormat = format,
                BackupStorage = config.getRawCopy(),
                UseBearerOnTrackEndpoint = trackerConfig.getUseBearerOnTrackEndpoint()
            };
            Debug.Log("[ANALYTICS] Settings: " + JsonConvert.SerializeObject(tracker_settings));
            TrackerAsset.Instance.StrictMode = false;
            TrackerAsset.Instance.Bridge = bridge ?? new UnityBridge();
            TrackerAsset.Instance.Settings = tracker_settings;
            TrackerAsset.Instance.StrictMode = false;

            var done = false;

            Debug.Log("[ANALYTICS] Starting tracker without login...");
            TrackerAsset.Instance.StartAsync(() => done = true);
            
            this.nextFlush = config.getFlushInterval();

            Debug.Log("[ANALYTICS] Waiting until start");
            yield return new WaitUntil(() => done);
            Debug.Log("[ANALYTICS] Start done, result: " + TrackerAsset.Instance.Started);
        }

        private void CheckTrackerFlush()
        {
            if (!TrackerAsset.Instance.Started && !TrackerAsset.Instance.Active)
            {
                return;
            }

            float delta = Time.deltaTime;
            if (trackerConfig.getFlushInterval() >= 0)
            {
                nextFlush -= delta;
                if (nextFlush <= 0)
                {
                    flushRequested = true;
                }
                while (nextFlush <= 0)
                {
                    nextFlush += trackerConfig.getFlushInterval();
                }
            }
            if (flushRequested)
            {
                flushRequested = false;
                TrackerAsset.Instance.Flush();
            }
        }

        private IEnumerator RunTarget(string target)
        {
            if(runner != null)
            {
                DestroyImmediate(runner.gameObject);
            }
            else
            {
                yield return SceneManager.LoadSceneAsync("Simva", LoadSceneMode.Additive);
            }
            runner = SimvaSceneHandler.Instantiate(target);
            runner.RenderScene();

            yield return new WaitUntil(() => runner.IsReady);
        }

        private bool ActivityHasDetails(Activity activity, params string[] details)
        {
            if (activity.Details == null)
            {
                return false;
            }

            return details.Any(d => IsTrue(activity.Details, d));
        }

        private static bool IsTrue(Dictionary<string, object> details, string key)
        {
            return details.ContainsKey(key) && details[key] is bool && (bool)details[key];
        }

        // NOTIFIERS

        public void AddResponseManager(SimvaResponseManager manager)
        {
            if (manager)
            {
                // To make sure we only have one instance of a notify per manager
                // We first remove (as it is ignored if not present)
                responseListeners -= manager.Notify;
                // Then we add it
                responseListeners += manager.Notify;
            }
        }

        public void RemoveResponseManager(SimvaResponseManager manager)
        {
            if (manager)
            {
                // If a delegate is not present the method gets ignored
                responseListeners -= manager.Notify;
            }
        }

        public void AddLoadingManager(SimvaLoadingManager manager)
        {
            if (manager)
            {
                // To make sure we only have one instance of a notify per manager
                // We first remove (as it is ignored if not present)
                loadingListeners -= manager.IsLoading;
                // Then we add it
                loadingListeners += manager.IsLoading;
            }
        }

        public void RemoveLoadingManager(SimvaLoadingManager manager)
        {
            if (manager)
            {
                // If a delegate is not present the method gets ignored
                loadingListeners -= manager.IsLoading;
            }
        }

        public void NotifyManagers(string message)
        {
            if (responseListeners != null)
            {
                responseListeners(message);
            }
        }

        public void NotifyLoading(bool state)
        {
            if (loadingListeners != null)
            {
                loadingListeners(state);
            }
        }

        private static bool HasLoginInfo()
        {
            return OpenIdUtility.HasLoginInfo();
        }

        public bool canBeInteracted()
        {
            return false;
        }

        public void setInteractuable(bool state)
        {
        }

        internal IEnumerator AsyncCoroutine(IEnumerator coroutine, IAsyncCompletionSource op)
        {
            yield return coroutine;
            op.SetCompleted();
        }
    }
}
