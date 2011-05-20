EntityGraph is an experimental project that adds the ability to operate on graphs of entities to RIA services.
An entity graph is a collection of entities which are connected using navigation properties marked with the EntityGraphAttribute. Entity graphs enable
operations on collections of associated wntities rather than on individual entities. Example operations are cloning and detaching entities from their
context.

New in this version is that graph operations are new defined in the class EntityGraph. The method EntityGraph() is an extension method on entities and returns the entity graph object that corresponds to the given entity. In addition to cloning, deletion, and detaching, an EntityGraph now has the following features:
1) It implements INotifyPropertyChanged. Whenever a property on any if the entities in the graph changes a property changed event is fired.
2) It implements INotifyCollectionChanged. Whenever a collection changes in any of the entities of the graph a notify collection changed event is fired.
3) It implements IEnumerable<Entity> so that you can iterate (and use Linq) over the entities in the graph
4) GetChanges() returns an object of type EntityGraphChangeSet containing the change set of the entities in the graoh
5) HasChanges() returns true whenever there exist an entity in the graph that has pending changes

This implementation is experimental and its main purpose is proof of concept. As a consequence, the implementation is not optimizaed for effenciency and such.
