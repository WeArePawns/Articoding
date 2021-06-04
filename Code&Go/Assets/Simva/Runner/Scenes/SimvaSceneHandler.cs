using uAdventure.Runner;
using UnityEngine;
using UnityEngine.EventSystems;

namespace uAdventure.Simva
{
    public class SimvaSceneHandler
    {
        public static IRunnerChapterTarget Instantiate(string scene)
        {
            GameObject form = null;
            switch (scene)
            {
                case "Simva.Login":
                    form = GameObject.Instantiate(Resources.Load<GameObject>("SimvaLogin"));
                    break;
                case "Simva.Survey":
                    form = GameObject.Instantiate(Resources.Load<GameObject>("SimvaSurvey"));
                    break;
                case "Simva.FlushAll":
                    form = GameObject.Instantiate(Resources.Load<GameObject>("SimvaFlushAll"));
                    break;
                case "Simva.Backup":
                    form = GameObject.Instantiate(Resources.Load<GameObject>("SimvaBackup"));
                    break;
                case "Simva.End":
                    form = GameObject.Instantiate(Resources.Load<GameObject>("SimvaEnd"));
                    break;
            }

            if (form != null)
            {
                var runner = form.GetComponent<IRunnerChapterTarget>();
                runner.Data = scene;
                return runner;
            }

            return null;
        }
    }
}
