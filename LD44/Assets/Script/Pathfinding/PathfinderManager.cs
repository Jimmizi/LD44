using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Pathfinding;
using UnityEngine;

namespace Pathfinding
{

    /// <summary>
    /// Manages all pathfinding requests. Dispatches work concurrently.
    /// </summary>

    public class PathfinderManager : MonoBehaviour
    {

        public int maxThreads = 3;
        
        public delegate void PathfindingComplete(List<Node> path);

        private List<Pathfinder> _jobs;
        private List<Pathfinder> _queue;

        void Start()
        {
            _jobs = new List<Pathfinder>();
            _queue = new List<Pathfinder>();

        }
    
        // TODO: Temporary
        private void TestCallback(List<Node> path)
        {
            if (path != null)
            {
                Debug.Log("Path found.");
            }
            else
            {
                Debug.Log("Path not found.");
            }
        }
        
        void Update()
        {

            // TODO: Temporary
            if (Input.GetKeyDown("space"))
            {
                RequestPathfind(new Vector3(0, 0, 0), new Vector3(6, 6, 0), TestCallback);
            }


            int i = 0;

            while (i < _jobs.Count)
            {
                if (_jobs[i].done)
                {
                    _jobs[i].NotifyComplete();
                    _jobs.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            if (_queue.Count > 0 && _jobs.Count < maxThreads)
            {
                Pathfinder finder = _queue[0];
                _queue.RemoveAt(0);
                _jobs.Add(finder);

                Thread thread = new Thread(finder.FindPath);
                thread.Start();

                /* As per the doc
                 * https://msdn.microsoft.com/en-us/library/system.threading.thread(v=vs.110).aspx
                 * It is not necessary to retain a reference to a Thread object once you have started the thread. 
                 * The thread continues to execute until the thread procedure is complete.
                 */

            }

        }

        public void RequestPathfind(Vector3 start, Vector3 target, PathfindingComplete callback)
        {
            Pathfinder finder = new Pathfinder(start, target, callback);
            _queue.Add(finder);
        }


        // Singleton
        private static PathfinderManager _instance;

        void Awake()
        {
            _instance = this;
        }

        public static PathfinderManager GetInstance()
        {
            return _instance;
        }
    }
}
