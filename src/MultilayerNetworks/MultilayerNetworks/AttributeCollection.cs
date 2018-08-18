using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using Attribute = MultilayerNetworks.Components.Attribute;

namespace MultilayerNetworks
{
    /// <summary>
    /// Custom collection for attributes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AttributeCollection<T> :
        ICollection<T> where T : Components.Attribute
    {
        private double DefaultNumeric = 0.0;

        private string DefaultString = "";
        //inner ArrayList object
        protected ArrayList InnerAttributeList;
        //flag for setting collection to read-only
        //mode (not used in this example)
        protected bool _IsReadOnly;

        // Id -> Attribute Name : Value
        protected Dictionary<string, Dictionary<int, string>> StringAttribute;
        // Id -> Attribute Name : Value
        protected Dictionary<string, Dictionary<int, double>> NumericAttribute;

        protected Dictionary<string, int> AttributeIds;

        // Default constructor
        public AttributeCollection()
        {
            InnerAttributeList = new ArrayList();
            StringAttribute = new Dictionary<string, Dictionary<int, string>>();
            NumericAttribute = new Dictionary<string, Dictionary<int, double>>();

            AttributeIds = new Dictionary<string, int>();
        }

        // Default accessor for the collection 
        public virtual T this[int index]
        {
            get
            {
                return (T)InnerAttributeList[index];
            }
            set
            {
                InnerAttributeList[index] = value;
            }
        }

        // Number of elements in the collection
        public virtual int Count
        {
            get
            {
                return InnerAttributeList.Count;
            }
        }

        // Flag sets whether or not this collection is read-only
        public virtual bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }
        }

        // Add an object to the collection
        public virtual void Add(string attributeName, AttributeType attrType)
        {
            switch (attrType)
            {
                case AttributeType.StringType:
                    if (StringAttribute.ContainsKey(attributeName))
                        throw new DuplicateNameException("Attribute " + attributeName);
                    StringAttribute.Add(attributeName, new Dictionary<int, string>());
                    break;
                case AttributeType.NumericType:
                    if (NumericAttribute.ContainsKey(attributeName))
                        throw new DuplicateNameException("Attribute " + attributeName);
                    NumericAttribute.Add(attributeName, new Dictionary<int, double>());
                    break;
            }
            var newAttr = new Components.Attribute(attributeName, attrType);
            InnerAttributeList.Add(newAttr);
            AttributeIds.Add(attributeName, InnerAttributeList.Count - 1);
        }

        // Remove first instance of a business object from the collection 
        public virtual bool Remove(T AttributeObject)
        {
            bool result = false;

            //loop through the inner array's indices
            for (int i = 0; i < InnerAttributeList.Count; i++)
            {
                //store current index being checked
                T obj = (T)InnerAttributeList[i];

                //compare the BusinessObjectBase UniqueId property
                if (obj.Name == AttributeObject.Name && obj.AttributeType == AttributeObject.AttributeType)
                {
                    //remove item from inner ArrayList at index i
                    InnerAttributeList.RemoveAt(i);
                    result = true;
                    break;
                }
            }

            return result;
        }

        // Returns true/false based on whether or not it finds
        // the requested object in the collection.
        public bool Contains(T AttributeObject)
        {
            //loop through the inner ArrayList
            foreach (T obj in InnerAttributeList)
            {
                //compare the BusinessObjectBase UniqueId property
                if (obj.Name == AttributeObject.Name && obj.AttributeType == AttributeObject.AttributeType)
                {
                    //if it matches return true
                    return true;
                }
            }
            //no match
            return false;
        }

        public virtual Components.Attribute GetAttribute(int idx)
        {
            if (InnerAttributeList.Count > idx)
            {
                return (Components.Attribute) InnerAttributeList[idx];
            }
            else return null;
        }

        public virtual Components.Attribute GetAttribute(string attrName)
        {
            if (AttributeIds.ContainsKey(attrName))
            {
                return (Attribute) InnerAttributeList[AttributeIds[attrName]];
            }
            else return null;
        }

        public virtual void SetNumeric(int objectId, string attrName, double val)
        {
            if (!NumericAttribute.ContainsKey(attrName))
                throw new KeyNotFoundException("String attribute " + attrName);
            if (!NumericAttribute[attrName].ContainsKey(objectId))
            {
                NumericAttribute[attrName].Add(objectId, val);
            }
            else
            {
                NumericAttribute[attrName][objectId] = val;
            }
        }

        public virtual void SetString(int objectId, string attrName, string val)
        {
            if (!StringAttribute.ContainsKey(attrName))
                throw new KeyNotFoundException("String attribute " + attrName);
            if (!StringAttribute[attrName].ContainsKey(objectId))
            {
                StringAttribute[attrName].Add(objectId, val);
            }
            else
            {
                StringAttribute[attrName][objectId] = val;
            }
            
        }

        public virtual double GetNumeric(int objectId, string attrName)
        {
            if (!NumericAttribute.ContainsKey(attrName))
                throw new KeyNotFoundException("Numeric attribute " + attrName);
            if (!NumericAttribute[attrName].ContainsKey(objectId))
                return DefaultNumeric;
            return NumericAttribute[attrName][objectId];
        }

        public virtual string GetString(int objectId, string attrName)
        {
            if (!StringAttribute.ContainsKey(attrName))
                throw new KeyNotFoundException("String attribute " + attrName);
            if (!StringAttribute[attrName].ContainsKey(objectId))
                return DefaultString;
            return StringAttribute[attrName][objectId];
        }

        public virtual void Reset(int objectId)
        {
            foreach (Components.Attribute attr in InnerAttributeList)
            {
                switch (attr.AttributeType)
                {
                    case AttributeType.NumericType:
                        NumericAttribute[attr.Name].Remove(objectId);
                        break;
                    case AttributeType.StringType:
                        StringAttribute[attr.Name].Remove(objectId);
                        break;
                }
            }
        }

        // Copy objects from this collection into another array
        public virtual void CopyTo(T[] BusinessObjectArray, int index)
        {
            throw new Exception(
                "This Method is not valid for this implementation.");
        }

        // Clear the collection of all it's elements
        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public virtual void Clear()
        {
            InnerAttributeList.Clear();
        }

        // Returns custom generic enumerator for this BusinessObjectCollection
        public virtual IEnumerator<T> GetEnumerator()
        {
            //return a custom enumerator object instantiated
            //to use this BusinessObjectCollection 
            return new AttributeObjectEnumerator<T>(this);
        }

        // Explicit non-generic interface implementation for IEnumerable
        // extended and required by ICollection (implemented by ICollection<T>)
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new AttributeObjectEnumerator<T>(this);
        }
    }
}