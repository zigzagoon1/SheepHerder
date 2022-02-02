using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
interface ICharacter
{
    void FSM_Die(FSM fsm, FSM.Step step, FSM.State state);
    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask);
    public IEnumerator FaceTarget();

}
