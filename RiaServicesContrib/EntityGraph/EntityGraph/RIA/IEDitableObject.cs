using System.ComponentModel;

namespace RiaServicesContrib.DomainServices.Client
{
    public partial class EntityGraph : IEditableObject 
    {
        private bool EditStarted { get; set; }
        public void BeginEdit() {
            EditStarted = true;
            EntityRelationGraph.Nodes.ForEach(e => ((IEditableObject)e.Node).BeginEdit());
        }

        public void CancelEdit() {
            EntityRelationGraph.Nodes.ForEach(e => ((IEditableObject)e.Node).CancelEdit());
            EditStarted = false;
        }

        public void EndEdit() {
            EntityRelationGraph.Nodes.ForEach(e => ((IEditableObject)e.Node).EndEdit());
            EditStarted = false;
        }
    }
}
