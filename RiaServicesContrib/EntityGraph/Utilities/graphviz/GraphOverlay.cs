
namespace Utilities.graphviz
{
    public abstract class GraphOverlay
    {
        private Graph Source;
        private Graph _graph;
        public Graph Graph
        {
            get
            {
                if(_graph == null)
                {
                    _graph = new Graph();
                    MakeOverlay(Source, _graph);
                }
                return _graph;
            }
        }
        public GraphOverlay(Graph source)
        {
            Source = source;
        }

        protected abstract void MakeOverlay(Graph source, Graph overlay);
    }
}
