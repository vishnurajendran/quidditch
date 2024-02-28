using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AgentControllers;

namespace BT
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE,
    }

    public class BaseNode
    {
        public NodeState state;
        public BaseNode parent;
        protected List<BaseNode> children = new List<BaseNode>();
        private Dictionary<string, object> context = new Dictionary<string, object>();
        public AgentController actor = null;

        public BaseNode()
        {
            parent = null;
            actor = null;
        }

        //initialize the action node
        public BaseNode(AgentController actor_)
        {
            parent = null;
            actor = actor_;
        }

        //initialize the composed nodes
        public BaseNode(List<BaseNode> children)
        {
            for (int i = 0; i < children.Count; i++)
            {
                AttackChild(children[i]);
            }
        }

        private void AttackChild(BaseNode node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Process() => NodeState.FAILURE;

        public void SetContext(string key, object value)
        {
            context[key] = value;
        }

        public object GetContext(string key)
        {
            object value = null;
            //local context has the key
            if (context.TryGetValue(key, out value) && value != null)
                return value;

            //recurssive search the root node
            BaseNode node = parent;
            while (node != null)
            {
                value = node.GetContext(key);
                if (value != null) return value;
                node = node.parent;
            }
            return null;
        }

        public bool ClearContext(string key)
        {
            object value = null;
            if (context.TryGetValue(key, out value))
            {
                context.Remove(key);
                return true;
            }

            //recurssive remove nodes in the behaviour tree
            BaseNode node = parent;
            while (node != null)
            {
                bool cleared = node.ClearContext(key);
                if (cleared)
                    return true;
                node = node.parent;
            }
            return false;
        }
    }

    public class ParallelNode : BaseNode
    {
        public ParallelNode() : base() { }
        public ParallelNode(List<BaseNode> children) : base(children) { }
        public override NodeState Process()
        {
            state = NodeState.SUCCESS;
            for (int i = 0; i < children.Count; ++i)
            {
                NodeState subNodeState = children[i].Process();

                if (state != NodeState.FAILURE && subNodeState == NodeState.RUNNING)
                {
                    state = NodeState.RUNNING;
                }

                if (subNodeState == NodeState.FAILURE)
                {
                    state = NodeState.FAILURE;
                }
            }
            return state;
        }
    }

    public class SelectorNode : BaseNode
    {
        public SelectorNode() : base() { }
        public SelectorNode(List<BaseNode> children) : base(children) { }
        public override NodeState Process()
        {
            for (int i = 0; i < children.Count; ++i)
            {
                NodeState subNodeState = children[i].Process();
                if (subNodeState == NodeState.RUNNING)
                {
                    state = NodeState.RUNNING;
                    return state;
                }
                else if (subNodeState == NodeState.SUCCESS)
                {
                    state = NodeState.SUCCESS;
                    return state;
                }
                else if (subNodeState == NodeState.FAILURE)
                {
                    continue;
                }
            }
            state = NodeState.FAILURE;
            return state;
        }
    }


    public class SequenceNode : BaseNode
    {
        public SequenceNode() : base() { }
        public SequenceNode(List<BaseNode> children) : base(children) { }

        public override NodeState Process()
        {
            bool isRunning = false;

            for (int i = 0; i < children.Count; ++i)
            {
                NodeState subNodeState = children[i].Process();
                if (subNodeState == NodeState.RUNNING)
                {
                    isRunning = true;
                }
                else if (subNodeState == NodeState.SUCCESS)
                {
                    continue;
                }
                else if (subNodeState == NodeState.FAILURE)
                {
                    state = NodeState.FAILURE;
                    return state;
                }
            }
            state = isRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}
