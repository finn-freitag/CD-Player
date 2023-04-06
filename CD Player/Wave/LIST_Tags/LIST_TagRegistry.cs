using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave.LIST_Tags
{
    public class LIST_TagRegistry
    {
        public static Dictionary<string, Type> ILIST_Tags = PreBuild();

        private static Dictionary<string, Type> PreBuild()
        {
            Dictionary<string, Type> tags = new Dictionary<string, Type>();
            tags.Add(new ILIST_Tag_Album().GetIdentifier(), typeof(ILIST_Tag_Album));
            tags.Add(new ILIST_Tag_Artist().GetIdentifier(), typeof(ILIST_Tag_Artist));
            tags.Add(new ILIST_Tag_Comment().GetIdentifier(), typeof(ILIST_Tag_Comment));
            tags.Add(new ILIST_Tag_ContentDescription().GetIdentifier(), typeof(ILIST_Tag_ContentDescription));
            tags.Add(new ILIST_Tag_EncodedBy().GetIdentifier(), typeof(ILIST_Tag_EncodedBy));
            tags.Add(new ILIST_Tag_Genre().GetIdentifier(), typeof(ILIST_Tag_Genre));
            tags.Add(new ILIST_Tag_Keywords().GetIdentifier(), typeof(ILIST_Tag_Keywords));
            tags.Add(new ILIST_Tag_Title().GetIdentifier(), typeof(ILIST_Tag_Title));
            return tags;
        }

        public static ILIST_Tag GetTag(string identifier)
        {
            try
            {
                return (ILIST_Tag)GetInstanceOf(ILIST_Tags[identifier]);
            }
            catch
            {
                return null;
            }
        }

        public static void LoadFromDLL(string filename)
        {
            IEnumerable<Type> dll = GetLoadableTypes(Assembly.LoadFile(filename));
            foreach (Type type in dll)
            {
                if (type.GetInterfaces().Contains(typeof(ILIST_Tag)))
                {
                    string identifier = ((ILIST_Tag)GetInstanceOf(type)).GetIdentifier();
                    if (!ILIST_Tags.ContainsKey(identifier))
                    {
                        ILIST_Tags.Add(identifier, type);
                    }
                }
            }
        }

        public static void LoadInternal()
        {
            IEnumerable<Type> dll = GetLoadableTypes(Assembly.GetExecutingAssembly());
            foreach (Type type in dll)
            {
                if (type.GetInterfaces().Contains(typeof(ILIST_Tag)))
                {
                    string identifier = ((ILIST_Tag)GetInstanceOf(type)).GetIdentifier();
                    if (!ILIST_Tags.ContainsKey(identifier))
                    {
                        ILIST_Tags.Add(identifier, type);
                    }
                }
            }
        }

        public static void LoadExternal()
        {
            string[] paths = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            for (int i = 0; i < paths.Length; i++)
            {
                try
                {
                    if (paths[i].ToLower().EndsWith("dll"))
                    {
                        IEnumerable<Type> dll = GetLoadableTypes(Assembly.LoadFile(paths[i]));
                        foreach (Type type in dll)
                        {
                            if (type.GetInterfaces().Contains(typeof(ILIST_Tag)))
                            {
                                string identifier = ((ILIST_Tag)GetInstanceOf(type)).GetIdentifier();
                                if (!ILIST_Tags.ContainsKey(identifier))
                                {
                                    ILIST_Tags.Add(identifier, type);
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private static object GetInstanceOf(Type type)
        {
            try
            {
                ConstructorInfo[] cis = type.GetConstructors();
                foreach (ConstructorInfo ci in cis)
                {
                    if (ci.GetParameters().Length == 0)
                    {
                        return type.Assembly.CreateInstance(type.FullName);
                    }
                }
            }
            catch { }
            return null;
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                List<Type> types = new List<Type>();
                foreach (TypeInfo ti in assembly.DefinedTypes)
                {
                    types.Add(ti.AsType());
                }
                return types;
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}
