using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private Vector3 GetRandomPointInBounds(Bounds bounds)
        {
	        return new Vector3(
		        Random.Range(bounds.min.x, bounds.max.x),
		        Random.Range(bounds.min.y, bounds.max.y),
		        Random.Range(bounds.min.z, bounds.max.z)
	        );
        }

        public bool IsPointWithinPlayableArea(Vector2 testPoint, bool testNotOnCharacter = false)
        {
	        var boundsGameObject = GameObject.FindGameObjectsWithTag("MapBounds");

	        if (boundsGameObject.Length > 0)
	        {
		        foreach (var boundsFound in boundsGameObject)
		        {
			        var applicableBounds = boundsFound.GetComponents<BoxCollider2D>();
			        List<Vector2> tempResults = new List<Vector2>();
			        bool containedWithinABounds = false;

			        foreach (var bound in applicableBounds)
			        {
				        if (bound.bounds.Contains(testPoint))
				        {
					        containedWithinABounds = true;
					        break;
				        }
			        }

			        if (testNotOnCharacter)
			        {
				        var allCharacters = GameObject.FindObjectsOfType<ActorStats>();

				        if (allCharacters.Any(actor =>
					        ((Vector2) actor.gameObject.transform.position - testPoint).magnitude <= 0.2f))
				        {
					        return false;
				        }
			        }

			        if (containedWithinABounds)
			        {
				        var tempNode = GridGenerator.GetInstance().NodeFromWorldPosition(testPoint);

				        return (tempNode != null && !tempNode.obstructed);
			        }
		        }
	        }

	        return false;
        }

		/// <summary>
		/// awayFromEnemies only for testing against infected
		/// </summary>
		/// <param name="awayFromEnemies"></param>
		/// <returns></returns>
		public Vector2 GetRandomPointInBounds(bool awayFromEnemies = false)
		{
			var boundsGameObject = GameObject.FindGameObjectWithTag("MapBounds");

			if (boundsGameObject)
			{
				var applicableBounds = boundsGameObject.GetComponents<BoxCollider2D>();
				List<Vector2> tempResults = new List<Vector2>();

				foreach (var bound in applicableBounds)
				{
					tempResults.Add(GetRandomPointInBounds(bound.bounds));
				}

				if (awayFromEnemies)
				{
					//Add a whole bunch more random points to work with
					for (int i = 0; i < 12; i++)
					{
						foreach (var bound in applicableBounds)
						{
							tempResults.Add(GetRandomPointInBounds(bound.bounds));
						}
					}

					var allCharacters = GameObject.FindObjectsOfType<ActorStats>();

					var furthestDist = 0.0f;
					var bestIndex = -1;
					var bestPosition = Vector2.zero;

					var averagePosition = Vector2.zero;
					var timesAdded = 0;

					//get average enemy position to weight results against
					for (int i = 0; i < allCharacters.Length; i++)
					{
						if (allCharacters[i].Infected)
						{
							averagePosition += (Vector2)allCharacters[i].gameObject.transform.position;
							timesAdded++;
						}
					}

					if (timesAdded > 0)
					{
						averagePosition /= timesAdded;
					}

					foreach (var result in tempResults)
					{
						var dist = (averagePosition - result).magnitude;

						if (dist > furthestDist)
						{
							furthestDist = dist;
							bestPosition = result;
						}
					}

					return bestPosition;
				}

				return tempResults[Random.Range(0, tempResults.Count)];
			}

			return new Vector2(0.0f, 0.0f);
		}
		
        void Update()
        {
			
            int i = 0;

            while (i < _jobs.Count)
            {
	            _jobs[i].FindPath();

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

            if (_queue.Count > 0)// && _jobs.Count < maxThreads)
            {
                Pathfinder finder = _queue[0];
                _queue.RemoveAt(0);
                _jobs.Add(finder);

                //Thread thread = new Thread(finder.FindPath);
                //thread.Start();

                ///* As per the doc
                // * https://msdn.microsoft.com/en-us/library/system.threading.thread(v=vs.110).aspx
                // * It is not necessary to retain a reference to a Thread object once you have started the thread. 
                // * The thread continues to execute until the thread procedure is complete.
                // */

            }

        }

        public bool RequestPathfind(Vector3 start, Vector3 target, AIController pathRequester, PathfindingComplete callback)
        {
	        if (!IsPointWithinPlayableArea(target))
	        {
		        return false;
	        }

            Pathfinder finder = new Pathfinder(start, target, pathRequester, callback);
            _queue.Add(finder);

            return true;
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
