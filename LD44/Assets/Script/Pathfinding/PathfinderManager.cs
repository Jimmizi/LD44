using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Pathfinding;
using UnityEngine;

namespace Pathfinding
{

    public class PathfinderManager : MonoBehaviour
    {

        public int maxThreads = 3;

        private List<Pathfinder> jobs;
        private List<Pathfinder> queue;

        void Start()
        {
            jobs = new List<Pathfinder>();
            queue = new List<Pathfinder>();

        }

        void Update()
        {

            // TODO: Temporary
            if (Input.GetKeyDown("space"))
            {
                RequestPathfind(new Vector3(0, 0, 0), new Vector3(6, 6, 0));
            }


            int i = 0;

            while (i < jobs.Count)
            {
                if (jobs[i].done)
                {
                    jobs.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            if (queue.Count > 0 && jobs.Count < maxThreads)
            {
                Pathfinder finder = queue[0];
                queue.RemoveAt(0);
                jobs.Add(finder);

                Thread thread = new Thread(finder.FindPath);
                thread.Start();

                /* As per the doc
                 * https://msdn.microsoft.com/en-us/library/system.threading.thread(v=vs.110).aspx
                 * It is not necessary to retain a reference to a Thread object once you have started the thread. 
                 * The thread continues to execute until the thread procedure is complete.
                 */

            }

        }

        public void RequestPathfind(Vector3 start, Vector3 target)
        {
            Pathfinder finder = new Pathfinder(start, target);
            queue.Add(finder);
        }


        // Singleton
        private static PathfinderManager instance;

        void Awake()
        {
            instance = this;
        }

        public static PathfinderManager GetInstance()
        {
            return instance;
        }
    }
}
