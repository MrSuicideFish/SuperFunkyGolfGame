using UnityEngine;
using System.Collections;

namespace Routine{
    public class CoroutineHelper {

		private static RoutineHndl _routineHndl;
		private static RoutineHndl routineHndl {
			get {
				if(_routineHndl == null) {
					GameObject newRoutineHndlGO = new GameObject( "ROUTINE_HANDLE" );
					_routineHndl = newRoutineHndlGO.AddComponent<RoutineHndl>();
					GameObject.DontDestroyOnLoad( newRoutineHndlGO );
				}
				return _routineHndl;
			}
		}

        public delegate T RoutineDelegate<T>();
        public static void DoAfter(System.Action action, float timeToWait) {
			routineHndl.StartCoroutine( Routine_DoAfter( action, timeToWait ) );
        }

		public static void DoAfter( System.Action action, IEnumerator routine ) {
			routineHndl.StartCoroutine( Routine_DoAfter( action, routine ) );
		}

		public static IEnumerator Routine_DoAfter(System.Action action, IEnumerator routine ) {
			yield return routine;
			action();
		}

        public static IEnumerator Routine_DoAfter( System.Action action, float timeToWait ) {
            if (action == null) {
                yield break;
            }
            yield return new WaitForSeconds( timeToWait );
            action();
        }

        public static IEnumerator UntilTrue(System.Action action, RoutineDelegate<bool> eval, float interval = 0.2f ) {
            while (!eval()) {
                yield return null;
            }
            action();
        }

        public static IEnumerator EnumerateDeferred<T>(IEnumerator _e, System.Action<T> inputAction, float interval = 0.2f) {
            while (_e.MoveNext()) {
                T obj = (T)_e.Current;
                if(obj != null) {
                    inputAction( obj );
                }
                
                yield return new WaitForSeconds( interval );
            }
            yield break;
        }

        public static void DoWhile(RoutineDelegate<bool> eval, bool isValue, float interval = 0.05f ) {
			routineHndl.StartCoroutine( While( eval, isValue, interval ) );
        }

        public static IEnumerator While( RoutineDelegate<bool> eval, bool isValue, float interval) {
            while (eval() == isValue) {
                yield return new WaitForSeconds( interval );
            }
        }
    } 
}