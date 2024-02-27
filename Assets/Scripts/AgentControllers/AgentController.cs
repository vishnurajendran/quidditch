using Agents;
using UnityEngine;

namespace AgentControllers
{
    [RequireComponent(typeof(Agent))]
    public class AgentController: MonoBehaviour
    {
        protected Agent _agent;
        protected virtual void Start()
        {
            _agent = GetComponent<Agent>();
        }
    }
}