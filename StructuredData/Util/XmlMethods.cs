using System.Linq;
using System.Xml;
using System.Xml.Linq;
using OneOf;

namespace Reductech.Sequence.Connectors.StructuredData.Util;

/// <summary>
/// Contains methods to help with xml conversion
/// </summary>
internal class XmlMethods
{
    public static ISCLObject ToSCLObject(XElement element)
    {
        if (element.IsEmpty)
            return SCLNull.Instance;

        if (element.HasElements)
        {
            var l = new List<EntityProperty>();

            //this is an entity
            foreach (var group in element.Elements().GroupBy(x => x.Name))
            {
                if (group.Count() == 1)
                {
                    var value = ToSCLObject(group.Single());
                    l.Add(new EntityProperty(group.Key.LocalName, value, l.Count));
                }
                else
                {
                    var array = group.Select(ToSCLObject).ToSCLArray();
                    l.Add(new EntityProperty(group.Key.LocalName, array, l.Count));
                }
            }

            var entity = new Entity(l);
            return entity;
        }
        else
        {
            return new StringStream(element.Value);
        }
    }

    public static OneOf<XElement, XElement[]> ToXmlElement(string name, ISCLObject obj)
    {
        switch (obj)
        {
            case Entity entity:
            {
                var parent = new XElement(name);

                foreach (var ep in entity)
                {
                    var child = ToXmlElement(ep.Name, ep.Value);

                    if (child.TryPickT0(out var singleElement, out var list))

                        parent.Add(singleElement);
                    else
                        foreach (var xElement in list)
                            parent.Add(xElement);
                }

                return parent;
            }
            case ISCLOneOf oneOf: return ToXmlElement(name, oneOf.Value);
            case Unit:            return new XElement(name);
            case SCLNull:         return new XElement(name);

            case IArray array1:
            {
                var l = array1.ListIfEvaluated();

                if (!l.HasValue)
                    throw new XmlException("Could not serialize Lazy Array");

                var list =
                    l.Value.Select(
                            x =>
                            {
                                var e = ToXmlElement(name, x);

                                if (e.TryPickT0(out var singleElement, out var list))
                                    return singleElement;

                                //Handle nested lists
                                var parent = new XElement(name);

                                foreach (var xElement in list)
                                {
                                    parent.Add(xElement);
                                }

                                return parent;
                            }
                        )
                        .ToArray();

                return list;
            }
            default:
            {
                var v       = obj.ToCSharpObject();
                var element = new XElement(name, v);

                return element;
            }
        }
    }
}
