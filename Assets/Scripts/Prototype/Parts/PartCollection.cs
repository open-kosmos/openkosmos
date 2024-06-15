using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kosmos.Prototype.Parts
{
    public class PartCollection : MonoBehaviour
    {
        public struct PartLink
        {
            public PartBase _parentPart;
            public PartSocket _parentSocket;
            public PartBase _childPart;
            public PartSocket _childSocket;
        }
        
        private Dictionary<PartBase, List<PartLink>> _partLinks = new();
        private HashSet<PartBase> _allParts = new();
        private HashSet<PartSocket> _unconnectedSockets = new();
        
        public PartBase AddPart(PartDefinition def)
        {
            var newPart = PartDictionary.SpawnPart(def);
            _allParts.Add(newPart);
            newPart.transform.SetParent(transform);
            _unconnectedSockets.UnionWith(newPart.GetSockets());

            return newPart;
        }

        public void RemovePart(PartBase part)
        {
            DisconnectFromParents(part);
            
            //Disconnect any children
            if (_partLinks.ContainsKey(part))
            {
                foreach (var link in _partLinks[part])
                {
                    _unconnectedSockets.Add(link._childSocket);
                }
                
                _partLinks.Remove(part);
            }

            //Take ourselves out of the unconnected sockets list
            foreach (var socket in part.GetSockets())
            {
                if (_unconnectedSockets.Contains(socket))
                {
                    _unconnectedSockets.Remove(socket);
                }
            }
            
            _allParts.Remove(part);
            DestroyImmediate(part.gameObject);
        }
        
        public void DisconnectFromParents(PartBase part)
        {
            //Might need a better way to find links containing children at some point
            List<PartLink> toRemove = new();
            foreach (var (parent, links) in _partLinks)
            {
                toRemove.Clear();
                foreach (var partLink in links)
                {
                    if (partLink._childPart == part)
                    {
                        toRemove.Add(partLink);
                    }
                }
                
                foreach (var partLink in toRemove)
                {
                    _unconnectedSockets.Add(partLink._parentSocket);
                    _unconnectedSockets.Add(partLink._childSocket);
                    links.Remove(partLink);
                }
            }
        }
        
        public void LinkParts(PartBase parentPart, PartSocket parentSocket, PartBase childPart, PartSocket childSocket)
        {
            if (!_unconnectedSockets.Contains(parentSocket) || !_unconnectedSockets.Contains(childSocket))
            {
                Debug.LogError("Attempted to link parts with sockets that are already connected");
                return;
            }
            
            if (!_partLinks.ContainsKey(parentPart))
            {
                _partLinks[parentPart] = new List<PartLink>();
            }
            
            _partLinks[parentPart].Add(new PartLink
            {
                _parentPart = parentPart,
                _parentSocket = parentSocket,
                _childPart = childPart,
                _childSocket = childSocket
            });
            
            _unconnectedSockets.Remove(parentSocket);
            _unconnectedSockets.Remove(childSocket);
            
            Vector3 offset = parentSocket.transform.position - childSocket.transform.position;
            MovePart(childPart, offset);
        }

        public void MovePart(PartBase part, Vector3 offset)
        {
            part.transform.position += offset;
            
            //Move children
            if (_partLinks.ContainsKey(part))
            {
                foreach (var link in _partLinks[part])
                {
                    Vector3 newOffset = link._parentSocket.transform.position - link._childSocket.transform.position;
                    MovePart(link._childPart, newOffset);
                }
            }
        }

        public bool GetClosestPotentialConnection(PartBase part, out PartLink partLink, Camera cam, float pixelRange)
        {
            partLink = default;

            //Cache screen space positions of all of our parts
            List<(PartSocket, Vector2)> testSockets = new();
            foreach (var socket in part.GetSockets())
            {
                if (_unconnectedSockets.Contains(socket))
                {
                    Vector3 screenPos = cam.WorldToScreenPoint(socket.transform.position);
                    testSockets.Add((socket, new Vector2(screenPos.x, screenPos.y)));
                }
            }

            if (testSockets.Count == 0)
            {
                return false;
            }
            
            //Test the available sockets on the part against all others
            float closestDistSq = float.MaxValue;
            float pixRangeSq = pixelRange * pixelRange;
            foreach (var other in _unconnectedSockets)
            {
                if (part.GetSockets().Contains(other))
                {
                    continue;
                }
                
                Vector2 otherScreenPos = cam.WorldToScreenPoint(other.transform.position);
                
                foreach (var (socket, screenPos) in testSockets)
                {
                    //Check orientation
                    float dot = Vector3.Dot(socket.transform.up, other.transform.up);
                    if (dot > 0.0f)
                    {
                        continue;
                    }
                    
                    //Check distance
                    float distSq = Vector2.SqrMagnitude(screenPos - otherScreenPos);
                    if (distSq < closestDistSq && distSq < pixRangeSq)
                    {
                        closestDistSq = distSq;
                        partLink = new PartLink
                        {
                            _childPart = part,
                            _childSocket = socket,
                            _parentPart = other.GetComponentInParent<PartBase>(),
                            _parentSocket = other
                        };
                    }
                }
            }

            return closestDistSq <= pixRangeSq;
        }

        public void Serialise(string path)
        {
            VehicleSpec vehicleSpec = new();
            vehicleSpec.Parts = new();
            Dictionary<PartBase, int> partIndex = new();    //So we're not making any assumptions about orders in a hashset

            //Serialise parts
            foreach (var part in _allParts)
            {
                PartSpec partSpec = new();
                partSpec.PartDefGuid = part.GetDefinition().Guid.ToString();
                partSpec.LocalPosition = part.transform.localPosition;
                partSpec.LocalRotation = part.transform.localRotation;
                
                //Get tweakables
                partSpec.Tweakables = new();
                foreach (var tweakableField in PartDictionary.GetPartTweakableFields(part))
                {
                    partSpec.Tweakables.Add(
                        new TweakableValue(tweakableField.Name, 
                            tweakableField.GetValue(part).ToString()));
                }

                partIndex[part] = vehicleSpec.Parts.Count;
                vehicleSpec.Parts.Add(partSpec);
            }
            
            //Serialise connections
            vehicleSpec.Connections = new();
            foreach (var link in _partLinks)
            {
                foreach (var partLink in link.Value)
                {
                    ConnectionSpec connectionSpec = new();
                    connectionSpec.ParentPartIndex = partIndex[partLink._parentPart];
                    connectionSpec.ParentSocketIndex = partLink._parentPart.GetSockets().IndexOf(partLink._parentSocket);
                    connectionSpec.ChildPartIndex = partIndex[partLink._childPart];
                    connectionSpec.ChildSockedIndex = partLink._childPart.GetSockets().IndexOf(partLink._childSocket);
                    vehicleSpec.Connections.Add(connectionSpec);
                }
            }
            
            vehicleSpec.Serialise(path);
        }

        public void Deserialise(string path)
        {
            //Delete all children of our transform
            var children = new List<GameObject>();
            foreach (Transform child in transform) children.Add(child.gameObject);
            children.ForEach(DestroyImmediate);
            
            VehicleSpec spec = VehicleSpec.Deserialise(path);

            Dictionary<int, PartBase> partIndexDict = new();  
            //Spawn parts
            for (var partIndex = 0; partIndex < spec.Parts.Count; partIndex++)
            {
                var partSpec = spec.Parts[partIndex];
                var partDef = PartDictionary.GetPart(System.Guid.Parse(partSpec.PartDefGuid));
                if (partDef != null)
                {
                    var part = AddPart(partDef);

                    part.transform.localPosition = partSpec.LocalPosition;
                    part.transform.localRotation = partSpec.LocalRotation;

                    partIndexDict[partIndex] = part;

                    //Set tweakables
                    foreach (var tweakableField in PartDictionary.GetPartTweakableFields(part))
                    {
                        TweakableValue? field = partSpec.Tweakables.FirstOrDefault(x => x.Name == tweakableField.Name);
                        if (field.HasValue)
                        {
                            if (tweakableField.FieldType == typeof(float))
                            {
                                tweakableField.SetValue(part, float.Parse(field.Value.Value));
                            }
                            else
                            {
                                Debug.LogError($"Failed to deserialise type {tweakableField.FieldType}");
                            }
                        }
                    }
                }
            }
            
            //Process links
            foreach (var connectionSpec in spec.Connections)
            {
                if (partIndexDict.TryGetValue(connectionSpec.ParentPartIndex, out var parentPart) &&
                    partIndexDict.TryGetValue(connectionSpec.ChildPartIndex, out var childPart))
                {
                    LinkParts(parentPart, parentPart.GetSockets()[connectionSpec.ParentSocketIndex], childPart, childPart.GetSockets()[connectionSpec.ChildSockedIndex]);
                }
            }
        }
        

        
    }//Class
}//Namespace