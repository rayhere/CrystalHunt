using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum OffMeshLinkMoveMethod
{
    Teleport,
    NormalSpeed,
    Parabola,
    Curve
}

/// <summary>
/// Move an agent when traversing a OffMeshLink given specific animated methods
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class AgentLinkMover : MonoBehaviour
{
    public OffMeshLinkMoveMethod Method = OffMeshLinkMoveMethod.Parabola;
    public AnimationCurve Curve = new AnimationCurve();
    public delegate void LinkEvent();
    public LinkEvent OnLinkStart;
    public LinkEvent OnLinkEnd;


    IEnumerator Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;
        while (true)
        {
            if (agent.isOnOffMeshLink)
            {
                OnLinkStart?.Invoke();
                if (Method == OffMeshLinkMoveMethod.NormalSpeed)
                {
                    yield return StartCoroutine(NormalSpeed(agent));
                }
                else if (Method == OffMeshLinkMoveMethod.Parabola)
                {
                    yield return StartCoroutine(Parabola(agent, 2.0f, 0.5f));
                }
                else if (Method == OffMeshLinkMoveMethod.Curve)
                {
                    yield return StartCoroutine(MoveAlongCurve(agent, 0.5f));
                }
                agent.CompleteOffMeshLink();
                OnLinkEnd?.Invoke();
            }
            yield return null;
        }
    }


    IEnumerator NormalSpeed(NavMeshAgent agent)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        while (agent.transform.position != endPos)
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
            yield return null;
        }
    }


    IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }


    IEnumerator MoveAlongCurve(NavMeshAgent agent, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = Curve.Evaluate(normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }
}
